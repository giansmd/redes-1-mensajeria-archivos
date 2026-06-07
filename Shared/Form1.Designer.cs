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
            flowChat = new FlowLayoutPanel();
            scrollChat = new Panel();
            labelTitulo = new Label();
            cbCOM = new ComboBox();
            txtUsuario = new TextBox();
            labelCOM = new Label();
            labelUsuario = new Label();
            labelVelocidad = new Label();
            cbVelocidad = new ComboBox();
            gbArchivos = new GroupBox();
            btnSeleccionarArchivos = new Button();
            lvArchivosSeleccionados = new ListView();
            btnEnviarArchivos = new Button();
            lblEstadoRecepcion = new Label();
            pbRecepcionArchivo = new ProgressBar();
            lblArchivoRecepcion = new Label();
            txtArchivoRecepcion = new TextBox();
            txtTamanoArchivo = new TextBox();
            labelBytes = new Label();
            btnCrearArchivo = new Button();
            panelHeader = new Panel();
            panelChat = new Panel();
            panelInput = new Panel();
            panelConfig = new Panel();
            gbArchivos.SuspendLayout();
            panelHeader.SuspendLayout();
            panelChat.SuspendLayout();
            panelInput.SuspendLayout();
            panelConfig.SuspendLayout();
            scrollChat.SuspendLayout();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(0, 150, 136);
            panelHeader.Controls.Add(labelTitulo);
            panelHeader.Controls.Add(panelConfig);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Padding = new Padding(12, 8, 12, 8);
            panelHeader.Size = new Size(600, 55);
            panelHeader.TabIndex = 0;
            // 
            // labelTitulo
            // 
            labelTitulo.AutoSize = true;
            labelTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            labelTitulo.ForeColor = Color.White;
            labelTitulo.Location = new Point(12, 10);
            labelTitulo.Name = "labelTitulo";
            labelTitulo.Size = new Size(100, 30);
            labelTitulo.TabIndex = 3;
            labelTitulo.Text = "SR Chat";
            // 
            // panelConfig
            // 
            panelConfig.BackColor = Color.Transparent;
            panelConfig.Controls.Add(labelCOM);
            panelConfig.Controls.Add(cbCOM);
            panelConfig.Controls.Add(labelVelocidad);
            panelConfig.Controls.Add(cbVelocidad);
            panelConfig.Controls.Add(labelUsuario);
            panelConfig.Controls.Add(txtUsuario);
            panelConfig.Dock = DockStyle.Right;
            panelConfig.Location = new Point(160, 8);
            panelConfig.Name = "panelConfig";
            panelConfig.Size = new Size(428, 39);
            panelConfig.TabIndex = 0;
            // 
            // labelCOM
            // 
            labelCOM.AutoSize = true;
            labelCOM.ForeColor = Color.White;
            labelCOM.Font = new Font("Segoe UI", 8F);
            labelCOM.Location = new Point(2, 3);
            labelCOM.Name = "labelCOM";
            labelCOM.Size = new Size(35, 13);
            labelCOM.TabIndex = 6;
            labelCOM.Text = "Puerto";
            // 
            // cbCOM
            // 
            cbCOM.BackColor = Color.FromArgb(0, 137, 123);
            cbCOM.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCOM.FlatStyle = FlatStyle.Flat;
            cbCOM.ForeColor = Color.White;
            cbCOM.Font = new Font("Segoe UI", 9F);
            cbCOM.Location = new Point(2, 18);
            cbCOM.Name = "cbCOM";
            cbCOM.Size = new Size(85, 23);
            cbCOM.TabIndex = 4;
            cbCOM.SelectedIndexChanged += cbCOM_SelectedIndexChanged;
            // 
            // labelVelocidad
            // 
            labelVelocidad.AutoSize = true;
            labelVelocidad.ForeColor = Color.White;
            labelVelocidad.Font = new Font("Segoe UI", 8F);
            labelVelocidad.Location = new Point(95, 3);
            labelVelocidad.Name = "labelVelocidad";
            labelVelocidad.Size = new Size(48, 13);
            labelVelocidad.TabIndex = 9;
            labelVelocidad.Text = "Vel. bps";
            // 
            // cbVelocidad
            // 
            cbVelocidad.BackColor = Color.FromArgb(0, 137, 123);
            cbVelocidad.DropDownStyle = ComboBoxStyle.DropDownList;
            cbVelocidad.FlatStyle = FlatStyle.Flat;
            cbVelocidad.ForeColor = Color.White;
            cbVelocidad.Font = new Font("Segoe UI", 9F);
            cbVelocidad.Location = new Point(95, 18);
            cbVelocidad.Name = "cbVelocidad";
            cbVelocidad.Size = new Size(85, 23);
            cbVelocidad.TabIndex = 8;
            cbVelocidad.SelectedIndexChanged += cbVelocidad_SelectedIndexChanged;
            // 
            // labelUsuario
            // 
            labelUsuario.AutoSize = true;
            labelUsuario.ForeColor = Color.White;
            labelUsuario.Font = new Font("Segoe UI", 8F);
            labelUsuario.Location = new Point(190, 3);
            labelUsuario.Name = "labelUsuario";
            labelUsuario.Size = new Size(46, 13);
            labelUsuario.TabIndex = 7;
            labelUsuario.Text = "Nombre";
            // 
            // txtUsuario
            // 
            txtUsuario.BackColor = Color.FromArgb(0, 137, 123);
            txtUsuario.BorderStyle = BorderStyle.FixedSingle;
            txtUsuario.ForeColor = Color.White;
            txtUsuario.Font = new Font("Segoe UI", 9F);
            txtUsuario.Location = new Point(190, 18);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.PlaceholderText = "Tu nombre";
            txtUsuario.Size = new Size(120, 23);
            txtUsuario.TabIndex = 5;
            // 
            // panelChat
            // 
            panelChat.BackColor = Color.FromArgb(229, 221, 212);
            panelChat.Controls.Add(scrollChat);
            panelChat.Dock = DockStyle.Fill;
            panelChat.Location = new Point(0, 55);
            panelChat.Name = "panelChat";
            panelChat.Padding = new Padding(0);
            panelChat.Size = new Size(600, 380);
            panelChat.TabIndex = 1;
            // 
            // scrollChat
            // 
            scrollChat.AutoScroll = true;
            scrollChat.BackColor = Color.FromArgb(229, 221, 212);
            scrollChat.Dock = DockStyle.Fill;
            scrollChat.Name = "scrollChat";
            scrollChat.Padding = new Padding(10, 8, 10, 8);
            scrollChat.Size = new Size(600, 380);
            scrollChat.AutoScrollMinSize = new Size(0, 0);
            // 
            // flowChat
            // 
            flowChat.Name = "flowChat";
            flowChat.Visible = false;
            // 
            // rchConversacion
            // 
            rchConversacion.Visible = false;
            rchConversacion.Name = "rchConversacion";
            // 
            // panelInput
            // 
            panelInput.BackColor = Color.FromArgb(245, 245, 245);
            panelInput.Controls.Add(txtMensaje);
            panelInput.Controls.Add(btnEnviaMensaje);
            panelInput.Dock = DockStyle.Bottom;
            panelInput.Location = new Point(0, 435);
            panelInput.Name = "panelInput";
            panelInput.Padding = new Padding(10, 8, 10, 8);
            panelInput.Size = new Size(600, 50);
            panelInput.TabIndex = 2;
            // 
            // txtMensaje
            // 
            txtMensaje.BackColor = Color.White;
            txtMensaje.BorderStyle = BorderStyle.FixedSingle;
            txtMensaje.Dock = DockStyle.Fill;
            txtMensaje.Font = new Font("Segoe UI", 10F);
            txtMensaje.ForeColor = Color.FromArgb(50, 50, 50);
            txtMensaje.Location = new Point(10, 8);
            txtMensaje.Multiline = true;
            txtMensaje.Name = "txtMensaje";
            txtMensaje.Padding = new Padding(8, 6, 8, 6);
            txtMensaje.PlaceholderText = "Escribe un mensaje...";
            txtMensaje.Size = new Size(460, 34);
            txtMensaje.TabIndex = 0;
            txtMensaje.KeyPress += txtMensaje_KeyPress;
            // 
            // btnEnviaMensaje
            // 
            btnEnviaMensaje.BackColor = Color.FromArgb(0, 150, 136);
            btnEnviaMensaje.Dock = DockStyle.Right;
            btnEnviaMensaje.FlatStyle = FlatStyle.Flat;
            btnEnviaMensaje.FlatAppearance.BorderSize = 0;
            btnEnviaMensaje.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnEnviaMensaje.ForeColor = Color.White;
            btnEnviaMensaje.Location = new Point(470, 8);
            btnEnviaMensaje.Name = "btnEnviaMensaje";
            btnEnviaMensaje.Size = new Size(120, 34);
            btnEnviaMensaje.TabIndex = 1;
            btnEnviaMensaje.Text = "Enviar";
            btnEnviaMensaje.UseVisualStyleBackColor = false;
            btnEnviaMensaje.Cursor = Cursors.Hand;
            btnEnviaMensaje.Click += btnEnviaMensaje_Click;
            // 
            // gbArchivos
            // 
            gbArchivos.BackColor = Color.FromArgb(245, 245, 245);
            gbArchivos.Controls.Add(pbRecepcionArchivo);
            gbArchivos.Controls.Add(lblEstadoRecepcion);
            gbArchivos.Controls.Add(btnEnviarArchivos);
            gbArchivos.Controls.Add(lvArchivosSeleccionados);
            gbArchivos.Controls.Add(btnSeleccionarArchivos);
            gbArchivos.Controls.Add(btnCrearArchivo);
            gbArchivos.Controls.Add(labelBytes);
            gbArchivos.Controls.Add(txtTamanoArchivo);
            gbArchivos.Controls.Add(txtArchivoRecepcion);
            gbArchivos.Controls.Add(lblArchivoRecepcion);
            gbArchivos.Dock = DockStyle.Bottom;
            gbArchivos.FlatStyle = FlatStyle.Flat;
            gbArchivos.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            gbArchivos.ForeColor = Color.FromArgb(50, 50, 50);
            gbArchivos.Location = new Point(0, 485);
            gbArchivos.Name = "gbArchivos";
            gbArchivos.Padding = new Padding(12, 6, 12, 10);
            gbArchivos.Size = new Size(600, 175);
            gbArchivos.TabIndex = 10;
            gbArchivos.TabStop = false;
            gbArchivos.Text = "Transferencia de Archivos";
            // 
            // btnSeleccionarArchivos
            // 
            btnSeleccionarArchivos.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSeleccionarArchivos.BackColor = Color.FromArgb(33, 150, 243);
            btnSeleccionarArchivos.FlatStyle = FlatStyle.Flat;
            btnSeleccionarArchivos.FlatAppearance.BorderSize = 0;
            btnSeleccionarArchivos.Font = new Font("Segoe UI", 7F, FontStyle.Bold);
            btnSeleccionarArchivos.ForeColor = Color.White;
            btnSeleccionarArchivos.Location = new Point(470, 20);
            btnSeleccionarArchivos.Name = "btnSeleccionarArchivos";
            btnSeleccionarArchivos.Size = new Size(118, 26);
            btnSeleccionarArchivos.TabIndex = 0;
            btnSeleccionarArchivos.Text = "SELECCIONAR...";
            btnSeleccionarArchivos.UseVisualStyleBackColor = false;
            btnSeleccionarArchivos.Cursor = Cursors.Hand;
            btnSeleccionarArchivos.Click += btnSeleccionarArchivos_Click;
            // 
            // lvArchivosSeleccionados
            // 
            lvArchivosSeleccionados.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lvArchivosSeleccionados.BackColor = Color.White;
            lvArchivosSeleccionados.BorderStyle = BorderStyle.FixedSingle;
            lvArchivosSeleccionados.Font = new Font("Segoe UI", 8F);
            lvArchivosSeleccionados.ForeColor = Color.FromArgb(50, 50, 50);
            lvArchivosSeleccionados.Location = new Point(12, 20);
            lvArchivosSeleccionados.Name = "lvArchivosSeleccionados";
            lvArchivosSeleccionados.Size = new Size(448, 65);
            lvArchivosSeleccionados.TabIndex = 1;
            lvArchivosSeleccionados.UseCompatibleStateImageBehavior = false;
            lvArchivosSeleccionados.View = View.Details;
            lvArchivosSeleccionados.FullRowSelect = true;
            // 
            // btnEnviarArchivos
            // 
            btnEnviarArchivos.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnEnviarArchivos.BackColor = Color.FromArgb(76, 175, 80);
            btnEnviarArchivos.FlatStyle = FlatStyle.Flat;
            btnEnviarArchivos.FlatAppearance.BorderSize = 0;
            btnEnviarArchivos.Font = new Font("Segoe UI", 7F, FontStyle.Bold);
            btnEnviarArchivos.ForeColor = Color.White;
            btnEnviarArchivos.Location = new Point(470, 50);
            btnEnviarArchivos.Name = "btnEnviarArchivos";
            btnEnviarArchivos.Size = new Size(118, 26);
            btnEnviarArchivos.TabIndex = 2;
            btnEnviarArchivos.Text = "ENVIAR ARCHIVOS";
            btnEnviarArchivos.UseVisualStyleBackColor = false;
            btnEnviarArchivos.Cursor = Cursors.Hand;
            btnEnviarArchivos.Click += btnEnviarArchivos_Click;
            // 
            // lblEstadoRecepcion
            // 
            lblEstadoRecepcion.AutoSize = true;
            lblEstadoRecepcion.Font = new Font("Segoe UI", 7F);
            lblEstadoRecepcion.ForeColor = Color.FromArgb(100, 100, 100);
            lblEstadoRecepcion.Location = new Point(12, 93);
            lblEstadoRecepcion.Name = "lblEstadoRecepcion";
            lblEstadoRecepcion.Size = new Size(105, 12);
            lblEstadoRecepcion.TabIndex = 5;
            lblEstadoRecepcion.Text = "Recepción: esperando...";
            // 
            // pbRecepcionArchivo
            // 
            pbRecepcionArchivo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbRecepcionArchivo.Location = new Point(12, 108);
            pbRecepcionArchivo.Name = "pbRecepcionArchivo";
            pbRecepcionArchivo.Size = new Size(576, 14);
            pbRecepcionArchivo.TabIndex = 6;
            // 
            // lblArchivoRecepcion
            // 
            lblArchivoRecepcion.AutoSize = true;
            lblArchivoRecepcion.Font = new Font("Segoe UI", 7F);
            lblArchivoRecepcion.ForeColor = Color.FromArgb(100, 100, 100);
            lblArchivoRecepcion.Location = new Point(12, 128);
            lblArchivoRecepcion.Name = "lblArchivoRecepcion";
            lblArchivoRecepcion.Size = new Size(90, 12);
            lblArchivoRecepcion.TabIndex = 8;
            lblArchivoRecepcion.Text = "Recepción manual:";
            // 
            // txtArchivoRecepcion
            // 
            txtArchivoRecepcion.BackColor = Color.White;
            txtArchivoRecepcion.BorderStyle = BorderStyle.FixedSingle;
            txtArchivoRecepcion.Font = new Font("Segoe UI", 8F);
            txtArchivoRecepcion.ForeColor = Color.FromArgb(50, 50, 50);
            txtArchivoRecepcion.Location = new Point(108, 125);
            txtArchivoRecepcion.Name = "txtArchivoRecepcion";
            txtArchivoRecepcion.PlaceholderText = "nombre archivo";
            txtArchivoRecepcion.Size = new Size(160, 22);
            txtArchivoRecepcion.TabIndex = 9;
            // 
            // txtTamanoArchivo
            // 
            txtTamanoArchivo.BackColor = Color.White;
            txtTamanoArchivo.BorderStyle = BorderStyle.FixedSingle;
            txtTamanoArchivo.Font = new Font("Segoe UI", 8F);
            txtTamanoArchivo.ForeColor = Color.FromArgb(50, 50, 50);
            txtTamanoArchivo.Location = new Point(275, 125);
            txtTamanoArchivo.Name = "txtTamanoArchivo";
            txtTamanoArchivo.PlaceholderText = "tamaño";
            txtTamanoArchivo.Size = new Size(90, 22);
            txtTamanoArchivo.TabIndex = 10;
            // 
            // labelBytes
            // 
            labelBytes.AutoSize = true;
            labelBytes.Font = new Font("Segoe UI", 7F);
            labelBytes.ForeColor = Color.FromArgb(100, 100, 100);
            labelBytes.Location = new Point(370, 128);
            labelBytes.Name = "labelBytes";
            labelBytes.Size = new Size(28, 12);
            labelBytes.TabIndex = 11;
            labelBytes.Text = "bytes";
            // 
            // btnCrearArchivo
            // 
            btnCrearArchivo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCrearArchivo.BackColor = Color.FromArgb(255, 152, 0);
            btnCrearArchivo.FlatStyle = FlatStyle.Flat;
            btnCrearArchivo.FlatAppearance.BorderSize = 0;
            btnCrearArchivo.Font = new Font("Segoe UI", 7F, FontStyle.Bold);
            btnCrearArchivo.ForeColor = Color.White;
            btnCrearArchivo.Location = new Point(470, 124);
            btnCrearArchivo.Name = "btnCrearArchivo";
            btnCrearArchivo.Size = new Size(118, 24);
            btnCrearArchivo.TabIndex = 12;
            btnCrearArchivo.Text = "CREAR ARCHIVO";
            btnCrearArchivo.UseVisualStyleBackColor = false;
            btnCrearArchivo.Cursor = Cursors.Hand;
            btnCrearArchivo.Click += btnCrearArchivo_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(229, 221, 212);
            ClientSize = new Size(600, 660);
            MinimumSize = new Size(600, 550);
            Controls.Add(panelChat);
            Controls.Add(panelInput);
            Controls.Add(gbArchivos);
            Controls.Add(panelHeader);
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.FromArgb(50, 50, 50);
            Name = "Form1";
            Text = "SRChat";
            Load += Form1_Load;
            gbArchivos.ResumeLayout(false);
            gbArchivos.PerformLayout();
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelChat.ResumeLayout(false);
            panelInput.ResumeLayout(false);
            panelInput.PerformLayout();
            panelConfig.ResumeLayout(false);
            panelConfig.PerformLayout();
            scrollChat.ResumeLayout(false);
            scrollChat.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private Panel panelHeader;
        private Panel panelChat;
        private Panel panelInput;
        private Panel panelConfig;
        private Panel scrollChat;
        private FlowLayoutPanel flowChat;

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
        private Button btnSeleccionarArchivos;
        private ListView lvArchivosSeleccionados;
        private Button btnEnviarArchivos;
        private Label lblEstadoRecepcion;
        private ProgressBar pbRecepcionArchivo;
        private Label lblArchivoRecepcion;
        private TextBox txtArchivoRecepcion;
        private TextBox txtTamanoArchivo;
        private Label labelBytes;
        private Button btnCrearArchivo;
    }
}
