using System;
using System.Drawing;
using System.Windows.Forms;
using AIGraph.Models;

namespace AIGraph.UI
{
    public class NodeEditForm : Form
    {
        public string NodeName { get; private set; }
        public double NodeWeight { get; private set; }
        public int NodeRadius { get; private set; }

        private readonly TextBox nameBox;
        private readonly NumericUpDown weightBox;
        private readonly NumericUpDown radiusBox;

        public NodeEditForm(Node node)
        {
            Text = $"Редактирование узла \"{node.Name}\"";
            Width = 320;
            Height = 260;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // === Имя узла ===
            Label nameLabel = new Label
            {
                Text = "Имя узла:",
                Left = 15,
                Top = 20,
                Width = 100
            };
            Controls.Add(nameLabel);

            nameBox = new TextBox
            {
                Left = 120,
                Top = 15,
                Width = 160,
                Text = node.Name
            };
            Controls.Add(nameBox);

            // === Вес узла ===
            Label weightLabel = new Label
            {
                Text = "Вес узла:",
                Left = 15,
                Top = 65,
                Width = 100
            };
            Controls.Add(weightLabel);

            weightBox = new NumericUpDown
            {
                Left = 120,
                Top = 60,
                Width = 160,
                DecimalPlaces = 2,
                Minimum = -1000,
                Maximum = 1000,
                Increment = 0.1M,
                Value = (decimal)node.Weight
            };
            Controls.Add(weightBox);

            // === Радиус ===
            Label radiusLabel = new Label
            {
                Text = "Радиус:",
                Left = 15,
                Top = 110,
                Width = 100
            };
            Controls.Add(radiusLabel);

            radiusBox = new NumericUpDown
            {
                Left = 120,
                Top = 105,
                Width = 160,
                Minimum = 5,
                Maximum = 200,
                Value = node.Radius
            };
            Controls.Add(radiusBox);

            // === Кнопки ===
            Button okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Left = 55,
                Top = 165,
                Width = 90
            };
            Controls.Add(okButton);

            Button cancelButton = new Button
            {
                Text = "Отмена",
                DialogResult = DialogResult.Cancel,
                Left = 170,
                Top = 165,
                Width = 90
            };
            Controls.Add(cancelButton);

            AcceptButton = okButton;
            CancelButton = cancelButton;

            okButton.Click += OkButton_Click;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            NodeName = nameBox.Text.Trim();
            NodeWeight = (double)weightBox.Value;
            NodeRadius = (int)radiusBox.Value;

            if (string.IsNullOrEmpty(NodeName))
            {
                MessageBox.Show("Имя узла не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None; // не закрываем окно
            }
        }
    }
}
