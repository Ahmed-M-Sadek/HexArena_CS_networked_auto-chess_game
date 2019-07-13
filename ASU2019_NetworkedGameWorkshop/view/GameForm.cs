using ASU2019_NetworkedGameWorkshop.controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop {
    public partial class GameForm : Form {
        private GameManager gameManager;

        public GameForm() {
            InitializeComponent();

            this.gameManager = new GameManager();
            gameManager.startTimer();
        }
    }
}
