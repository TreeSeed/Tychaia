// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
namespace Tychaia.Threading
{
    public class TaskPipelineEntry<T>
    {
        public static readonly TaskPipelineEntry<T> Sentinel = new TaskPipelineEntry<T>();

        public TaskPipelineEntry<T> Next;
        public T Value;
    }
}