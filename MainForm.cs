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

        // ���ڴ洢��ǰ��ͼ���ڴ��е�ͼƬ
        private Bitmap currentScreenshot;

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

            String ocrRes = ExtractTextFromBitmap(currentScreenshot);
            if (!string.IsNullOrEmpty(ocrRes)) {
                String translateRes = Translate(ocrRes);

         

                this.form2.UpdateTextBox(translateRes);
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
        protected override void WndProc(ref Message m)
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