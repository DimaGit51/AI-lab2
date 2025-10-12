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
            tabPage2 = new TabPage();
            tabControl1.SuspendLayout();
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
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1266, 620);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Graph";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(242, 92);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Matrix";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            ClientSize = new Size(1280, 720);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "AIGraph";
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);

            // Инициализация DataGridView для матрицы
            matrixGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            tabPage2.Controls.Add(matrixGrid);

            // Обработка изменения значения в матрице
            matrixGrid.CellValueChanged += MatrixGrid_CellValueChanged;
            matrixGrid.CellEndEdit += (s, e) => matrixGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            UpdateMatrix();

            // Кнопка "Анализ структуры"
            analyzeButton = new MaterialSkin.Controls.MaterialButton
            {
                Text = "Анализ структуры",
                Location = new Point(10, 10),
                AutoSize = true
            };
            analyzeButton.Click += analyzeButton_Click;
            this.Controls.Add(analyzeButton);

            // ComboBox для выбора стартовой вершины
            sourceNodeComboBox = new ComboBox
            {
                Location = new Point(10, 10),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            tabPage1.Controls.Add(sourceNodeComboBox);

            // Кнопка запуска симуляции
            simulateButton = new MaterialSkin.Controls.MaterialButton
            {
                Text = "Запустить импульс",
                Location = new Point(170, 10),
                AutoSize = true
            };
            simulateButton.Click += SimulateButton_Click;
            tabPage1.Controls.Add(simulateButton);

            // Chart для визуализации активности узлов
            impulseChart = new System.Windows.Forms.DataVisualization.Charting.Chart
            {
                Location = new Point(10, 50),
                Size = new Size(600, 250)
            };
            tabPage1.Controls.Add(impulseChart);

            // Настройка Chart
            var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            impulseChart.ChartAreas.Add(chartArea);
            impulseChart.Legends.Add(new System.Windows.Forms.DataVisualization.Charting.Legend());
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
    }
}
