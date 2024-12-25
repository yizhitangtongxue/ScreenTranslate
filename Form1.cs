using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Tesseract;
using Timer = System.Windows.Forms.Timer;

namespace ScreenTranslate
{
    public partial class Form1 : Form
    {
        private Form2 form2;
        private Timer timer;

        // ���ڴ洢��ǰ��ͼ���ڴ��е�ͼƬ
        private Bitmap currentScreenshot;

        // ���캯������ʼ������
        public Form1()
        {
            InitializeComponent();
            this.Resize += MainForm_Resize;

            form2 = new Form2();
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
            this.form2.UpdateTextBox(ocrRes);

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

        public string ExtractTextFromImage(string imagePath)
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(imagePath))
                {
                    var page = engine.Process(img);
                    return page.GetText();
                }
            }
        }

        public string ExtractTextFromImageInMemory(byte[] imageData)
        {
            // ��ͼƬ����ת��ΪMemoryStream
            using (var ms = new MemoryStream(imageData))
            {
                // ʹ��Tesseract����OCRʶ��
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    // ����ͼƬ��
                    using (var img = Pix.LoadFromMemory(ms.ToArray()))
                    {
                        // ʶ��ͼƬ�е����ֲ�����
                        var page = engine.Process(img);
                        return page.GetText();
                    }
                }
            }
        }
    }


}