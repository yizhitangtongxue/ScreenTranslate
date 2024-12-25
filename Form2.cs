using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenTranslate
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.TopLevel = true;
            this.TopMost = true;
        }

        public void UpdateTextBox(string text)
        {
            this.textBox1.Text = text;
        }
        public void ClearTextBox()
        {
            this.textBox1.Clear();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
