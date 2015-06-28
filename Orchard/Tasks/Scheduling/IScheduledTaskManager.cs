using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Tasks.Scheduling
{
    public interface IScheduledTaskManager : IDependency
    {
        void CreateTask(string taskType, DateTime scheduledUtc);

        IEnumerable<IScheduledTask> GetTasks(string taskType, DateTime? scheduledBeforeUtc = null);

        void DeleteTasks(Func<IScheduledTask, bool> predicate = null);
    }

}
