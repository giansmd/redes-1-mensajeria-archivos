namespace winProyComunicacion
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
            txtMensaje = new TextBox();
            btnEnviaMensaje = new Button();
            rchConversacion = new RichTextBox();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            SuspendLayout();
            // 
            // txtMensaje
            // 
            txtMensaje.Location = new Point(78, 424);
            txtMensaje.Multiline = true;
            txtMensaje.Name = "txtMensaje";
            txtMensaje.Size = new Size(322, 103);
            txtMensaje.TabIndex = 0;
            // 
            // btnEnviaMensaje
            // 
            btnEnviaMensaje.Location = new Point(297, 557);
            btnEnviaMensaje.Name = "btnEnviaMensaje";
            btnEnviaMensaje.Size = new Size(145, 23);
            btnEnviaMensaje.TabIndex = 1;
            btnEnviaMensaje.Text = "ENVIAR MENSAJE";
            btnEnviaMensaje.UseVisualStyleBackColor = true;
            btnEnviaMensaje.Click += btnEnviaMensaje_Click;
            // 
            // rchConversacion
            // 
            rchConversacion.Location = new Point(41, 23);
            rchConversacion.Name = "rchConversacion";
            rchConversacion.Size = new Size(411, 395);
            rchConversacion.TabIndex = 2;
            rchConversacion.Text = "";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(75, 607);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(191, 23);
            textBox1.TabIndex = 3;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(75, 693);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(188, 23);
            textBox2.TabIndex = 4;
            // 
            // button1
            // 
            button1.Location = new Point(316, 606);
            button1.Name = "button1";
            button1.Size = new Size(117, 23);
            button1.TabIndex = 5;
            button1.Text = "ABRE Y CARGA ARCHIVO";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(316, 650);
            button2.Name = "button2";
            button2.Size = new Size(117, 23);
            button2.TabIndex = 6;
            button2.Text = "ENVIAR ARCHIVO";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(316, 693);
            button3.Name = "button3";
            button3.Size = new Size(108, 23);
            button3.TabIndex = 7;
            button3.Text = "CREAR ARCHIVO";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(481, 741);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(rchConversacion);
            Controls.Add(btnEnviaMensaje);
            Controls.Add(txtMensaje);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtMensaje;
        private Button btnEnviaMensaje;
        private RichTextBox rchConversacion;
        private TextBox textBox1;
        private TextBox textBox2;
        private Button button1;
        private Button button2;
        private Button button3;
    }
}
