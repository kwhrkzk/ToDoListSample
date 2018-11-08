using Prism.Events;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ToDoListSample.Detail.Models
{
    public class DetailModel : IDisposable
    {
        private Domain.ITaskRepository TaskRepository { get; }
        private IEventAggregator EventAggregator { get; }
        private Domain.ITaskFactory TaskFactory { get; }
        private Domain.ITaskCommandFactory TaskCommandFactory { get; }

        public ReactivePropertySlim<Domain.Task> ShownTask { get; } = new ReactivePropertySlim<Domain.Task>();

        public DetailModel(
            Domain.ITaskRepository _taskRepository
            , IEventAggregator _eventAggregator
            , Domain.ITaskFactory _taskFactory
            , Domain.ITaskCommandFactory _taskCommandFactory
            )
        {
            TaskRepository = _taskRepository;
            EventAggregator = _eventAggregator;
            TaskFactory = _taskFactory;
            TaskCommandFactory = _taskCommandFactory;

            EventAggregator.GetEvent<SelectedTaskEvent>().Subscribe(ChangeShownTask).AddTo(DisposeCollection);
        }

        public void Save(Domain.Status _status) => TaskCommandFactory.ChangeStatusCommand(new Domain.ChangeStatusCommandParam(ShownTask.Value.ID, _status)).Execute();
        public void Save(Domain.Explain _explain) => TaskCommandFactory.EditTaskCommand(new Domain.EditTaskCommandParam(TaskFactory.Replace(_explain, ShownTask.Value))).Execute();
        public void Save(Domain.Title _title) => TaskCommandFactory.EditTaskCommand(new Domain.EditTaskCommandParam(TaskFactory.Replace(_title, ShownTask.Value))).Execute();

        private void ChangeShownTask(Domain.TaskID _id) => ShownTask.Value = TaskRepository.Get(_id);

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
    }
}
