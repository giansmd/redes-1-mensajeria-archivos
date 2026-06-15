namespace winProyComunicacion
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
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
            pnlProgreso = new Panel();
            lblPorcentaje = new Label();
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

            // ══════════════════════════════════════════════════
            //  COLORES — modo claro por defecto
            //  AplicarTema() en Form1.cs se encarga de cambiar
            //  estos valores al alternar el modo oscuro.
            // ══════════════════════════════════════════════════

            // ══════════════════════════════════════════════════
            //  PANEL HEADER  — verde pizarra oscuro
            // ══════════════════════════════════════════════════
            panelHeader.BackColor = ClaroHeader;
            panelHeader.Controls.Add(labelTitulo);
            panelHeader.Controls.Add(panelConfig);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Padding = new Padding(14, 0, 14, 0);
            panelHeader.Size = new Size(620, 72);
            panelHeader.TabIndex = 0;

            // Título "SR Chat"
            labelTitulo.AutoSize = true;
            labelTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            labelTitulo.ForeColor = Color.White;
            labelTitulo.Location = new Point(56, 20);     // desplazado por btnModo
            labelTitulo.Name = "labelTitulo";
            labelTitulo.TabIndex = 3;
            labelTitulo.Text = "SR Chat";

            // ── panelConfig ────────────────────────────────────
            panelConfig.BackColor = Color.Transparent;
            panelConfig.Controls.Add(labelCOM);
            panelConfig.Controls.Add(cbCOM);
            panelConfig.Controls.Add(labelVelocidad);
            panelConfig.Controls.Add(cbVelocidad);
            panelConfig.Controls.Add(labelUsuario);
            panelConfig.Controls.Add(txtUsuario);
            panelConfig.Dock = DockStyle.Right;
            panelConfig.Name = "panelConfig";
            panelConfig.Size = new Size(400, 72);
            panelConfig.TabIndex = 0;

            // labelCOM
            labelCOM.AutoSize = true;
            labelCOM.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            labelCOM.ForeColor = Color.FromArgb(180, 230, 225);
            labelCOM.Location = new Point(4, 12);
            labelCOM.Name = "labelCOM";
            labelCOM.Text = "PUERTO";

            // cbCOM
            cbCOM.BackColor = ClaroComboBg;
            cbCOM.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCOM.FlatStyle = FlatStyle.Flat;
            cbCOM.ForeColor = Color.White;
            cbCOM.Font = new Font("Consolas", 10F, FontStyle.Bold);
            cbCOM.Location = new Point(4, 32);
            cbCOM.Name = "cbCOM";
            cbCOM.Size = new Size(98, 26);
            cbCOM.TabIndex = 4;
            cbCOM.SelectedIndexChanged += cbCOM_SelectedIndexChanged;

            // labelVelocidad
            labelVelocidad.AutoSize = true;
            labelVelocidad.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            labelVelocidad.ForeColor = Color.FromArgb(180, 230, 225);
            labelVelocidad.Location = new Point(112, 12);
            labelVelocidad.Name = "labelVelocidad";
            labelVelocidad.Text = "VEL. BPS";

            // cbVelocidad
            cbVelocidad.BackColor = ClaroComboBg;
            cbVelocidad.DropDownStyle = ComboBoxStyle.DropDownList;
            cbVelocidad.FlatStyle = FlatStyle.Flat;
            cbVelocidad.ForeColor = Color.White;
            cbVelocidad.Font = new Font("Consolas", 10F, FontStyle.Bold);
            cbVelocidad.Location = new Point(112, 32);
            cbVelocidad.Name = "cbVelocidad";
            cbVelocidad.Size = new Size(104, 26);
            cbVelocidad.TabIndex = 8;
            cbVelocidad.SelectedIndexChanged += cbVelocidad_SelectedIndexChanged;

            // labelUsuario
            labelUsuario.AutoSize = true;
            labelUsuario.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            labelUsuario.ForeColor = Color.FromArgb(180, 230, 225);
            labelUsuario.Location = new Point(226, 12);
            labelUsuario.Name = "labelUsuario";
            labelUsuario.Text = "NOMBRE";

            // txtUsuario
            txtUsuario.BackColor = ClaroComboBg;
            txtUsuario.BorderStyle = BorderStyle.FixedSingle;
            txtUsuario.ForeColor = Color.White;
            txtUsuario.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            txtUsuario.Location = new Point(226, 32);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.PlaceholderText = "Tu nombre";
            txtUsuario.Size = new Size(162, 26);
            txtUsuario.TabIndex = 5;

            // ══════════════════════════════════════════════════
            //  PANEL CHAT
            // ══════════════════════════════════════════════════
            panelChat.BackColor = ClaroChat;
            panelChat.Controls.Add(scrollChat);
            panelChat.Dock = DockStyle.Fill;
            panelChat.Location = new Point(0, 72);
            panelChat.Name = "panelChat";
            panelChat.Size = new Size(620, 380);
            panelChat.TabIndex = 1;

            scrollChat.AutoScroll = true;
            scrollChat.BackColor = ClaroChat;
            scrollChat.Dock = DockStyle.Fill;
            scrollChat.Name = "scrollChat";
            scrollChat.Padding = new Padding(12, 10, 12, 10);
            scrollChat.AutoScrollMinSize = new Size(0, 0);

            flowChat.Name = "flowChat";
            flowChat.Visible = false;
            rchConversacion.Visible = false;
            rchConversacion.Name = "rchConversacion";

            // ══════════════════════════════════════════════════
            //  PANEL INPUT
            // ══════════════════════════════════════════════════
            panelInput.BackColor = ClaroInput;
            panelInput.Controls.Add(txtMensaje);
            panelInput.Controls.Add(btnEnviaMensaje);
            panelInput.Dock = DockStyle.Bottom;
            panelInput.Name = "panelInput";
            panelInput.Padding = new Padding(12, 9, 12, 9);
            panelInput.Size = new Size(620, 56);
            panelInput.TabIndex = 2;

            txtMensaje.BackColor = ClaroTxtBg;
            txtMensaje.BorderStyle = BorderStyle.FixedSingle;
            txtMensaje.Dock = DockStyle.Fill;
            txtMensaje.Font = new Font("Segoe UI", 10F);
            txtMensaje.ForeColor = ClaroTxtFg;
            txtMensaje.Multiline = true;
            txtMensaje.Name = "txtMensaje";
            txtMensaje.Padding = new Padding(8, 6, 8, 6);
            txtMensaje.PlaceholderText = "Escribe un mensaje...";
            txtMensaje.Size = new Size(460, 38);
            txtMensaje.TabIndex = 0;
            txtMensaje.KeyPress += txtMensaje_KeyPress;

            // Botón Enviar — solo texto, sin Paint redundante
            btnEnviaMensaje.BackColor = ClaroEnviarBtn;
            btnEnviaMensaje.Dock = DockStyle.Right;
            btnEnviaMensaje.FlatStyle = FlatStyle.Flat;
            btnEnviaMensaje.FlatAppearance.BorderSize = 0;
            btnEnviaMensaje.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 180, 160);
            btnEnviaMensaje.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnEnviaMensaje.ForeColor = Color.White;
            btnEnviaMensaje.Name = "btnEnviaMensaje";
            btnEnviaMensaje.Size = new Size(120, 38);
            btnEnviaMensaje.TabIndex = 1;
            btnEnviaMensaje.Text = "Enviar ➤";
            btnEnviaMensaje.TextAlign = ContentAlignment.MiddleCenter;
            btnEnviaMensaje.UseVisualStyleBackColor = false;
            btnEnviaMensaje.Cursor = Cursors.Hand;
            btnEnviaMensaje.Click += btnEnviaMensaje_Click;

            // ══════════════════════════════════════════════════
            //  GROUP BOX ARCHIVOS
            // ══════════════════════════════════════════════════
            gbArchivos.BackColor = ClaroArchivos;
            gbArchivos.Controls.Add(pbRecepcionArchivo);
            gbArchivos.Controls.Add(pnlProgreso);
            gbArchivos.Controls.Add(lblPorcentaje);
            gbArchivos.Controls.Add(lblEstadoRecepcion);
            gbArchivos.Controls.Add(btnEnviarArchivos);
            gbArchivos.Controls.Add(lvArchivosSeleccionados);
            gbArchivos.Controls.Add(btnSeleccionarArchivos);
            gbArchivos.Dock = DockStyle.Bottom;
            gbArchivos.FlatStyle = FlatStyle.Flat;
            gbArchivos.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            gbArchivos.ForeColor = ClaroGbFg;
            gbArchivos.Name = "gbArchivos";
            gbArchivos.Padding = new Padding(12, 8, 12, 10);
            gbArchivos.Size = new Size(620, 140);
            gbArchivos.TabIndex = 10;
            gbArchivos.TabStop = false;
            gbArchivos.Text = "  📁  Transferencia de Archivos";

            // ── btnSeleccionarArchivos ─────────────────────────
            btnSeleccionarArchivos.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSeleccionarArchivos.BackColor = Color.FromArgb(33, 150, 243);
            btnSeleccionarArchivos.FlatStyle = FlatStyle.Flat;
            btnSeleccionarArchivos.FlatAppearance.BorderSize = 0;
            btnSeleccionarArchivos.FlatAppearance.MouseOverBackColor = Color.FromArgb(66, 165, 245);
            btnSeleccionarArchivos.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            btnSeleccionarArchivos.ForeColor = Color.White;
            btnSeleccionarArchivos.Location = new Point(488, 20);
            btnSeleccionarArchivos.Name = "btnSeleccionarArchivos";
            btnSeleccionarArchivos.Size = new Size(118, 30);
            btnSeleccionarArchivos.TabIndex = 0;
            btnSeleccionarArchivos.Text = "    Seleccionar";
            btnSeleccionarArchivos.UseVisualStyleBackColor = false;
            btnSeleccionarArchivos.Cursor = Cursors.Hand;
            btnSeleccionarArchivos.Click += btnSeleccionarArchivos_Click;
            btnSeleccionarArchivos.Paint += btnSeleccionarArchivos_Paint;

            // ── lvArchivosSeleccionados ────────────────────────
            lvArchivosSeleccionados.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lvArchivosSeleccionados.BackColor = ClaroLvBg;
            lvArchivosSeleccionados.BorderStyle = BorderStyle.FixedSingle;
            lvArchivosSeleccionados.Font = new Font("Consolas", 8.5F);
            lvArchivosSeleccionados.ForeColor = ClaroLvFg;
            lvArchivosSeleccionados.Location = new Point(12, 20);
            lvArchivosSeleccionados.Name = "lvArchivosSeleccionados";
            lvArchivosSeleccionados.Size = new Size(466, 66);
            lvArchivosSeleccionados.TabIndex = 1;
            lvArchivosSeleccionados.UseCompatibleStateImageBehavior = false;
            lvArchivosSeleccionados.View = View.Details;
            lvArchivosSeleccionados.FullRowSelect = true;

            // ── btnEnviarArchivos ──────────────────────────────
            btnEnviarArchivos.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnEnviarArchivos.BackColor = Color.FromArgb(67, 160, 71);
            btnEnviarArchivos.FlatStyle = FlatStyle.Flat;
            btnEnviarArchivos.FlatAppearance.BorderSize = 0;
            btnEnviarArchivos.FlatAppearance.MouseOverBackColor = Color.FromArgb(102, 187, 106);
            btnEnviarArchivos.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            btnEnviarArchivos.ForeColor = Color.White;
            btnEnviarArchivos.Location = new Point(488, 54);
            btnEnviarArchivos.Name = "btnEnviarArchivos";
            btnEnviarArchivos.Size = new Size(118, 30);
            btnEnviarArchivos.TabIndex = 2;
            btnEnviarArchivos.Text = "    Enviar arch.";
            btnEnviarArchivos.UseVisualStyleBackColor = false;
            btnEnviarArchivos.Cursor = Cursors.Hand;
            btnEnviarArchivos.Click += btnEnviarArchivos_Click;

            // lblEstadoRecepcion
            lblEstadoRecepcion.AutoSize = true;
            lblEstadoRecepcion.Font = new Font("Segoe UI", 7.5F);
            lblEstadoRecepcion.ForeColor = ClaroLabelFg;
            lblEstadoRecepcion.Location = new Point(12, 93);
            lblEstadoRecepcion.Name = "lblEstadoRecepcion";
            lblEstadoRecepcion.Text = "Recepción: esperando...";

            // pbRecepcionArchivo — panel con Paint custom (barra pill redondeada + porcentaje)
            // ── LAYOUT ZONA RECEPCIÓN ─────────────────────────
            // Área útil del GroupBox: 596px (620 - 12pad - 12pad)
            // Columna izquierda (estado + barra + manual): X=12..466
            // Columna derecha (botones 118px): X=490..608
            // Fila 1 – ListView:          Y=20, H=66  → bottom=86
            // Fila 2 – lblEstado:         Y=93
            // Fila 3 – barra + pct:       Y=108, H=18 → bottom=126
            // Fila 4 – manual inputs:     Y=138, H=22 → bottom=160
            // GroupBox total H = 178 (12pad-top + 160 + 6pad-bot)

            pbRecepcionArchivo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbRecepcionArchivo.BackColor = Color.FromArgb(210, 215, 213);
            pbRecepcionArchivo.ForeColor = ClaroEnviarBtn;
            pbRecepcionArchivo.Location = new Point(12, 108);
            pbRecepcionArchivo.Name = "pbRecepcionArchivo";
            pbRecepcionArchivo.Size = new Size(390, 18);
            pbRecepcionArchivo.Style = ProgressBarStyle.Continuous;
            pbRecepcionArchivo.TabIndex = 6;
            // Ocultamos el ProgressBar estándar; usamos pnlProgreso pintado a mano
            pbRecepcionArchivo.Visible = false;

            // pnlProgreso — 390px de ancho, deja 76px libres antes de los botones
            // Anchor: Left+Top (NO Right) para que no se extienda bajo los botones
            pnlProgreso.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            pnlProgreso.Location = new Point(12, 108);
            pnlProgreso.Name = "pnlProgreso";
            pnlProgreso.Size = new Size(390, 18);
            pnlProgreso.TabIndex = 20;
            pnlProgreso.BackColor = Color.Transparent;
            pnlProgreso.Tag = (int)0;  // porcentaje 0–100

            // lblPorcentaje — pegado a la derecha de la barra, centrado verticalmente
            lblPorcentaje.AutoSize = false;
            lblPorcentaje.Size = new Size(42, 18);
            lblPorcentaje.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblPorcentaje.ForeColor = ClaroLabelFg;
            lblPorcentaje.Location = new Point(406, 108);   // 12 + 390 + 4 = 406
            lblPorcentaje.TextAlign = ContentAlignment.MiddleLeft;
            lblPorcentaje.Name = "lblPorcentaje";
            lblPorcentaje.Text = "0%";
            lblPorcentaje.TabIndex = 21;

            // ══════════════════════════════════════════════════
            //  FORM
            // ══════════════════════════════════════════════════
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = ClaroChat;
            ClientSize = new Size(620, 680);
            MinimumSize = new Size(620, 560);
            Controls.Add(panelChat);
            Controls.Add(panelInput);
            Controls.Add(gbArchivos);
            Controls.Add(panelHeader);
            Font = new Font("Segoe UI", 9F);
            ForeColor = ClaroTxtFg;
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

        #endregion

        private Panel panelHeader;
        private Panel panelChat;
        private Panel panelInput;
        private Panel panelConfig;
        private Panel scrollChat;
        private Panel pnlProgreso;
        private Label lblPorcentaje;
        private FlowLayoutPanel flowChat;

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
    }
}