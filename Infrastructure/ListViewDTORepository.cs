using AppService;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ListViewDTORepository : IListViewDTORepository
    {
        private ITaskRepository TaskRepository { get; }

        public ListViewDTORepository(ITaskRepository _taskRepository)
        {
            TaskRepository = _taskRepository;
        }

        public IEnumerable<ListViewDTO> All() => TaskRepository.All().Select(t => new ListViewDTO(t.ID, t.Title, t.Status));
    }
}
