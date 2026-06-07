using System.IO.Ports;
using System.Text;

namespace winProyComunicacion
{
    internal class ClassTransferenciaArchivo
    {
        public int Indice { get; private set; }
        public string NombreArchivo { get; private set; }
        public long TamanoArchivo { get; private set; }
        public long Avance { get; private set; }
        public bool EstaActivo { get; private set; }

        private FileStream _flujoArchivo;
        private BinaryReader _lectorArchivo;
        private BinaryWriter _escritorArchivo;

        private SerialPort _puerto;
        private SemaphoreSlim _semaforoPuerto;
        private SemaphoreSlim _semaforoEnvios;
        private string _usuarioRemitente;

        private Thread _hebraTrabajo;

        public event Action<string, long, long> ProgresoEnvio;
        public event Action<string> ArchivoEnvioCompletado;
        public event Action<long, long> ProgresoRecepcion;

        public ClassTransferenciaArchivo()
        {
            NombreArchivo = "";
            _usuarioRemitente = "";
            LimpiarEstado();
        }

        private void LimpiarEstado()
        {
            Indice = -1;
            NombreArchivo = "";
            TamanoArchivo = 0;
            Avance = 0;
            EstaActivo = false;
            _flujoArchivo = null;
            _lectorArchivo = null;
            _escritorArchivo = null;
        }

        public void Cerrar()
        {
            _lectorArchivo?.Dispose();
            _lectorArchivo = null;
            _escritorArchivo?.Dispose();
            _escritorArchivo = null;
            _flujoArchivo?.Dispose();
            _flujoArchivo = null;
            EstaActivo = false;
        }

        public void PrepararEnvio(int indice, string rutaArchivo, string nombreArchivo,
            SerialPort puerto, SemaphoreSlim semaforoPuerto, SemaphoreSlim semaforoEnvios,
            string usuarioRemitente)
        {
            Cerrar();
            Indice = indice;
            NombreArchivo = nombreArchivo;
            _puerto = puerto;
            _semaforoPuerto = semaforoPuerto;
            _semaforoEnvios = semaforoEnvios;
            _usuarioRemitente = usuarioRemitente;

            _flujoArchivo = new FileStream(rutaArchivo, FileMode.Open, FileAccess.Read);
            _lectorArchivo = new BinaryReader(_flujoArchivo);
            TamanoArchivo = _flujoArchivo.Length;
            Avance = 0;
            EstaActivo = true;
        }

        public void IniciarEnvio()
        {
            if (!EstaActivo || _lectorArchivo == null)
                return;

            _hebraTrabajo = new Thread(EjecutarEnvio);
            _hebraTrabajo.IsBackground = true;
            _hebraTrabajo.Start();
        }

        private void EjecutarEnvio()
        {
            _semaforoEnvios.Wait();
            try
            {
                try
                {
                    string metadatos = _usuarioRemitente + "|" + NombreArchivo + "|" + TamanoArchivo.ToString();
                    byte[] tramaMetadatos = Encoding.UTF8.GetBytes(metadatos);
                    int longitudMetadatos = tramaMetadatos.Length;

                    if (longitudMetadatos > 1018)
                    {
                        MessageBox.Show("Nombre de archivo demasiado largo: " + NombreArchivo);
                        return;
                    }

                    string cabeceraMeta = "F" + Indice.ToString() + longitudMetadatos.ToString("D4");
                    byte[] tramaCabeceraMeta = Encoding.UTF8.GetBytes(cabeceraMeta);
                    byte[] relleno = new byte[1018 - longitudMetadatos];
                    for (int i = 0; i < relleno.Length; i++) relleno[i] = 64;

                    _semaforoPuerto.Wait();
                    try
                    {
                        _puerto.Write(tramaCabeceraMeta, 0, 6);
                        _puerto.Write(tramaMetadatos, 0, longitudMetadatos);
                        _puerto.Write(relleno, 0, relleno.Length);
                    }
                    finally
                    {
                        _semaforoPuerto.Release();
                    }

                    byte[] bufferLectura = new byte[1018];
                    long avance = 0;

                    while (TamanoArchivo - avance >= 1018)
                    {
                        byte[] cabeceraDatos = Encoding.UTF8.GetBytes("A" + Indice.ToString() + "1018");
                        _lectorArchivo.Read(bufferLectura, 0, 1018);
                        avance += 1018;

                        _semaforoPuerto.Wait();
                        try
                        {
                            _puerto.Write(cabeceraDatos, 0, 6);
                            _puerto.Write(bufferLectura, 0, 1018);
                        }
                        finally
                        {
                            _semaforoPuerto.Release();
                        }

                        Avance = avance;
                        ProgresoEnvio?.Invoke(NombreArchivo, avance, TamanoArchivo);
                    }

                    int bytesRestantes = Convert.ToInt32(TamanoArchivo - avance);
                    if (bytesRestantes > 0)
                    {
                        string cabeceraFinalStr = "A" + Indice.ToString() + bytesRestantes.ToString("D4");
                        byte[] cabeceraFinal = Encoding.UTF8.GetBytes(cabeceraFinalStr);
                        _lectorArchivo.Read(bufferLectura, 0, bytesRestantes);

                        byte[] rellenoFinal = new byte[1018 - bytesRestantes];
                        for (int i = 0; i < rellenoFinal.Length; i++) rellenoFinal[i] = 64;

                        _semaforoPuerto.Wait();
                        try
                        {
                            _puerto.Write(cabeceraFinal, 0, 6);
                            _puerto.Write(bufferLectura, 0, bytesRestantes);
                            _puerto.Write(rellenoFinal, 0, rellenoFinal.Length);
                        }
                        finally
                        {
                            _semaforoPuerto.Release();
                        }

                        avance += bytesRestantes;
                        Avance = avance;
                        ProgresoEnvio?.Invoke(NombreArchivo, avance, TamanoArchivo);
                    }

                    ArchivoEnvioCompletado?.Invoke(NombreArchivo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al enviar " + NombreArchivo + ": " + ex.Message);
                }
                finally
                {
                    Cerrar();
                }
            }
            finally
            {
                _semaforoEnvios.Release();
            }
        }

        public void PrepararRecepcion(int indice, string nombreArchivo, long tamano)
        {
            Cerrar();
            Indice = indice;
            TamanoArchivo = tamano;
            Avance = 0;

            string directorio = "C:\\REDES1\\";
            if (!Directory.Exists(directorio))
                Directory.CreateDirectory(directorio);

            string ruta = Path.Combine(directorio, nombreArchivo);

            int contador = 1;
            string rutaOriginal = ruta;
            string nombreSinExt = Path.GetFileNameWithoutExtension(rutaOriginal);
            string ext = Path.GetExtension(rutaOriginal);

            while (File.Exists(ruta))
            {
                ruta = Path.Combine(directorio, nombreSinExt + " (" + contador + ")" + ext);
                contador++;
            }

            _flujoArchivo = new FileStream(ruta, FileMode.CreateNew, FileAccess.Write);
            _escritorArchivo = new BinaryWriter(_flujoArchivo);

            NombreArchivo = Path.GetFileName(ruta);
            EstaActivo = true;
        }

        public void EscribirChunk(byte[] buffer, int offset, int longitud)
        {
            if (!EstaActivo || _escritorArchivo == null)
                return;

            _escritorArchivo.Write(buffer, offset, longitud);
            Avance += longitud;
            ProgresoRecepcion?.Invoke(Avance, TamanoArchivo);
        }

        public void FinalizarRecepcion()
        {
            _escritorArchivo?.Dispose();
            _escritorArchivo = null;
            _flujoArchivo?.Dispose();
            _flujoArchivo = null;
            EstaActivo = false;
        }
    }
}
