using System;

using WUApiLib;

namespace WVDCUS.Core
{
    internal sealed class WUApiManager
    {
        public void InstallUpdates(IUpdatesManagerTelemetrySession telemetry, string updatesSearchQuery)
        {
            if (ListAvailableUpdates(telemetry, updatesSearchQuery) > 0)
            {
                EnableUpdateServices(telemetry);
                InstallUpdates(DownloadUpdates(telemetry, updatesSearchQuery), telemetry);
            }
        }

        private int ListAvailableUpdates(IUpdatesManagerTelemetrySession telemetry, string updatesSearchQuery)
        {
            Logger.WriteInfo("Available Updates Listing ....");

            var updateSession = new UpdateSession();
            var updateSearchResult = updateSession.CreateUpdateSearcher();
            updateSearchResult.Online = true;//checks for updates online
            var searchResults = updateSearchResult.Search(updatesSearchQuery);

            foreach (IUpdate update in searchResults.Updates)
            {
                Logger.WriteInfo("Available Update: '{0}'", update.Title);
                telemetry.SendInfoUpdateAvailable(update);
            }

            if (searchResults.Updates.Count == 0)
            {
                Logger.WriteInfo("No Updates Available");
                telemetry.SendInfoNoUpdatesAvailable();
            }

            Logger.WriteInfo("Available Updates Listed");
            return searchResults.Updates.Count;
        }

        private static void EnableUpdateServices(IUpdatesManagerTelemetrySession telemetry)
        {
            IAutomaticUpdates updates = new AutomaticUpdates();
            if (!updates.ServiceEnabled)
            {
                Logger.WriteInfo("Automatic Updates Enabling ....");
                updates.EnableService();
                Logger.WriteInfo("Automatic Updates Enabled");
                telemetry.SendInfoAutomaticUpdatesEnabled();
            }
        }

        private static UpdateCollection DownloadUpdates(IUpdatesManagerTelemetrySession telemetry, string updatesSearchQuery)
        {
            Logger.WriteInfo("Updates Downloading ....");
            telemetry.SendInfoUpdatesDownloading();

            UpdateSession updateSession = new UpdateSession();
            IUpdateSearcher searchUpdates = updateSession.CreateUpdateSearcher();
            ISearchResult updateSearchResult = searchUpdates.Search(updatesSearchQuery);
            UpdateCollection updateCollection = new UpdateCollection();
            for (int i = 0; i < updateSearchResult.Updates.Count; i++)
            {
                IUpdate updates = updateSearchResult.Updates[i];
                if (updates.EulaAccepted == false)
                    updates.AcceptEula();
                updateCollection.Add(updates);
            }

            if (updateSearchResult.Updates.Count > 0)
            {
                UpdateCollection downloadCollection = new UpdateCollection();
                UpdateDownloader downloader = updateSession.CreateUpdateDownloader();

                for (int i = 0; i < updateCollection.Count; i++)
                    downloadCollection.Add(updateCollection[i]);

                downloader.Updates = downloadCollection;

                IDownloadResult downloadResult = downloader.Download();
                UpdateCollection installCollection = new UpdateCollection();
                for (int i = 0; i < updateCollection.Count; i++)
                {
                    if (downloadCollection[i].IsDownloaded)
                    {
                        installCollection.Add(downloadCollection[i]);
                    }
                }

                Logger.WriteInfo("Updates Downloaded");
                telemetry.SendInfoUpdatesDownloaded();
                return installCollection;
            }

            Logger.WriteInfo("Updates Downloaded");
            telemetry.SendInfoUpdatesDownloaded();
            return updateCollection;
        }

        private static void InstallUpdates(UpdateCollection downloadedUpdates, IUpdatesManagerTelemetrySession telemetry)
        {
            Logger.WriteInfo("Updates Installing ....");
            telemetry.SendInfoUpdatesInstalling();

            UpdateSession updateSession = new UpdateSession();
            UpdateInstaller installAgent = updateSession.CreateUpdateInstaller() as UpdateInstaller;
            installAgent.Updates = downloadedUpdates;

            if (downloadedUpdates.Count <= 0)
            {
                Logger.WriteInfo("Updates Installed - No Updates");
                telemetry.SendInfoUpdatesInstalledNoUpdates();
                return;
            }

            IInstallationResult installResult = installAgent.Install();
            if (installResult.ResultCode != OperationResultCode.orcSucceeded)
            {
                Logger.WriteInfo("Updates NOT Installed - Failed");
                telemetry.SendInfoUpdatesNotInstalledFailed();
                return;
            }

            if (installResult.RebootRequired == true)
            {
                Logger.WriteInfo("Updates Installed - Reboot Required");
                telemetry.SendInfoUpdatesInstalledRebootRequired();
                return;
            }

            Logger.WriteInfo("Updates Installed - All OK");
            telemetry.SendInfoUpdatesInstalledAllOK();
        }
    }
}
