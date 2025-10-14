using AIGraph.Helpers;
using AIGraph.Models;
using AIGraph.UI;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using MathNet.Numerics.LinearAlgebra;

namespace AIGraph
{
    public partial class Form1 : MaterialForm
    {
        private List<Node> nodes = new List<Node>();
        private List<Edge> edges = new List<Edge>();
        private Node selectedNode = null;
        private int nodeRadius = 40;
        private bool isDragging = false;
        private Node draggingNode = null;
        private Point dragOffset;
        private float zoomFactor = 1.0f;       // текущий масштаб
        private const float zoomStep = 0.1f;   // шаг масштабирования
        private PointF panOffset = new PointF(0, 0); // смещение холста
        private bool isPanning = false;             // зажато колесико
        private Point lastMousePos;                 // предыдущая позиция мыши

        private TabPage exitTabPage;
        private Label exitLabel;
        private MaterialButton yesButton;
        private MaterialButton noButton;

        private Panel canvasPanel;
        private ComboBox sourceNodeComboBox;
        private MaterialSkin.Controls.MaterialButton simulateButton;
        private System.Windows.Forms.DataVisualization.Charting.Chart impulseChart;

        private bool chartVisible = true;


        public Form1()
        {
            InitializeComponent();

            // ====== Перенес кусок кода сюда ====== до UpdateMatrix(); <<<
            // === DataGridView для матрицы ===
            matrixGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            tabPage2.Controls.Add(matrixGrid);

            matrixGrid.CellValueChanged += MatrixGrid_CellValueChanged;
            matrixGrid.CellEndEdit += (s, e) => matrixGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);

            // === Chart для визуализации активности узлов ===
            impulseChart = new System.Windows.Forms.DataVisualization.Charting.Chart
            {
                Location = new Point(10, 50),
                Size = new Size(600, 250)
            };
            tabPage1.Controls.Add(impulseChart);

            var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            impulseChart.ChartAreas.Add(chartArea);
            impulseChart.Legends.Add(new System.Windows.Forms.DataVisualization.Charting.Legend());

            // === Обновляем матрицу при запуске ===
            UpdateMatrix();
            // ====== Перенес кусок кода сюда ====== >>>

            // Настройка MaterialSkin
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.BlueGrey800, Primary.BlueGrey900,
                Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

            // Привязка TabSelector к TabControl

            canvasPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            tabPage1.Controls.Add(canvasPanel);

            canvasPanel.MouseDown += CanvasPanel_MouseDown;
            canvasPanel.Paint += CanvasPanel_Paint;
            canvasPanel.MouseMove += CanvasPanel_MouseMove;
            canvasPanel.MouseUp += CanvasPanel_MouseUp;
            canvasPanel.MouseWheel += CanvasPanel_MouseWheel;
            canvasPanel.Focus(); // чтобы получать события колесика

            this.KeyPreview = true; // форма будет получать события клавиш
            this.KeyDown += Form1_KeyDown;

            toggleChartButton.Click += (s, e) =>
            {
                chartVisible = !chartVisible;
                impulseChart.Visible = chartVisible;
                toggleChartButton.Text = chartVisible ? "–" : "+";
            };

        }

        private void CanvasPanel_MouseDown(object sender, MouseEventArgs e)
        {
            Panel panel = (Panel)sender;
            Point clickedPoint = GetCanvasPoint(e.Location);
            Node clicked = GraphHelper.GetNodeAt(nodes, clickedPoint, nodeRadius);



            // Начало панорамирования (средняя кнопка)
            if (e.Button == MouseButtons.Middle)
            {
                isPanning = true;
                lastMousePos = e.Location;
                canvasPanel.Cursor = Cursors.SizeAll; // курсор руки
                return;
            }


            // Ctrl + ЛКМ — создание связи
            if (e.Button == MouseButtons.Left && (ModifierKeys & Keys.Control) == Keys.Control && clicked != null)
            {
                if (selectedNode == null)
                {
                    // Первый выбранный узел
                    selectedNode = clicked;
                    clicked.IsSelected = true;
                    panel.Invalidate();
                }
                else if (selectedNode != clicked)
                {
                    // Второй узел — открываем окно для ввода веса
                    clicked.IsSelected = true;
                    panel.Invalidate();

                    using (WeightInputForm inputForm = new WeightInputForm())
                    {
                        DialogResult result = inputForm.ShowDialog(this);

                        if (result == DialogResult.OK && inputForm.EnteredWeight.HasValue)
                        {
                            double weight = inputForm.EnteredWeight.Value;
                            Edge newEdge = new Edge(selectedNode, clicked, weight);
                            edges.Add(newEdge);
                        }

                        // Сбрасываем выделение вне зависимости от результата
                        GraphHelper.ResetSelectionNode(nodes);
                        GraphHelper.ResetSelectionEdge(edges);
                        selectedNode = null;
                        panel.Invalidate();
                        UpdateMatrix();
                        // Восстанавливаем фокус на холст
                        panel.Focus();
                    }
                }
                return;
            }

            // ЛКМ без Ctrl — начинаем перетаскивание или выделение
            if (e.Button == MouseButtons.Left && (ModifierKeys & Keys.Control) != Keys.Control)
            {
                if (clicked != null)
                {
                    // Начало перетаскивания
                    isDragging = true;
                    draggingNode = clicked;
                    Point mouseCanvas = GetCanvasPoint(e.Location);
                    dragOffset = new Point(mouseCanvas.X - clicked.Position.X, mouseCanvas.Y - clicked.Position.Y);
                    clicked.Click = true; // можно выделять при начале перетаскивания
                    panel.Invalidate();
                }
                else
                {
                    // Проверка на ребро
                    Point p = GetCanvasPoint(e.Location);
                    Edge clickedEdge = GetEdgeAtPoint(p);
                    if (clickedEdge != null)
                    {
                        clickedEdge.IsSelected = true;


                        canvasPanel.Invalidate();
                    }
                    else
                    {
                        // Если кликнули в пустое место — снимаем выделение узлов и ребер
                        GraphHelper.ResetSelectionNode(nodes);
                        GraphHelper.ResetSelectionEdge(edges);
                        selectedNode = null;
                        canvasPanel.Invalidate();
                    }
                }
                return;
            }

            // ПКМ — меню создания узла
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip menu = new ContextMenuStrip();
                ToolStripMenuItem createNodeItem = new ToolStripMenuItem("Создать узел");
                createNodeItem.Click += delegate (object s, EventArgs ev)
                {
                    Point canvasPoint = GetCanvasPoint(e.Location);

                    using (NodeNameInputForm inputForm = new NodeNameInputForm(nodes))
                    {
                        if (inputForm.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(inputForm.NodeName))
                        {
                            string nodeName = inputForm.NodeName;
                            double nodeWeight = inputForm.NodeWeight;
                            nodes.Add(new Node(nodeName, canvasPoint, nodeWeight));
                            UpdateMatrix();
                            UpdateSourceNodeComboBox();
                            canvasPanel.Invalidate();
                        }
                    }
                };
                menu.Items.Add(createNodeItem);
                menu.Show(panel, e.Location);
                return;
            }

            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip menu = new ContextMenuStrip();
                Node clickedNode = GraphHelper.GetNodeAt(nodes, GetCanvasPoint(e.Location), nodeRadius);

                if (clickedNode != null)
                {
                    // Удалить узел
                    ToolStripMenuItem deleteNodeItem = new ToolStripMenuItem("Удалить узел");
                    deleteNodeItem.Click += (s, ev) =>
                    {
                        selectedNode = clickedNode;
                        DeleteSelectedNode();
                    };
                    menu.Items.Add(deleteNodeItem);
                }
                else
                {
                    // Создать новый узел
                    ToolStripMenuItem createNodeItem = new ToolStripMenuItem("Создать узел");
                    createNodeItem.Click += delegate (object s, EventArgs ev)
                    {
                        Point canvasPoint = GetCanvasPoint(e.Location);
                        using (NodeNameInputForm inputForm = new NodeNameInputForm(nodes))
                        {
                            if (inputForm.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(inputForm.NodeName))
                            {
                                string nodeName = inputForm.NodeName;
                                double nodeWeight = inputForm.NodeWeight;
                                nodes.Add(new Node(nodeName, canvasPoint, nodeWeight));
                                UpdateMatrix();
                                UpdateSourceNodeComboBox();
                                canvasPanel.Invalidate();
                            }
                        }
                    };
                    menu.Items.Add(createNodeItem);
                }

                menu.Show(canvasPanel, e.Location);
            }

            // Ctrl + ПКМ на ребро — удалить
            if (e.Button == MouseButtons.Right && (ModifierKeys & Keys.Control) == Keys.Control)
            {
                Point p = GetCanvasPoint(e.Location);
                Edge clickedEdge = GetEdgeAtPoint(p);
                if (clickedEdge != null)
                {
                    edges.Remove(clickedEdge);
                    UpdateMatrix();
                    canvasPanel.Invalidate();
                    return;
                }
            }


        }

        private void CanvasPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.ResetTransform();
            g.ScaleTransform(zoomFactor, zoomFactor);
            g.TranslateTransform(panOffset.X, panOffset.Y);

            // Рисуем связи
            foreach (var edge in edges)
            {
                Point from = edge.From.Position;
                Point to = edge.To.Position;

                float dx = to.X - from.X;
                float dy = to.Y - from.Y;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                if (dist < 0.001f) continue;

                float offsetX = nodeRadius * dx / dist;
                float offsetY = nodeRadius * dy / dist;

                PointF start = new PointF(from.X + offsetX, from.Y + offsetY);
                PointF end = new PointF(to.X - offsetX, to.Y - offsetY);

                using (Pen pen = new Pen(Color.Black, 2))
                {
                    pen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(6, 8);

                    // Настраиваем стиль линии
                    if (edge.Weight < 0 && edge.IsSelected)
                    {
                        pen.DashPattern = new float[] { 4, 2 };
                    }
                    else if (edge.Weight < 0)
                    {
                        pen.DashPattern = new float[] { 8, 6 };
                    }
                    else if (edge.IsSelected)
                    {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    }

                    g.DrawLine(pen, start, end);
                }

                // Вес ребра
                float midX = (start.X + end.X) / 2;
                float midY = (start.Y + end.Y) / 2;
                string weightText = edge.Weight.ToString("0.00");
                SizeF textSize = g.MeasureString(weightText, SystemFonts.DefaultFont);
                g.DrawString(weightText, SystemFonts.DefaultFont, Brushes.Black,
                    midX - textSize.Width / 2, midY - textSize.Height / 2 - 10);
            }

            foreach (var node in nodes)
            {
                float x = node.Position.X - nodeRadius;
                float y = node.Position.Y - nodeRadius;

                Brush fill = node.IsSelected ? Brushes.LightGray : Brushes.White;
                g.FillEllipse(fill, x, y, nodeRadius * 2, nodeRadius * 2);
                g.DrawEllipse(Pens.Black, x, y, nodeRadius * 2, nodeRadius * 2);

                // === Надпись над узлом: w = [вес узла] ===
                string weightLabel = $"w = {node.Weight:0.##}";
                SizeF weightSize = g.MeasureString(weightLabel, SystemFonts.DefaultFont);
                float weightX = node.Position.X - weightSize.Width / 2;
                float weightY = node.Position.Y - nodeRadius - weightSize.Height - 5; // чуть выше круга
                g.DrawString(weightLabel, SystemFonts.DefaultFont, Brushes.DarkSlateGray, weightX, weightY);

                // === Имя узла (в центре) ===
                SizeF nameSize = g.MeasureString(node.Name, SystemFonts.DefaultFont);
                float nameX = node.Position.X - nameSize.Width / 2;
                float nameY = node.Position.Y - nameSize.Height / 2;
                g.DrawString(node.Name, SystemFonts.DefaultFont, Brushes.Black, nameX, nameY);
            }


            // Рамка вокруг выбранных
            foreach (var node in nodes)
            {
                if (node.Click)
                {
                    float margin = 6;
                    float rectX = node.Position.X - nodeRadius - margin;
                    float rectY = node.Position.Y - nodeRadius - margin;
                    float rectSize = (nodeRadius + margin) * 2;

                    using (Pen dashedPen = new Pen(Color.Black, 1))
                    {
                        dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawRectangle(dashedPen, rectX, rectY, rectSize, rectSize);
                    }
                }
            }
        }




        // Обработчик MouseMove — перемещение узла
        private void CanvasPanel_MouseMove(object sender, MouseEventArgs e)
        {
            // Панорамирование
            if (isPanning)
            {
                float dx = (e.X - lastMousePos.X) / zoomFactor;
                float dy = (e.Y - lastMousePos.Y) / zoomFactor;

                panOffset.X += dx;
                panOffset.Y += dy;

                lastMousePos = e.Location;
                canvasPanel.Invalidate();
                return;
            }

            if (isDragging && draggingNode != null)
            {
                Point mouseCanvas = GetCanvasPoint(e.Location);
                draggingNode.Position = new Point(mouseCanvas.X - dragOffset.X, mouseCanvas.Y - dragOffset.Y);
                canvasPanel.Invalidate();
            }
        }

        // Обработчик MouseUp — завершение перетаскивания
        private void CanvasPanel_MouseUp(object sender, MouseEventArgs e)
        {
            // Завершение панорамирования
            if (e.Button == MouseButtons.Middle)
            {
                isPanning = false;
                canvasPanel.Cursor = Cursors.Default;
                return;
            }

            if (isDragging)
            {
                isDragging = false;
                draggingNode = null;
                canvasPanel.Invalidate();
            }
        }
        private void CanvasPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (e.Delta > 0)
                    zoomFactor += zoomStep;
                else
                    zoomFactor -= zoomStep;

                if (zoomFactor < 0.1f) zoomFactor = 0.1f; // минимальный масштаб
                if (zoomFactor > 5.0f) zoomFactor = 5.0f; // максимальный масштаб

                canvasPanel.Invalidate();
            }
        }
        private Point GetCanvasPoint(Point mousePoint)
        {
            float x = (mousePoint.X / zoomFactor) - panOffset.X;
            float y = (mousePoint.Y / zoomFactor) - panOffset.Y;
            return new Point((int)x, (int)y);
        }

        private void UpdateMatrix()
        {
            if (matrixGrid == null) return;

            // временно отключаем событие, чтобы не было рекурсии
            matrixGrid.CellValueChanged -= MatrixGrid_CellValueChanged;

            matrixGrid.Columns.Clear();
            matrixGrid.Rows.Clear();

            int N = nodes.Count;

            // создаём колонки
            for (int i = 0; i < N; i++)
                matrixGrid.Columns.Add(i.ToString(), nodes[i].Name);

            // создаём строки и заголовки
            for (int i = 0; i < N; i++)
            {
                matrixGrid.Rows.Add();
                matrixGrid.Rows[i].HeaderCell.Value = nodes[i].Name;
            }

            // заполняем веса
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    double weight = 0;
                    foreach (var e in edges)
                    {
                        if (e.From == nodes[i] && e.To == nodes[j])
                        {
                            weight = e.Weight;
                            break;
                        }
                    }

                    matrixGrid.Rows[i].Cells[j].Value = weight == 0 ? "" : weight.ToString("0.##");
                }
            }

            // возвращаем обработчик обратно
            matrixGrid.CellValueChanged += MatrixGrid_CellValueChanged;
        }

        private double[,] BuildAdjacencyMatrix()
        {
            int N = nodes.Count;
            double[,] matrix = new double[N, N];

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                    matrix[i, j] = 0;
            }

            foreach (var edge in edges)
            {
                int from = nodes.IndexOf(edge.From);
                int to = nodes.IndexOf(edge.To);

                if (from >= 0 && to >= 0)
                {
                    // Вес связи From → To
                    matrix[to, from] = edge.Weight;
                }
            }

            return matrix;
        }

        private string AnalyzeSpectralStability()
        {
            if (nodes.Count == 0)
                return "Нет вершин для анализа.\n";

            double[,] M = BuildAdjacencyMatrix();
            var matrix = Matrix<double>.Build.DenseOfArray(M);
            var evd = matrix.Evd();

            var eigenvalues = evd.EigenValues;
            double spectralRadius = eigenvalues.Select(ev => ev.Magnitude).Max();

            string eigenStr = string.Join(", ", eigenvalues.Select(ev => $"{ev.Real:F2}{(ev.Imaginary >= 0 ? "+" : "")}{ev.Imaginary:F2}i"));

            string result = "=== СПЕКТРАЛЬНЫЙ АНАЛИЗ ===\n";
            result += $"Собственные значения: {eigenStr}\n";
            result += $"Спектральный радиус (|λmax|): {spectralRadius:F3}\n";

            if (spectralRadius < 1)
                result += "Модель устойчива по спектральному критерию (|λmax| < 1)\n";
            else
                result += "Модель неустойчива по спектральному критерию (|λmax| > 1)\n";

            return result;
        }


        private void MatrixGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var rowNode = nodes[e.RowIndex];
            var colNode = nodes[e.ColumnIndex];

            string val = matrixGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
            if (string.IsNullOrWhiteSpace(val))
            {
                edges.RemoveAll(edge => (edge.From == rowNode && edge.To == colNode) ||
                                        (edge.From == colNode && edge.To == rowNode));
            }
            else if (double.TryParse(val, out double weight))
            {
                var edge = edges.Find(edge => (edge.From == rowNode && edge.To == colNode) ||
                                              (edge.From == colNode && edge.To == rowNode));
                if (edge != null)
                    edge.Weight = weight;
                else
                    edges.Add(new Edge(rowNode, colNode, weight));
            }

            UpdateMatrix();
            canvasPanel.Invalidate();
        }


        private int CountConnectedComponents()
        {
            HashSet<Node> visited = new HashSet<Node>();
            int count = 0;

            foreach (var node in nodes)
            {
                if (!visited.Contains(node))
                {
                    DFS(node, visited);
                    count++;
                }
            }
            return count;
        }

        private void DFS(Node node, HashSet<Node> visited)
        {
            visited.Add(node);
            foreach (var edge in edges)
            {
                Node neighbor = null;
                if (edge.From == node) neighbor = edge.To;
                else if (edge.To == node) neighbor = edge.From;

                if (neighbor != null && !visited.Contains(neighbor))
                    DFS(neighbor, visited);
            }
        }

        private int GetCyclomaticNumber()
        {
            int E = edges.Count;
            int N = nodes.Count;
            int P = CountConnectedComponents();
            return E - N + P;
        }

        private List<Node> FindArticulationPoints()
        {
            int time = 0;
            Dictionary<Node, int> disc = new Dictionary<Node, int>();
            Dictionary<Node, int> low = new Dictionary<Node, int>();
            Dictionary<Node, Node> parent = new Dictionary<Node, Node>();
            HashSet<Node> ap = new HashSet<Node>();

            foreach (var node in nodes)
            {
                disc[node] = -1;
                low[node] = -1;
                parent[node] = null;
            }

            foreach (var node in nodes)
            {
                if (disc[node] == -1)
                    APUtil(node, ref time, disc, low, parent, ap);
            }

            return new List<Node>(ap);
        }

        private void APUtil(Node u, ref int time, Dictionary<Node, int> disc, Dictionary<Node, int> low, Dictionary<Node, Node> parent, HashSet<Node> ap)
        {
            int children = 0;
            disc[u] = low[u] = ++time;

            foreach (var edge in edges)
            {
                Node v = null;
                if (edge.From == u) v = edge.To;
                else if (edge.To == u) v = edge.From;

                if (v == null) continue;

                if (disc[v] == -1)
                {
                    children++;
                    parent[v] = u;
                    APUtil(v, ref time, disc, low, parent, ap);

                    low[u] = Math.Min(low[u], low[v]);

                    if (parent[u] == null && children > 1)
                        ap.Add(u);

                    if (parent[u] != null && low[v] >= disc[u])
                        ap.Add(u);
                }
                else if (v != parent[u])
                {
                    low[u] = Math.Min(low[u], disc[v]);
                }
            }
        }

        private void AnalyzeGraph()
        {
            // === ТОПОЛОГИЧЕСКИЙ АНАЛИЗ ===
            int components = CountConnectedComponents();
            int cyclomaticNumber = GetCyclomaticNumber();
            var articulationPoints = FindArticulationPoints();
            bool isTopologicallyStable = articulationPoints.Count == 0;

            string topologicalInfo =
                "=== ТОПОЛОГИЧЕСКИЙ АНАЛИЗ ===\n" +
                $"Компоненты связности: {components}\n" +
                $"Цикломатическое число: {cyclomaticNumber}\n" +
                $"Артикуляционные вершины: {(articulationPoints.Count == 0 ? "нет" : string.Join(", ", articulationPoints.ConvertAll(n => n.Name)))}\n" +
                $"Структурная устойчивость (по топологии): {(isTopologicallyStable ? "устойчива" : "неустойчива")}\n\n";

            // === СПЕКТРАЛЬНЫЙ АНАЛИЗ ===
            string spectralInfo = AnalyzeSpectralStability();

            // === ИТОГ ===
            string finalResult = topologicalInfo + spectralInfo;
            MessageBox.Show(finalResult, "Анализ устойчивости", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SimulateImpulse(Node source)
        {
            int steps = 10; // количество шагов
            Dictionary<Node, bool> active = new Dictionary<Node, bool>();
            foreach (var n in nodes) active[n] = false;

            List<List<string>> history = new List<List<string>>(); // активные узлы по шагам

            active[source] = true;

            for (int t = 0; t < steps; t++)
            {
                List<string> activeNodes = new List<string>();
                foreach (var n in nodes)
                    if (active[n])
                        activeNodes.Add(n.Name);
                history.Add(activeNodes);

                // Распространение импульса
                Dictionary<Node, bool> newActive = new Dictionary<Node, bool>(active);
                foreach (var edge in edges)
                {
                    if (active[edge.From]) newActive[edge.To] = true;
                    if (active[edge.To]) newActive[edge.From] = true;
                }
                active = newActive;
            }

            DrawImpulseChart(history);
        }

        private void DrawImpulseChart(List<List<string>> history)
        {
            impulseChart.Series.Clear();

            foreach (var node in nodes)
            {
                var series = new System.Windows.Forms.DataVisualization.Charting.Series(node.Name)
                {
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
                };

                for (int t = 0; t < history.Count; t++)
                {
                    double value = history[t].Contains(node.Name) ? 1 : 0;
                    series.Points.AddXY(t, value);
                }
                impulseChart.Series.Add(series);
            }
        }

        private void SimulateButton_Click(object sender, EventArgs e)
        {
            if (sourceNodeComboBox.SelectedIndex < 0) return;

            string nodeName = sourceNodeComboBox.SelectedItem.ToString();
            Node source = nodes.Find(n => n.Name == nodeName);
            if (source == null) return;

            SimulateImpulse(source);
        }

        private void DeleteSelectedNode()
        {
            if (selectedNode == null) return;

            // Удаляем все рёбра, связанные с узлом
            edges.RemoveAll(edge => edge.From == selectedNode || edge.To == selectedNode);

            // Удаляем сам узел
            nodes.Remove(selectedNode);
            selectedNode = null;

            UpdateMatrix();           // обновляем матрицу
            UpdateSourceNodeComboBox(); // если используешь ComboBox для симуляции
            canvasPanel.Invalidate(); // перерисовываем холст
        }

        private Edge GetEdgeAtPoint(Point p)
        {
            foreach (var edge in edges)
            {
                Point from = edge.From.Position;
                Point to = edge.To.Position;
                float dx = to.X - from.X;
                float dy = to.Y - from.Y;
                float length = (float)Math.Sqrt(dx * dx + dy * dy);
                if (length < 0.001f) continue;

                float t = ((p.X - from.X) * dx + (p.Y - from.Y) * dy) / (length * length);
                t = Math.Max(0, Math.Min(1, t));
                float closestX = from.X + t * dx;
                float closestY = from.Y + t * dy;

                float dist = (float)Math.Sqrt((p.X - closestX) * (p.X - closestX) + (p.Y - closestY) * (p.Y - closestY));
                if (dist <= 5) return edge; // порог 5 пикселей
            }
            return null;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl + S — сохранить
            if (e.Control && e.KeyCode == Keys.S)
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "Graph Board Files (*.gb)|*.gb",
                    DefaultExt = "gb"
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveLoad.SaveGraph(sfd.FileName, nodes, edges);
                    MessageBox.Show("Граф сохранён!", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            // Ctrl + O — открыть
            if (e.Control && e.KeyCode == Keys.O)
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Filter = "Graph Board Files (*.gb)|*.gb",
                    DefaultExt = "gb"
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    SaveLoad.LoadGraph(ofd.FileName, out nodes, out edges);
                    UpdateMatrix();
                    UpdateSourceNodeComboBox();
                    canvasPanel.Invalidate();
                    MessageBox.Show("Граф загружен!", "Загрузка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedObjects();
            }
        }
        private void DeleteSelectedObjects()
        {
            // Удаляем все рёбра, которые выделены
            edges.RemoveAll(edge => edge.IsSelected);

            // Удаляем все узлы, которые выделены
            nodes.RemoveAll(node =>
            {
                if (node.Click)
                {
                    // Удаляем все рёбра, связанные с этим узлом
                    edges.RemoveAll(edge => edge.From == node || edge.To == node);
                    return true;
                }
                return false;
            });

            // Сброс выделения
            foreach (var node in nodes)
                node.Click = false;
            foreach (var edge in edges)
                edge.IsSelected = false;

            UpdateMatrix();
            UpdateSourceNodeComboBox();
            canvasPanel.Invalidate();
        }
    }
}
