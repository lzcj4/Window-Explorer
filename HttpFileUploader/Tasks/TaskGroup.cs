using System.Collections.Generic;

namespace HttpFileUploader.Tasks
{
    public class TaskGroup
    {
        public string Name { get; set; }

        public string FilePath { get; set; }
        public IList<ITask> Tasks { get; private set; }

        public TaskGroup()
        {
            this.Tasks = new List<ITask>();
        }
    }
}
