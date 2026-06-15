using System.Text;
using System.IO.Ports;
using System.Collections.Concurrent;

namespace winProyComunicacion
{
    internal class ClassComunicacion
    {
        public SerialPort sPuerto;

        public delegate void miManejador(string m);
        public event miManejador LlegoMensaje;

        public event Action<string, long, long> ProgresoEnvio;
        public event Action<string, string> ArchivoEnvioCompletado;
        public event Action<string, string> ArchivoRecibidoCompletado;
        public event Action<string, long> MetadatosRecibidos;
        public event Action<int, string, long> MetadatosRecibidosConIndice;
        public event Action<long, long> ProgresoRecepcion;
        public event Action<int, string, long, long> ProgresoRecepcionConIndice;

        public string NombreUsuario { get; set; } = "";
        public string DirectorioDescarga { get; set; } = "C:\\REDES1\\";

        private Thread hebraEnvio;
        private string MensajeRecibido;
        private byte[] tramaEnvioBytes;
        private volatile bool _mensajePendiente;
        private EnvioArchivo[] _envios = new EnvioArchivo[5];
        private ConcurrentDictionary<int, ArchivoRecepcion> _recepcionesActivas = new ConcurrentDictionary<int, ArchivoRecepcion>();
        private readonly SemaphoreSlim _semaforoPuerto = new SemaphoreSlim(1, 1);

        public ClassComunicacion()
        {
            MensajeRecibido = "";
            sPuerto = new SerialPort();
            tramaEnvioBytes = new byte[1024];
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
                sPuerto.WriteBufferSize = 8192;
                sPuerto.ReceivedBytesThreshold = 1024;
                sPuerto.Handshake = Handshake.RequestToSend;
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

        private void AsegurarDirectorioRedes1()
        {
            string directorio = DirectorioDescarga;
            if (!Directory.Exists(directorio))
                Directory.CreateDirectory(directorio);
        }

        public void EnviarArchivos(string[] rutasArchivos)
        {
            if (rutasArchivos == null || rutasArchivos.Length == 0)
                return;

            AsegurarDirectorioRedes1();

            int limite = Math.Min(rutasArchivos.Length, 5);
            for (int i = 0; i < limite; i++)
            {
                var envio = new EnvioArchivo(rutasArchivos[i], i);
                _envios[i] = envio;

                int idx = i;
                envio.ProgresoChanged += (a, enviado, total) =>
                    ProgresoEnvio?.Invoke(a.Nombre, enviado, total);
                envio.EnvioCompletado += (a) =>
                {
                    _envios[idx] = null;
                    ArchivoEnvioCompletado?.Invoke(a.Nombre, a.Ruta);
                };

                if (!envio.Abrir())
                    continue;

                envio.Iniciar(NombreUsuario, sPuerto, _semaforoPuerto, () => _mensajePendiente);

                Thread hilo = new Thread(() =>
                {
                    try
                    {
                        while (true)
                        {
                            var e = _envios[idx];
                            if (e == null || e.Error) break;
                            if (!e.EnviarBloque(sPuerto, _semaforoPuerto, () => _mensajePendiente)) break;
                        }
                    }
                    catch { }
                });
                hilo.IsBackground = true;
                hilo.Start();
            }
        }

        private void SPuerto_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                while (sPuerto.BytesToRead >= 1024)
                {
                    byte[] frame = new byte[1024];
                    sPuerto.Read(frame, 0, 1024);

                    string TAREA = Encoding.UTF8.GetString(frame, 0, 1);

                    switch (TAREA)
                    {
                        case "M":
                            RecibiendoMensaje(frame);
                            break;
                        case "A":
                            EscribiendoRecepcionArchivo(frame);
                            break;
                        case "F":
                            ProcesandoMetadatosRecepcion(frame);
                            break;
                        case "I":
                            break;
                        default:
                            System.Diagnostics.Debug.WriteLine("Frame ignorado: tipo " + (int)frame[0]);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en SPuerto_DataReceived: " + ex.Message);
            }
        }

        private void ProcesandoMetadatosRecepcion(byte[] frame)
        {
            int longitudMeta = Convert.ToInt32(Encoding.UTF8.GetString(frame, 1, 4));
            string metadatos = Encoding.UTF8.GetString(frame, 5, longitudMeta);
            string[] partes = metadatos.Split('|');
            if (partes.Length == 4 && int.TryParse(partes[0], out int indice) && long.TryParse(partes[3], out long tamano))
            {
                string usuarioRemitente = partes[1];
                string nombreArchivo = partes[2];
                string nombreReal = usuarioRemitente + "_" + nombreArchivo;

                if (_recepcionesActivas.ContainsKey(indice))
                {
                    ArchivoRecepcion existente;
                    if (_recepcionesActivas.TryRemove(indice, out existente))
                        existente.Cerrar();
                }

                var archivoRec = new ArchivoRecepcion(indice, usuarioRemitente, nombreArchivo, tamano, DirectorioDescarga, nombreReal);
                if (_recepcionesActivas.TryAdd(indice, archivoRec))
                {
                    MetadatosRecibidos?.Invoke(nombreReal, tamano);
                    MetadatosRecibidosConIndice?.Invoke(indice, nombreReal, tamano);
                }
            }
        }

        private void RecibiendoMensaje(byte[] frame)
        {
            int longitudVerdadera = Convert.ToInt16(Encoding.UTF8.GetString(frame, 1, 4));
            MensajeRecibido = Encoding.UTF8.GetString(frame, 5, longitudVerdadera);

            OnLlegomensaje(MensajeRecibido);
        }

        protected virtual void OnLlegomensaje(string m)
        {
            LlegoMensaje?.Invoke(m);
        }

        public void EnviarMensaje(string m)
        {
            byte[] mensajeBytes = Encoding.UTF8.GetBytes(m);

            if (mensajeBytes.Length > 1019)
            {
                MessageBox.Show("Mensaje superior a los 1019 caracteres");
                return;
            }

            _mensajePendiente = true;
            hebraEnvio = new Thread(() => EnviandoMensaje(mensajeBytes));
            hebraEnvio.Start();
        }

        private void EnviandoMensaje(byte[] mensajeBytes)
        {
            string longitudStr = mensajeBytes.Length.ToString("D4");
            byte[] cabecera = Encoding.UTF8.GetBytes("M" + longitudStr);

            _semaforoPuerto.Wait();
            try
            {
                sPuerto.Write(cabecera, 0, 5);
                sPuerto.Write(mensajeBytes, 0, mensajeBytes.Length);
                sPuerto.Write(tramaEnvioBytes, 0, 1019 - mensajeBytes.Length);
            }
            finally
            {
                _semaforoPuerto.Release();
                _mensajePendiente = false;
            }
        }

        public void CrearArchivo(string nombrecito, long TamanoArchivo, int indice = -1)
        {
            AsegurarDirectorioRedes1();

            if (indice < 0)
            {
                indice = _recepcionesActivas.Keys.Count > 0 ? _recepcionesActivas.Keys.Max() + 1 : 0;
                while (_recepcionesActivas.ContainsKey(indice)) indice++;
            }

            ArchivoRecepcion existente;
            if (_recepcionesActivas.TryRemove(indice, out existente))
                existente.Cerrar();

            var archivoRec = new ArchivoRecepcion(indice, "", nombrecito, TamanoArchivo, DirectorioDescarga, nombrecito);
            _recepcionesActivas.TryAdd(indice, archivoRec);
        }

        public void EscribiendoRecepcionArchivo(byte[] frame)
        {
            int indice = frame[1] - (byte)'0';

            if (!_recepcionesActivas.TryGetValue(indice, out ArchivoRecepcion archivoRec))
                return;

            if (archivoRec.Completado)
                return;

            if (archivoRec.TamanoTotal - archivoRec.BytesRecibidos >= 1019)
            {
                archivoRec.EscribirDatos(frame, 5, 1019);
                ProgresoRecepcion?.Invoke(archivoRec.BytesRecibidos, archivoRec.TamanoTotal);
                ProgresoRecepcionConIndice?.Invoke(indice, archivoRec.NombreReal, archivoRec.BytesRecibidos, archivoRec.TamanoTotal);
                if (archivoRec.Completado)
                {
                    string rutaCompleta = Path.Combine(DirectorioDescarga, archivoRec.NombreReal);
                    archivoRec.Cerrar();
                    _recepcionesActivas.TryRemove(indice, out _);
                    ArchivoRecibidoCompletado?.Invoke(archivoRec.UsuarioRemitente, rutaCompleta);
                }
            }
            else
            {
                int bytesRestantes = Convert.ToInt16(archivoRec.TamanoTotal - archivoRec.BytesRecibidos);
                archivoRec.EscribirDatos(frame, 5, bytesRestantes);
                ProgresoRecepcion?.Invoke(archivoRec.BytesRecibidos, archivoRec.TamanoTotal);
                ProgresoRecepcionConIndice?.Invoke(indice, archivoRec.NombreReal, archivoRec.BytesRecibidos, archivoRec.TamanoTotal);
                string rutaCompleta = Path.Combine(DirectorioDescarga, archivoRec.NombreReal);
                archivoRec.Cerrar();
                _recepcionesActivas.TryRemove(indice, out _);
                ArchivoRecibidoCompletado?.Invoke(archivoRec.UsuarioRemitente, rutaCompleta);
            }
        }
    }
}
