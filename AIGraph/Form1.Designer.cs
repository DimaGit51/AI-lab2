namespace AIGraph
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            toggleChartButton = new MaterialSkin.Controls.MaterialButton();
            analyzeButton = new MaterialSkin.Controls.MaterialButton();
            simulateButton = new MaterialSkin.Controls.MaterialButton();
            sourceNodeComboBox = new ComboBox();
            tabPage2 = new TabPage();
            impulseChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)impulseChart).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(3, 64);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1274, 653);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(toggleChartButton);
            tabPage1.Controls.Add(analyzeButton);
            tabPage1.Controls.Add(simulateButton);
            tabPage1.Controls.Add(sourceNodeComboBox);
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1266, 620);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Graph";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // toggleChartButton
            // 
            toggleChartButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            toggleChartButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            toggleChartButton.Depth = 0;
            toggleChartButton.HighEmphasis = true;
            toggleChartButton.Icon = null;
            toggleChartButton.Location = new Point(505, 6);
            toggleChartButton.Margin = new Padding(4, 6, 4, 6);
            toggleChartButton.MouseState = MaterialSkin.MouseState.HOVER;
            toggleChartButton.Name = "toggleChartButton";
            toggleChartButton.NoAccentTextColor = Color.Empty;
            toggleChartButton.Size = new Size(64, 36);
            toggleChartButton.TabIndex = 4;
            toggleChartButton.Text = "-";
            toggleChartButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            toggleChartButton.UseAccentColor = false;
            // 
            // analyzeButton
            // 
            analyzeButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            analyzeButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            analyzeButton.Depth = 0;
            analyzeButton.HighEmphasis = true;
            analyzeButton.Icon = null;
            analyzeButton.Location = new Point(325, 6);
            analyzeButton.Margin = new Padding(4, 6, 4, 6);
            analyzeButton.MouseState = MaterialSkin.MouseState.HOVER;
            analyzeButton.Name = "analyzeButton";
            analyzeButton.NoAccentTextColor = Color.Empty;
            analyzeButton.Size = new Size(172, 36);
            analyzeButton.TabIndex = 1;
            analyzeButton.Text = "Анализ структуры";
            analyzeButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            analyzeButton.UseAccentColor = false;
            analyzeButton.Click += analyzeButton_Click;
            // 
            // simulateButton
            // 
            simulateButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            simulateButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            simulateButton.Depth = 0;
            simulateButton.HighEmphasis = true;
            simulateButton.Icon = null;
            simulateButton.Location = new Point(134, 6);
            simulateButton.Margin = new Padding(4, 6, 4, 6);
            simulateButton.MouseState = MaterialSkin.MouseState.HOVER;
            simulateButton.Name = "simulateButton";
            simulateButton.NoAccentTextColor = Color.Empty;
            simulateButton.Size = new Size(183, 36);
            simulateButton.TabIndex = 2;
            simulateButton.Text = "Запустить импульс";
            simulateButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            simulateButton.UseAccentColor = false;
            simulateButton.Click += SimulateButton_Click;
            // 
            // sourceNodeComboBox
            // 
            sourceNodeComboBox.Location = new Point(6, 6);
            sourceNodeComboBox.Name = "sourceNodeComboBox";
            sourceNodeComboBox.Size = new Size(121, 28);
            sourceNodeComboBox.TabIndex = 3;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1266, 620);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Matrix";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // impulseChart
            // 
            impulseChart.Location = new Point(0, 0);
            impulseChart.Name = "impulseChart";
            impulseChart.Size = new Size(300, 300);
            impulseChart.TabIndex = 0;
            // 
            // Form1
            // 
            ClientSize = new Size(1280, 720);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "AIGraph";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)impulseChart).EndInit();
            ResumeLayout(false);
        }

        private void analyzeButton_Click(object sender, EventArgs e)
        {
            AnalyzeGraph();
        }

        private void UpdateSourceNodeComboBox()
        {
            sourceNodeComboBox.Items.Clear();
            foreach (var node in nodes)
                sourceNodeComboBox.Items.Add(node.Name);

            if (nodes.Count > 0)
                sourceNodeComboBox.SelectedIndex = 0;
        }



        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private DataGridView matrixGrid;
        private MaterialSkin.Controls.MaterialButton analyzeButton;
        private MaterialSkin.Controls.MaterialButton toggleChartButton;
    }
}
