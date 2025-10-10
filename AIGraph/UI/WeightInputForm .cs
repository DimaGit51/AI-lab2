using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace AIGraph.UI
{
    public class WeightInputForm : Form
    {
        private Label label;
        private TextBox weightInput;
        private Button okButton;
        private Button cancelButton;

        public double? EnteredWeight { get; private set; } = null;

        public WeightInputForm()
        {
            // Основные параметры формы
            this.Text = "Создание связи";
            this.Size = new Size(360, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Метка
            label = new Label
            {
                Text = "Вес связи:",
                Location = new Point(25, 50),
                AutoSize = true
            };

            // Поле ввода
            weightInput = new TextBox
            {
                Location = new Point(120, 45),
                Width = 120,
                Text = "0.0"
            };

            // Кнопка OK
            okButton = new Button
            {
                Text = "ОК",
                Location = new Point(200, 100),
                AutoSize = true
            };
            okButton.Click += OkButton_Click;

            // Кнопка Отмена
            cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(80, 100),
                AutoSize = true
            };
            cancelButton.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            // Добавляем элементы
            this.Controls.Add(label);
            this.Controls.Add(weightInput);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            string text = weightInput.Text.Replace(',', '.');
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                EnteredWeight = value;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Введите корректное число (например, 0.5)",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
