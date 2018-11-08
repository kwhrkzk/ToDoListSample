using Domain;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class TaskFactory : ITaskFactory
    {
        public Domain.Task Create(string _title) => new Domain.Task(TaskID.NewID(), Title.Create(_title), Explain.Empty, Status.Ready);

        public Domain.Task Replace(Status _status, Domain.Task _task) => new Domain.Task(_task.ID, _task.Title, _task.Explain, _status);
        public Domain.Task Replace(Explain _explain, Domain.Task _task) => new Domain.Task(_task.ID, _task.Title, _explain, _task.Status);
        public Domain.Task Replace(Title _title, Domain.Task _task) => new Domain.Task(_task.ID, _title, _task.Explain, _task.Status);
    }
}
