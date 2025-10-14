using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AIGraph.Models;

namespace AIGraph.UI
{
    public class NodeNameInputForm : Form
    {
        public string NodeName { get; private set; }
        public double NodeWeight { get; private set; }

        private RichTextBox nameTextBox;
        private NumericUpDown weightInput;
        private Button okButton;
        private Button cancelButton;
        private readonly List<Node> existingNodes;

        public NodeNameInputForm(List<Node> nodes)
        {
            existingNodes = nodes;

            this.Text = "Добавить узел";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(300, 230);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Метка для имени
            Label nameLabel = new Label
            {
                Text = "Имя узла:",
                Location = new Point(10, 10),
                AutoSize = true
            };

            // RichTextBox для имени
            nameTextBox = new RichTextBox
            {
                Location = new Point(100, 8),
                Width = 170,
                Height = 60,
                WordWrap = true,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10),
                Multiline = true
            };

            nameTextBox.SelectionAlignment = HorizontalAlignment.Center;

            // Вертикальное центрирование при изменении текста и прокрутке
            nameTextBox.TextChanged += (s, e) => CenterText(nameTextBox);
            nameTextBox.VScroll += (s, e) => CenterText(nameTextBox);

            // Обработка клавиш
            nameTextBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && e.Shift)
                {
                    // Shift+Enter → перенос строки
                    int pos = nameTextBox.SelectionStart;
                    nameTextBox.Text = nameTextBox.Text.Insert(pos, "\n");
                    nameTextBox.SelectionStart = pos + 1;
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    // Просто Enter → срабатывает OK
                    e.SuppressKeyPress = true;
                    okButton.PerformClick();
                }
            };

            // Метка для веса
            Label weightLabel = new Label
            {
                Text = "Вес узла:",
                Location = new Point(10, 80),
                AutoSize = true
            };

            // Поле ввода веса
            weightInput = new NumericUpDown
            {
                Location = new Point(100, 78),
                Width = 80,
                DecimalPlaces = 2,
                Minimum = -1000,
                Maximum = 1000,
                Increment = 0.1M,
                Value = 0
            };

            // Кнопка Отмена
            cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(45, 130),
                DialogResult = DialogResult.Cancel,
                AutoSize = true
            };
            cancelButton.Click += (s, e) => this.Close();

            // Кнопка OK
            okButton = new Button
            {
                Text = "OK",
                Location = new Point(160, 130),
                DialogResult = DialogResult.OK,
                AutoSize = true
            };
            okButton.Click += OkButton_Click;

            // Добавляем контролы
            Controls.Add(nameLabel);
            Controls.Add(nameTextBox);
            Controls.Add(weightLabel);
            Controls.Add(weightInput);
            Controls.Add(okButton);
            Controls.Add(cancelButton);

            this.AcceptButton = null; // Enter не вызывает глобально OK
            this.CancelButton = cancelButton;

            // Центрируем текст при старте
            CenterText(nameTextBox);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            string name = nameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Имя узла не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool exists = existingNodes.Exists(n =>
                n.Name.Replace("\n", "").Equals(name.Replace("\n", ""), StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                MessageBox.Show($"Узел с именем \"{name.Replace("\n", " ")}\" уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            NodeName = name;
            NodeWeight = (double)weightInput.Value;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Горизонтальное и вертикальное центрирование текста
        private void CenterText(RichTextBox rtb)
        {
            rtb.SelectionAlignment = HorizontalAlignment.Center;

            int textHeight = TextRenderer.MeasureText(rtb.Text + " ", rtb.Font).Height;
            int padding = (rtb.ClientSize.Height - textHeight) / 2;
            rtb.Padding = new Padding(0, Math.Max(0, padding), 0, 0);
        }
    }
}
