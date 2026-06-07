using System.Text;
using System.IO.Ports;

namespace winProyComunicacion
{
    internal class ClassComunicacion
    {
        public SerialPort sPuerto;

        public delegate void miManejador(string m);
        public event miManejador LlegoMensaje;

        public event Action<string, long, long> ProgresoEnvio;
        public event Action<string> ArchivoEnvioCompletado;
        public event Action<string, long> MetadatosRecibidos;
        public event Action<long, long> ProgresoRecepcion;

        public string NombreUsuario { get; set; } = "";

        private Thread hebraEnvio;
        private Thread procesoRecibirMensaje;

        private string MensajeRecibido;

        private byte[] tramaMensajeEnvio;
        private byte[] tramaEnvioBytes;
        private byte[] tramaRecepcionMensaje;
        private byte[] tramaCabacera;

        private readonly SemaphoreSlim _semaforoPuerto = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _semaforoEnvios = new SemaphoreSlim(5, 5);

        private ClassTransferenciaArchivo[] _envios;
        private ClassTransferenciaArchivo[] _recepciones;

        private int _enviosActivos = 0;

        private const int MAX_ARCHIVOS = 5;
        private const int TAMANO_CHUNK = 1018;
        private const int TAMANO_TRAMA = 1024;

        public ClassComunicacion()
        {
            hebraEnvio = new Thread(EnviandoMensaje);
            MensajeRecibido = "";
            sPuerto = new SerialPort();
            tramaCabacera = new byte[6];
            tramaEnvioBytes = new byte[TAMANO_TRAMA];
            tramaMensajeEnvio = new byte[TAMANO_TRAMA];
            tramaRecepcionMensaje = new byte[TAMANO_TRAMA];

            _envios = new ClassTransferenciaArchivo[MAX_ARCHIVOS];
            _recepciones = new ClassTransferenciaArchivo[MAX_ARCHIVOS];

            for (int i = 0; i < MAX_ARCHIVOS; i++)
            {
                _envios[i] = new ClassTransferenciaArchivo();
                _recepciones[i] = new ClassTransferenciaArchivo();
            }

            VincularEventos();
        }

        private void VincularEventos()
        {
            for (int i = 0; i < MAX_ARCHIVOS; i++)
            {
                int indice = i;
                _envios[i].ProgresoEnvio += (nombre, enviado, total) =>
                    ProgresoEnvio?.Invoke(nombre, enviado, total);
                _envios[i].ArchivoEnvioCompletado += (nombre) =>
                {
                    ArchivoEnvioCompletado?.Invoke(nombre);
                    Interlocked.Decrement(ref _enviosActivos);
                };
                _recepciones[i].ProgresoRecepcion += (recibido, total) =>
                    ProgresoRecepcion?.Invoke(recibido, total);
            }
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
                sPuerto.ReceivedBytesThreshold = TAMANO_TRAMA;
                sPuerto.Encoding = Encoding.UTF8;
                sPuerto.Open();

                for (int i = 0; i < TAMANO_TRAMA; i++)
                    tramaEnvioBytes[i] = 64;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir {nombreP}: {ex.Message}");
            }
        }

        public bool AbrirArchivo(string nombreArchivo)
        {
            try
            {
                AsegurarDirectorioRedes1();
                string ruta = Path.Combine("C:\\REDES1\\", nombreArchivo);

                _envios[0].Cerrar();
                _envios[0].PrepararEnvio(0, ruta, nombreArchivo,
                    sPuerto, _semaforoPuerto, _semaforoEnvios, NombreUsuario);

                MessageBox.Show("SE ABRIO EL ARCHIVO Y SE CARGO PARA ENVIAR");
                return true;
            }
            catch (Exception e)
            {
                _envios[0].Cerrar();
                MessageBox.Show("Error al abrir archivo", e.Message.ToString());
                return false;
            }
        }

        private void AsegurarDirectorioRedes1()
        {
            string directorio = "C:\\REDES1\\";
            if (!Directory.Exists(directorio))
                Directory.CreateDirectory(directorio);
        }

        public bool inicioTransmisionArchivo1()
        {
            if (!_envios[0].EstaActivo)
            {
                MessageBox.Show("Primero debe abrir un archivo con \"ABRIR ARCHIVO\".");
                return false;
            }

            _envios[0].IniciarEnvio();
            Interlocked.Increment(ref _enviosActivos);
            return true;
        }

        public void EnviarArchivos(string[] rutasArchivos)
        {
            if (rutasArchivos == null || rutasArchivos.Length == 0)
                return;

            AsegurarDirectorioRedes1();

            int limite = Math.Min(rutasArchivos.Length, MAX_ARCHIVOS);

            for (int i = 0; i < limite; i++)
            {
                string rutaCompleta = rutasArchivos[i];
                string nombreArchivo = Path.GetFileName(rutaCompleta);
                int indice = i;

                _envios[indice].Cerrar();
                _envios[indice].PrepararEnvio(indice, rutaCompleta, nombreArchivo,
                    sPuerto, _semaforoPuerto, _semaforoEnvios, NombreUsuario);
                _envios[indice].IniciarEnvio();
                Interlocked.Increment(ref _enviosActivos);
            }
        }

        private void SPuerto_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (sPuerto.BytesToRead >= TAMANO_TRAMA)
            {
                sPuerto.Read(tramaRecepcionMensaje, 0, TAMANO_TRAMA);

                string TAREA = ASCIIEncoding.UTF8.GetString(tramaRecepcionMensaje, 0, 1);

                switch (TAREA)
                {
                    case "M":
                        procesoRecibirMensaje = new Thread(RecibiendoMensaje);
                        procesoRecibirMensaje.Start();
                        break;
                    case "A":
                        ProcesandoChunkArchivo();
                        break;
                    case "F":
                        ProcesandoMetadatosRecepcion();
                        break;
                    case "I":
                        break;
                    default:
                        MessageBox.Show("Trama no reconocida");
                        break;
                }
            }
        }

        private void ProcesandoChunkArchivo()
        {
            int indice = int.Parse(ASCIIEncoding.UTF8.GetString(tramaRecepcionMensaje, 1, 1));
            int longitudChunk = Convert.ToInt32(Encoding.UTF8.GetString(tramaRecepcionMensaje, 2, 4));

            if (indice < 0 || indice >= MAX_ARCHIVOS)
                return;

            var recepcion = _recepciones[indice];

            if (!recepcion.EstaActivo)
                return;

            if (longitudChunk > 0)
            {
                recepcion.EscribirChunk(tramaRecepcionMensaje, 6, longitudChunk);

                if (recepcion.Avance >= recepcion.TamanoArchivo)
                {
                    recepcion.FinalizarRecepcion();
                }
            }
        }

        private void ProcesandoMetadatosRecepcion()
        {
            int indice = int.Parse(ASCIIEncoding.UTF8.GetString(tramaRecepcionMensaje, 1, 1));
            int longitudMeta = Convert.ToInt32(Encoding.UTF8.GetString(tramaRecepcionMensaje, 2, 4));

            if (indice < 0 || indice >= MAX_ARCHIVOS)
                return;

            string metadatos = Encoding.UTF8.GetString(tramaRecepcionMensaje, 6, longitudMeta);
            string[] partes = metadatos.Split('|');

            if (partes.Length == 3 && long.TryParse(partes[2], out long tamano))
            {
                string usuarioRemitente = partes[0];
                string nombreArchivo = partes[1];
                string nombreConPrefijo = usuarioRemitente + "_" + nombreArchivo;

                _recepciones[indice].Cerrar();
                _recepciones[indice].PrepararRecepcion(indice, nombreConPrefijo, tamano);
                MetadatosRecibidos?.Invoke(nombreConPrefijo, tamano);
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

            _semaforoPuerto.Wait();
            try
            {
                sPuerto.Write(tramaCabacera, 0, 5);
                sPuerto.Write(tramaMensajeEnvio, 0, longitudmensaje);
                sPuerto.Write(tramaEnvioBytes, 0, 1019 - longitudmensaje);
            }
            finally
            {
                _semaforoPuerto.Release();
            }
        }

        public void CrearArchivo(string nombrecito, long TamanoArchivo)
        {
            AsegurarDirectorioRedes1();

            _recepciones[0].Cerrar();
            _recepciones[0].PrepararRecepcion(0, nombrecito, TamanoArchivo);
        }
    }
}
