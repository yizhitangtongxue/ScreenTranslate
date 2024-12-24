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

        // RECT �ṹ�����ڴ洢���ڵ�λ����Ϣ�����ϡ��ҡ��£�
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;   // �������Ͻ� X ����
            public int Top;    // �������Ͻ� Y ����
            public int Right;  // �������½� X ����
            public int Bottom; // �������½� Y ����
        }

        // ���캯������ʼ������
        public Form1()
        {
            InitializeComponent();
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

        // ��ť����¼�������ָ������Ĵ��ڲ���ȡ���ڵ��ı���������Ϣ
        private void Button_Click(object sender, EventArgs e)
        {
            // ����ı�������
            this.textBox1.Clear();

            // ��ȡ��ǰʱ�䲢��ʽ��
            DateTime currentTime = DateTime.Now;
            string formattedTime = currentTime.ToString("yyyy-MM-dd HH:mm:ss");
            this.textBox1.Text = formattedTime;
            Point point = extendedPanel1.PointToScreen(Point.Empty);
            // ָ���������򣨱������Ͻ����Ϊ(100,100)����800����600��
            Rectangle captureArea = new Rectangle(point.X,point.Y, this.extendedPanel1.Width, this.extendedPanel1.Height);

            // ����Bitmap���󣬴�С���������һ��
            using (Bitmap bmp = new Bitmap(captureArea.Width, captureArea.Height))
            {
                // ʹ��Graphics�����ȡָ���������Ļ
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(captureArea.Location, Point.Empty, captureArea.Size);
                }

                // ����ͼƬ���ļ�
                string filePath = $"Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                bmp.Save(filePath, ImageFormat.Png);

                // ��ʾ����·��
                //MessageBox.Show($"��ͼ�ѱ��浽: {filePath}", "��ͼ�ɹ�", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
        }


        // ��ť����¼����� Button_Click ����
        private void button1_Click(object sender, EventArgs e)
        {
            this.Button_Click(sender, e);
        }
    }
}