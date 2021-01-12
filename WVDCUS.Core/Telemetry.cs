using System;
using System.Net;
using System.Collections.Generic;

using Microsoft.ApplicationInsights;

namespace WVDCUS.Core
{
    internal sealed class UpdatesManagerTelemetrySession : IUpdatesManagerTelemetrySession
    {
        public UpdatesManagerTelemetrySession(string telemetryKey)
        {
            _telemetry = new TelemetryClient(new Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration(telemetryKey));
            _telemetry.Context.Component.Version = "1.0.0.0";
            _telemetry.Context.Operation.Id = Guid.NewGuid().ToString("D");
            _telemetry.Context.Operation.Name = "WVDCUS.Event";

            SendTelemetrySessionHeader();
        }

        private void SendTelemetrySessionHeader()
        {
            var imdsDataTask = IMDSClient.GetInfoAsync();
            imdsDataTask.Wait();
            var imdsData = imdsDataTask.Result;

            string vmResourceName = null;
            string resourceGroupName = null;
            string subscriptionId = null;

            // If this runs in Azure, it should not be null
            if (imdsData != null)
            {
                vmResourceName = imdsData.VMResourceName;
                resourceGroupName = imdsData.ResourceGroupName;
                subscriptionId = imdsData.SubscriptionId;
            }

            var machineName = Environment.MachineName;
            Send("TelemetryStart", machineName,
                Dns.GetHostEntry(machineName).AddressList[0].ToString(),
                Environment.OSVersion.VersionString, Environment.Is64BitOperatingSystem,
                vmResourceName, resourceGroupName, subscriptionId);
        }

        public void SendInfoNoUpdatesAvailable()
        {
            Send("NoUpdatesAvailable");
        }

        public void SendInfoUpdateAvailable(WUApiLib.IUpdate update)
        {
            Send("UpdateAvailable", update.Title);
        }

        public void SendInfoAutomaticUpdatesEnabled()
        {
            Send("AutomaticUpdatesEnabled");
        }

        public void SendInfoUpdatesDownloading()
        {
            Send("UpdatesDownloading");
        }

        public void SendInfoUpdatesDownloaded()
        {
            Send("UpdatesDownloaded");
        }

        public void SendInfoUpdatesInstalling()
        {
            Send("UpdatesInstalling");
        }

        public void SendInfoUpdatesInstalledNoUpdates()
        {
            Send("UpdatesInstalledNoUpdates");
        }

        public void SendInfoUpdatesNotInstalledFailed()
        {
            Send("UpdatesNotInstalledFailed");
        }

        public void SendInfoUpdatesInstalledRebootRequired()
        {
            Send("UpdatesInstalledRebootRequired");
        }

        public void SendInfoUpdatesInstalledAllOK()
        {
            Send("UpdatesInstalledAllOK");
        }

        public void SendInfoException(Exception e)
        {
            Send("Error", e);
        }

        private void Send(string eventName)
        {
            _telemetry.TrackEvent(eventName);
        }

        private void Send(string eventName, string machineName, string ip, string osVersion, bool is64BitOS,
            string vmResourceName, string resourceGroupName, string subscriptionId)
        {
            var properties = new Dictionary<string, string>();
            properties.Add("MachineName", machineName);
            properties.Add("IPAddress", ip);
            properties.Add("OSVersion", osVersion);
            properties.Add("Is64BitOS", is64BitOS.ToString());

            if (!String.IsNullOrWhiteSpace(vmResourceName))
                properties.Add("VMResourceName", vmResourceName);
            if (!String.IsNullOrWhiteSpace(resourceGroupName))
                properties.Add("ResourceGroupName", resourceGroupName);
            if (!String.IsNullOrWhiteSpace(subscriptionId))
                properties.Add("SubscriptionId", subscriptionId);

            _telemetry.TrackEvent(eventName, properties);
        }

        private void Send(string eventName, string updateTitle)
        {
            var properties = new Dictionary<string, string>();
            properties.Add("UpdateTitle", updateTitle);
            _telemetry.TrackEvent(eventName, properties);
        }

        private void Send(string eventName, Exception e)
        {
            var properties = new Dictionary<string, string>();
            properties.Add("Exception", e.ToString());
            properties.Add("ExceptionMessage", e.Message);
            _telemetry.TrackEvent(eventName, properties);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _telemetry.Flush();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        private TelemetryClient _telemetry = null;
    }

    internal interface IUpdatesManagerTelemetrySession : IDisposable
    {
        void SendInfoNoUpdatesAvailable();
        void SendInfoUpdateAvailable(WUApiLib.IUpdate update);
        void SendInfoAutomaticUpdatesEnabled();

        void SendInfoUpdatesDownloading();
        void SendInfoUpdatesDownloaded();
        void SendInfoUpdatesInstalling();

        void SendInfoUpdatesInstalledNoUpdates();
        void SendInfoUpdatesNotInstalledFailed();
        void SendInfoUpdatesInstalledRebootRequired();
        void SendInfoUpdatesInstalledAllOK();

        void SendInfoException(Exception e);
    }
}
