// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Tychaia.Globals
{
    internal class DynamicDictionary : System.Dynamic.DynamicObject, INotifyPropertyChanged
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            var name = binder.Name;
            if(!dictionary.TryGetValue(name, out result))
                result = null;
            return true;
        }
        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        {
            dictionary[binder.Name] = value;
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(binder.Name));
            return true;
        }
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return dictionary.Keys;
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}

