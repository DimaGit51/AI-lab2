using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AIGraph.Models; // предполагается, что класс Node здесь

namespace AIGraph.UI
{
    public class NodeNameInputForm : Form
    {
        public string NodeName { get; private set; }
        private TextBox textBox;
        private Button okButton;
        private Button cancelButton;
        private readonly List<Node> existingNodes;

        public NodeNameInputForm(List<Node> nodes)
        {
            existingNodes = nodes;

            this.Text = "Введите имя узла";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(250, 150);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Поле ввода
            textBox = new TextBox { Location = new Point(10, 10), Width = 210 };

            // Кнопка Отмена (слева)
            cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(25, 50),
                DialogResult = DialogResult.Cancel,
                AutoSize = true
            };
            cancelButton.Click += (s, e) => { this.Close(); };

            // Кнопка OK (справа)
            okButton = new Button
            {
                Text = "OK",
                Location = new Point(125, 50),
                DialogResult = DialogResult.OK,
                AutoSize = true
            };
            okButton.Click += OkButton_Click;

            Controls.Add(textBox);
            Controls.Add(okButton);
            Controls.Add(cancelButton);

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            string name = textBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Имя узла не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверка на существование узла с таким же именем
            bool exists = existingNodes.Exists(n => n.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (exists)
            {
                MessageBox.Show($"Узел с именем \"{name}\" уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            NodeName = name;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
