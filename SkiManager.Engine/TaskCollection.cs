using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkiManager.Engine
{
    /// <summary>
    /// A collection where multiple tasks can be added
    /// and completion of all tasks can be awaited.
    /// </summary>
    public class TaskCollection
    {
        private bool _completionStarted = false;
        private List<Task> _tasks;

        internal Task CompleteAllAsync()
        {
            _completionStarted = true;
            return Task.WhenAll(_tasks);
        }

        public void Add(Task task)
        {
            if (_completionStarted)
                throw new InvalidOperationException("Tasks cannot be added because completion is already awaited");

            _tasks.Add(task);
        }
    }
}
