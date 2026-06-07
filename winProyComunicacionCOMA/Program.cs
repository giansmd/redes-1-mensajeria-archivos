namespace winProyComunicacion
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Form1.PuertoPreferido = "COM10";
            Application.Run(new Form1());
        }
    }
}
