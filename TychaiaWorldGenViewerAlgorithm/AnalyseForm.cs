using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tychaia.ProceduralGeneration.Analysis;
using Tychaia.ProceduralGeneration;
using Redpoint.FlowGraph;
using Tychaia.ProceduralGeneration.Flow;

namespace TychaiaWorldGenViewerAlgorithm
{
    public partial class AnalyseForm : Form
    {
        private RuntimeLayer m_Layer;

        private class TypeWrapper : ToolStripItem
        {
            public Type Type;

            public TypeWrapper(Type t)
            {
                this.Type = t;
            }

            public override string ToString()
            {
                return this.Type.Name;
            }
        }

        public AnalyseForm(FlowElement flowElement)
        {
            InitializeComponent();

            this.m_Layer = StorageAccess.ToRuntime((flowElement as AlgorithmFlowElement).Layer);
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
    }
}
