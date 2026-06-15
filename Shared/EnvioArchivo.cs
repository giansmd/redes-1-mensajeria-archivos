using System.Text;
using System.IO.Ports;

namespace winProyComunicacion
{
    internal class EnvioArchivo
    {
        public int Indice { get; }
        public string Ruta { get; }
        public string Nombre { get; }
        public long Tamano { get; private set; }
        public long BytesEnviados { get; private set; }
        public bool Completado { get; private set; }
        public bool Error { get; private set; }

        public event Action<EnvioArchivo, long, long> ProgresoChanged;
        public event Action<EnvioArchivo> EnvioCompletado;

        private FileStream _flujo;
        private BinaryReader _lector;
        private long _avance;

        public EnvioArchivo(string ruta, int indice)
        {
            Ruta = ruta;
            Nombre = Path.GetFileName(ruta);
            Indice = indice;
        }

        private volatile bool _cancelado = false;
        public bool Cancelado => _cancelado;

        public void Cancelar()
        {
            _cancelado = true;
        }

        public bool Abrir()
        {
            try
            {
                Cerrar();
                _flujo = new FileStream(Ruta, FileMode.Open, FileAccess.Read);
                _lector = new BinaryReader(_flujo);
                Tamano = _flujo.Length;
                _avance = 0;
                BytesEnviados = 0;
                Completado = false;
                Error = false;
                _cancelado = false;
                return true;
            }
            catch
            {
                Error = true;
                return false;
            }
        }

        public void Iniciar(string nombreUsuario, SerialPort puerto, SemaphoreSlim semaforo, Func<bool> hayMensajePendiente)
        {
            if (Error) return;

            string metadatos = Indice + "|" + nombreUsuario + "|" + Nombre + "|" + Tamano;
            byte[] tramaMeta = Encoding.UTF8.GetBytes(metadatos);
            int longitudMeta = tramaMeta.Length;

            if (longitudMeta > 1019)
            {
                Error = true;
                return;
            }

            DarPrioridadMensaje(hayMensajePendiente);

            string cabeceraStr = "F" + longitudMeta.ToString("D4");
            byte[] cabecera = Encoding.UTF8.GetBytes(cabeceraStr);

            byte[] relleno = new byte[1019 - longitudMeta];
            for (int i = 0; i < relleno.Length; i++) relleno[i] = 64;

            semaforo.Wait();
            try
            {
                puerto.Write(cabecera, 0, 5);
                puerto.Write(tramaMeta, 0, longitudMeta);
                puerto.Write(relleno, 0, relleno.Length);
            }
            finally
            {
                semaforo.Release();
            }
        }

        public bool EnviarBloque(SerialPort puerto, SemaphoreSlim semaforo, Func<bool> hayMensajePendiente)
        {
            if (Completado || Error) return false;
            if (_cancelado) { Error = true; Cerrar(); return false; }

            int bytesPorEnviar = (int)Math.Min(Tamano - _avance, 1019);
            if (bytesPorEnviar <= 0)
            {
                Completado = true;
                EnvioCompletado?.Invoke(this);
                return false;
            }

            DarPrioridadMensaje(hayMensajePendiente);

            string cabeceraStr = bytesPorEnviar == 1019
                ? "A" + Indice.ToString() + "019"
                : "A" + Indice.ToString() + "HHH";
            byte[] cabecera = Encoding.UTF8.GetBytes(cabeceraStr);

            byte[] datos = new byte[1019];
            _lector.Read(datos, 0, bytesPorEnviar);

            semaforo.Wait();
            try
            {
                puerto.Write(cabecera, 0, 5);
                puerto.Write(datos, 0, bytesPorEnviar);
                if (bytesPorEnviar < 1019)
                {
                    byte[] relleno = new byte[1019 - bytesPorEnviar];
                    for (int i = 0; i < relleno.Length; i++) relleno[i] = 64;
                    puerto.Write(relleno, 0, relleno.Length);
                }
            }
            finally
            {
                semaforo.Release();
            }

            _avance += bytesPorEnviar;
            BytesEnviados = _avance;
            ProgresoChanged?.Invoke(this, _avance, Tamano);

            if (_avance >= Tamano)
            {
                Completado = true;
                EnvioCompletado?.Invoke(this);
                return false;
            }

            return true;
        }

        private void DarPrioridadMensaje(Func<bool> hayMensajePendiente)
        {
            if (hayMensajePendiente == null) return;
            for (int i = 0; i < 50 && hayMensajePendiente(); i++)
                Thread.Sleep(2);
        }

        public void Cerrar()
        {
            _lector?.Dispose();
            _lector = null;
            _flujo?.Dispose();
            _flujo = null;
        }
    }
}
