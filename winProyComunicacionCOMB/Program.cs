namespace winProyComunicacion
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using var dialogo = new FolderBrowserDialog();
            dialogo.Description = "Seleccione la carpeta donde se guardarán los archivos recibidos";
            dialogo.SelectedPath = "C:\\REDES1\\";
            if (dialogo.ShowDialog() == DialogResult.OK)
                Form1.DirectorioDescarga = dialogo.SelectedPath;
            else
                Form1.DirectorioDescarga = "C:\\REDES1\\";
            if (!Form1.DirectorioDescarga.EndsWith("\\"))
                Form1.DirectorioDescarga += "\\";

            Form1.PuertoPreferido = "COM11";
            Application.Run(new Form1());
        }
    }
}
