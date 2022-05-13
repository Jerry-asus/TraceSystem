using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraceSystem.Extension;
using TraceSystem.Models;

namespace TraceSystem.ViewModels
{
    public class MainViewModel:BindableBase, INavigationAware
    {

        private ObservableCollection<MainMenu> menus;
        public DelegateCommand<MainMenu> SelectChangeCommand { get; set; }
        public DelegateCommand GoBackCommand { get; set; }
        public DelegateCommand GoForwardCommad { get; set; }
        public IRegionManager RegionManager { get; }
        private  IRegionNavigationJournal journal;
        public MainViewModel(IRegionManager regionManager)
        {
            Menus = new ObservableCollection<MainMenu>();
            CreateMenus();
            SelectChangeCommand = new DelegateCommand<MainMenu>(OpenView);
            RegionManager = regionManager;

            GoBackCommand = new DelegateCommand(() =>
            {
                if (journal!=null&&journal.CanGoBack)
                {
                    journal.GoBack();
                }
            });
            GoForwardCommad = new DelegateCommand(() =>
            {
                if (journal != null && journal.CanGoForward)
                {
                    journal.GoForward();
                }
            });
        }

        private void GoForwardCMD()
        {
            throw new NotImplementedException();
        }

        private void GoBackCMD()
        {
            throw new NotImplementedException();
        }

        private void OpenView(MainMenu obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.NameSpace)) return;
            RegionManager.Regions[RegionMangerServer.MainReginName].RequestNavigate(obj.NameSpace,back=>
            {
                journal = back.Context.NavigationService.Journal;
            });

            
        }
        /// <summary>
        /// 创建菜单
        /// </summary>
        private void CreateMenus()
        {
            Menus.Add(new MainMenu() { Name = "首页", Icon = "Home", NameSpace = "IndexView" });
            Menus.Add(new MainMenu() { Name = "通讯监控配置",Icon= "AlphaXCircleOutline", NameSpace= "CommView" });
            Menus.Add(new MainMenu() { Name = "数据库配置", Icon = "DatabaseArrowUp", NameSpace = "EFView" });
            Menus.Add(new MainMenu() { Name = "系统设置", Icon = "TuneVertical", NameSpace = "SysView" });
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
       

        public ObservableCollection<MainMenu> Menus
        {
            get { return menus; }
            set { menus = value; }
        }

        
    }
}
