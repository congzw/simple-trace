namespace SimpleTrace.Server
{
    partial class DemoForm
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
            this.btnRun = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnThrowEx = new System.Windows.Forms.Button();
            this.btnTransient = new System.Windows.Forms.Button();
            this.btnSingleton = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRun
            // 
            this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRun.Location = new System.Drawing.Point(590, 14);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(96, 23);
            this.btnRun.TabIndex = 29;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.btnRun);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 407);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(800, 43);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(692, 14);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(96, 23);
            this.btnClear.TabIndex = 30;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnThrowEx);
            this.splitContainer1.Panel1.Controls.Add(this.btnTransient);
            this.splitContainer1.Panel1.Controls.Add(this.btnSingleton);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtMessage);
            this.splitContainer1.Size = new System.Drawing.Size(800, 407);
            this.splitContainer1.SplitterDistance = 160;
            this.splitContainer1.TabIndex = 42;
            // 
            // btnThrowEx
            // 
            this.btnThrowEx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnThrowEx.Location = new System.Drawing.Point(590, 12);
            this.btnThrowEx.Name = "btnThrowEx";
            this.btnThrowEx.Size = new System.Drawing.Size(96, 23);
            this.btnThrowEx.TabIndex = 39;
            this.btnThrowEx.Text = "ThrowEx";
            this.btnThrowEx.UseVisualStyleBackColor = true;
            this.btnThrowEx.Click += new System.EventHandler(this.btnThrowEx_Click);
            // 
            // btnTransient
            // 
            this.btnTransient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTransient.Location = new System.Drawing.Point(692, 41);
            this.btnTransient.Name = "btnTransient";
            this.btnTransient.Size = new System.Drawing.Size(96, 23);
            this.btnTransient.TabIndex = 38;
            this.btnTransient.Text = "Transient";
            this.btnTransient.UseVisualStyleBackColor = true;
            this.btnTransient.Click += new System.EventHandler(this.btnTransient_Click);
            // 
            // btnSingleton
            // 
            this.btnSingleton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSingleton.Location = new System.Drawing.Point(692, 12);
            this.btnSingleton.Name = "btnSingleton";
            this.btnSingleton.Size = new System.Drawing.Size(96, 23);
            this.btnSingleton.TabIndex = 37;
            this.btnSingleton.Text = "Singleton";
            this.btnSingleton.UseVisualStyleBackColor = true;
            this.btnSingleton.Click += new System.EventHandler(this.btnSingleton_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessage.Location = new System.Drawing.Point(0, 0);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(800, 243);
            this.txtMessage.TabIndex = 24;
            // 
            // DemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.groupBox1);
            this.Name = "DemoForm";
            this.Text = "DemoForm";
            this.Load += new System.EventHandler(this.ServicesForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnThrowEx;
        private System.Windows.Forms.Button btnTransient;
        private System.Windows.Forms.Button btnSingleton;
        private System.Windows.Forms.TextBox txtMessage;
    }
}