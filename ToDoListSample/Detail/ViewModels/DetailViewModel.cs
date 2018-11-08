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
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListSample.Detail.Models;

namespace ToDoListSample.Detail.ViewModels
{
    public class DetailViewModel : IConfirmNavigationRequest, IRegionMemberLifetime, IDisposable, INotifyPropertyChanged
    {
#pragma warning disable 0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

        public bool KeepAlive => false;

        public ReactiveProperty<string> InputTitle { get; }
        public ReactiveProperty<string> InputExplain { get; }
        public ReactiveProperty<Status> InputStatus { get; }

        public DetailModel Model { get; }

        public DetailViewModel(DetailModel _model)
        {
            Model = _model.AddTo(DisposeCollection);

            InputTitle = Model.ShownTask.Select(t => t?.TitleString ?? "").ToReactiveProperty("", ReactivePropertyMode.DistinctUntilChanged).AddTo(DisposeCollection);
            InputExplain = Model.ShownTask.Select(t => t?.ExplainString ?? "").ToReactiveProperty("", ReactivePropertyMode.DistinctUntilChanged).AddTo(DisposeCollection);
            InputStatus = Model.ShownTask.Select(t => t?.Status ?? Status.Ready).ToReactiveProperty(Status.Ready, ReactivePropertyMode.DistinctUntilChanged).AddTo(DisposeCollection);

            InputTitle.Where(s => (Model.ShownTask.Value?.Title ?? null) != Domain.Title.Create(s)).Subscribe(s => Model.Save(Domain.Title.Create(s))).AddTo(DisposeCollection);
            InputExplain.Where(s => (Model.ShownTask.Value?.Explain ?? null) != Domain.Explain.Create(s)).Subscribe(s => Model.Save(Domain.Explain.Create(s))).AddTo(DisposeCollection);
            InputStatus.Where(s => (Model.ShownTask.Value?.Status ?? null) != s).Subscribe(s => Model.Save(s)).AddTo(DisposeCollection);
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
