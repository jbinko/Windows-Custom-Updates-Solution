using System;
using System.ServiceProcess;

using WVDCUS.Core;

namespace WVDCUS.Service
{
    static internal class Program
    {
        static void Main()
        {
            try
            {
                ServiceBase.Run(new[] { new WVDCUSService() });
            }
            catch (Exception e)
            {
                Logger.WriteException(e);
            }
        }
    }
}
