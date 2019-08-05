using ASU2019_NetworkedGameWorkshop.controller;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop
{
    public partial class GameForm : Form
    {
        private readonly GameManager gameManager;

        public GameForm(string playername, int connectedPort) : this()
        {
            gameManager = new GameManager(this, playername, true, null, connectedPort);
            gameManager.startGame();
        }

        public GameForm(string playername, string connectedIP, int connectedPort) : this()
        {
            gameManager = new GameManager(this, playername, false, connectedIP, connectedPort);
            gameManager.startGame();
        }

        public GameForm()
        {
            InitializeComponent();

            DoubleBuffered = true;
        }

        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            gameManager.updatePaint(e);
        }

        private void GameForm_MouseDown(object sender, MouseEventArgs e)
        {
            gameManager.mouseClick(e);
        }

        private void GameForm_Load(object sender, System.EventArgs e) {

        }
    }
}
