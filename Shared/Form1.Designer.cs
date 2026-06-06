namespace winProyComunicacion
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            txtMensaje = new TextBox();
            btnEnviaMensaje = new Button();
            rchConversacion = new RichTextBox();
            labelTitulo = new Label();
            cbCOM = new ComboBox();
            txtUsuario = new TextBox();
            labelCOM = new Label();
            labelUsuario = new Label();
            labelVelocidad = new Label();
            cbVelocidad = new ComboBox();
            SuspendLayout();
            // 
            // txtMensaje
            // 
            txtMensaje.Location = new Point(14, 457);
            txtMensaje.Margin = new Padding(3, 4, 3, 4);
            txtMensaje.Multiline = true;
            txtMensaje.Name = "txtMensaje";
            txtMensaje.PlaceholderText = "Escriba su mensaje...";
            txtMensaje.Size = new Size(412, 51);
            txtMensaje.TabIndex = 0;
            // 
            // btnEnviaMensaje
            // 
            btnEnviaMensaje.Location = new Point(272, 517);
            btnEnviaMensaje.Margin = new Padding(3, 4, 3, 4);
            btnEnviaMensaje.Name = "btnEnviaMensaje";
            btnEnviaMensaje.Size = new Size(154, 31);
            btnEnviaMensaje.TabIndex = 1;
            btnEnviaMensaje.Text = "ENVIAR MENSAJE";
            btnEnviaMensaje.UseVisualStyleBackColor = true;
            btnEnviaMensaje.Click += btnEnviaMensaje_Click;
            // 
            // rchConversacion
            // 
            rchConversacion.Location = new Point(14, 92);
            rchConversacion.Margin = new Padding(3, 4, 3, 4);
            rchConversacion.Name = "rchConversacion";
            rchConversacion.Size = new Size(412, 356);
            rchConversacion.TabIndex = 2;
            rchConversacion.Text = "";
            // 
            // labelTitulo
            // 
            labelTitulo.AutoSize = true;
            labelTitulo.Font = new Font("Showcard Gothic", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelTitulo.Location = new Point(14, 12);
            labelTitulo.Name = "labelTitulo";
            labelTitulo.Size = new Size(184, 50);
            labelTitulo.TabIndex = 3;
            labelTitulo.Text = "SR Chat";
            // 
            // cbCOM
            // 
            cbCOM.FormattingEnabled = true;
            cbCOM.Items.AddRange(new object[] { "COM1", "COM2", "COM4", "COM5", "COM8", "COM9" });
            cbCOM.Location = new Point(312, 12);
            cbCOM.Margin = new Padding(3, 4, 3, 4);
            cbCOM.Name = "cbCOM";
            cbCOM.Size = new Size(114, 28);
            cbCOM.TabIndex = 4;
            cbCOM.Text = "COM1";
            cbCOM.SelectedIndexChanged += cbCOM_SelectedIndexChanged;
            // 
            // txtUsuario
            // 
            txtUsuario.Location = new Point(312, 53);
            txtUsuario.Margin = new Padding(3, 4, 3, 4);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(114, 27);
            txtUsuario.TabIndex = 5;
            // 
            // labelCOM
            // 
            labelCOM.AutoSize = true;
            labelCOM.Location = new Point(205, 16);
            labelCOM.Name = "labelCOM";
            labelCOM.Size = new Size(110, 20);
            labelCOM.TabIndex = 6;
            labelCOM.Text = "COM a utilizar: ";
            // 
            // labelUsuario
            // 
            labelUsuario.AutoSize = true;
            labelUsuario.Location = new Point(195, 57);
            labelUsuario.Name = "labelUsuario";
            labelUsuario.Size = new Size(119, 20);
            labelUsuario.TabIndex = 7;
            labelUsuario.Text = "Nombre usuario:";
            // 
            // labelVelocidad
            // 
            labelVelocidad.AutoSize = true;
            labelVelocidad.Location = new Point(14, 523);
            labelVelocidad.Name = "labelVelocidad";
            labelVelocidad.Size = new Size(120, 20);
            labelVelocidad.TabIndex = 9;
            labelVelocidad.Text = "Velocidad (bps): ";
            // 
            // cbVelocidad
            // 
            cbVelocidad.FormattingEnabled = true;
            cbVelocidad.Items.AddRange(new object[] { "1200", "4800", "9600", "19200", "128000", "460800" });
            cbVelocidad.Location = new Point(147, 517);
            cbVelocidad.Margin = new Padding(3, 4, 3, 4);
            cbVelocidad.Name = "cbVelocidad";
            cbVelocidad.Size = new Size(114, 28);
            cbVelocidad.TabIndex = 8;
            cbVelocidad.Text = "9600";
            cbVelocidad.SelectedIndexChanged += cbVelocidad_SelectedIndexChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(440, 564);
            Controls.Add(labelVelocidad);
            Controls.Add(cbVelocidad);
            Controls.Add(labelUsuario);
            Controls.Add(labelCOM);
            Controls.Add(txtUsuario);
            Controls.Add(cbCOM);
            Controls.Add(labelTitulo);
            Controls.Add(rchConversacion);
            Controls.Add(btnEnviaMensaje);
            Controls.Add(txtMensaje);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "SRChat";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtMensaje;
        private Button btnEnviaMensaje;
        private RichTextBox rchConversacion;
        private Label labelTitulo;
        private ComboBox cbCOM;
        private TextBox txtUsuario;
        private Label labelCOM;
        private Label labelUsuario;
        private Label labelVelocidad;
        private ComboBox cbVelocidad;
    }
}
