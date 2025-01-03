namespace ScreenTranslate
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            extendedPanel1 = new ExtendedPanel();
            SuspendLayout();
            // 
            // extendedPanel1
            // 
            extendedPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            extendedPanel1.Location = new Point(-4, 2);
            extendedPanel1.Margin = new Padding(0);
            extendedPanel1.Name = "extendedPanel1";
            extendedPanel1.Size = new Size(1242, 157);
            extendedPanel1.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1242, 160);
            Controls.Add(extendedPanel1);
            Margin = new Padding(4, 4, 4, 4);
            Name = "Form1";
            Text = "选区翻译工具";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
        private ExtendedPanel extendedPanel1;
    }
}
