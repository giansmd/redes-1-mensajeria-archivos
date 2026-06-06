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
            gbArchivos = new GroupBox();
            labelArchivoEnvio = new Label();
            txtArchivoEnvio = new TextBox();
            btnAbrirArchivo = new Button();
            btnEnviarArchivo = new Button();
            labelRecibirArchivo = new Label();
            txtTamanoArchivo = new TextBox();
            labelBytes = new Label();
            btnCrearArchivo = new Button();
            gbArchivos.SuspendLayout();
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
            // gbArchivos
            // 
            gbArchivos.Controls.Add(btnCrearArchivo);
            gbArchivos.Controls.Add(labelBytes);
            gbArchivos.Controls.Add(txtTamanoArchivo);
            gbArchivos.Controls.Add(labelRecibirArchivo);
            gbArchivos.Controls.Add(btnEnviarArchivo);
            gbArchivos.Controls.Add(btnAbrirArchivo);
            gbArchivos.Controls.Add(txtArchivoEnvio);
            gbArchivos.Controls.Add(labelArchivoEnvio);
            gbArchivos.Location = new Point(14, 557);
            gbArchivos.Margin = new Padding(3, 4, 3, 4);
            gbArchivos.Name = "gbArchivos";
            gbArchivos.Padding = new Padding(3, 4, 3, 4);
            gbArchivos.Size = new Size(412, 125);
            gbArchivos.TabIndex = 10;
            gbArchivos.TabStop = false;
            gbArchivos.Text = "Transferencia de Archivos";
            // 
            // labelArchivoEnvio
            // 
            labelArchivoEnvio.AutoSize = true;
            labelArchivoEnvio.Location = new Point(10, 25);
            labelArchivoEnvio.Name = "labelArchivoEnvio";
            labelArchivoEnvio.Size = new Size(118, 20);
            labelArchivoEnvio.TabIndex = 0;
            labelArchivoEnvio.Text = "Archivo a enviar:";
            // 
            // txtArchivoEnvio
            // 
            txtArchivoEnvio.Location = new Point(130, 22);
            txtArchivoEnvio.Margin = new Padding(3, 4, 3, 4);
            txtArchivoEnvio.Name = "txtArchivoEnvio";
            txtArchivoEnvio.Size = new Size(170, 27);
            txtArchivoEnvio.TabIndex = 1;
            // 
            // btnAbrirArchivo
            // 
            btnAbrirArchivo.Location = new Point(310, 20);
            btnAbrirArchivo.Margin = new Padding(3, 4, 3, 4);
            btnAbrirArchivo.Name = "btnAbrirArchivo";
            btnAbrirArchivo.Size = new Size(92, 28);
            btnAbrirArchivo.TabIndex = 2;
            btnAbrirArchivo.Text = "ABRIR ARCHIVO";
            btnAbrirArchivo.UseVisualStyleBackColor = true;
            btnAbrirArchivo.Click += btnAbrirArchivo_Click;
            // 
            // btnEnviarArchivo
            // 
            btnEnviarArchivo.Location = new Point(310, 55);
            btnEnviarArchivo.Margin = new Padding(3, 4, 3, 4);
            btnEnviarArchivo.Name = "btnEnviarArchivo";
            btnEnviarArchivo.Size = new Size(92, 28);
            btnEnviarArchivo.TabIndex = 3;
            btnEnviarArchivo.Text = "ENVIAR ARCHIVO";
            btnEnviarArchivo.UseVisualStyleBackColor = true;
            btnEnviarArchivo.Click += btnEnviarArchivo_Click;
            // 
            // labelRecibirArchivo
            // 
            labelRecibirArchivo.AutoSize = true;
            labelRecibirArchivo.Location = new Point(10, 65);
            labelRecibirArchivo.Name = "labelRecibirArchivo";
            labelRecibirArchivo.Size = new Size(114, 20);
            labelRecibirArchivo.TabIndex = 4;
            labelRecibirArchivo.Text = "Recibir archivo:";
            // 
            // txtTamanoArchivo
            // 
            txtTamanoArchivo.Location = new Point(130, 62);
            txtTamanoArchivo.Margin = new Padding(3, 4, 3, 4);
            txtTamanoArchivo.Name = "txtTamanoArchivo";
            txtTamanoArchivo.Size = new Size(105, 27);
            txtTamanoArchivo.TabIndex = 5;
            // 
            // labelBytes
            // 
            labelBytes.AutoSize = true;
            labelBytes.Location = new Point(240, 65);
            labelBytes.Name = "labelBytes";
            labelBytes.Size = new Size(42, 20);
            labelBytes.TabIndex = 6;
            labelBytes.Text = "bytes";
            // 
            // btnCrearArchivo
            // 
            btnCrearArchivo.Location = new Point(310, 90);
            btnCrearArchivo.Margin = new Padding(3, 4, 3, 4);
            btnCrearArchivo.Name = "btnCrearArchivo";
            btnCrearArchivo.Size = new Size(92, 28);
            btnCrearArchivo.TabIndex = 7;
            btnCrearArchivo.Text = "CREAR ARCHIVO";
            btnCrearArchivo.UseVisualStyleBackColor = true;
            btnCrearArchivo.Click += btnCrearArchivo_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(440, 695);
            Controls.Add(gbArchivos);
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
            gbArchivos.ResumeLayout(false);
            gbArchivos.PerformLayout();
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
        private GroupBox gbArchivos;
        private Label labelArchivoEnvio;
        private TextBox txtArchivoEnvio;
        private Button btnAbrirArchivo;
        private Button btnEnviarArchivo;
        private Label labelRecibirArchivo;
        private TextBox txtTamanoArchivo;
        private Label labelBytes;
        private Button btnCrearArchivo;
    }
}
