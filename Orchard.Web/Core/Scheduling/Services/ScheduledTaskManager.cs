using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Core.Scheduling.Models;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Tasks.Scheduling;
using Orchard.Utility.Extensions;

namespace Orchard.Core.Scheduling.Services
{
    public class ScheduledTaskManager : IScheduledTaskManager
    {
        private readonly IRepository<ScheduledTaskRecord> _repository;

        public ScheduledTaskManager(
             IRepository<ScheduledTaskRecord> repository)
        {
            _repository = repository;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void CreateTask(string action, DateTime scheduledUtc)
        {
            var taskRecord = new ScheduledTaskRecord
            {
                TaskType = action,
                ScheduledUtc = scheduledUtc,
            };

            _repository.Create(taskRecord);
        }


        public IEnumerable<IScheduledTask> GetTasks(string taskType, DateTime? scheduledBeforeUtc = null)
        {
            var query = scheduledBeforeUtc != null
                ? _repository.Fetch(t => t.TaskType == taskType && t.ScheduledUtc <= scheduledBeforeUtc)
                : _repository.Fetch(t => t.TaskType == taskType);

            return
                query.Select(x => new Task(x))
                .Cast<IScheduledTask>()
                .ToReadOnlyCollection();
        }

        public void DeleteTasks(Func<IScheduledTask, bool> predicate = null)
        {
            // if contentItem is null, all tasks are used
            var tasks = _repository.Table;

            foreach (var task in tasks)
            {
                if (predicate == null || predicate(new Task(task)))
                {
                    _repository.Delete(task);
                }
            }

            _repository.Flush();
        }
    }
}
