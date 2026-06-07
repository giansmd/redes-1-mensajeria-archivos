namespace winProyComunicacion
{
    public partial class Form1 : Form
    {
        private const string DefaultRemoteUser = "Remoto";
        private static readonly int[] VelocidadesSoportadas = [900, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800];

        public static string? PuertoPreferido { get; set; }

        ClassComunicacion Enlace;

        private delegate void AccedeControl(string mens);
        AccedeControl MuestraMensajeRCH;

        private delegate void AccedeControlProgreso(string nombreArchivo, long enviado, long total);
        AccedeControlProgreso MuestraProgresoArchivo;

        private bool PuertoSeleccionadoAutomatico = false;

        public Form1()
        {
            InitializeComponent();
            Enlace = new ClassComunicacion();
            MuestraMensajeRCH = new AccedeControl(MostrandoMensaje);
            MuestraProgresoArchivo = new AccedeControlProgreso(ActualizandoProgresoArchivo);

            lvArchivosSeleccionados.Columns.Add("Archivo", 140);
            lvArchivosSeleccionados.Columns.Add("Tamaño (bytes)", 80);
            lvArchivosSeleccionados.Columns.Add("Progreso", 70);
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
            if (!PuedeEnviar())
            {
                MessageBox.Show("Complete nombre, puerto COM y velocidad antes de enviar.");
                return;
            }

            var usuario = txtUsuario.Text.Trim();
            var mensaje = txtMensaje.Text.Trim();
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                MessageBox.Show("Ingrese un mensaje antes de enviar.");
                return;
            }

            Enlace.NombreUsuario = usuario;
            Enlace.EnviarMensaje($"{usuario}|{mensaje}");
            MostrarMensajeLocal(usuario, mensaje);
            txtMensaje.Clear();
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
            rchConversacion.SelectionStart = rchConversacion.TextLength;
            rchConversacion.SelectionLength = 0;

            HorizontalAlignment alineacion = esPropio ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            rchConversacion.SelectionAlignment = alineacion;

            Color colorFondo = esPropio ? Color.FromArgb(220, 248, 198) : Color.FromArgb(230, 230, 230);
            rchConversacion.SelectionBackColor = colorFondo;

            rchConversacion.SelectionFont = new Font(rchConversacion.Font, FontStyle.Bold);
            rchConversacion.AppendText(usuario + Environment.NewLine);
            rchConversacion.SelectionFont = rchConversacion.Font;
            rchConversacion.AppendText(mensaje + Environment.NewLine + Environment.NewLine);

            rchConversacion.ScrollToCaret();
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
            rchConversacion.Clear();
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
