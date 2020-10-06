using System;
using System.Threading;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace WVDCUS.Core
{
    public sealed class UpdatesManager
    {
        public bool IsStarted { get; private set; }

        public void Start()
        {
            if (_mainTask != null)
                return;

            try
            {
                Logger.WriteInfo("Updates Manager Starting ....");

                _mainTask = Task.Run(() =>
                {
                    try
                    {
                        ReadConfiguration();

                        IsStarted = true;

                        for (; ; )
                        {
                            CheckInstallUpdates();

                            try { Task.Delay(CheckForUpdatesIntervalHours * 60 * 60 * 1000, _cts.Token).Wait(); }
                            catch (Exception) { }

                            if (_cts.IsCancellationRequested)
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.WriteException(e);
                        StopAndCleanTask(true);
                    }
                });

                Logger.WriteInfo("Updates Manager Started");
            }
            catch (Exception e)
            {
                Logger.WriteException(e);
                StopAndCleanTask();
            }
        }

        public void Stop()
        {
            if (_mainTask == null)
                return;

            try
            {
                Logger.WriteInfo("Updates Manager Stopping ....");

                StopAndCleanTask();

                Logger.WriteInfo("Updates Manager Stopped");
            }
            catch (Exception e)
            {
                Logger.WriteException(e);
            }
        }

        private void StopAndCleanTask(bool taskSelfCleanup = false)
        {
            if (_mainTask != null)
            {
                if (!_mainTask.IsCompleted)
                {
                    _cts.Cancel();
                    if (!taskSelfCleanup)
                        _mainTask.Wait();
                }

                if (_mainTask.IsCompleted)
                    _mainTask.Dispose();
                _mainTask = null;

                IsStarted = false;
            }
        }

        // https://ourcodeworld.com/articles/read/886/how-to-determine-whether-windows-update-is-enabled-or-not-with-c-in-winforms
        private void CheckInstallUpdates()
        {
            Logger.WriteInfo("Updates Starting ....");

            using (var telemetrySession = new UpdatesManagerTelemetrySession(TelemetryKey))
            {
                try
                {
                    new WUApiManager().InstallUpdates(telemetrySession, UpdatesSearchQuery);
                }
                catch (Exception e)
                {
                    Logger.WriteException(e);
                    telemetrySession.SendInfoException(e);
                }
            }

            Logger.WriteInfo("Updates Finished");
        }

        private void ReadConfiguration()
        {
            Logger.WriteInfo("Reading configuration ....");

            var appSettings = ConfigurationManager.AppSettings;

            ReadCheckForUpdatesIntervalHours(appSettings);
            ReadTelemetryKey(appSettings);
            ReadUpdatesSearchQuery(appSettings);

            Logger.WriteInfo("The value 'CheckForUpdatesIntervalHours' = '{0}'.", CheckForUpdatesIntervalHours);
            Logger.WriteInfo("The value 'TelemetryKey' = '{0}'.", TelemetryKey);
            Logger.WriteInfo("The value 'UpdatesSearchQuery' = '{0}'.", UpdatesSearchQuery);

            Logger.WriteInfo("Reading configuration Finished");
        }

        private void ReadCheckForUpdatesIntervalHours(NameValueCollection appSettings)
        {
            int checkForUpdatesIntervalHoursTemp;
            var checkForUpdatesIntervalHoursString = appSettings.Get("CheckForUpdatesIntervalHours");
            if (!Int32.TryParse(checkForUpdatesIntervalHoursString, out checkForUpdatesIntervalHoursTemp))
            {
                checkForUpdatesIntervalHoursTemp = CheckForUpdatesIntervalHoursDefault;
                Logger.WriteInfo("Unable to read value 'CheckForUpdatesIntervalHours', defaulting to value: '{0}'.", checkForUpdatesIntervalHoursTemp);
            }

            if (checkForUpdatesIntervalHoursTemp < CheckForUpdatesIntervalHoursMin || checkForUpdatesIntervalHoursTemp > CheckForUpdatesIntervalHoursMax)
            {
                checkForUpdatesIntervalHoursTemp = CheckForUpdatesIntervalHoursDefault;
                Logger.WriteInfo("The value 'CheckForUpdatesIntervalHours' must be between '{0}' and '{1}', defaulting to value: '{2}'.",
                    CheckForUpdatesIntervalHoursMin, CheckForUpdatesIntervalHoursMax, checkForUpdatesIntervalHoursTemp);
            }

            CheckForUpdatesIntervalHours = checkForUpdatesIntervalHoursTemp;
        }

        private void ReadTelemetryKey(NameValueCollection appSettings)
        {
            const string ErrorMsg = "Unable to read value 'TelemetryKey'. See README.md how to obtain valid telemetry key.";

            Guid telemetryKeyGuid;
            var telemetryKey = appSettings.Get("TelemetryKey");
            if (String.IsNullOrWhiteSpace(telemetryKey) || !Guid.TryParse(telemetryKey, out telemetryKeyGuid))
                throw new ConfigurationErrorsException(ErrorMsg);

            if (telemetryKeyGuid == Guid.Empty)
                throw new ConfigurationErrorsException(ErrorMsg);

            TelemetryKey = telemetryKey;
        }

        private void ReadUpdatesSearchQuery(NameValueCollection appSettings)
        {
            var updatesSearchQuery = appSettings.Get("UpdatesSearchQuery");
            if (String.IsNullOrWhiteSpace(updatesSearchQuery))
            {
                updatesSearchQuery = UpdatesSearchQueryDefault;
                Logger.WriteInfo("Unable to read value 'UpdatesSearchQuery', defaulting to value: '{0}'.", updatesSearchQuery);
            }

            UpdatesSearchQuery = updatesSearchQuery;
        }

        private int CheckForUpdatesIntervalHours { get; set; } = CheckForUpdatesIntervalHoursDefault;

        private const int CheckForUpdatesIntervalHoursDefault = 4;
        private const int CheckForUpdatesIntervalHoursMin = 1;
        private const int CheckForUpdatesIntervalHoursMax = 590; // Month has about 730 hours - we need to be able to fit into int type

        private string TelemetryKey { get; set; } = TelemetryKeyDefault;
        private const string TelemetryKeyDefault = null;

        private string UpdatesSearchQuery { get; set; } = UpdatesSearchQueryDefault;
        private const string UpdatesSearchQueryDefault = "IsInstalled=0 AND IsPresent=0 AND CategoryIDs contains 'E6CF1350-C01B-414D-A61F-263D14D133B4' OR CategoryIDs contains 'E0789628-CE08-4437-BE74-2495B842F43B' OR CategoryIDs contains '0FA1201D-4330-4FA8-8AE9-B877473B6441'";

        private Task _mainTask = null;
        private CancellationTokenSource _cts = new CancellationTokenSource();
    }
}
