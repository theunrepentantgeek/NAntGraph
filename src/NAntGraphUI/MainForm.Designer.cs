namespace NAntGraph
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.paneWelcome = new System.Windows.Forms.TableLayoutPanel();
            this.showGlyph = new System.Windows.Forms.PictureBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.labelWelcome = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelLink = new System.Windows.Forms.LinkLabel();
            this.dialogOpen = new System.Windows.Forms.OpenFileDialog();
            this.paneImageTools = new System.Windows.Forms.ToolStrip();
            this.buttonOpen = new System.Windows.Forms.ToolStripButton();
            this.buttonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.labelOptions = new System.Windows.Forms.ToolStripLabel();
            this.buttonDescriptions = new System.Windows.Forms.ToolStripButton();
            this.showGraph = new System.Windows.Forms.PictureBox();
            this.dialogSave = new System.Windows.Forms.SaveFileDialog();
            this.paneWelcome.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.showGlyph)).BeginInit();
            this.paneImageTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.showGraph)).BeginInit();
            this.SuspendLayout();
            // 
            // paneWelcome
            // 
            this.paneWelcome.BackColor = System.Drawing.SystemColors.Window;
            this.paneWelcome.ColumnCount = 4;
            this.paneWelcome.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.paneWelcome.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.paneWelcome.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.paneWelcome.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.paneWelcome.Controls.Add(this.showGlyph, 1, 1);
            this.paneWelcome.Controls.Add(this.labelDescription, 2, 2);
            this.paneWelcome.Controls.Add(this.labelWelcome, 2, 1);
            this.paneWelcome.Controls.Add(this.label1, 2, 3);
            this.paneWelcome.Controls.Add(this.labelLink, 2, 4);
            this.paneWelcome.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paneWelcome.Location = new System.Drawing.Point(0, 25);
            this.paneWelcome.Name = "paneWelcome";
            this.paneWelcome.Padding = new System.Windows.Forms.Padding(8);
            this.paneWelcome.RowCount = 5;
            this.paneWelcome.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.paneWelcome.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.paneWelcome.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.paneWelcome.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.paneWelcome.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.paneWelcome.Size = new System.Drawing.Size(951, 541);
            this.paneWelcome.TabIndex = 1;
            // 
            // showGlyph
            // 
            this.showGlyph.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.showGlyph.Image = global::NAntGraph.Properties.Resources.graphIcon;
            this.showGlyph.Location = new System.Drawing.Point(177, 137);
            this.showGlyph.Name = "showGlyph";
            this.showGlyph.Size = new System.Drawing.Size(99, 64);
            this.showGlyph.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.showGlyph.TabIndex = 2;
            this.showGlyph.TabStop = false;
            // 
            // labelDescription
            // 
            this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDescription.AutoSize = true;
            this.labelDescription.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDescription.ForeColor = System.Drawing.Color.SteelBlue;
            this.labelDescription.Location = new System.Drawing.Point(282, 220);
            this.labelDescription.Margin = new System.Windows.Forms.Padding(3, 16, 3, 16);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(492, 38);
            this.labelDescription.TabIndex = 1;
            this.labelDescription.Text = "NAntGraph generates a directed graph displaying the dependencies between targets " +
                "in a NAnt or Ant build file.";
            // 
            // labelWelcome
            // 
            this.labelWelcome.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelWelcome.AutoSize = true;
            this.labelWelcome.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWelcome.Location = new System.Drawing.Point(282, 159);
            this.labelWelcome.Name = "labelWelcome";
            this.labelWelcome.Size = new System.Drawing.Size(181, 19);
            this.labelWelcome.TabIndex = 0;
            this.labelWelcome.Text = "Welcome to NAnt Graph";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.SteelBlue;
            this.label1.Location = new System.Drawing.Point(282, 290);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 16, 3, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(492, 38);
            this.label1.TabIndex = 1;
            this.label1.Text = "To begin, open an Ant or NAnt build file, then select options to create a graph s" +
                "howing target dependencies from the selected file.";
            // 
            // labelLink
            // 
            this.labelLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelLink.AutoSize = true;
            this.paneWelcome.SetColumnSpan(this.labelLink, 2);
            this.labelLink.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLink.Location = new System.Drawing.Point(794, 504);
            this.labelLink.Margin = new System.Windows.Forms.Padding(16);
            this.labelLink.Name = "labelLink";
            this.labelLink.Size = new System.Drawing.Size(133, 13);
            this.labelLink.TabIndex = 3;
            this.labelLink.TabStop = true;
            this.labelLink.Text = "www.nichesoftware.co.nz";
            this.labelLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelLink_LinkClicked);
            // 
            // dialogOpen
            // 
            this.dialogOpen.Filter = "Build Files (*.xml *.build)|*.xml;*.build|Ant Build Files (*.xml)|*.xml|NAnt Buil" +
                "d Files (*.build)|*.build|All Files (*.*)|*.*";
            this.dialogOpen.Title = "Select Ant or NAnt build file";
            // 
            // paneImageTools
            // 
            this.paneImageTools.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.paneImageTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonOpen,
            this.buttonSave,
            this.toolStripSeparator1,
            this.labelOptions,
            this.buttonDescriptions});
            this.paneImageTools.Location = new System.Drawing.Point(0, 0);
            this.paneImageTools.Name = "paneImageTools";
            this.paneImageTools.Size = new System.Drawing.Size(951, 25);
            this.paneImageTools.TabIndex = 4;
            this.paneImageTools.Text = "toolStrip1";
            // 
            // buttonOpen
            // 
            this.buttonOpen.Image = global::NAntGraph.Properties.Resources.open_document_16;
            this.buttonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(107, 22);
            this.buttonOpen.Text = "Open Build File";
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Image = global::NAntGraph.Properties.Resources.save_16;
            this.buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(105, 22);
            this.buttonSave.Text = "Save Image File";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // labelOptions
            // 
            this.labelOptions.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelOptions.Name = "labelOptions";
            this.labelOptions.Size = new System.Drawing.Size(90, 22);
            this.labelOptions.Text = "Graph Options:";
            // 
            // buttonDescriptions
            // 
            this.buttonDescriptions.CheckOnClick = true;
            this.buttonDescriptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonDescriptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonDescriptions.Image")));
            this.buttonDescriptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDescriptions.Name = "buttonDescriptions";
            this.buttonDescriptions.Size = new System.Drawing.Size(151, 22);
            this.buttonDescriptions.Text = "Include Target Descriptions";
            this.buttonDescriptions.Click += new System.EventHandler(this.buttonDescriptions_Click);
            // 
            // showGraph
            // 
            this.showGraph.BackColor = System.Drawing.SystemColors.Window;
            this.showGraph.Location = new System.Drawing.Point(713, 371);
            this.showGraph.Name = "showGraph";
            this.showGraph.Size = new System.Drawing.Size(214, 166);
            this.showGraph.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.showGraph.TabIndex = 5;
            this.showGraph.TabStop = false;
            // 
            // dialogSave
            // 
            this.dialogSave.DefaultExt = "png";
            this.dialogSave.Filter = "PNG (*.png)|*.png|JPG (*.jpg)|*.jpg";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(951, 566);
            this.Controls.Add(this.paneWelcome);
            this.Controls.Add(this.showGraph);
            this.Controls.Add(this.paneImageTools);
            this.Name = "MainForm";
            this.Text = "NAntGraph by Niche Software";
            this.paneWelcome.ResumeLayout(false);
            this.paneWelcome.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.showGlyph)).EndInit();
            this.paneImageTools.ResumeLayout(false);
            this.paneImageTools.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.showGraph)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel paneWelcome;
        private System.Windows.Forms.Label labelWelcome;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.PictureBox showGlyph;
        private System.Windows.Forms.OpenFileDialog dialogOpen;
        private System.Windows.Forms.ToolStrip paneImageTools;
        private System.Windows.Forms.PictureBox showGraph;
        private System.Windows.Forms.ToolStripButton buttonOpen;
        private System.Windows.Forms.ToolStripButton buttonSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel labelOptions;
        private System.Windows.Forms.ToolStripButton buttonDescriptions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel labelLink;
        private System.Windows.Forms.SaveFileDialog dialogSave;
    }
}

