namespace winProyComunicacion
{
    public partial class Form1 : Form
    {

        ClassComunicacion Enlace;
        private delegate void AccedeControl(string mesaje);
        AccedeControl MuestraMensajeRCH;


        public Form1()
        {
            InitializeComponent();
            Enlace = new ClassComunicacion();
            MuestraMensajeRCH = new AccedeControl(mostrandoMensaje);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // enlace = new ClassComunicacion();
            Enlace.LlegoMensaje += Enlace_llegoMensaje;
            Enlace.InicializaPuerto("COM2", 115200);
        }

        private void Enlace_llegoMensaje(string m)
        {
            // throw new NotImplementedException();
            // MessageBox.Show("se disparo el evnto de mi clase Mensajito = " + m);
            Invoke(MuestraMensajeRCH, m);

        }

        private void btnEnviaMensaje_Click(object sender, EventArgs e)
        {
            Enlace.EnviarMensaje(txtMensaje.Text.Trim());
        }

        private void mostrandoMensaje(string mens)
        {
            rchConversacion.Text += mens;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Enlace.AbrirArchivo(textBox1.Text.Trim());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Enlace.inicioTransmisionArchivo1();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Enlace.CrearArchivo(textBox1.Text.Trim(), Convert.ToInt64(textBox2.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
