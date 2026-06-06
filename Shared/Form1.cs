namespace winProyComunicacion
{
    public partial class Form1 : Form
    {
        private const string DefaultRemoteUser = "Remoto";
        private static readonly int[] VelocidadesSoportadas = [900, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800];

        ClassComunicacion Enlace;

        private delegate void AccedeControl(string mens);
        AccedeControl MuestraMensajeRCH;

        public Form1()
        {
            InitializeComponent();
            Enlace = new ClassComunicacion();
            MuestraMensajeRCH = new AccedeControl(MostrandoMensaje);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Enlace.LlegoMensaje += Enlace_llegoMensaje;
            CargarPuertos();
            CargarVelocidades();
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

            Enlace.EnviarMensaje($"{usuario}|{mensaje}");
            MostrarMensajeLocal(usuario, mensaje);
            txtMensaje.Clear();
        }

        private void MostrandoMensaje(string mensaje)
        {
            var (usuario, texto) = SepararMensaje(mensaje);
            AgregarMensajeFormateado(usuario, texto);
        }

        private void MostrarMensajeLocal(string usuario, string mensaje)
        {
            AgregarMensajeFormateado(usuario, mensaje);
        }

        private void AgregarMensajeFormateado(string usuario, string mensaje)
        {
            AgregarLineaEnNegrita(usuario);
            rchConversacion.AppendText(mensaje + Environment.NewLine + Environment.NewLine);
        }

        private void AgregarLineaEnNegrita(string texto)
        {
            rchConversacion.SelectionStart = rchConversacion.TextLength;
            rchConversacion.SelectionLength = 0;
            rchConversacion.SelectionFont = new Font(rchConversacion.Font, FontStyle.Bold);
            rchConversacion.AppendText(texto + Environment.NewLine);
            rchConversacion.SelectionFont = rchConversacion.Font;
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
            if (cbCOM.Items.Count > 0)
            {
                var indiceCom1 = cbCOM.Items.IndexOf("COM2");
                cbCOM.SelectedIndex = indiceCom1 >= 0 ? indiceCom1 : 0;
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

            var nombrePuerto = cbCOM.SelectedItem?.ToString();
            if (!int.TryParse(cbVelocidad.SelectedItem?.ToString(), out var velocidad))
            {
                return;
            }

            Enlace.InicializaPuerto(nombrePuerto!, velocidad);
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
    }
}
