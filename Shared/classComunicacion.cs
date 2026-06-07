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

        private readonly SemaphoreSlim _semaforoPuerto = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _semaforoEnvios = new SemaphoreSlim(5, 5);

        private int _enviosActivos = 0;

        public string NombreUsuario { get; set; } = "";

        public ClassComunicacion()
        {
            hebraEnvio = new Thread(EnviandoMensaje);
            procesoEnvioArchivo = new Thread(LeyendoTransmitiendoArchivo);
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

        public bool AbrirArchivo(string nombreArchivo)
        {
            try
            {
                CerrarFlujoEnvio();

                string ruta = Path.Combine("C:\\REDES1\\", nombreArchivo);
                AsegurarDirectorioRedes1();
                FlujoLecturaArchivo = new FileStream(ruta, FileMode.Open, FileAccess.Read);
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

        private void AsegurarDirectorioRedes1()
        {
            string directorio = "C:\\REDES1\\";
            if (!Directory.Exists(directorio))
                Directory.CreateDirectory(directorio);
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

        public void EnviarArchivos(string[] rutasArchivos)
        {
            if (rutasArchivos == null || rutasArchivos.Length == 0)
                return;

            AsegurarDirectorioRedes1();

            foreach (string rutaCompleta in rutasArchivos)
            {
                string nombreArchivo = Path.GetFileName(rutaCompleta);
                Thread hiloEnvio = new Thread(() => EnviarArchivoIndividual(rutaCompleta, nombreArchivo));
                hiloEnvio.IsBackground = true;
                hiloEnvio.Start();
            }
        }

        private void EnviarArchivoIndividual(string rutaCompleta, string nombreArchivo)
        {
            _semaforoEnvios.Wait();
            try
            {
                Interlocked.Increment(ref _enviosActivos);

                FileStream flujoLectura = null;
                BinaryReader lector = null;
                long tamanoArchivoIndividual = 0;

                try
                {
                    flujoLectura = new FileStream(rutaCompleta, FileMode.Open, FileAccess.Read);
                    lector = new BinaryReader(flujoLectura);
                    tamanoArchivoIndividual = flujoLectura.Length;

                    string metadatos = NombreUsuario + "|" + nombreArchivo + "|" + tamanoArchivoIndividual.ToString();
                    byte[] tramaMetadatos = Encoding.UTF8.GetBytes(metadatos);
                    int longitudMetadatos = tramaMetadatos.Length;

                    if (longitudMetadatos > 1019)
                    {
                        MessageBox.Show("Nombre de archivo demasiado largo: " + nombreArchivo);
                        return;
                    }

                    string cabeceraMeta = "F" + longitudMetadatos.ToString("D4");
                    byte[] tramaCabeceraMeta = Encoding.UTF8.GetBytes(cabeceraMeta);
                    byte[] relleno = new byte[1019 - longitudMetadatos];
                    for (int i = 0; i < relleno.Length; i++) relleno[i] = 64;

                    _semaforoPuerto.Wait();
                    try
                    {
                        sPuerto.Write(tramaCabeceraMeta, 0, 5);
                        sPuerto.Write(tramaMetadatos, 0, longitudMetadatos);
                        sPuerto.Write(relleno, 0, relleno.Length);
                    }
                    finally
                    {
                        _semaforoPuerto.Release();
                    }

                    long avanceEnvio = 0;
                    byte[] bufferLectura = new byte[1019];

                    while (tamanoArchivoIndividual - avanceEnvio >= 1019)
                    {
                        byte[] cabeceraDatos = Encoding.UTF8.GetBytes("A" + "1019");
                        lector.Read(bufferLectura, 0, 1019);
                        avanceEnvio += 1019;

                        _semaforoPuerto.Wait();
                        try
                        {
                            sPuerto.Write(cabeceraDatos, 0, 5);
                            sPuerto.Write(bufferLectura, 0, 1019);
                        }
                        finally
                        {
                            _semaforoPuerto.Release();
                        }

                        ProgresoEnvio?.Invoke(nombreArchivo, avanceEnvio, tamanoArchivoIndividual);
                    }

                    int bytesRestantes = Convert.ToInt32(tamanoArchivoIndividual - avanceEnvio);
                    if (bytesRestantes > 0)
                    {
                        string cabeceraFinalStr = "A" + bytesRestantes.ToString() + "HHH";
                        byte[] cabeceraFinal = Encoding.UTF8.GetBytes(cabeceraFinalStr);
                        lector.Read(bufferLectura, 0, bytesRestantes);

                        _semaforoPuerto.Wait();
                        try
                        {
                            sPuerto.Write(cabeceraFinal, 0, 5);
                            sPuerto.Write(bufferLectura, 0, bytesRestantes);
                            sPuerto.Write(tramaEnvioBytes, 0, 1019 - bytesRestantes);
                        }
                        finally
                        {
                            _semaforoPuerto.Release();
                        }

                        avanceEnvio += bytesRestantes;
                        ProgresoEnvio?.Invoke(nombreArchivo, avanceEnvio, tamanoArchivoIndividual);
                    }

                    ArchivoEnvioCompletado?.Invoke(nombreArchivo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al enviar archivo " + nombreArchivo + ": " + ex.Message);
                }
                finally
                {
                    lector?.Dispose();
                    flujoLectura?.Dispose();
                    Interlocked.Decrement(ref _enviosActivos);
                }
            }
            finally
            {
                _semaforoEnvios.Release();
            }
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

                _semaforoPuerto.Wait();
                try
                {
                    sPuerto.Write(tramaCabacera, 0, 5);
                    sPuerto.Write(tramaEnvioArchivo, 0, 1019);
                }
                finally
                {
                    _semaforoPuerto.Release();
                }
            }
            int bytesRestantes = Convert.ToInt16(tamanoArchivo - avanceEnvio);
            tramaCabacera = ASCIIEncoding.UTF8.GetBytes("A" + bytesRestantes.ToString() + "HHH");
            leyendoTramaArchivoEnvio.Read(tramaEnvioArchivo, 0, bytesRestantes);

            _semaforoPuerto.Wait();
            try
            {
                sPuerto.Write(tramaCabacera, 0, 5);
                sPuerto.Write(tramaEnvioArchivo, 0, bytesRestantes);
                sPuerto.Write(tramaEnvioBytes, 0, 1019 - bytesRestantes);
            }
            finally
            {
                _semaforoPuerto.Release();
            }

            avanceEnvio = avanceEnvio + bytesRestantes;

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

        private void ProcesandoMetadatosRecepcion()
        {
            int longitudMeta = Convert.ToInt32(Encoding.UTF8.GetString(tramaRecepcionMensaje, 1, 4));
            string metadatos = Encoding.UTF8.GetString(tramaRecepcionMensaje, 5, longitudMeta);
            string[] partes = metadatos.Split('|');
            if (partes.Length == 3 && long.TryParse(partes[2], out long tamano))
            {
                string usuarioRemitente = partes[0];
                string nombreArchivo = partes[1];
                string nombreConPrefijo = usuarioRemitente + "_" + nombreArchivo;
                CrearArchivo(nombreConPrefijo, tamano);
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
            CerrarFlujoRecepcion();

            AsegurarDirectorioRedes1();
            FlujoEscrituraArchivo = new FileStream(Path.Combine("C:\\REDES1\\", nombrecito), FileMode.Create, FileAccess.Write);
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
                ProgresoRecepcion?.Invoke(avanceRecepcionArchivo, tamanoArchivoRecepcion);
            }
            else
            {
                int bytesRestantes = Convert.ToInt16(tamanoArchivoRecepcion - avanceRecepcionArchivo);
                EscribiendoTramaArchivoRecepcion.Write(tramaRecepcionMensaje, 5, bytesRestantes);
                avanceRecepcionArchivo += bytesRestantes;
                ProgresoRecepcion?.Invoke(avanceRecepcionArchivo, tamanoArchivoRecepcion);
                EscribiendoTramaArchivoRecepcion.Dispose();
                EscribiendoTramaArchivoRecepcion = null;
                FlujoEscrituraArchivo.Dispose();
                FlujoEscrituraArchivo = null;
            }
        }
    }
}
