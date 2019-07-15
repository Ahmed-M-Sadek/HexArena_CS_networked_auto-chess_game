﻿using ASU2019_NetworkedGameWorkshop.model.grid;
using System;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.controller {
    class GameManager {
        private Timer timer;
        private Grid grid;
        private Tile selectedTile;

        public GameManager() {
            grid = new Grid(10, 8, 10, 10);
        }
        public void startTimer() {
            timer = new Timer();
            timer.Interval = 50; //Arbitrary: 20 ticks per sec
            timer.Tick += new EventHandler(gameLoop);
            timer.Start();
        }

        public void updatePaint(PaintEventArgs e) {
            grid.draw(e.Graphics);
        }

        internal void mouseClick(MouseEventArgs e) {
            Tile tile = grid.getSelectedHexagon(e.X, e.Y);
            if(tile != null)
                Console.WriteLine(tile.X + " " + tile.Y);
        }

        private void gameLoop(object sender, EventArgs e) {
            //throw new NotImplementedException();
        }
    }
}
