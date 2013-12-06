// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Redpoint.FlowGraph;
using Tychaia.ProceduralGeneration;
using Tychaia.ProceduralGeneration.Analysis;
using Tychaia.ProceduralGeneration.Flow;

namespace TychaiaWorldGenViewerAlgorithm
{
    partial class AnalyseForm : Form
    {
        private RuntimeLayer m_Layer;

        public AnalyseForm(
            IStorageAccess storageAccess,
            FlowElement flowElement)
        {
            this.InitializeComponent();

            this.m_Layer = storageAccess.ToRuntime(((AlgorithmFlowElement)flowElement).Layer);
            this.c_AnalysisAddOptionsMenu.Items.AddRange((
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where typeof(AnalysisEngine).IsAssignableFrom(type) && !type.IsGenericType && !type.IsAbstract
                select new TypeWrapper(type)).ToArray());
        }

        private void AnalysisForm_Load(object sender, EventArgs e)
        {
        }

        private void c_AddAnalysisButton_Click(object sender, EventArgs e)
        {
            this.c_AnalysisAddOptionsMenu.Show(this.c_AddAnalysisButton, new Point(0, this.c_AddAnalysisButton.Height));
        }

        private class TypeWrapper : ToolStripItem
        {
            private readonly Type m_Type;

            public TypeWrapper(Type t)
            {
                this.m_Type = t;
            }

            public override string ToString()
            {
                return this.m_Type.Name;
            }
        }
    }
}
