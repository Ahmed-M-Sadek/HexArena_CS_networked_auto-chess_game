using ASU2019_NetworkedGameWorkshop.controller;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop
{
    public partial class GameForm : Form
    {
        private readonly GameManager gameManager;

        public GameForm()
        {
            InitializeComponent();

            DoubleBuffered = true;

            gameManager = new GameManager(this);
            gameManager.startTimer();
        }

        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            gameManager.updatePaint(e);
        }

        private void GameForm_MouseDown(object sender, MouseEventArgs e)
        {
            gameManager.mouseClick(e);
        }
    }
}
