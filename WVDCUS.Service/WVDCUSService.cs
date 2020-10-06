using System;
using System.ServiceProcess;

using WVDCUS.Core;

namespace WVDCUS.Service
{
    public sealed partial class WVDCUSService : ServiceBase
    {
        public WVDCUSService()
        {
            try
            {
                InitializeComponent();
                _updatesManager = new UpdatesManager();
            }
            catch (Exception e)
            {
                Logger.WriteException(e);
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Logger.WriteInfo("WVDCUS Service Starting ....");
                _updatesManager.Start();
                Logger.WriteInfo("WVDCUS Service Started");
            }
            catch (Exception e)
            {
                Logger.WriteException(e);
            }
        }

        protected override void OnStop()
        {
            try
            {
                Logger.WriteInfo("WVDCUS Service Stopping ....");
                _updatesManager.Stop();
                Logger.WriteInfo("WVDCUS Service Stopped");
            }
            catch (Exception e)
            {
                Logger.WriteException(e);
            }
        }

        private UpdatesManager _updatesManager = null;
    }
}
