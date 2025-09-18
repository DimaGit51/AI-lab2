using MaterialSkin;
using MaterialSkin.Controls;
using System.Drawing;
using System.Windows.Forms;

namespace AIGraph
{
    public partial class Form1 : MaterialForm
    {
        private SplitContainer splitContainer;
        private Panel leftPanel;
        private Panel canvasPanel;
        public Form1()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

            SetupUI();
        }

        private void SetupUI()
        {
            // Настройка SplitContainer
            splitContainer = new SplitContainer();
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.SplitterDistance = (int)(Width * 0.166); // 1/6
            splitContainer.FixedPanel = FixedPanel.Panel1;

            // Левая панель (меню) - применяем MaterialSkin стиль
            leftPanel = new Panel();
            leftPanel.Dock = DockStyle.Fill;
            leftPanel.BackColor = MaterialSkinManager.Instance.ColorScheme.PrimaryColor;
            leftPanel.Padding = new Padding(10);

            // Правая панель (холст)
            canvasPanel = new Panel();
            canvasPanel.Dock = DockStyle.Fill;
            canvasPanel.BackColor = Color.White;
            canvasPanel.AllowDrop = true;

            splitContainer.Panel1.Controls.Add(leftPanel);
            splitContainer.Panel2.Controls.Add(canvasPanel);
            Controls.Add(splitContainer);
        }
    }
}
