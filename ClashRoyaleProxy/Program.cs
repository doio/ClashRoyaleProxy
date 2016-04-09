using System;

namespace ClashRoyaleProxy
{
    class Program
    {
        static void Main()
        {
            // Check whether the proxy runs more than once
            if (Helper.OpenedInstances > 1)
            {
                Logger.Log("You seem to run this proxy more than once.", LogType.WARNING);
                Logger.Log("Aborting..", LogType.WARNING);
                System.Threading.Thread.Sleep(3500);
                Environment.Exit(0);
            }

            // UI
            Console.Title = "Clash Royale Proxy " + Helper.AssemblyVersion + " | © " + DateTime.UtcNow.Year;
            Console.SetCursorPosition(0, 0);
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Proxy
            Proxy.Start();
        }
    }
}