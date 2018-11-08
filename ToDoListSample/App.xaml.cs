using AppService;
using Domain;
using Infrastructure;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Regions;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ToDoListSample.Detail.Views;
using ToDoListSample.Input.Views;

namespace ToDoListSample
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell() => new MainWindow();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Container.Resolve<IRegionManager>().RequestNavigate("ListRegion", nameof(List.Views.ListView));
            Container.Resolve<IRegionManager>().RequestNavigate("InputRegion", nameof(InputView));
            Container.Resolve<IRegionManager>().RequestNavigate("DetailRegion", nameof(DetailView));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ITaskRepository, TaskRepository>();
            containerRegistry.RegisterSingleton<ITaskFactory, Infrastructure.TaskFactory>();
            containerRegistry.RegisterSingleton<ITaskCommandFactory, TaskCommandFactory>();
            containerRegistry.RegisterSingleton<IListViewDTORepository, ListViewDTORepository>();
            containerRegistry.RegisterInstance(MessageBroker.Default);

            containerRegistry.RegisterForNavigation<List.Views.ListView>();
            containerRegistry.RegisterForNavigation<InputView>();
            containerRegistry.RegisterForNavigation<DetailView>();
        }
    }
}
