namespace WVDCUS.Service
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.WVDCUSServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.WVDCUSServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // WVDCUSServiceProcessInstaller
            // 
            this.WVDCUSServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.WVDCUSServiceProcessInstaller.Password = null;
            this.WVDCUSServiceProcessInstaller.Username = null;
            // 
            // WVDCUSServiceInstaller
            // 
            this.WVDCUSServiceInstaller.Description = "Provides custom solution for patch management of Windows Virtual Desktop Service " +
    "host";
            this.WVDCUSServiceInstaller.DisplayName = "WVDCUS Service";
            this.WVDCUSServiceInstaller.ServiceName = "WVDCUS";
            this.WVDCUSServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.WVDCUSServiceProcessInstaller,
            this.WVDCUSServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller WVDCUSServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller WVDCUSServiceInstaller;
    }
}