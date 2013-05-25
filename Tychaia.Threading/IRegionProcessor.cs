using System;

namespace Tychaia.Threading
{
    public interface IRegionProcessor<TResult>
    {
        void Process(TaskRegionEntry<TResult> entry);
    }
}

