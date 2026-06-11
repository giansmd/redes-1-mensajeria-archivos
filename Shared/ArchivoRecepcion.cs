namespace winProyComunicacion
{
    internal class ArchivoRecepcion
    {
        public int Indice { get; }
        public string NombreOriginal { get; }
        public string NombreReal { get; set; }
        public long TamanoTotal { get; }
        public long BytesRecibidos { get; set; }
        public string UsuarioRemitente { get; }
        public FileStream FlujoEscritura { get; private set; }
        public BinaryWriter Escritor { get; private set; }
        public bool Completado => BytesRecibidos >= TamanoTotal;

        public ArchivoRecepcion(int indice, string usuario, string nombreOriginal, long tamano, string directorio, string nombreReal)
        {
            Indice = indice;
            UsuarioRemitente = usuario;
            NombreOriginal = nombreOriginal;
            NombreReal = nombreReal;
            TamanoTotal = tamano;
            BytesRecibidos = 0;

            string ruta = Path.Combine(directorio, nombreReal);
            FlujoEscritura = new FileStream(ruta, FileMode.Create, FileAccess.Write);
            Escritor = new BinaryWriter(FlujoEscritura);
        }

        public void EscribirDatos(byte[] buffer, int offset, int count)
        {
            Escritor.Write(buffer, offset, count);
            BytesRecibidos += count;
        }

        public void Cerrar()
        {
            Escritor?.Dispose();
            Escritor = null;
            FlujoEscritura?.Dispose();
            FlujoEscritura = null;
        }
    }
}
