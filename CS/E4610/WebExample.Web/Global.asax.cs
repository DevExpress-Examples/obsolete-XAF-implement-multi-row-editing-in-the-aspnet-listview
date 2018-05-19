using System;
using System.Configuration;
using System.Globalization;
using System.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Web.ASPxClasses;

namespace WebExample.Web {
    public class Global : System.Web.HttpApplication {
        public Global() {
            InitializeComponent();
        }
        protected void Application_Start(object sender, EventArgs e) {
            RenderHelper.RenderMode = DevExpress.Web.ASPxClasses.ControlRenderMode.Lightweight;
            ASPxWebControl.CallbackError += new EventHandler(Application_Error);
#if DEBUG
            TestScriptsManager.EasyTestEnabled = true;
#endif
        }
        protected void Session_Start(object sender, EventArgs e) {
            WebApplication.SetInstance(Session, new WebExampleAspNetApplication());
            InMemoryDataStoreProvider.Register();
            WebApplication.Instance.ConnectionString = InMemoryDataStoreProvider.ConnectionString;
            WebApplication.Instance.Setup();
            WebApplication.Instance.Start();
        }
        protected void Application_BeginRequest(object sender, EventArgs e) {
            string filePath = HttpContext.Current.Request.PhysicalPath;
            if(!string.IsNullOrEmpty(filePath)
                && (filePath.IndexOf("Images") >= 0) && !System.IO.File.Exists(filePath)) {
                HttpContext.Current.Response.End();
            }
        }
        protected void Application_EndRequest(object sender, EventArgs e) {
        }
        protected void Application_AuthenticateRequest(object sender, EventArgs e) {
        }
        protected void Application_Error(object sender, EventArgs e) {
            ErrorHandling.Instance.ProcessApplicationError();
        }
        protected void Session_End(object sender, EventArgs e) {
            WebApplication.DisposeInstance(Session);
        }
        protected void Application_End(object sender, EventArgs e) {
        }

        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
        #endregion
    }
}
