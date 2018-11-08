using Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Infrastructure
{
    public class TaskRepository : ITaskRepository
    {
        private Tasks tasks = new Tasks();

        private ITaskFactory TaskFactory { get; }

        public TaskRepository(ITaskFactory _taskFactory)
        {
            TaskFactory = _taskFactory;
        }

        public Tasks All() => tasks;

        public void Unshift(Domain.Task _task) => tasks.Insert(0, _task);

        public Domain.Task Get(TaskID _id) => tasks.First(t => t.ID.Equals(_id));

        public void Replace(Domain.Task _task)
        {
            var index = tasks.FindIndex(task => task.ID.Equals(_task.ID));
            if (index != -1)
                tasks[index] = _task;
        }

        public Domain.Task Replace(TaskID _id, Status _status)
        {
            var index = tasks.FindIndex(task => task.ID.Equals(_id));
            tasks[index] = TaskFactory.Replace(_status, tasks[index]);
            return tasks[index];
        }
    }
}
