namespace ScreenTranslate
{
    partial class SettingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ok_btn = new Button();
            s = new Button();
            AccessKeyLabel = new TextBox();
            AccessKey = new Label();
            SecretKeyLabel = new TextBox();
            SecretKey = new Label();
            SuspendLayout();
            // 
            // ok_btn
            // 
            ok_btn.Location = new Point(14, 126);
            ok_btn.Margin = new Padding(2);
            ok_btn.Name = "ok_btn";
            ok_btn.Size = new Size(180, 43);
            ok_btn.TabIndex = 0;
            ok_btn.Text = "确定";
            ok_btn.UseVisualStyleBackColor = true;
            ok_btn.Click += ok_btn_Click;
            // 
            // s
            // 
            s.Location = new Point(208, 126);
            s.Margin = new Padding(2);
            s.Name = "s";
            s.Size = new Size(180, 43);
            s.TabIndex = 1;
            s.Text = "取消";
            s.UseVisualStyleBackColor = true;
            s.Click += s_Click;
            // 
            // AccessKeyLabel
            // 
            AccessKeyLabel.Location = new Point(94, 35);
            AccessKeyLabel.Margin = new Padding(2);
            AccessKeyLabel.Name = "AccessKeyLabel";
            AccessKeyLabel.Size = new Size(292, 23);
            AccessKeyLabel.TabIndex = 2;
            // 
            // AccessKey
            // 
            AccessKey.Font = new Font("Microsoft YaHei UI", 9F);
            AccessKey.Location = new Point(4, 38);
            AccessKey.Margin = new Padding(2, 0, 2, 0);
            AccessKey.Name = "AccessKey";
            AccessKey.Size = new Size(86, 43);
            AccessKey.TabIndex = 3;
            AccessKey.Text = "AccessKey：";
            AccessKey.Click += label1_Click;
            // 
            // SecretKeyLabel
            // 
            SecretKeyLabel.Location = new Point(94, 78);
            SecretKeyLabel.Margin = new Padding(2);
            SecretKeyLabel.Name = "SecretKeyLabel";
            SecretKeyLabel.Size = new Size(292, 23);
            SecretKeyLabel.TabIndex = 4;
            // 
            // SecretKey
            // 
            SecretKey.Font = new Font("Microsoft YaHei UI", 9F);
            SecretKey.Location = new Point(4, 81);
            SecretKey.Margin = new Padding(2, 0, 2, 0);
            SecretKey.Name = "SecretKey";
            SecretKey.Size = new Size(86, 43);
            SecretKey.TabIndex = 5;
            SecretKey.Text = "SecretKey：";
            // 
            // SettingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 192);
            Controls.Add(SecretKey);
            Controls.Add(SecretKeyLabel);
            Controls.Add(AccessKey);
            Controls.Add(AccessKeyLabel);
            Controls.Add(s);
            Controls.Add(ok_btn);
            Margin = new Padding(2);
            Name = "SettingForm";
            Text = "设置";
            Load += SettingForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ok_btn;
        private Button s;
        private TextBox AccessKeyLabel;
        private Label AccessKey;
        private TextBox SecretKeyLabel;
        private Label SecretKey;
    }
}