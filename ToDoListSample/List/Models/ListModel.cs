using AppService;
using Domain;
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
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ToDoListSample.List.Models
{
    public class ListViewDTOWrapper : IDisposable
    {
        private ITaskCommandFactory TaskCommandFactory { get; }
        private ListViewDTO DTO { get; }

        public Domain.TaskID TaskID { get; }
        public ReactiveProperty<Domain.Status> Status { get; }
        public Domain.Title Title { get; }

        public ListViewDTOWrapper(ListViewDTO _dto, ITaskCommandFactory _taskCommandFactory)
        {
            TaskCommandFactory = _taskCommandFactory;
            DTO = _dto;
            TaskID = _dto.TaskID;
            Status = new ReactiveProperty<Domain.Status>(_dto.Status, ReactivePropertyMode.DistinctUntilChanged).AddTo(DisposeCollection);
            Title = _dto.Title;

            Status.Subscribe(Save).AddTo(DisposeCollection);
        }

        private void Save(Status _) => TaskCommandFactory.ChangeStatusCommand(new ChangeStatusCommandParam(TaskID, Status.Value)).Execute();

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

    public class ListModel : IDisposable
    {
        private IListViewDTORepository ListViewDTORepository { get; }
        private IMessageBroker MessageBroker { get; }
        private IEventAggregator EventAggregator { get; }
        private ITaskCommandFactory TaskCommandFactory { get; }

        public ReactiveCollection<ListViewDTOWrapper> Tasks { get; } = new ReactiveCollection<ListViewDTOWrapper>();
        public ReactiveProperty<Domain.TaskID> SelectedTask { get; } = new ReactiveProperty<Domain.TaskID>();

        public ListModel(
            IListViewDTORepository _listViewDTORepository
            , IMessageBroker _messageBroker
            , IEventAggregator _eventAggregator
            , ITaskCommandFactory _taskCommandFactory
            )
        {
            ListViewDTORepository = _listViewDTORepository;
            MessageBroker = _messageBroker;
            EventAggregator = _eventAggregator;
            TaskCommandFactory = _taskCommandFactory;

            BindingOperations.EnableCollectionSynchronization(Tasks, new object());

            MessageBroker.Subscribe<AddedTaskEvent>(AddedTask).AddTo(DisposeCollection);
            MessageBroker.Subscribe<EditedTaskEvent>(EditedTask).AddTo(DisposeCollection);
            MessageBroker.Subscribe<ChangedTaskEvent>(ChangedTask).AddTo(DisposeCollection);

            SelectedTask.Where(t => t != null).Subscribe(PublishSelectedTaskEvent).AddTo(DisposeCollection);
        }

        private void PublishSelectedTaskEvent(TaskID _id) => EventAggregator.GetEvent<SelectedTaskEvent>().Publish(_id);

        private void ChangedTask(ChangedTaskEvent e)
        {
            Reflesh();
            SelectedTask.Value = e.Task.ID;
        }

        private void EditedTask(EditedTaskEvent e)
        {
            Reflesh();
            SelectedTask.Value = e.Task.ID;
        }

        private void AddedTask(AddedTaskEvent e)
        {
            Reflesh();
            SelectedTask.Value = e.Task.ID;
        }

        public System.Threading.Tasks.Task InitializeAsync() => System.Threading.Tasks.Task.Run(() => Reflesh());

        private void Reflesh()
        {
            Tasks.ToList().ForEach(t => t.Dispose());
            Tasks.Clear();
            Tasks.AddRange(ListViewDTORepository.All().Select(t => new ListViewDTOWrapper(t, TaskCommandFactory)));
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
    }
}
