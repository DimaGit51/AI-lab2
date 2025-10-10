using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AIGraph.Models;
using AIGraph.Helpers;
using AIGraph.UI;

namespace AIGraph
{
    public partial class Form1 : MaterialForm
    {
        private List<Node> nodes = new List<Node>();
        private List<Edge> edges = new List<Edge>();
        private Node selectedNode = null;
        private int nodeRadius = 20;
        private bool isDragging = false;
        private Node draggingNode = null;
        private Point dragOffset;
        private float zoomFactor = 1.0f;       // ������� �������
        private const float zoomStep = 0.1f;   // ��� ���������������
        private PointF panOffset = new PointF(0, 0); // �������� ������
        private bool isPanning = false;             // ������ ��������
        private Point lastMousePos;                 // ���������� ������� ����



        private Panel canvasPanel;

        public Form1()
        {
            InitializeComponent();

            // ��������� MaterialSkin
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.BlueGrey800, Primary.BlueGrey900,
                Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

            // ������ ��� ���������
            canvasPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            Controls.Add(canvasPanel);

            canvasPanel.MouseDown += CanvasPanel_MouseDown;
            canvasPanel.Paint += CanvasPanel_Paint;
            canvasPanel.MouseMove += CanvasPanel_MouseMove;
            canvasPanel.MouseUp += CanvasPanel_MouseUp;
            canvasPanel.MouseWheel += CanvasPanel_MouseWheel;
            canvasPanel.Focus(); // ����� �������� ������� ��������

        }

        private void CanvasPanel_MouseDown(object sender, MouseEventArgs e)
        {
            Panel panel = (Panel)sender;
            Point clickedPoint = GetCanvasPoint(e.Location);
            Node clicked = GraphHelper.GetNodeAt(nodes, clickedPoint, nodeRadius);



            // ������ ��������������� (������� ������)
            if (e.Button == MouseButtons.Middle)
            {
                isPanning = true;
                lastMousePos = e.Location;
                canvasPanel.Cursor = Cursors.SizeAll; // ������ ����
                return;
            }


            // Ctrl + ��� � �������� �����
            if (e.Button == MouseButtons.Left && (ModifierKeys & Keys.Control) == Keys.Control && clicked != null)
            {
                if (selectedNode == null)
                {
                    // ������ ��������� ����
                    selectedNode = clicked;
                    clicked.IsSelected = true;
                    panel.Invalidate();
                }
                else if (selectedNode != clicked)
                {
                    // ������ ���� � ��������� ���� ��� ����� ����
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

                        // ���������� ��������� ��� ����������� �� ����������
                        GraphHelper.ResetSelection(nodes);
                        selectedNode = null;
                        panel.Invalidate();

                        // ��������������� ����� �� �����
                        panel.Focus();
                    }
                }
                return;
            }

            // ��� ��� Ctrl � �������� �������������� ��� ���������
            if (e.Button == MouseButtons.Left && (ModifierKeys & Keys.Control) != Keys.Control)
            {
                if (clicked != null)
                {
                    // ������ ��������������
                    isDragging = true;
                    draggingNode = clicked;
                    Point mouseCanvas = GetCanvasPoint(e.Location);
                    dragOffset = new Point(mouseCanvas.X - clicked.Position.X, mouseCanvas.Y - clicked.Position.Y);
                    clicked.IsSelected = true; // ����� �������� ��� ������ ��������������
                    panel.Invalidate();
                }
                else
                {
                    // ���� �������� � ������ ����� � ������� ���������
                    GraphHelper.ResetSelection(nodes);
                    panel.Invalidate();
                }
                return;
            }

            // ��� � ���� �������� ����
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip menu = new ContextMenuStrip();
                ToolStripMenuItem createNodeItem = new ToolStripMenuItem("������� ����");
                createNodeItem.Click += delegate (object s, EventArgs ev)
                {
                    Point canvasPoint = GetCanvasPoint(e.Location);

                    using (NodeNameInputForm inputForm = new NodeNameInputForm(nodes))
                    {
                        if (inputForm.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(inputForm.NodeName))
                        {
                            string nodeName = inputForm.NodeName;
                            nodes.Add(new Node(nodeName, canvasPoint));
                            canvasPanel.Invalidate();
                        }
                    }
                };
                menu.Items.Add(createNodeItem);
                menu.Show(panel, e.Location);
                return;
            }



        }


        private void CanvasPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.ResetTransform();
            g.ScaleTransform(zoomFactor, zoomFactor);
            g.TranslateTransform(panOffset.X, panOffset.Y);


            Pen pen = new Pen(Color.Black, 2);
            pen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(6, 8);

            // ������ �����
            for (int i = 0; i < edges.Count; i++)
            {
                Edge edge = edges[i];
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

                g.DrawLine(pen, start, end);

                // ��� ��� ������
                float midX = (start.X + end.X) / 2;
                float midY = (start.Y + end.Y) / 2;
                string weightText = edge.Weight.ToString("0.00");
                SizeF textSize = g.MeasureString(weightText, SystemFonts.DefaultFont);
                g.DrawString(weightText, SystemFonts.DefaultFont, Brushes.Black,
                    midX - textSize.Width / 2, midY - textSize.Height / 2 - 10);
            }

            // ������ ����
            for (int i = 0; i < nodes.Count; i++)
            {
                Node node = nodes[i];
                float x = node.Position.X - nodeRadius;
                float y = node.Position.Y - nodeRadius;

                Brush fill = node.IsSelected ? Brushes.LightGray : Brushes.White;
                g.FillEllipse(fill, x, y, nodeRadius * 2, nodeRadius * 2);
                g.DrawEllipse(Pens.Black, x, y, nodeRadius * 2, nodeRadius * 2);

                SizeF nameSize = g.MeasureString(node.Name, SystemFonts.DefaultFont);
                g.DrawString(node.Name, SystemFonts.DefaultFont, Brushes.Black,
                    node.Position.X - nameSize.Width / 2,
                    node.Position.Y - nameSize.Height / 2);
            }
        }
        // ���������� MouseMove � ����������� ����
        private void CanvasPanel_MouseMove(object sender, MouseEventArgs e)
        {
            // ���������������
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

        // ���������� MouseUp � ���������� ��������������
        private void CanvasPanel_MouseUp(object sender, MouseEventArgs e)
        {
            // ���������� ���������������
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

                if (zoomFactor < 0.1f) zoomFactor = 0.1f; // ����������� �������
                if (zoomFactor > 5.0f) zoomFactor = 5.0f; // ������������ �������

                canvasPanel.Invalidate();
            }
        }
        private Point GetCanvasPoint(Point mousePoint)
        {
            float x = (mousePoint.X / zoomFactor) - panOffset.X;
            float y = (mousePoint.Y / zoomFactor) - panOffset.Y;
            return new Point((int)x, (int)y);
        }


    }
}
