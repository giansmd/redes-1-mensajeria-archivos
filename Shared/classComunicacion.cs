using System.Text;
using System.IO.Ports;

namespace winProyComunicacion
{
    internal class ClassComunicacion
    {
        public SerialPort sPuerto;

        public delegate void miManejador(string m);
        public event miManejador LlegoMensaje;

        private Thread hebraEnvio;
        private Thread procesoRecibirMensaje;

        private string MensajeRecibido;

        private byte[] tramaMensajeEnvio;
        private byte[] tramaEnvioBytes;
        private byte[] tramaRecepcionMensaje;
        private byte[] tramaCabacera;
        private byte[] tramaEnvioArchivo;

        private FileStream FlujoLecturaArchivo;
        private BinaryReader leyendoTramaArchivoEnvio;
        private FileStream FlujoEscrituraArchivo;
        private BinaryWriter EscribiendoTramaArchivoRecepcion;

        private Thread procesoEnvioArchivo;

        long tamanoArchivo;

        long tamanoArchivoRecepcion;

        long avanceRecepcionArchivo;

        public ClassComunicacion()
        {
            hebraEnvio = new Thread(EnviandoMensaje);
            procesoEnvioArchivo = new Thread(EscribiendoRecepcionArchivo);
            MensajeRecibido = "";
            sPuerto = new SerialPort();
            tramaCabacera = new byte[5];
            tramaEnvioBytes = new byte[1024];
            tramaMensajeEnvio = new byte[1024];
            tramaRecepcionMensaje = new byte[1024];
            tramaEnvioArchivo = new byte[1024];
        }

        public void InicializaPuerto(string nombreP, int velocidad)
        {
            try
            {
                if (sPuerto.IsOpen)
                {
                    sPuerto.Close();
                }

                sPuerto.DataReceived -= SPuerto_DataReceived;
                sPuerto.DataReceived += SPuerto_DataReceived;

                sPuerto.PortName = nombreP;
                sPuerto.BaudRate = velocidad;
                sPuerto.DataBits = 8;
                sPuerto.StopBits = StopBits.Two;
                sPuerto.Parity = Parity.Odd;
                sPuerto.ReadBufferSize = 4096;
                sPuerto.WriteBufferSize = 3072;
                sPuerto.ReceivedBytesThreshold = 1024;
                sPuerto.Encoding = Encoding.UTF8;
                sPuerto.Open();

                for (int i = 0; i < 1024; i++)
                    tramaEnvioBytes[i] = 64;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir {nombreP}: {ex.Message}");
            }
        }

        public bool AbrirArchivo(string nombrecito)
        {
            try
            {
                CerrarFlujoEnvio();

                FlujoLecturaArchivo = new FileStream("C:\\REDES1\\" + nombrecito, FileMode.Open, FileAccess.Read);

                leyendoTramaArchivoEnvio = new BinaryReader(FlujoLecturaArchivo);
                tamanoArchivo = FlujoLecturaArchivo.Length;
                MessageBox.Show("SE ABRIO EL ARCHIVO Y SE CARGO PARA ENVIAR");
                return true;
            }
            catch (Exception e)
            {
                FlujoLecturaArchivo = null;
                leyendoTramaArchivoEnvio = null;
                tamanoArchivo = 0;
                MessageBox.Show("Error al abrir archivo", e.Message.ToString());
                return false;
            }
        }

        private void CerrarFlujoEnvio()
        {
            if (leyendoTramaArchivoEnvio != null)
            {
                leyendoTramaArchivoEnvio.Dispose();
                leyendoTramaArchivoEnvio = null;
            }
            if (FlujoLecturaArchivo != null)
            {
                FlujoLecturaArchivo.Dispose();
                FlujoLecturaArchivo = null;
            }
        }

        public bool inicioTransmisionArchivo1()
        {
            if (FlujoLecturaArchivo == null || leyendoTramaArchivoEnvio == null)
            {
                MessageBox.Show("Primero debe abrir un archivo con \"ABRIR ARCHIVO\".");
                return false;
            }

            if (!FlujoLecturaArchivo.CanRead)
            {
                MessageBox.Show("El archivo ya fue cerrado. Vuélvalo a abrir con \"ABRIR ARCHIVO\".");
                return false;
            }

            tamanoArchivo = FlujoLecturaArchivo.Length;
            procesoEnvioArchivo = new Thread(LeyendoTransmitiendoArchivo);
            procesoEnvioArchivo.Start();
            return true;
        }

        public void LeyendoTransmitiendoArchivo()
        {
            if (FlujoLecturaArchivo == null || leyendoTramaArchivoEnvio == null || !FlujoLecturaArchivo.CanRead)
                return;

            long avanceEnvio = 0;

            while (tamanoArchivo - avanceEnvio >= 1019)
            {
                tramaCabacera = ASCIIEncoding.UTF8.GetBytes("A" + "1019");
                leyendoTramaArchivoEnvio.Read(tramaEnvioArchivo, 0, 1019);
                avanceEnvio += 1019;

                sPuerto.Write(tramaCabacera, 0, 5);
                sPuerto.Write(tramaEnvioArchivo, 0, 1019);
            }
            tramaCabacera = ASCIIEncoding.UTF8.GetBytes("A" + Convert.ToInt16(tamanoArchivo - avanceEnvio).ToString() + "HHH");
            leyendoTramaArchivoEnvio.Read(tramaEnvioArchivo, 0, Convert.ToInt16(tamanoArchivo - avanceEnvio));

            sPuerto.Write(tramaCabacera, 0, 5);
            sPuerto.Write(tramaEnvioArchivo, 0, Convert.ToInt16(tamanoArchivo - avanceEnvio));
            sPuerto.Write(tramaEnvioBytes, 0, 1019 - Convert.ToInt16(tamanoArchivo - avanceEnvio));

            avanceEnvio = avanceEnvio + Convert.ToInt16(tamanoArchivo - avanceEnvio);

            CerrarFlujoEnvio();
        }

        private void SPuerto_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (sPuerto.BytesToRead >= 1024)
            {
                sPuerto.Read(tramaRecepcionMensaje, 0, 1024);

                string TAREA = ASCIIEncoding.UTF8.GetString(tramaRecepcionMensaje, 0, 1);

                switch (TAREA)
                {
                    case "M":
                        procesoRecibirMensaje = new Thread(RecibiendoMensaje);
                        procesoRecibirMensaje.Start();
                        break;
                    case "A":
                        EscribiendoRecepcionArchivo();
                        break;
                    case "I":
                        break;
                    default:
                        MessageBox.Show("Trama no reconocida");
                        break;
                }
            }
        }

        private void RecibiendoMensaje()
        {
            int longitudVerdadera = Convert.ToInt16(Encoding.UTF8.GetString(tramaRecepcionMensaje, 1, 4));
            MensajeRecibido = Encoding.UTF8.GetString(tramaRecepcionMensaje, 5, longitudVerdadera);

            OnLlegomensaje(MensajeRecibido);
        }

        protected virtual void OnLlegomensaje(string m)
        {
            LlegoMensaje?.Invoke(m);
        }

        public void EnviarMensaje(string m)
        {
            tramaMensajeEnvio = Encoding.UTF8.GetBytes(m);
            int longitudBytes = tramaMensajeEnvio.Length;

            if (longitudBytes > 1019)
            {
                MessageBox.Show("Mensaje superior a los 1019 caracteres");
                return;
            }

            string longitudMensajeStr = longitudBytes.ToString("D4");
            tramaCabacera = Encoding.UTF8.GetBytes("M" + longitudMensajeStr);

            hebraEnvio = new Thread(EnviandoMensaje);
            hebraEnvio.Start();
        }

        public void EnviandoMensaje()
        {
            int longitudmensaje = tramaMensajeEnvio.Length;
            sPuerto.Write(tramaCabacera, 0, 5);
            sPuerto.Write(tramaMensajeEnvio, 0, longitudmensaje);
            sPuerto.Write(tramaEnvioBytes, 0, 1019 - longitudmensaje);
        }

        public void CrearArchivo(string nombrecito, long TamanoArchivo)
        {
            CerrarFlujoRecepcion();

            FlujoEscrituraArchivo = new FileStream("C:\\REDES1\\" + nombrecito, FileMode.Create, FileAccess.Write);
            EscribiendoTramaArchivoRecepcion = new BinaryWriter(FlujoEscrituraArchivo);
            avanceRecepcionArchivo = 0;
            tamanoArchivoRecepcion = TamanoArchivo;
        }

        private void CerrarFlujoRecepcion()
        {
            if (EscribiendoTramaArchivoRecepcion != null)
            {
                EscribiendoTramaArchivoRecepcion.Dispose();
                EscribiendoTramaArchivoRecepcion = null;
            }
            if (FlujoEscrituraArchivo != null)
            {
                FlujoEscrituraArchivo.Dispose();
                FlujoEscrituraArchivo = null;
            }
        }

        public void EscribiendoRecepcionArchivo()
        {
            if (EscribiendoTramaArchivoRecepcion == null || FlujoEscrituraArchivo == null)
                return;

            if (avanceRecepcionArchivo >= tamanoArchivoRecepcion)
                return;

            if (tamanoArchivoRecepcion - avanceRecepcionArchivo >= 1019)
            {
                avanceRecepcionArchivo += 1019;
                EscribiendoTramaArchivoRecepcion.Write(tramaRecepcionMensaje, 5, 1019);
            }
            else
            {
                int bytesRestantes = Convert.ToInt16(tamanoArchivoRecepcion - avanceRecepcionArchivo);
                EscribiendoTramaArchivoRecepcion.Write(tramaRecepcionMensaje, 5, bytesRestantes);
                EscribiendoTramaArchivoRecepcion.Dispose();
                EscribiendoTramaArchivoRecepcion = null;
                FlujoEscrituraArchivo.Dispose();
                FlujoEscrituraArchivo = null;
            }
        }
    }
}
