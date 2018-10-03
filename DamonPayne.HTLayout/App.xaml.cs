using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DamonPayne.HTLayout.Services;
using DamonPayne.AGT.Design.Services;
using DamonPayne.AGT.Design.Contracts;
using DamonPayne.AGT;
using DamonPayne.AG.Core;
using DamonPayne.AG.Core.DataTypes;
using DamonPayne.AG.Core.Events;
using DamonPayne.AG.Core.Modules;

namespace DamonPayne.HTLayout
{
    public partial class App : Application
    {

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {            
            var container = ContainerHelper.I;
            container.RegisterType<INameProvider, TypeScopedNameProvider>();
            container.RegisterType<IDesignableControlInspector, DefaultInspector>();
            container.RegisterType<IDesignTypeCreator, DefaultDesignTypeCreator>();
            container.RegisterInstance<ILogService>(new MemoryLogger());
            var mainPage = new MainPage();
            RootVisual = mainPage;
            container.RegisterInstance<IRegionManager>(mainPage);
            container.RegisterInstance<IDragTypeCreator>(new DefaultDragTypeCreator());
            
            var dragDrpManager = new DefaultDragDropManager();
            container.BuildUp(dragDrpManager);
            container.RegisterInstance<IDragDropManager>(dragDrpManager);
            
            container.RegisterType<IKeyboardService, CrossPlatformKeyboardService>();
            var selSvc = new DefaultSelectionService();
            container.BuildUp(selSvc);
            container.RegisterInstance<ISelectionService>(selSvc);            
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;

                try
                {
                    string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                    errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");
                    System.Windows.Browser.HtmlPage.Window.Alert("Unhandled Error in Silverlight 2 Application " + errorMsg);
                    ILogService svc = ContainerHelper.I.Resolve<ILogService>();
                    svc.Log( new LogMessage{
                        Level = LogLevels.Error,
                        Message = errorMsg
                    });
                    //System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight 2 Application " + errorMsg + "\");");
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
