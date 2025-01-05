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
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            // 设置窗体为顶层窗口，并始终保持在其他窗口之上
            TopLevel = true;
            TopMost = true;

            var db = SqlSugarHelper.GetSugar();
            var res = db.Queryable<KeyConfigEntity>().InSingle(1);
            if (res != null) {
                this.AccessKeyLabel.Text = res.AccessKey;
                this.SecretKeyLabel.Text = res.SecretKey;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void s_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ok_btn_Click(object sender, EventArgs e)
        {
            var db = SqlSugarHelper.GetSugar();
            db.Updateable(new KeyConfigEntity( ) { AccessKey = this.AccessKeyLabel.Text,SecretKey = this.SecretKeyLabel.Text }).Where(it => it.Id.Equals(1)).ExecuteCommand();
            this.Close();
        }
    }
}
