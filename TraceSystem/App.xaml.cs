using Prism.DryIoc;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TraceSystem.CommCore.OPCDa;
using TraceSystem.ViewModels;
using TraceSystem.Views;

namespace TraceSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
           return Container.Resolve<MainView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
           // containerRegistry.RegisterForNavigation<Mainview, MainViewModel>();
            containerRegistry.RegisterForNavigation<IndexView, IndexViewModel>();
            containerRegistry.RegisterForNavigation<EFConnfigView, EFViewModel>();
            containerRegistry.RegisterForNavigation<EIPView, EIPViewModel>();
            containerRegistry.RegisterForNavigation<OPCClientView, OPCClientViewModel>();
            containerRegistry.RegisterForNavigation<SIEView, SIEViewModel>();
            containerRegistry.RegisterForNavigation<SysView, SysViewModel>();

            containerRegistry.Register<IOPCClient, OpcClient>();


        }
    }
}
