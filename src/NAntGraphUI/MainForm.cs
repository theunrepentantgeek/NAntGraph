using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using NAntGraph2;

using Niche.NAntGraph;

namespace NAntGraph
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            paneWelcome.Dock = DockStyle.Fill;
            paneWelcome.Visible = true;

            showGraph.Dock = DockStyle.Fill;
            showGraph.Visible = false;
            buttonSave.Enabled = false;
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (dialogOpen.ShowDialog() == DialogResult.OK)
            {
                var project = NAntProject.Load(dialogOpen.FileName);
                mProjects.Clear();
                mProjects.Add(project);
            }

            UpdateGraph();
        }

        private void UpdateGraph()
        {
            var includeDescriptions = buttonDescriptions.Checked;
            var labelFont = string.Empty;
            var renderer = new GraphRenderer(includeDescriptions, labelFont, 12);

            showGraph.Image = renderer.Render(mProjects);

            bool haveGraph = showGraph.Image != null;
            buttonSave.Enabled = haveGraph;
            paneWelcome.Visible = !haveGraph;
            showGraph.Visible = haveGraph;
        }

        private List<NAntProject> mProjects = new List<NAntProject>();

        private void buttonDescriptions_Click(object sender, EventArgs e)
        {
            UpdateGraph();
        }

        private void labelLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.nichesoftware.co.nz");
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (dialogSave.ShowDialog() == DialogResult.OK)
            {
                showGraph.Image.Save(dialogSave.FileName);   
            }
        }
    }
}
