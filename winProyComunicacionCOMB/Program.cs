namespace winProyComunicacion
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Form1.PuertoPreferido = "COM11";
            Application.Run(new Form1());
        }
    }
}
