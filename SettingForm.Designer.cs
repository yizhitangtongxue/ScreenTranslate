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
            textBox1 = new TextBox();
            AccessKey = new Label();
            textBox2 = new TextBox();
            SuspendLayout();
            // 
            // ok_btn
            // 
            ok_btn.Location = new Point(28, 345);
            ok_btn.Name = "ok_btn";
            ok_btn.Size = new Size(361, 78);
            ok_btn.TabIndex = 0;
            ok_btn.Text = "确定";
            ok_btn.UseVisualStyleBackColor = true;
            // 
            // s
            // 
            s.Location = new Point(415, 345);
            s.Name = "s";
            s.Size = new Size(361, 78);
            s.TabIndex = 1;
            s.Text = "取消";
            s.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(189, 63);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(580, 78);
            textBox1.TabIndex = 2;
            // 
            // AccessKey
            // 
            AccessKey.Location = new Point(28, 63);
            AccessKey.Name = "AccessKey";
            AccessKey.Size = new Size(142, 78);
            AccessKey.TabIndex = 3;
            AccessKey.Text = "AccessKey";
            AccessKey.Click += label1_Click;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(189, 202);
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(580, 78);
            textBox2.TabIndex = 4;
            // 
            // SettingForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(textBox2);
            Controls.Add(AccessKey);
            Controls.Add(textBox1);
            Controls.Add(s);
            Controls.Add(ok_btn);
            Name = "SettingForm";
            Text = "SettingForm";
            Load += SettingForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ok_btn;
        private Button s;
        private TextBox textBox1;
        private Label AccessKey;
        private TextBox textBox2;
    }
}