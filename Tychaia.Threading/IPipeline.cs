//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

namespace Tychaia.Threading
{
    public interface IPipeline<T>
    {
        void InputConnect();
        void InputDisconnect();
        void OutputConnect();
        void OutputDisconnect();
        void Put(T value);
        T Take();
        T Take(out bool retrieved);
    }
}

