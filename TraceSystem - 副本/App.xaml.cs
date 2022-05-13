using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using SciChart.Charting.Visuals;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TraceSystem.CommCore.OPCDa;
using TraceSystem.CommCore.S7Core;
using TraceSystem.Context;
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
            SciChartSurface.SetRuntimeLicenseKey(@"2wm36aBC+DCauadgsoU7eO6FU/WLg4SJelM1d6JZcg8rn7HuOIuKlo5xcYbJYtwQuNXymNffgQcBuLIPN9UsRMRMjk9lk0XB+DtEX/PrJd4E1uBme8aA0GMUgmsFU35V4GrLnpGJsD23KM+aECxjt+6qBhYIP9zyaUvdrQOm3BCmoMailDOIixdPx8+yfn9pNBvG+Jac80bsn36+F9MpdJfGAfjY+qMoDSGv7+9SvuLrGV3FEOXY4AT3Yd/pFeCdbd4nPEQVfTZlakWeBFo6rjVumGIEM5UrZlg7+f+PcyDNbTTypA631ZcfehvfvQYK+pSZjA9vUc6dzjovSNfq2FVTo1EJqBIxZSEkSRFcXgVSHEfhfmTf0RlSDSwZT8lcBuULzuu7KJ1hU9DH6Xl7Wul8i8WAPXG5r5kDdjhx0u7CKkEynTODIH52ObAtURnVoFEvcJm/u7Rsyh3zJZO779Mtm/qgn9wOWE7aMEEm24C9I+yMDX9l+4pvxrrzZ9UA4vM5syaWwe8Ga53PZUXEAAuo1h00Ma98NxsomdRieOOkQQ8amDjuFX6xOxjMLM/UFjtfCl75BCGDslkSlcQH7xmz/JSfpg==");
            return Container.Resolve<MainView>();

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

            
            // containerRegistry.RegisterForNavigation<Mainview, MainViewModel>();
            containerRegistry.RegisterForNavigation<IndexView, IndexViewModel>();

            containerRegistry.RegisterForNavigation<SysView, SysViewModel>();
            containerRegistry.RegisterForNavigation<CommView, CommViewModel>();


            containerRegistry.RegisterSingleton<IOPCClient, OpcClient>();
            containerRegistry.RegisterSingleton<Is7Client, S7Client>();


        }

      

    }
}
