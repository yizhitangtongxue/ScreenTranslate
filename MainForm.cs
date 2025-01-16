using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Tmt.V20180321;
using TencentCloud.Tmt.V20180321.Models;
using Tesseract;
using Timer = System.Windows.Forms.Timer;

namespace ScreenTranslate
{
    public partial class Form1 : Form
    {
        private ResultForm form2;
        private Timer timer;

        // 用于存储当前截图的内存中的图片
        private Bitmap currentScreenshot;

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

            String ocrRes = ExtractTextFromBitmap(currentScreenshot);
            if (!string.IsNullOrEmpty(ocrRes)) {
                String translateRes = Translate(ocrRes);

         

                this.form2.UpdateTextBox(translateRes);
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
        protected override void WndProc(ref Message m)
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