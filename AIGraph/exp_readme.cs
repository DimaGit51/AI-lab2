using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AIGraph
{
    internal class exp_readme
    {
    }
}

// Минимальный пример программы рисования

//using MaterialSkin;
//using MaterialSkin.Controls;
//using System.Drawing;
//using System.Windows.Forms;

//namespace AIGraph
//{
//    public partial class MainForm : MaterialForm
//    {
//        // 1. Объявляем компоненты как поля класса
//        private SplitContainer splitContainer;
//        private Panel leftPanel;
//        private Panel canvasPanel;
//        private Panel circleTool; // Элемент "Круг" для перетаскивания

//        public MainForm()
//        {
//            InitializeComponent();

//            // 2. Инициализация MaterialSkin
//            var materialSkinManager = MaterialSkinManager.Instance;
//            materialSkinManager.AddFormToManage(this);
//            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
//            materialSkinManager.ColorScheme = new ColorScheme(
//                Primary.BlueGrey800, Primary.BlueGrey900,
//                Primary.BlueGrey500, Accent.LightBlue200,
//                TextShade.WHITE);

//            // 3. Настройка интерфейса
//            SetupUI();
//        }

//        private void SetupUI()
//        {
//            // 4. Создаем разделитель экрана
//            splitContainer = new SplitContainer();
//            splitContainer.Dock = DockStyle.Fill; // Занимает всю форму
//            splitContainer.SplitterDistance = 200; // Фиксированная ширина левой панели
//            splitContainer.FixedPanel = FixedPanel.Panel1; // Левая панель не меняет размер

//            // 5. Левая панель - меню инструментов
//            leftPanel = new Panel();
//            leftPanel.Dock = DockStyle.Fill; // Заполняет левую часть
//            leftPanel.BackColor = Color.LightGray; // Цвет фона
//            leftPanel.Padding = new Padding(10); // Отступы внутри

//            // 6. Правая панель - холст для рисования
//            canvasPanel = new Panel();
//            canvasPanel.Dock = DockStyle.Fill; // Заполняет правую часть
//            canvasPanel.BackColor = Color.White; // Белый фон
//            canvasPanel.AllowDrop = true; // Разрешаем перетаскивание

//            // 7. Добавляем обработчики событий перетаскивания
//            canvasPanel.DragEnter += CanvasPanel_DragEnter;
//            canvasPanel.DragDrop += CanvasPanel_DragDrop;

//            // 8. Собираем интерфейс вместе
//            splitContainer.Panel1.Controls.Add(leftPanel); // Меню слева
//            splitContainer.Panel2.Controls.Add(canvasPanel); // Холст справа
//            Controls.Add(splitContainer); // Добавляем разделитель на форму

//            // 9. Создаем элемент для перетаскивания
//            CreateCircleTool();
//        }

//        private void CreateCircleTool()
//        {
//            // 10. Создаем визуальный элемент "Круг"
//            circleTool = new Panel();
//            circleTool.Size = new Size(80, 40); // Размер элемента
//            circleTool.Location = new Point(10, 10); // Позиция в меню
//            circleTool.BackColor = Color.Red; // Красный цвет
//            circleTool.BorderStyle = BorderStyle.FixedSingle; // Рамка
//            circleTool.Cursor = Cursors.Hand; // Курсор-рука при наведении
//            circleTool.Tag = "circle"; // Метка для идентификации

//            // 11. Добавляем текст на элемент
//            var label = new Label();
//            label.Text = "Круг";
//            label.ForeColor = Color.White;
//            label.Location = new Point(5, 10);
//            label.Font = new Font("Arial", 10);
//            label.BackColor = Color.Transparent;

//            // 12. Обработчик начала перетаскивания
//            circleTool.MouseDown += (sender, e) =>
//            {
//                // Начинаем перетаскивание с данными "circle"
//                circleTool.DoDragDrop(circleTool.Tag, DragDropEffects.Copy);
//            };

//            // 13. Добавляем элементы на панель
//            circleTool.Controls.Add(label);
//            leftPanel.Controls.Add(circleTool);
//        }

//        // 14. Обработчик входа перетаскиваемого элемента в холст
//        private void CanvasPanel_DragEnter(object sender, DragEventArgs e)
//        {
//            // Проверяем, что перетаскиваются правильные данные
//            if (e.Data.GetDataPresent(DataFormats.StringFormat))
//            {
//                e.Effect = DragDropEffects.Copy; // Разрешаем копирование
//            }
//        }

//        // 15. Обработчик отпускания элемента на холсте
//        private void CanvasPanel_DragDrop(object sender, DragEventArgs e)
//        {
//            // 16. Получаем координаты мыши на холсте
//            Point dropPoint = canvasPanel.PointToClient(new Point(e.X, e.Y));

//            // 17. Получаем тип элемента (в нашем случае "circle")
//            string elementType = e.Data.GetData(DataFormats.StringFormat).ToString();

//            // 18. Создаем элемент на холсте
//            if (elementType == "circle")
//            {
//                CreateCircleOnCanvas(dropPoint);
//            }
//        }

//        private void CreateCircleOnCanvas(Point location)
//        {
//            // 19. Создаем новый круг на холсте
//            Panel circle = new Panel();
//            circle.Size = new Size(50, 50); // Размер круга
//            circle.Location = new Point(location.X - 25, location.Y - 25); // Центрируем
//            circle.BackColor = Color.Red;
//            circle.BorderStyle = BorderStyle.FixedSingle;

//            // 20. Делаем круглую форму
//            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
//            {
//                path.AddEllipse(0, 0, circle.Width, circle.Height);
//                circle.Region = new Region(path);
//            }

//            // 21. Добавляем на холст
//            canvasPanel.Controls.Add(circle);
//        }
//    }
//}