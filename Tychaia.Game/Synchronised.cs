using System;
using Dx.Runtime;

namespace Tychaia.Game
{
    public class Synchronised
    {
        private bool m_Networked;
        private ILocalNode m_Node;
        private string m_Name;
        private bool m_Authoritive;
    
        public void Connect(
            ILocalNode node,
            string name,
            bool authoritive)
        {
            if (this.m_Networked)
                throw new InvalidOperationException();
            this.m_Networked = true;
            this.m_Node = node;
            this.m_Name = name;
            this.m_Authoritive = authoritive;
        }
        
        public void Update()
        {
            if (!this.m_Networked)
                return;
            this.m_Node.Synchronise(
                this,
                this.m_Name,
                this.m_Authoritive); 
        }
    }
}
