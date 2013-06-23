//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;

namespace Tychaia.Assets
{
    public class TextAsset : MarshalByRefObject, IAsset
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public TextAsset(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T) == typeof(TextAsset))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to TextAsset.");
        }
    }
}

