﻿using ASU2019_NetworkedGameWorkshop.model.grid;
using System;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.controller {
    class GameManager {
        private Timer timer;
        private readonly Grid grid;
        private Tile selectedTile;
        private readonly GameForm gameForm;

        public GameManager(GameForm gameForm) {
            grid = new Grid(10, 8, 10, 10);
            this.gameForm = gameForm;

            //Debugging 
            grid.Tiles[0, 5].CurrentCharacter = new model.Character();
            grid.Tiles[2, 6].CurrentCharacter = new model.Character();
            grid.Tiles[4, 1].CurrentCharacter = new model.Character();
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
            if(tile != null) {
                Console.WriteLine("Clicked Tile: ({0}, {1})", tile.X, tile.Y);//Debugging

                if(selectedTile == null) {
                    tile.Selected = true;
                    selectedTile = tile;
                } else {
                    if(tile.CurrentCharacter == null) {
                        tile.CurrentCharacter =  selectedTile.CurrentCharacter;
                        selectedTile.CurrentCharacter = null;
                    }

                    selectedTile.Selected = false;
                    selectedTile = null;
                }
                
                gameForm.Invalidate();
            }
        }

        private void gameLoop(object sender, EventArgs e) {

        }
    }
}
