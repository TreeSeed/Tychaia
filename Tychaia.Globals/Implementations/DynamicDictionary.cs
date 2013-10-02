// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Tychaia.Globals
{
    internal class DynamicDictionary : System.Dynamic.DynamicObject, INotifyPropertyChanged
    {
        private Dictionary<string, object> dictionary = new Dictionary<string, object>();

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        
        public int Count
        {
            get
            {
                return this.dictionary.Count;
            }
        }
        
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            var name = binder.Name;
            if (!this.dictionary.TryGetValue(name, out result))
                result = null;
            return true;
        }
        
        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        {
            this.dictionary[binder.Name] = value;
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(binder.Name));
            return true;
        }
        
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.dictionary.Keys;
        }
    }
}
