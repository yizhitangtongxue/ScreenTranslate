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

        // 用于存储当前截图的内存中的图片
        private Bitmap currentScreenshot;

        // 构造函数，初始化窗体
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
            this.form2.UpdateTextBox(ocrRes);

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
            // 将图片数据转换为MemoryStream
            using (var ms = new MemoryStream(imageData))
            {
                // 使用Tesseract进行OCR识别
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    // 加载图片流
                    using (var img = Pix.LoadFromMemory(ms.ToArray()))
                    {
                        // 识别图片中的文字并返回
                        var page = engine.Process(img);
                        return page.GetText();
                    }
                }
            }
        }
    }


}