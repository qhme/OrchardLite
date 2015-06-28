using System;
using Orchard.ContentManagement.Records;

namespace Orchard.Tasks.Scheduling
{
    public interface IScheduledTask
    {
        string TaskType { get; }
        DateTime? ScheduledUtc { get; }
    }
}
