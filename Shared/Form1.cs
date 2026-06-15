using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace winProyComunicacion
{
    public partial class Form1 : Form
    {
        private const string DefaultRemoteUser = "Remoto";
        private static readonly int[] VelocidadesSoportadas = new[] { 900, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800 };

        [DllImport("user32.dll")]
        private static extern bool LockWindowUpdate(IntPtr hWnd);

        public static string? PuertoPreferido { get; set; }
        public static string DirectorioDescarga { get; set; } = "C:\\REDES1\\";

        ClassComunicacion Enlace;

        private delegate void AccedeControl(string mens);
        AccedeControl MuestraMensajeRCH;

        private delegate void AccedeControlProgreso(string nombreArchivo, long enviado, long total);
        AccedeControlProgreso MuestraProgresoArchivo;

        private bool PuertoSeleccionadoAutomatico = false;
        private Panel anclaScroll = new Panel();

        // ── Modo oscuro ──────────────────────────────────────────────
        private bool _modoOscuro = false;
        private Dictionary<int, ListViewItem> _itemsRecepcion = new Dictionary<int, ListViewItem>();

        // Colores modo claro
        private static readonly Color ClaroHeader = Color.FromArgb(0, 150, 136);
        private static readonly Color ClaroChat = Color.FromArgb(229, 221, 212);
        private static readonly Color ClaroInput = Color.FromArgb(245, 245, 245);
        private static readonly Color ClaroArchivos = Color.FromArgb(245, 245, 245);
        private static readonly Color ClaroTxtBg = Color.White;
        private static readonly Color ClaroTxtFg = Color.FromArgb(50, 50, 50);
        private static readonly Color ClaroComboBg = Color.FromArgb(0, 137, 123);
        private static readonly Color ClaroBurbPropio = Color.FromArgb(220, 248, 198);
        private static readonly Color ClaroBurbAjeno = Color.White;
        private static readonly Color ClaroBurbTexto = Color.FromArgb(50, 50, 50);
        private static readonly Color ClaroEnviarBtn = Color.FromArgb(0, 150, 136);
        private static readonly Color ClaroLvBg = Color.White;
        private static readonly Color ClaroLvFg = Color.FromArgb(50, 50, 50);
        private static readonly Color ClaroLabelFg = Color.FromArgb(100, 100, 100);
        private static readonly Color ClaroGbFg = Color.FromArgb(50, 50, 50);

        // Colores modo oscuro
        private static readonly Color OscuroHeader = Color.FromArgb(31, 44, 52);
        private static readonly Color OscuroChat = Color.FromArgb(11, 20, 26);
        private static readonly Color OscuroInput = Color.FromArgb(31, 44, 52);
        private static readonly Color OscuroArchivos = Color.FromArgb(17, 27, 33);
        private static readonly Color OscuroTxtBg = Color.FromArgb(42, 57, 66);
        private static readonly Color OscuroTxtFg = Color.FromArgb(233, 237, 239);
        private static readonly Color OscuroComboBg = Color.FromArgb(42, 57, 66);
        private static readonly Color OscuroBurbPropio = Color.FromArgb(0, 92, 75);
        private static readonly Color OscuroBurbAjeno = Color.FromArgb(31, 44, 52);
        private static readonly Color OscuroBurbTexto = Color.FromArgb(233, 237, 239);
        private static readonly Color OscuroEnviarBtn = Color.FromArgb(0, 168, 132);
        private static readonly Color OscuroLvBg = Color.FromArgb(31, 44, 52);
        private static readonly Color OscuroLvFg = Color.FromArgb(233, 237, 239);
        private static readonly Color OscuroLabelFg = Color.FromArgb(134, 150, 160);
        private static readonly Color OscuroGbFg = Color.FromArgb(134, 150, 160);

        private Button btnModo;
        // ─────────────────────────────────────────────────────────────

        public Form1()
        {
            InitializeComponent();
            Enlace = new ClassComunicacion();
            Enlace.DirectorioDescarga = DirectorioDescarga;
            MuestraMensajeRCH = new AccedeControl(MostrandoMensaje);
            MuestraProgresoArchivo = new AccedeControlProgreso(ActualizandoProgresoArchivo);

            lvArchivosSeleccionados.Columns.Add("Archivo", 140);
            lvArchivosSeleccionados.Columns.Add("Tamaño (bytes)", 80);
            lvArchivosSeleccionados.Columns.Add("Progreso", 70);

            AgregarBotonModo();
            AplicarBotonesRedondeados();

            anclaScroll.Size = new Size(1, 1);
            anclaScroll.BackColor = Color.Transparent;
            scrollChat.Controls.Add(anclaScroll);

            scrollChat.Resize += (s, e) =>
            {
                LockWindowUpdate(scrollChat.Handle);
                ReposicionarMensajes();
                LockWindowUpdate(IntPtr.Zero);
                IrAlUltimoMensaje();
            };
            scrollChat.Click += (s, e) => txtMensaje.Focus();
        }

        private void btnSeleccionarArchivos_Paint(object sender, PaintEventArgs e)
        {
            var btn = (Button)sender;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int x = 10, y = btn.Height / 2 - 7;
            Point[] arrow = {
                new Point(x,      y),
                new Point(x,      y + 14),
                new Point(x + 4,  y + 10),
                new Point(x + 7,  y + 15),
                new Point(x + 9,  y + 14),
                new Point(x + 6,  y + 9),
                new Point(x + 11, y + 9)
            };
            using (var brush = new SolidBrush(Color.White))
            {
                e.Graphics.FillPolygon(brush, arrow);
            }
        }

        private void AgregarBotonModo()
        {
            btnModo = new Button();
            btnModo.Text = "🌙";
            btnModo.Font = new Font("Segoe UI", 14F);
            btnModo.FlatStyle = FlatStyle.Flat;
            btnModo.FlatAppearance.BorderSize = 0;
            btnModo.BackColor = Color.Transparent;
            btnModo.ForeColor = Color.White;
            btnModo.Size = new Size(40, 36);
            btnModo.Cursor = Cursors.Hand;
            btnModo.Location = new Point(12, 10);
            btnModo.Click += BtnModo_Click;
            panelHeader.Controls.Add(btnModo);
            btnModo.BringToFront();

            labelTitulo.Location = new Point(56, 10);
        }

        private void BtnModo_Click(object sender, EventArgs e)
        {
            _modoOscuro = !_modoOscuro;
            btnModo.Text = _modoOscuro ? "☀️" : "🌙";
            AplicarTema();
        }

        private void AplicarTema()
        {
            bool d = _modoOscuro;

            panelHeader.BackColor = d ? OscuroHeader : ClaroHeader;
            cbCOM.BackColor = d ? OscuroComboBg : ClaroComboBg;
            cbVelocidad.BackColor = d ? OscuroComboBg : ClaroComboBg;
            txtUsuario.BackColor = d ? OscuroComboBg : ClaroComboBg;

            panelChat.BackColor = d ? OscuroChat : ClaroChat;
            scrollChat.BackColor = d ? OscuroChat : ClaroChat;

            panelInput.BackColor = d ? OscuroInput : ClaroInput;
            txtMensaje.BackColor = d ? OscuroTxtBg : ClaroTxtBg;
            txtMensaje.ForeColor = d ? OscuroTxtFg : ClaroTxtFg;
            btnEnviaMensaje.BackColor = d ? OscuroEnviarBtn : ClaroEnviarBtn;

            gbArchivos.BackColor = d ? OscuroArchivos : ClaroArchivos;
            gbArchivos.ForeColor = d ? OscuroGbFg : ClaroGbFg;
            lvArchivosSeleccionados.BackColor = d ? OscuroLvBg : ClaroLvBg;
            lvArchivosSeleccionados.ForeColor = d ? OscuroLvFg : ClaroLvFg;
            lblEstadoRecepcion.ForeColor = d ? OscuroLabelFg : ClaroLabelFg;
            pbRecepcionArchivo.BackColor = d ? OscuroInput : ClaroTxtBg;

            ActualizarColorBurbujas();
        }

        private void ActualizarColorBurbujas()
        {
            bool d = _modoOscuro;
            foreach (Control c in scrollChat.Controls)
            {
                if (c is Panel contenedor && contenedor.Tag is bool esPropio)
                {
                    Color colorFondo = esPropio
                        ? (d ? OscuroBurbPropio : ClaroBurbPropio)
                        : (d ? OscuroBurbAjeno : ClaroBurbAjeno);
                    Color colorTexto = d ? OscuroBurbTexto : ClaroBurbTexto;

                    foreach (Control hijo in contenedor.Controls)
                    {
                        if (hijo is Label lbl)
                        {
                            lbl.BackColor = colorFondo;
                            lbl.ForeColor = colorTexto;
                        }
                        else if (hijo is Panel cola && cola.Name.StartsWith("cola_"))
                        {
                            cola.Invalidate();
                        }
                        else if (hijo is Panel burb && burb.Name.StartsWith("burb_"))
                        {
                            burb.BackColor = colorFondo;
                            foreach (Control nieto in burb.Controls)
                            {
                                if (nieto is Label nieto_lbl)
                                {
                                    nieto_lbl.BackColor = colorFondo;
                                    nieto_lbl.ForeColor = colorTexto;
                                }
                                else if (nieto is PictureBox pb)
                                {
                                    pb.BackColor = colorFondo;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AplicarBotonesRedondeados()
        {
            RedondearBoton(btnEnviaMensaje, 20);
            RedondearBoton(btnSeleccionarArchivos, 15);
            RedondearBoton(btnEnviarArchivos, 15);
        }

        private void RedondearBoton(Button boton, int radio)
        {
            Rectangle rect = new Rectangle(0, 0, boton.Width, boton.Height);
            using GraphicsPath path = CrearRutaRedondeada(rect, radio);
            boton.Region = new Region(path);
        }

        private GraphicsPath CrearRutaRedondeada(Rectangle rect, int radio)
        {
            GraphicsPath path = new GraphicsPath();
            int diametro = radio * 2;
            Size size = new Size(diametro, diametro);
            Rectangle arc = new Rectangle(rect.Location, size);

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diametro;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diametro;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Enlace.LlegoMensaje += Enlace_llegoMensaje;
            Enlace.ProgresoEnvio += Enlace_ProgresoEnvio;
            Enlace.ArchivoEnvioCompletado += Enlace_ArchivoEnvioCompletado;
            Enlace.ArchivoRecibidoCompletado += Enlace_ArchivoRecibidoCompletado;
            Enlace.MetadatosRecibidos += Enlace_MetadatosRecibidos;
            Enlace.ProgresoRecepcion += Enlace_ProgresoRecepcion;
            Enlace.MetadatosRecibidosConIndice += Enlace_MetadatosRecibidosConIndice;
            Enlace.ProgresoRecepcionConIndice += Enlace_ProgresoRecepcionConIndice;
            CargarVelocidades();
            CargarPuertos();

            cbCOM.SelectedIndexChanged -= cbCOM_SelectedIndexChanged;
            SeleccionarPuertoPreferido();
            cbCOM.SelectedIndexChanged += cbCOM_SelectedIndexChanged;

            this.Text = string.IsNullOrEmpty(PuertoPreferido) ? "SRChat" : $"SRChat - {PuertoPreferido}";
            ActualizarConfiguracionPuerto();
        }

        private void Enlace_llegoMensaje(string m)
        {
            Invoke(MuestraMensajeRCH, m);
        }

        private void btnEnviaMensaje_Click(object sender, EventArgs e)
        {
            EnviarMensajeActual();
        }

        private void txtMensaje_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !e.Handled)
            {
                e.Handled = true;
                EnviarMensajeActual();
            }
        }

        private void EnviarMensajeActual()
        {
            if (!PuedeEnviar())
            {
                MessageBox.Show("Complete nombre, puerto COM y velocidad antes de enviar.");
                return;
            }

            var usuario = txtUsuario.Text.Trim();
            var mensaje = txtMensaje.Text.Trim();
            if (string.IsNullOrWhiteSpace(mensaje))
                return;

            Enlace.NombreUsuario = usuario;
            Enlace.EnviarMensaje($"{usuario}|{mensaje}");
            MostrarMensajeLocal(usuario, mensaje);
            txtMensaje.Clear();
            txtMensaje.Focus();
        }

        private void MostrandoMensaje(string mensaje)
        {
            var (usuario, texto) = SepararMensaje(mensaje);
            string usuarioLocal = txtUsuario.Text.Trim();
            bool esPropio = string.Equals(usuario, usuarioLocal, StringComparison.OrdinalIgnoreCase);
            AgregarMensajeFormateado(usuario, texto, esPropio);
        }

        private void MostrarMensajeLocal(string usuario, string mensaje)
        {
            AgregarMensajeFormateado(usuario, mensaje, true);
        }

        private void AgregarMensajeFormateado(string usuario, string mensaje, bool esPropio)
        {
            bool d = _modoOscuro;
            int anchoDisponible = scrollChat.ClientSize.Width - scrollChat.Padding.Horizontal;
            int anchoMax = (int)(anchoDisponible * 0.65);

            Label burbuja = new Label();
            burbuja.Text = mensaje;
            burbuja.Font = new Font("Segoe UI", 10F);
            burbuja.ForeColor = d ? OscuroBurbTexto : ClaroBurbTexto;
            burbuja.AutoSize = true;
            burbuja.MaximumSize = new Size(anchoMax, 0);
            burbuja.Padding = new Padding(12, 8, 12, 8);
            burbuja.BackColor = esPropio
                ? (d ? OscuroBurbPropio : ClaroBurbPropio)
                : (d ? OscuroBurbAjeno : ClaroBurbAjeno);
            burbuja.Tag = esPropio;

            Panel contenedor = new Panel();
            contenedor.BackColor = Color.Transparent;
            contenedor.Tag = esPropio;
            contenedor.Name = "cont_" + scrollChat.Controls.Count;

            Panel cola = new Panel();
            cola.Size = new Size(10, 12);
            cola.BackColor = Color.Transparent;
            cola.Tag = esPropio;
            cola.Name = "cola_" + scrollChat.Controls.Count;
            cola.Paint += (s, e) => DibujarCola(e.Graphics, cola.ClientRectangle, esPropio);

            contenedor.Controls.Add(burbuja);
            contenedor.Controls.Add(cola);

            scrollChat.Controls.Add(contenedor);
            ReposicionarMensajes();
            IrAlUltimoMensaje();
        }

        private void DibujarCola(Graphics g, Rectangle rect, bool esPropio)
        {
            bool d = _modoOscuro;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color color = esPropio
                ? (d ? OscuroBurbPropio : ClaroBurbPropio)
                : (d ? OscuroBurbAjeno : ClaroBurbAjeno);
            using SolidBrush brush = new SolidBrush(color);

            Point[] puntos;
            if (esPropio)
            {
                puntos = new Point[]
                {
                    new Point(0, 0),
                    new Point(rect.Width, rect.Height / 2),
                    new Point(0, rect.Height)
                };
            }
            else
            {
                puntos = new Point[]
                {
                    new Point(rect.Width, 0),
                    new Point(0, rect.Height / 2),
                    new Point(rect.Width, rect.Height)
                };
            }
            g.FillPolygon(brush, puntos);
        }

        private void ReposicionarMensajes()
        {
            scrollChat.SuspendLayout();

            int espacioEntre = 6;
            int anchoDisponible = scrollChat.ClientSize.Width - scrollChat.Padding.Horizontal;
            int anchoBurbuja = (int)(anchoDisponible * 0.65);

            foreach (Control c in scrollChat.Controls)
            {
                if (c is Panel contenedor && contenedor.Tag is bool esPropio)
                {
                    Label burbujaLabel = null;
                    Panel burbujaPanel = null;
                    Panel cola = null;
                    foreach (Control hijo in contenedor.Controls)
                    {
                        if (hijo is Label lbl) burbujaLabel = lbl;
                        else if (hijo is Panel pnl && pnl.Name.StartsWith("burb_")) burbujaPanel = pnl;
                        else if (hijo is Panel pnl2 && pnl2.Name.StartsWith("cola_")) cola = pnl2;
                    }

                    Control burbuja = (Control)burbujaLabel ?? burbujaPanel;
                    if (burbuja == null) continue;

                    contenedor.AutoSize = false;
                    int colaAncho = 10;

                    if (burbujaLabel != null)
                        burbujaLabel.MaximumSize = new Size(anchoBurbuja, 0);

                    int burbujaW = burbujaLabel != null ? burbujaLabel.PreferredSize.Width : burbuja.Width;
                    int burbujaH = burbujaLabel != null ? burbujaLabel.PreferredSize.Height : burbuja.Height;

                    if (cola != null)
                    {
                        cola.Size = new Size(colaAncho, 12);
                        if (esPropio)
                        {
                            burbuja.Location = new Point(0, 0);
                            cola.Location = new Point(burbujaW, 0);
                        }
                        else
                        {
                            cola.Location = new Point(0, 0);
                            burbuja.Location = new Point(colaAncho, 0);
                        }
                    }

                    contenedor.Size = new Size(burbujaW + colaAncho, burbujaH);
                }
            }

            int y = scrollChat.Padding.Top;
            foreach (Control c in scrollChat.Controls)
            {
                if (c is Panel contenedor && contenedor.Tag is bool esPropio)
                {
                    int x = esPropio
                        ? scrollChat.ClientSize.Width - contenedor.Width - scrollChat.Padding.Right
                        : scrollChat.Padding.Left;
                    contenedor.Location = new Point(x, y);
                    y = contenedor.Bottom + espacioEntre;
                }
            }

            int alturaTotal = y - espacioEntre + scrollChat.Padding.Bottom;
            anclaScroll.Location = new Point(0, alturaTotal);

            scrollChat.ResumeLayout();
        }

        private void IrAlUltimoMensaje()
        {
            scrollChat.AutoScrollPosition = new Point(0, int.MaxValue);
        }

        private (string Usuario, string Mensaje) SepararMensaje(string mensaje)
        {
            var partes = mensaje.Split('|', 2, StringSplitOptions.TrimEntries);
            if (partes.Length == 2 && !string.IsNullOrWhiteSpace(partes[0]))
                return (partes[0], partes[1]);

            return (DefaultRemoteUser, mensaje);
        }

        private void CargarPuertos()
        {
            cbCOM.Items.Clear();
            var puertos = System.IO.Ports.SerialPort.GetPortNames();
            Array.Sort(puertos, StringComparer.OrdinalIgnoreCase);
            cbCOM.Items.AddRange(puertos);

            if (cbCOM.Items.Count == 0)
            {
                cbCOM.Items.Add("(sin puertos)");
                cbCOM.SelectedIndex = 0;
            }
        }

        private void SeleccionarPuertoPreferido()
        {
            if (PuertoSeleccionadoAutomatico)
                return;

            string? preferido = PuertoPreferido;
            int indiceSeleccionado = -1;

            for (int i = 0; i < cbCOM.Items.Count; i++)
            {
                string item = cbCOM.Items[i]!.ToString()!;

                if (preferido != null && item == preferido)
                {
                    indiceSeleccionado = i;
                    break;
                }

                if (indiceSeleccionado == -1)
                    indiceSeleccionado = i;
            }

            if (indiceSeleccionado >= 0)
            {
                cbCOM.SelectedIndex = indiceSeleccionado;
                PuertoSeleccionadoAutomatico = true;
            }
            else if (cbCOM.Items.Count > 0)
            {
                cbCOM.SelectedIndex = 0;
                PuertoSeleccionadoAutomatico = true;
            }
        }

        private void CargarVelocidades()
        {
            cbVelocidad.Items.Clear();
            foreach (var velocidad in VelocidadesSoportadas)
                cbVelocidad.Items.Add(velocidad.ToString());

            if (cbVelocidad.Items.Count > 0)
                cbVelocidad.SelectedItem = "115200";
        }

        private void cbCOM_SelectedIndexChanged(object sender, EventArgs e)
        {
            LimpiarConversacion();
            ActualizarConfiguracionPuerto();
        }

        private void cbVelocidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarConfiguracionPuerto();
        }

        private void LimpiarConversacion()
        {
            scrollChat.Controls.Clear();
            txtUsuario.Clear();
            txtMensaje.Clear();
        }

        private void ActualizarConfiguracionPuerto()
        {
            if (!PuedeConfigurarPuerto())
                return;

            var nombrePuerto = cbCOM.SelectedItem?.ToString() ?? "";

            if (!int.TryParse(cbVelocidad.SelectedItem?.ToString(), out var velocidad))
                return;

            Enlace.InicializaPuerto(nombrePuerto, velocidad);
        }

        private bool PuedeConfigurarPuerto() =>
            cbCOM.SelectedItem is not null && cbVelocidad.SelectedItem is not null;

        private bool PuedeEnviar() =>
            !string.IsNullOrWhiteSpace(txtUsuario.Text)
            && cbCOM.SelectedItem is not null
            && cbVelocidad.SelectedItem is not null;

        private void btnSeleccionarArchivos_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialogo = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Seleccionar archivos para enviar",
                Filter = "Todos los archivos (*.*)|*.*"
            };

            if (dialogo.ShowDialog() == DialogResult.OK && dialogo.FileNames.Length > 0)
            {
                int enviosActivos = Enlace.ObtenerEnviosActivos();
                int slotsLibres = 5 - enviosActivos;

                if (slotsLibres <= 0)
                {
                    MessageBox.Show($"Ya hay {enviosActivos} archivos en envío. Espere a que terminen para seleccionar más.");
                    return;
                }

                if (dialogo.FileNames.Length > slotsLibres)
                {
                    MessageBox.Show($"Solo puede seleccionar {slotsLibres} archivo(s) nuevo(s). Se enviarán los primeros {slotsLibres}.");
                }

                // No limpiar la lista para mantener archivos en tránsito
                int limite = Math.Min(dialogo.FileNames.Length, slotsLibres);
                for (int i = 0; i < limite; i++)
                {
                    string ruta = dialogo.FileNames[i];
                    string nombre = Path.GetFileName(ruta);
                    long tamano = new FileInfo(ruta).Length;

                    // Verificar que no esté duplicado
                    bool existe = false;
                    foreach (ListViewItem existing in lvArchivosSeleccionados.Items)
                    {
                        if (existing.Tag as string == ruta)
                        {
                            existe = true;
                            break;
                        }
                    }

                    if (existe) continue;

                    ListViewItem item = new ListViewItem(nombre);
                    item.SubItems.Add(tamano.ToString());
                    item.SubItems.Add("Listo");
                    item.Tag = ruta;
                    item.BackColor = _modoOscuro
                        ? Color.FromArgb(42, 57, 66)
                        : Color.White;
                    lvArchivosSeleccionados.Items.Add(item);
                }

                ActualizarIndicadorArchivos();
            }
        }

        private void ActualizarIndicadorArchivos()
        {
            int activos = Enlace.ObtenerEnviosActivos();
            int total = lvArchivosSeleccionados.Items.Count;
            int listos = 0;

            foreach (ListViewItem item in lvArchivosSeleccionados.Items)
            {
                if (item.SubItems[2].Text == "Listo")
                    listos++;
            }

            gbArchivos.Text = $"  📁  Transferencia de Archivos ({activos}/5 enviando - {listos} listos)";
        }

        private void btnEnviarArchivos_Click(object sender, EventArgs e)
        {
            if (lvArchivosSeleccionados.Items.Count == 0)
            {
                MessageBox.Show("Primero seleccione archivos con \"SELECCIONAR...\".");
                return;
            }

            if (!PuedeEnviar())
            {
                MessageBox.Show("Complete nombre, puerto COM y velocidad antes de enviar.");
                return;
            }

            // Filtrar solo archivos en estado "Listo" para enviar
            var rutasListas = new List<string>();
            foreach (ListViewItem item in lvArchivosSeleccionados.Items)
            {
                if (item.SubItems[2].Text == "Listo")
                {
                    rutasListas.Add(item.Tag as string);
                }
            }

            if (rutasListas.Count == 0)
            {
                MessageBox.Show("No hay archivos listos para enviar. Todos ya están en envío o completados.");
                return;
            }

            int slotsLibres = 5 - Enlace.ObtenerEnviosActivos();
            if (rutasListas.Count > slotsLibres)
            {
                MessageBox.Show($"Solo hay {slotsLibres} slot(s) libre(s). Se enviarán los primeros {slotsLibres} archivos.");
                rutasListas = rutasListas.Take(slotsLibres).ToList();
            }

            // Marcar como pendientes solo los que se van a enviar
            int enviados = 0;
            foreach (ListViewItem item in lvArchivosSeleccionados.Items)
            {
                if (item.SubItems[2].Text == "Listo" && enviados < slotsLibres)
                {
                    item.SubItems[2].Text = "pendiente";
                    item.BackColor = _modoOscuro
                        ? Color.FromArgb(0, 92, 75)
                        : Color.FromArgb(220, 248, 198);
                    enviados++;
                }
            }

            Enlace.NombreUsuario = txtUsuario.Text.Trim();
            Enlace.EnviarArchivos(rutasListas.ToArray());
            ActualizarIndicadorArchivos();
        }

        private void Enlace_ProgresoEnvio(string nombreArchivo, long enviado, long total)
        {
            BeginInvoke(MuestraProgresoArchivo, nombreArchivo, enviado, total);
        }

        private void ActualizandoProgresoArchivo(string nombreArchivo, long enviado, long total)
        {
            foreach (ListViewItem item in lvArchivosSeleccionados.Items)
            {
                if (item.Text == nombreArchivo)
                {
                    int porcentaje = total > 0 ? (int)(enviado * 100 / total) : 100;
                    item.SubItems[2].Text = porcentaje + "%";
                    break;
                }
            }
        }

        private void Enlace_ArchivoEnvioCompletado(string nombreArchivo, string rutaArchivo)
        {
            BeginInvoke(() =>
            {
                foreach (ListViewItem item in lvArchivosSeleccionados.Items)
                {
                    if (item.Text == nombreArchivo)
                    {
                        item.SubItems[2].Text = "COMPLETADO";
                        item.BackColor = _modoOscuro
                            ? Color.FromArgb(0, 92, 75)
                            : Color.LightGreen;
                        break;
                    }
                }
                ActualizarIndicadorArchivos();
                AgregarBurbujaArchivo(txtUsuario.Text.Trim(), rutaArchivo, true);
            });
        }

        private void Enlace_ArchivoRecibidoCompletado(string usuario, string rutaArchivo)
        {
            BeginInvoke(() => AgregarBurbujaArchivo(usuario, rutaArchivo, false));
        }

        private void Enlace_MetadatosRecibidos(string nombreArchivo, long tamano)
        {
            BeginInvoke(() =>
            {
                lblEstadoRecepcion.Text = "Recibiendo: " + nombreArchivo + " (" + tamano + " bytes)";
                pbRecepcionArchivo.Value = 0;
                pbRecepcionArchivo.Maximum = 100;
            });
        }

        private void Enlace_ProgresoRecepcion(long recibido, long total)
        {
            BeginInvoke(() =>
            {
                if (total > 0)
                {
                    int porcentaje = (int)(recibido * 100 / total);
                    if (porcentaje > 100) porcentaje = 100;
                    pbRecepcionArchivo.Value = porcentaje;
                }

                if (recibido >= total && total > 0)
                    lblEstadoRecepcion.Text = "Recepción: completado ✓";
            });
        }

        private void Enlace_MetadatosRecibidosConIndice(int indice, string nombreArchivo, long tamano)
        {
            BeginInvoke(() =>
            {
                lblEstadoRecepcion.Text = "Recibiendo: " + nombreArchivo + " (" + tamano + " bytes)";
                pbRecepcionArchivo.Value = 0;
                pbRecepcionArchivo.Maximum = 100;

                ListViewItem item = new ListViewItem("← " + nombreArchivo);
                item.SubItems.Add(tamano.ToString());
                item.SubItems.Add("0%");
                item.Tag = indice;
                lvArchivosSeleccionados.Items.Add(item);
                _itemsRecepcion[indice] = item;
            });
        }

        private void Enlace_ProgresoRecepcionConIndice(int indice, string nombreArchivo, long recibido, long total)
        {
            BeginInvoke(() =>
            {
                if (_itemsRecepcion.TryGetValue(indice, out ListViewItem item))
                {
                    int porcentaje = total > 0 ? (int)(recibido * 100 / total) : 100;
                    if (porcentaje > 100) porcentaje = 100;
                    item.SubItems[2].Text = porcentaje + "%";

                    if (recibido >= total && total > 0)
                    {
                        item.SubItems[2].Text = "COMPLETADO ✓";
                        item.BackColor = _modoOscuro
                            ? Color.FromArgb(0, 92, 75)
                            : Color.LightGreen;
                        _itemsRecepcion.Remove(indice);
                    }
                }

                if (total > 0)
                {
                    int pct = (int)(recibido * 100 / total);
                    if (pct > 100) pct = 100;
                    pbRecepcionArchivo.Value = pct;
                }

                if (recibido >= total && total > 0)
                    lblEstadoRecepcion.Text = "Recepción: completado ✓";
            });
        }

        private void AgregarBurbujaArchivo(string usuario, string rutaArchivo, bool esPropio)
        {
            bool d = _modoOscuro;
            string nombreArchivo = Path.GetFileName(rutaArchivo);
            string ext = Path.GetExtension(rutaArchivo).ToLowerInvariant();

            Color colorFondo = esPropio
                ? (d ? OscuroBurbPropio : ClaroBurbPropio)
                : (d ? OscuroBurbAjeno : ClaroBurbAjeno);
            Color colorTexto = d ? OscuroBurbTexto : ClaroBurbTexto;

            bool esImagen = ext is ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp";
            bool esVideo  = ext is ".mp4" or ".avi" or ".mov" or ".mkv";
            bool esPdf    = ext == ".pdf";
            bool esDocx   = ext is ".docx" or ".doc";

            Panel burbujaPanel = new Panel();
            burbujaPanel.Name = "burb_" + scrollChat.Controls.Count;
            burbujaPanel.BackColor = colorFondo;
            burbujaPanel.Cursor = Cursors.Hand;

            if (esImagen && File.Exists(rutaArchivo))
            {
                PictureBox pb = new PictureBox();
                pb.Size = new Size(200, 140);
                pb.SizeMode = PictureBoxSizeMode.Zoom;
                pb.BackColor = colorFondo;
                pb.Location = new Point(8, 8);
                pb.Cursor = Cursors.Hand;
                try { pb.Image = Image.FromFile(rutaArchivo); }
                catch { }

                Label lblNombreImg = new Label();
                lblNombreImg.Text = nombreArchivo;
                lblNombreImg.Font = new Font("Segoe UI", 8F);
                lblNombreImg.ForeColor = colorTexto;
                lblNombreImg.BackColor = colorFondo;
                lblNombreImg.Size = new Size(200, 18);
                lblNombreImg.Location = new Point(8, pb.Bottom + 4);
                lblNombreImg.AutoEllipsis = true;

                burbujaPanel.Size = new Size(216, lblNombreImg.Bottom + 8);
                burbujaPanel.Controls.Add(pb);
                burbujaPanel.Controls.Add(lblNombreImg);
            }
            else
            {
                string icono = esVideo ? "🎬" : esPdf ? "📄" : esDocx ? "📝" : "📎";

                Label lblIcono = new Label();
                lblIcono.Text = icono;
                lblIcono.Font = new Font("Segoe UI", 20F);
                lblIcono.Size = new Size(44, 44);
                lblIcono.Location = new Point(8, 10);
                lblIcono.TextAlign = ContentAlignment.MiddleCenter;
                lblIcono.BackColor = colorFondo;
                lblIcono.ForeColor = colorTexto;

                Label lblNombre = new Label();
                lblNombre.Text = nombreArchivo;
                lblNombre.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                lblNombre.ForeColor = colorTexto;
                lblNombre.BackColor = colorFondo;
                lblNombre.Size = new Size(160, 38);
                lblNombre.Location = new Point(lblIcono.Right + 6, 8);
                lblNombre.AutoEllipsis = true;

                Label lblAbrir = new Label();
                lblAbrir.Text = "Click para abrir";
                lblAbrir.Font = new Font("Segoe UI", 7.5F, FontStyle.Italic);
                lblAbrir.ForeColor = Color.FromArgb(160, colorTexto);
                lblAbrir.BackColor = colorFondo;
                lblAbrir.AutoSize = true;
                lblAbrir.Location = new Point(lblIcono.Right + 6, lblNombre.Bottom + 2);

                burbujaPanel.Size = new Size(230, 70);
                burbujaPanel.Controls.Add(lblIcono);
                burbujaPanel.Controls.Add(lblNombre);
                burbujaPanel.Controls.Add(lblAbrir);
            }

            EventHandler abrirArchivo = (s, ev) =>
            {
                if (File.Exists(rutaArchivo))
                {
                    try { Process.Start(new ProcessStartInfo(rutaArchivo) { UseShellExecute = true }); }
                    catch (Exception ex) { MessageBox.Show("No se pudo abrir el archivo:\n" + ex.Message); }
                }
                else
                {
                    MessageBox.Show("El archivo ya no está disponible:\n" + rutaArchivo);
                }
            };

            burbujaPanel.Click += abrirArchivo;
            foreach (Control hijo in burbujaPanel.Controls)
                hijo.Click += abrirArchivo;

            Panel contenedor = new Panel();
            contenedor.BackColor = Color.Transparent;
            contenedor.Tag = esPropio;
            contenedor.Name = "cont_" + scrollChat.Controls.Count;

            Panel cola = new Panel();
            cola.Size = new Size(10, 12);
            cola.BackColor = Color.Transparent;
            cola.Tag = esPropio;
            cola.Name = "cola_" + scrollChat.Controls.Count;
            cola.Paint += (s, e) => DibujarCola(e.Graphics, cola.ClientRectangle, esPropio);

            contenedor.Controls.Add(burbujaPanel);
            contenedor.Controls.Add(cola);

            scrollChat.Controls.Add(contenedor);
            ReposicionarMensajes();
            IrAlUltimoMensaje();
        }
    }
}