namespace step2_server
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
            this.richTextBox_info = new System.Windows.Forms.RichTextBox();
            this.button_listen = new System.Windows.Forms.Button();
            this.button_startgame = new System.Windows.Forms.Button();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.textBox_num = new System.Windows.Forms.TextBox();
            this.textBox_filename = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // richTextBox_info
            // 
            this.richTextBox_info.Location = new System.Drawing.Point(20, 179);
            this.richTextBox_info.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.richTextBox_info.Name = "richTextBox_info";
            this.richTextBox_info.Size = new System.Drawing.Size(417, 379);
            this.richTextBox_info.TabIndex = 0;
            this.richTextBox_info.Text = "";
            // 
            // button_listen
            // 
            this.button_listen.Location = new System.Drawing.Point(266, 28);
            this.button_listen.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_listen.Name = "button_listen";
            this.button_listen.Size = new System.Drawing.Size(92, 28);
            this.button_listen.TabIndex = 1;
            this.button_listen.Text = "listen";
            this.button_listen.UseVisualStyleBackColor = true;
            this.button_listen.Click += new System.EventHandler(this.button_listen_Click_1);
            // 
            // button_startgame
            // 
            this.button_startgame.Enabled = false;
            this.button_startgame.Location = new System.Drawing.Point(306, 122);
            this.button_startgame.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_startgame.Name = "button_startgame";
            this.button_startgame.Size = new System.Drawing.Size(129, 46);
            this.button_startgame.TabIndex = 2;
            this.button_startgame.Text = "Start Game";
            this.button_startgame.UseVisualStyleBackColor = true;
            this.button_startgame.Click += new System.EventHandler(this.button_startgame_Click);
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(142, 31);
            this.textBox_port.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(93, 22);
            this.textBox_port.TabIndex = 3;
            // 
            // textBox_num
            // 
            this.textBox_num.Location = new System.Drawing.Point(142, 71);
            this.textBox_num.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_num.Name = "textBox_num";
            this.textBox_num.Size = new System.Drawing.Size(93, 22);
            this.textBox_num.TabIndex = 4;
            // 
            // textBox_filename
            // 
            this.textBox_filename.Location = new System.Drawing.Point(142, 111);
            this.textBox_filename.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_filename.Name = "textBox_filename";
            this.textBox_filename.Size = new System.Drawing.Size(93, 22);
            this.textBox_filename.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 33);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "port:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 73);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "num of questions:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 113);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "file name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 152);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "info:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 575);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_filename);
            this.Controls.Add(this.textBox_num);
            this.Controls.Add(this.textBox_port);
            this.Controls.Add(this.button_startgame);
            this.Controls.Add(this.button_listen);
            this.Controls.Add(this.richTextBox_info);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox_info;
        private System.Windows.Forms.Button button_listen;
        private System.Windows.Forms.Button button_startgame;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.TextBox textBox_num;
        private System.Windows.Forms.TextBox textBox_filename;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

