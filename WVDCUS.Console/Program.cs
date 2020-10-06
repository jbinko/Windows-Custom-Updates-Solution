using System;

using WVDCUS.Core;

namespace WVDCUS.Console
{
    internal sealed class Program
    {
        static void Main()
        {
            try
            {
                System.Console.WriteLine("Press any key to stop the service");
                System.Console.WriteLine();

                var updatesManager = new UpdatesManager();
                updatesManager.Start();

                // Run until any key pressed
                System.Console.ReadKey();

                updatesManager.Stop();
            }
            catch (Exception e)
            {
                Logger.WriteException(e);
            }
        }
    }
}
