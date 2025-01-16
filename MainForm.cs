using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http.Headers;
using System.Windows.Forms;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Tmt.V20180321;
using TencentCloud.Tmt.V20180321.Models;
using Tesseract;
using Timer = System.Windows.Forms.Timer;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
namespace ScreenTranslate
{
    public partial class Form1 : Form
    {
        private ResultForm form2;
        private Timer timer;
        private static readonly HttpClient client = new HttpClient();

      
        // 用于存储当前截图的内存中的图片
        private Bitmap currentScreenshot;
        public string QwenToken { get; private set; }
        public string AccessKey { get; private set; }
        public string SecretKey { get; private set; }

        private bool NoError { get; set; }

        // 构造函数，初始化窗体
        public Form1()
        {
            InitializeComponent();

            this.Resize += MainForm_Resize;

            form2 = new ResultForm();
            form2.Show();

            timer = new Timer { Interval = 5000 };
            timer.Tick += DoCapture;
            timer.Start();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            this.extendedPanel1.Width = this.ClientSize.Width;
            this.extendedPanel1.Height = this.ClientSize.Height;
        }

        // 在窗体加载时进行初始化设置
        private void Form1_Load(object sender, EventArgs e)
        {
            // 设置窗体背景色为 LimeGreen 并使其透明
            this.BackColor = Color.LimeGreen;
            this.TransparencyKey = Color.LimeGreen;

            // 设置窗体为顶层窗口，并始终保持在其他窗口之上
            TopLevel = true;
            TopMost = true;
        }

        private void DoCapture(object sender, EventArgs e)
        {
            // 清空文本框内容
            this.form2.ClearTextBox();

            // 获取截屏区域
            Point point = extendedPanel1.PointToScreen(Point.Empty);
            Rectangle captureArea = new Rectangle(point.X, point.Y, this.extendedPanel1.Width, this.extendedPanel1.Height);

            // 创建新的截图
            Bitmap newScreenshot = new Bitmap(captureArea.Width, captureArea.Height);
            using (Graphics g = Graphics.FromImage(newScreenshot))
            {
                g.CopyFromScreen(captureArea.Location, Point.Empty, captureArea.Size);
            }

            // 释放旧的截图资源
            currentScreenshot?.Dispose();

            // 更新为新的截图
            currentScreenshot = newScreenshot;

            //String ocrRes = ExtractTextFromBitmap(currentScreenshot);

            String ocrRes = ReadTextByUseQwenOCR(currentScreenshot);
            if (!string.IsNullOrEmpty(ocrRes))
            {
                this.form2.UpdateTextBox(ocrRes);
            }
        }
// 用QwenOCR读取文件转为文字，使用QwenAI进行翻译，去除腾讯云的翻译

        private string ReadTextByUseQwenOCR(Bitmap currentScreenshot)
        {
            if (currentScreenshot == null)
            {
                throw new ArgumentNullException(nameof(currentScreenshot), "Screenshot cannot be null");
            }

            var db = SqlSugarHelper.GetSugar();
            var res = db.Queryable<KeyConfigEntity>().InSingle(1);
            if (res != null && !res.QwenToken.Equals("qwen_token"))
            {
                this.QwenToken = res.QwenToken;
            }else { 
                throw new Exception("未设置千问Token");
            }
            var imageId = UploadFileAsync(this.QwenToken, currentScreenshot);

            var prompt = "我提供了一张图片，图片中包含来自多个国家的文字。请你识别图片中的文字，确定它们所使用的语言，并将这些文字翻译成简体中文。请确保每一段文字的翻译准确，并保留原文的意思。图片中可能包含多种字符，包括但不限于拉丁字母、斯拉夫字母、汉字、日文、俄语、英语以及其他文字形式。";
            var trans = GetChatCompletionAsync(this.QwenToken, prompt, imageId);

            return trans;
        }

        public string UploadFileAsync(string token, Bitmap bitmap)
        {
            try
            {
                var url = "https://chat.qwenlm.ai/api/v1/files/";

                // 将Bitmap转换为字节数组
                byte[] fileContent = ConvertBitmapToByteArray(bitmap);

                // 创建multipart/form-data请求内容
                var content = new MultipartFormDataContent();

                // 将文件内容添加到表单数据
                var fileContentContent = new ByteArrayContent(fileContent);
                fileContentContent.Headers.ContentType = new MediaTypeHeaderValue("image/png"); // 设置文件的MIME类型
                content.Add(fileContentContent, "file", "pot_screenshot_cut.png");

                // 设置请求头
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // 发送POST请求
                var response = client.PostAsync(url, content);

                if (!response.IsCompletedSuccessfully)
                {
                    throw new Exception("文件上传失败，HTTP状态码: " + response.Status);
                }

                // 解析响应数据
                var uploadData = response.Result.Content.ToString();

                // 假设上传成功并且返回了JSON数据
                dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(uploadData);
                var imageId = responseData.id;

                if (imageId == null)
                {
                    throw new Exception("文件上传失败，未获取到文件ID");
                }

                return imageId;
            }
            catch (Exception ex)
            {
                throw new Exception("上传过程中发生错误: " + ex.Message);
            }
        }

        public string GetChatCompletionAsync(string token, string prompt, string imageId)
        {
            try
            {
                var url = "https://chat.qwenlm.ai/api/chat/completions";

                // 构建请求体
                var requestBody = new ChatCompletionRequest
                {
                    stream = false,
                    model = "qwen-vl-max-latest",
                    messages = new[]
                    {
                    new Message
                    {
                        role = "user",
                        content = new[]
                        {
                            new Content { type = "text", text = prompt },
                            new Content { type = "image", image = imageId }
                        }
                    }
                },
                    session_id = "1",
                    chat_id = "2",
                    id = "3"
                };

                var jsonBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // 设置请求头
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Add("accept", "*/*");
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0");

                // 发送POST请求
                var response = client.PostAsync(url, content);

                if (response.IsCompleted)
                {
                    var responseData = response.Result.Content.ToString();
                    var data = JsonConvert.DeserializeObject<dynamic>(responseData);
                    return data.choices[0].message.content.ToString();
                }
                else
                {
                    var errorData = response.Result.Content.ToString();
                    throw new Exception($"Http Request Error\nHttp Status: {response.Status}\n{errorData}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("请求发生错误: " + ex.Message);
                return null;
            }
        }

        // 将Bitmap转换为字节数组
        private byte[] ConvertBitmapToByteArray(Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }


        public string ExtractTextFromBitmap(Bitmap bitmap)
        {
            // 将Bitmap转换为MemoryStream
            using (var ms = new MemoryStream())
            {
                // 将Bitmap保存到MemoryStream
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                // 使用Tesseract进行OCR识别
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    // 从MemoryStream读取图片
                    using (var img = Pix.LoadFromMemory(ms.ToArray()))
                    {
                        // 识别图片中的文字并返回
                        var page = engine.Process(img);
                        return page.GetText();
                    }
                }
            }
        }

        // 重写 WndProc 方法捕获窗口消息
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_NCLBUTTONDBLCLK = 0xA3; // 标题栏双击消息

            if (m.Msg == WM_NCLBUTTONDBLCLK)
            {
                // 捕获到标题栏双击事件
                OpenSettings();
                return; // 阻止默认行为（如窗口最大化/还原）
            }

            base.WndProc(ref m);
        }

        // 打开设置窗口的方法
        private void OpenSettings()
        {

            // 例如，显示一个设置窗体
            SettingForm settingsForm = new SettingForm();
            settingsForm.ShowDialog();
        }

        public string Translate(String originText)
        {
            var db = SqlSugarHelper.GetSugar();
            var res = db.Queryable<KeyConfigEntity>().InSingle(1);
            if (res != null && (!res.AccessKey.Equals("access") && !res.SecretKey.Equals("secret")))
            {
                this.AccessKey = res.AccessKey;
                this.SecretKey = res.SecretKey;

                Credential cred = new Credential
                {
                    SecretId = this.AccessKey,
                    SecretKey = this.SecretKey
                };

                TmtClient tmt = new TmtClient(cred, "ap-guangzhou", new ClientProfile { Language = Language.ZH_CN });
                var req = new TextTranslateRequest { Source = "auto",Target="zh",ProjectId = 1329154,SourceText=originText };

                var tres = tmt.TextTranslateSync(req);

                Console.WriteLine(tres);
                return $"目标语言为：{tres.Source}，翻译结果：{tres.TargetText}";
            }

            return "未识别到文字";
        }


    }


}