namespace winProyComunicacion
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Form1.PuertoPreferido = "COMA";
            Application.Run(new Form1());
        }
    }
}
