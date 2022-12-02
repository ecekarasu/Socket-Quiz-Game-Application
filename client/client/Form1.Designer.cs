namespace client
{
    partial class Form1
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
            this.textBox_ip = new System.Windows.Forms.TextBox();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.logs = new System.Windows.Forms.RichTextBox();
            this.button_connect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_answer = new System.Windows.Forms.TextBox();
            this.button_submit = new System.Windows.Forms.Button();
            this.button_disconnect = new System.Windows.Forms.Button();


            this.SuspendLayout();
            // 
            // textBox_ip
            // 
            this.textBox_ip.Location = new System.Drawing.Point(106, 20);
            this.textBox_ip.Name = "textBox_ip";
            this.textBox_ip.Size = new System.Drawing.Size(100, 26);
            this.textBox_ip.TabIndex = 0;
            this.textBox_ip.Tag = "";
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(106, 55);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(100, 26);
            this.textBox_port.TabIndex = 1;
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(106, 90);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(100, 26);
            this.textBox_name.TabIndex = 3;
            // 
            // logs
            // 
            this.logs.Location = new System.Drawing.Point(36, 189);
            this.logs.Name = "logs";
            this.logs.Size = new System.Drawing.Size(294, 206);
            this.logs.TabIndex = 4;
            this.logs.Text = "";
            // 
            // button_connect
            // 
            this.button_connect.AccessibleName = "";
            this.button_connect.Location = new System.Drawing.Point(230, 18);
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(100, 37);
            this.button_connect.TabIndex = 5;
            this.button_connect.Text = "connect";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "IP:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "port:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "answer:";
            // 
            // textBox_answer
            // 
            this.textBox_answer.Location = new System.Drawing.Point(106, 128);
            this.textBox_answer.Name = "textBox_answer";
            this.textBox_answer.Size = new System.Drawing.Size(100, 26);
            this.textBox_answer.TabIndex = 10;
            // 
            // button_submit
            // 

            this.button_submit.Location = new System.Drawing.Point(230, 123);

            this.button_submit.Enabled = false;
            this.button_submit.Location = new System.Drawing.Point(230, 117);


            this.button_submit.Enabled = false;
            this.button_submit.Location = new System.Drawing.Point(230, 117);


            this.button_submit.Enabled = false;
            this.button_submit.Location = new System.Drawing.Point(230, 117);

            this.button_submit.Name = "button_submit";
            this.button_submit.Size = new System.Drawing.Size(100, 37);
            this.button_submit.TabIndex = 11;
            this.button_submit.Text = "submit";
            this.button_submit.UseVisualStyleBackColor = true;
            this.button_submit.Click += new System.EventHandler(this.button_submit_Click);
            // 
            // button_disconnect
            // 
            this.button_disconnect.Enabled = false;
            this.button_disconnect.Location = new System.Drawing.Point(230, 61);
            this.button_disconnect.Name = "button_disconnect";
            this.button_disconnect.Size = new System.Drawing.Size(100, 40);
            this.button_disconnect.TabIndex = 12;
            this.button_disconnect.Text = "disconnect";
            this.button_disconnect.UseVisualStyleBackColor = true;
            this.button_disconnect.Click += new System.EventHandler(this.button_disconnect_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 407);


            this.Controls.Add(this.button_disconnect);

            this.Controls.Add(this.button_disconnect);


            this.Controls.Add(this.button_disconnect);

            this.Controls.Add(this.button_submit);
            this.Controls.Add(this.textBox_answer);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_connect);
            this.Controls.Add(this.logs);
            this.Controls.Add(this.textBox_name);
            this.Controls.Add(this.textBox_port);
            this.Controls.Add(this.textBox_ip);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_ip;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.RichTextBox logs;
        private System.Windows.Forms.Button button_connect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_answer;
        private System.Windows.Forms.Button button_submit;

        private System.Windows.Forms.Button button_disconnect;

       

    }
}
