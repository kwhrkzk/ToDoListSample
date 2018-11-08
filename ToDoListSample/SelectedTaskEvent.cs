using Domain;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListSample
{
    public class SelectedTaskEvent : PubSubEvent<TaskID>
    {
    }
}
