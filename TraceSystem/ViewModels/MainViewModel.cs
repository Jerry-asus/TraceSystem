using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trace.Industry.Extenal;
using TraceSystem.Models;

namespace TraceSystem.ViewModels
{
    public class MainViewModel:BindableBase, INavigationAware
    {
        public MainViewModel(IRegionManager regionManager)
        {
            Menus = new ObservableCollection<MainMenu>();
            CreateMenus();
            SelectChangeCommand = new DelegateCommand<MainMenu>(OpenView);
            RegionManager = regionManager;
        }

        private void OpenView(MainMenu obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.NameSpace)) return;
            RegionManager.Regions[RegionMangerServer.MainReginName].RequestNavigate(obj.NameSpace);
        }
        /// <summary>
        /// 创建菜单
        /// </summary>
        private void CreateMenus()
        {
            Menus.Add(new MainMenu() { Name = "首页", Icon = "Home", NameSpace = "IndexView" });
            Menus.Add(new MainMenu() { Name = "OPCDa客户端",Icon="Home",NameSpace="OPCClientView" });
            Menus.Add(new MainMenu() { Name = "西门子PLC", Icon = "Home", NameSpace = "SIEView" });
            Menus.Add(new MainMenu() { Name = "罗克韦尔PLC", Icon = "Home", NameSpace = "EIPView" });
            Menus.Add(new MainMenu() { Name = "数据库配置", Icon = "Home", NameSpace = "EFView" });
            Menus.Add(new MainMenu() { Name = "系统设置", Icon = "Home", NameSpace = "SysView" });
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 菜单集合
        /// </summary>
        private ObservableCollection<MainMenu> menus;

        public ObservableCollection<MainMenu> Menus
        {
            get { return menus; }
            set { menus = value; }
        }


        public DelegateCommand<MainMenu> SelectChangeCommand { get; set; }
        public IRegionManager RegionManager { get; }
    }
}
