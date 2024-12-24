using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace ScreenTranslate
{
    public partial class Form1 : Form
    {

        // RECT 结构体用于存储窗口的位置信息（左、上、右、下）
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;   // 窗口左上角 X 坐标
            public int Top;    // 窗口左上角 Y 坐标
            public int Right;  // 窗口右下角 X 坐标
            public int Bottom; // 窗口右下角 Y 坐标
        }

        // 构造函数，初始化窗体
        public Form1()
        {
            InitializeComponent();
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

        // 按钮点击事件：查找指定标题的窗口并获取窗口的文本和区域信息
        private void Button_Click(object sender, EventArgs e)
        {
            // 清空文本框内容
            this.textBox1.Clear();

            // 获取当前时间并格式化
            DateTime currentTime = DateTime.Now;
            string formattedTime = currentTime.ToString("yyyy-MM-dd HH:mm:ss");
            this.textBox1.Text = formattedTime;
            Point point = extendedPanel1.PointToScreen(Point.Empty);
            // 指定截屏区域（比如左上角起点为(100,100)，宽800，高600）
            Rectangle captureArea = new Rectangle(point.X,point.Y, this.extendedPanel1.Width, this.extendedPanel1.Height);

            // 创建Bitmap对象，大小与截屏区域一致
            using (Bitmap bmp = new Bitmap(captureArea.Width, captureArea.Height))
            {
                // 使用Graphics对象截取指定区域的屏幕
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(captureArea.Location, Point.Empty, captureArea.Size);
                }

                // 保存图片到文件
                string filePath = $"Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                bmp.Save(filePath, ImageFormat.Png);

                // 显示保存路径
                //MessageBox.Show($"截图已保存到: {filePath}", "截图成功", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
        }


        // 按钮点击事件调用 Button_Click 方法
        private void button1_Click(object sender, EventArgs e)
        {
            this.Button_Click(sender, e);
        }
    }
}