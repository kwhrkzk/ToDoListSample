using Domain;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class TaskCommandFactory : ITaskCommandFactory
    {
        private ITaskRepository TaskRepository { get; }
        private IMessageBroker MessageBroker { get; }

        public TaskCommandFactory(ITaskRepository _taskRepository, IMessageBroker _messageBroker)
        {
            TaskRepository = _taskRepository;
            MessageBroker = _messageBroker;
        }

        public AddTaskCommand AddTaskCommand(AddTaskCommandParam _param) => new AddTaskCommand(_param, TaskRepository, MessageBroker);
        public EditTaskCommand EditTaskCommand(EditTaskCommandParam _param) => new EditTaskCommand(_param, TaskRepository, MessageBroker);
        public ChangeStatusCommand ChangeStatusCommand(ChangeStatusCommandParam _param) => new ChangeStatusCommand(_param, TaskRepository, MessageBroker);
    }
}
