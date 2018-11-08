using Domain;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListSample.Input.ViewModels
{
    public class InputViewModel : IConfirmNavigationRequest, IRegionMemberLifetime, IDisposable, INotifyPropertyChanged
    {
#pragma warning disable 0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

        public bool KeepAlive => false;

        private ITaskFactory TaskFactory { get; }
        private ITaskCommandFactory TaskCommandFactory { get; }

        public ReactiveProperty<string> InputTitle { get; } = new ReactiveProperty<string>("");
        public ReactiveCommand SaveCommand { get; } = new ReactiveCommand();

        public InputViewModel(
            ITaskFactory _taskFactory
            , ITaskCommandFactory _taskCommandFactory
            )
        {
            TaskFactory = _taskFactory;
            TaskCommandFactory = _taskCommandFactory;

            SaveCommand.Subscribe(Save).AddTo(DisposeCollection);
        }

        private void Save()
        {
            TaskCommandFactory.AddTaskCommand(new AddTaskCommandParam(TaskFactory.Create(InputTitle.Value))).Execute();
            InputTitle.Value = "";
        }

        private CompositeDisposable DisposeCollection = new CompositeDisposable();
        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed")]
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisposeCollection.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose() => Dispose(true);
        #endregion

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback) => continuationCallback(true);
        public void OnNavigatedTo(NavigationContext navigationContext) { }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
        public void OnNavigatedFrom(NavigationContext navigationContext) => Dispose();
    }
}
