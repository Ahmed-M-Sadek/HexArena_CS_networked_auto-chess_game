using ASU2019_NetworkedGameWorkshop.controller;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop
{
    public partial class GameForm : Form
    {
        public GameManager GameManager { get; set; }

        public GameForm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            this.GameManager = new GameManager(this);
            GameManager.startTimer();
        }

        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            GameManager.updatePaint(e);
        }

        private void GameForm_MouseDown(object sender, MouseEventArgs e)
        {
            GameManager.mouseClick(e);
        }
    }
}
