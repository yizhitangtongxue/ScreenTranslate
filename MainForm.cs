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

      
        // ���ڴ洢��ǰ��ͼ���ڴ��е�ͼƬ
        private Bitmap currentScreenshot;
        public string QwenToken { get; private set; }
        public string AccessKey { get; private set; }
        public string SecretKey { get; private set; }

        private bool NoError { get; set; }

        // ���캯������ʼ������
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

        // �ڴ������ʱ���г�ʼ������
        private void Form1_Load(object sender, EventArgs e)
        {
            // ���ô��屳��ɫΪ LimeGreen ��ʹ��͸��
            this.BackColor = Color.LimeGreen;
            this.TransparencyKey = Color.LimeGreen;

            // ���ô���Ϊ���㴰�ڣ���ʼ�ձ�������������֮��
            TopLevel = true;
            TopMost = true;
        }

        private void DoCapture(object sender, EventArgs e)
        {
            // ����ı�������
            this.form2.ClearTextBox();

            // ��ȡ��������
            Point point = extendedPanel1.PointToScreen(Point.Empty);
            Rectangle captureArea = new Rectangle(point.X, point.Y, this.extendedPanel1.Width, this.extendedPanel1.Height);

            // �����µĽ�ͼ
            Bitmap newScreenshot = new Bitmap(captureArea.Width, captureArea.Height);
            using (Graphics g = Graphics.FromImage(newScreenshot))
            {
                g.CopyFromScreen(captureArea.Location, Point.Empty, captureArea.Size);
            }

            // �ͷžɵĽ�ͼ��Դ
            currentScreenshot?.Dispose();

            // ����Ϊ�µĽ�ͼ
            currentScreenshot = newScreenshot;

            //String ocrRes = ExtractTextFromBitmap(currentScreenshot);

            String ocrRes = ReadTextByUseQwenOCR(currentScreenshot);
            if (!string.IsNullOrEmpty(ocrRes))
            {
                this.form2.UpdateTextBox(ocrRes);
            }
        }
// ��QwenOCR��ȡ�ļ�תΪ���֣�ʹ��QwenAI���з��룬ȥ����Ѷ�Ƶķ���

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
                throw new Exception("δ����ǧ��Token");
            }
            var imageId = UploadFileAsync(this.QwenToken, currentScreenshot);

            var prompt = "���ṩ��һ��ͼƬ��ͼƬ�а������Զ�����ҵ����֡�����ʶ��ͼƬ�е����֣�ȷ��������ʹ�õ����ԣ�������Щ���ַ���ɼ������ġ���ȷ��ÿһ�����ֵķ���׼ȷ��������ԭ�ĵ���˼��ͼƬ�п��ܰ��������ַ���������������������ĸ��˹������ĸ�����֡����ġ����Ӣ���Լ�����������ʽ��";
            var trans = GetChatCompletionAsync(this.QwenToken, prompt, imageId);

            return trans;
        }

        public string UploadFileAsync(string token, Bitmap bitmap)
        {
            try
            {
                var url = "https://chat.qwenlm.ai/api/v1/files/";

                // ��Bitmapת��Ϊ�ֽ�����
                byte[] fileContent = ConvertBitmapToByteArray(bitmap);

                // ����multipart/form-data��������
                var content = new MultipartFormDataContent();

                // ���ļ�������ӵ�������
                var fileContentContent = new ByteArrayContent(fileContent);
                fileContentContent.Headers.ContentType = new MediaTypeHeaderValue("image/png"); // �����ļ���MIME����
                content.Add(fileContentContent, "file", "pot_screenshot_cut.png");

                // ��������ͷ
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // ����POST����
                var response = client.PostAsync(url, content);

                if (!response.IsCompletedSuccessfully)
                {
                    throw new Exception("�ļ��ϴ�ʧ�ܣ�HTTP״̬��: " + response.Status);
                }

                // ������Ӧ����
                var uploadData = response.Result.Content.ToString();

                // �����ϴ��ɹ����ҷ�����JSON����
                dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(uploadData);
                var imageId = responseData.id;

                if (imageId == null)
                {
                    throw new Exception("�ļ��ϴ�ʧ�ܣ�δ��ȡ���ļ�ID");
                }

                return imageId;
            }
            catch (Exception ex)
            {
                throw new Exception("�ϴ������з�������: " + ex.Message);
            }
        }

        public string GetChatCompletionAsync(string token, string prompt, string imageId)
        {
            try
            {
                var url = "https://chat.qwenlm.ai/api/chat/completions";

                // ����������
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

                // ��������ͷ
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Add("accept", "*/*");
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0");

                // ����POST����
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
                Console.WriteLine("����������: " + ex.Message);
                return null;
            }
        }

        // ��Bitmapת��Ϊ�ֽ�����
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
            // ��Bitmapת��ΪMemoryStream
            using (var ms = new MemoryStream())
            {
                // ��Bitmap���浽MemoryStream
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                // ʹ��Tesseract����OCRʶ��
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    // ��MemoryStream��ȡͼƬ
                    using (var img = Pix.LoadFromMemory(ms.ToArray()))
                    {
                        // ʶ��ͼƬ�е����ֲ�����
                        var page = engine.Process(img);
                        return page.GetText();
                    }
                }
            }
        }

        // ��д WndProc �������񴰿���Ϣ
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_NCLBUTTONDBLCLK = 0xA3; // ������˫����Ϣ

            if (m.Msg == WM_NCLBUTTONDBLCLK)
            {
                // ���񵽱�����˫���¼�
                OpenSettings();
                return; // ��ֹĬ����Ϊ���細�����/��ԭ��
            }

            base.WndProc(ref m);
        }

        // �����ô��ڵķ���
        private void OpenSettings()
        {

            // ���磬��ʾһ�����ô���
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
                return $"Ŀ������Ϊ��{tres.Source}����������{tres.TargetText}";
            }

            return "δʶ������";
        }


    }


}