namespace EuroOrderServices
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
            this.serviceProcessInstallerEuro = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerEuro = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerEuro
            // 
            this.serviceProcessInstallerEuro.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerEuro.Password = null;
            this.serviceProcessInstallerEuro.Username = null;
            // 
            // serviceInstallerEuro
            // 
            this.serviceInstallerEuro.ServiceName = "EuroOrderServices";
            this.serviceInstallerEuro.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerEuro,
            this.serviceInstallerEuro});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerEuro;
        private System.ServiceProcess.ServiceInstaller serviceInstallerEuro;
    }
}