using System;
using System.Drawing;
using System.Windows.Forms;
using AIGraph.Models;

namespace AIGraph.UI
{
    public class EdgeEditForm : Form
    {
        public double EdgeWeight { get; private set; }

        private readonly NumericUpDown weightBox;

        public EdgeEditForm(Edge edge)
        {
            Text = $"Редактирование связи \"{edge.From.Name} → {edge.To.Name}\"";
            Width = 320;
            Height = 180;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // === Метка для веса ===
            Label weightLabel = new Label
            {
                Text = "Вес связи:",
                Left = 15,
                Top = 30,
                Width = 100
            };
            Controls.Add(weightLabel);

            // === Поле для веса ===
            weightBox = new NumericUpDown
            {
                Left = 120,
                Top = 25,
                Width = 160,
                DecimalPlaces = 2,
                Minimum = -1,
                Maximum = 1,
                Increment = 0.1M,
                Value = (decimal)edge.Weight
            };
            Controls.Add(weightBox);

            // === Кнопка OK ===
            Button okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Left = 55,
                Top = 80,
                Width = 90
            };
            Controls.Add(okButton);

            // === Кнопка Отмена ===
            Button cancelButton = new Button
            {
                Text = "Отмена",
                DialogResult = DialogResult.Cancel,
                Left = 170,
                Top = 80,
                Width = 90
            };
            Controls.Add(cancelButton);

            AcceptButton = okButton;
            CancelButton = cancelButton;

            okButton.Click += (s, e) => OkButton_Click(edge);
        }

        private void OkButton_Click(Edge edge)
        {
            decimal value;
            if (!decimal.TryParse(weightBox.Text, out value))
            {
                MessageBox.Show("Введите число.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            if (value < -1 || value > 1)
            {
                MessageBox.Show("Вес связи должен быть в диапазоне от -1 до 1.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            EdgeWeight = (double)value;
            edge.Weight = EdgeWeight;
        }

    }
}
