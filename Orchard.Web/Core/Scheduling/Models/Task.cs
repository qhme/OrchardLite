using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Tasks.Scheduling;

namespace Orchard.Core.Scheduling.Models
{
    /// <summary>
    /// 去除了与内容管理相关的代码，需要测试效果
    /// </summary>
    public class Task : IScheduledTask
    {
        private readonly ScheduledTaskRecord _record;

        public Task(ScheduledTaskRecord record)
        {
            _record = record;
        }

        public string TaskType
        {
            get { return _record.TaskType; }
        }

        public DateTime? ScheduledUtc
        {
            get { return _record.ScheduledUtc; }
        }




      
    }
}