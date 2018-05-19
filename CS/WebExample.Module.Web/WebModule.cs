using System;
using System.ComponentModel;

using DevExpress.ExpressApp;

namespace WebExample.Module.Web
{
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class WebExampleAspNetModule : ModuleBase
    {
        public WebExampleAspNetModule()
        {
            InitializeComponent();
        }
    }
}
