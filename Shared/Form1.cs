using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace winProyComunicacion
{
    public partial class Form1 : Form
    {
        private const string DefaultRemoteUser = "Remoto";
        private static readonly int[] VelocidadesSoportadas = [900, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800];

        [DllImport("user32.dll")]
        private static extern bool LockWindowUpdate(IntPtr hWnd);

        public static string? PuertoPreferido { get; set; }

        ClassComunicacion Enlace;

        private delegate void AccedeControl(string mens);
        AccedeControl MuestraMensajeRCH;

        private delegate void AccedeControlProgreso(string nombreArchivo, long enviado, long total);
        AccedeControlProgreso MuestraProgresoArchivo;

        private bool PuertoSeleccionadoAutomatico = false;
        private Panel anclaScroll = new Panel();

        public Form1()
        {
            InitializeComponent();
            Enlace = new ClassComunicacion();
            MuestraMensajeRCH = new AccedeControl(MostrandoMensaje);
            MuestraProgresoArchivo = new AccedeControlProgreso(ActualizandoProgresoArchivo);

            lvArchivosSeleccionados.Columns.Add("Archivo", 140);
            lvArchivosSeleccionados.Columns.Add("Tamaño (bytes)", 80);
            lvArchivosSeleccionados.Columns.Add("Progreso", 70);

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

        private void AplicarBotonesRedondeados()
        {
            RedondearBoton(btnEnviaMensaje, 20);
            RedondearBoton(btnSeleccionarArchivos, 15);
            RedondearBoton(btnEnviarArchivos, 15);
            RedondearBoton(btnCrearArchivo, 15);
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
            Enlace.MetadatosRecibidos += Enlace_MetadatosRecibidos;
            Enlace.ProgresoRecepcion += Enlace_ProgresoRecepcion;
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
            {
                return;
            }

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
            int anchoDisponible = scrollChat.ClientSize.Width - scrollChat.Padding.Horizontal;
            int anchoMax = (int)(anchoDisponible * 0.65);

            Label burbuja = new Label();
            burbuja.Text = mensaje;
            burbuja.Font = new Font("Segoe UI", 10F);
            burbuja.ForeColor = Color.FromArgb(50, 50, 50);
            burbuja.AutoSize = true;
            burbuja.MaximumSize = new Size(anchoMax, 0);
            burbuja.Padding = new Padding(12, 8, 12, 8);
            burbuja.BackColor = esPropio ? Color.FromArgb(220, 248, 198) : Color.White;
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
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color color = esPropio ? Color.FromArgb(220, 248, 198) : Color.White;
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
                    Label burbuja = null;
                    Panel cola = null;
                    foreach (Control hijo in contenedor.Controls)
                    {
                        if (hijo is Label lbl) burbuja = lbl;
                        else if (hijo is Panel pnl && pnl.Name.StartsWith("cola_")) cola = pnl;
                    }

                    if (burbuja != null)
                    {
                        burbuja.MaximumSize = new Size(anchoBurbuja, 0);
                        contenedor.AutoSize = false;

                        int colaAncho = 10;
                        if (cola != null)
                        {
                            cola.Size = new Size(colaAncho, 12);
                            if (esPropio)
                            {
                                burbuja.Location = new Point(0, 0);
                                cola.Location = new Point(burbuja.PreferredSize.Width, 0);
                            }
                            else
                            {
                                cola.Location = new Point(0, 0);
                                burbuja.Location = new Point(colaAncho, 0);
                            }
                        }

                        contenedor.Size = new Size(
                            burbuja.PreferredSize.Width + colaAncho,
                            burbuja.PreferredSize.Height);
                    }
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
            {
                return (partes[0], partes[1]);
            }

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
            {
                cbVelocidad.Items.Add(velocidad.ToString());
            }

            if (cbVelocidad.Items.Count > 0)
            {
                cbVelocidad.SelectedItem = "115200";
            }
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
            {
                return;
            }

            var nombrePuerto = cbCOM.SelectedItem?.ToString() ?? "";

            if (!int.TryParse(cbVelocidad.SelectedItem?.ToString(), out var velocidad))
            {
                return;
            }

            Enlace.InicializaPuerto(nombrePuerto, velocidad);
        }

        private bool PuedeConfigurarPuerto()
        {
            return cbCOM.SelectedItem is not null && cbVelocidad.SelectedItem is not null;
        }

        private bool PuedeEnviar()
        {
            return !string.IsNullOrWhiteSpace(txtUsuario.Text)
                && cbCOM.SelectedItem is not null
                && cbVelocidad.SelectedItem is not null;
        }

        private void btnSeleccionarArchivos_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialogo = new OpenFileDialog())
            {
                dialogo.Multiselect = true;
                dialogo.Title = "Seleccionar archivos para enviar";
                dialogo.Filter = "Todos los archivos (*.*)|*.*";

                if (dialogo.ShowDialog() == DialogResult.OK && dialogo.FileNames.Length > 0)
                {
                    if (dialogo.FileNames.Length > 5)
                    {
                        MessageBox.Show("Máximo 5 archivos simultáneos. Se enviarán los primeros 5.");
                    }

                    lvArchivosSeleccionados.Items.Clear();

                    int limite = Math.Min(dialogo.FileNames.Length, 5);
                    for (int i = 0; i < limite; i++)
                    {
                        string ruta = dialogo.FileNames[i];
                        string nombre = Path.GetFileName(ruta);
                        long tamano = new FileInfo(ruta).Length;

                        ListViewItem item = new ListViewItem(nombre);
                        item.SubItems.Add(tamano.ToString());
                        item.SubItems.Add("0%");
                        item.Tag = ruta;
                        lvArchivosSeleccionados.Items.Add(item);
                    }
                }
            }
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

            string[] rutas = new string[lvArchivosSeleccionados.Items.Count];
            for (int i = 0; i < lvArchivosSeleccionados.Items.Count; i++)
            {
                rutas[i] = lvArchivosSeleccionados.Items[i].Tag as string;
            }

            foreach (ListViewItem item in lvArchivosSeleccionados.Items)
            {
                item.SubItems[2].Text = "pendiente";
            }

            Enlace.NombreUsuario = txtUsuario.Text.Trim();
            Enlace.EnviarArchivos(rutas);
        }

        private void btnCrearArchivo_Click(object sender, EventArgs e)
        {
            var nombre = txtArchivoRecepcion.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Ingrese un nombre de archivo a recibir.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtTamanoArchivo.Text))
            {
                MessageBox.Show("Ingrese el tamaño del archivo en bytes.");
                return;
            }
            if (!long.TryParse(txtTamanoArchivo.Text, out long tamano))
            {
                MessageBox.Show("El tamaño debe ser un numero entero (bytes).");
                return;
            }
            Enlace.CrearArchivo(nombre, tamano);
            lblEstadoRecepcion.Text = "Recepción manual: " + nombre + " (" + tamano + " bytes)";
        }

        private void Enlace_ProgresoEnvio(string nombreArchivo, long enviado, long total)
        {
            Invoke(MuestraProgresoArchivo, nombreArchivo, enviado, total);
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

        private void Enlace_ArchivoEnvioCompletado(string nombreArchivo)
        {
            Invoke(() =>
            {
                foreach (ListViewItem item in lvArchivosSeleccionados.Items)
                {
                    if (item.Text == nombreArchivo)
                    {
                        item.SubItems[2].Text = "COMPLETADO";
                        item.BackColor = Color.LightGreen;
                        break;
                    }
                }
            });
        }

        private void Enlace_MetadatosRecibidos(string nombreArchivo, long tamano)
        {
            Invoke(() =>
            {
                lblEstadoRecepcion.Text = "Recibiendo: " + nombreArchivo + " (" + tamano + " bytes)";
                pbRecepcionArchivo.Value = 0;
                pbRecepcionArchivo.Maximum = 100;
            });
        }

        private void Enlace_ProgresoRecepcion(long recibido, long total)
        {
            Invoke(() =>
            {
                if (total > 0)
                {
                    int porcentaje = (int)(recibido * 100 / total);
                    if (porcentaje > 100) porcentaje = 100;
                    pbRecepcionArchivo.Value = porcentaje;
                }

                if (recibido >= total && total > 0)
                {
                    lblEstadoRecepcion.Text = "Recepción: completado ✓";
                }
            });
        }
    }
}
