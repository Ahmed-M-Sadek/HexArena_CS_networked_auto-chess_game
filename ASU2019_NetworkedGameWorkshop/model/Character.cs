using ASU2019_NetworkedGameWorkshop.model.grid;
using System.Collections.Generic;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model {
    class Character : GraphicsObject {
        private Grid grid;
        private List<Tile> path;
        private Teams team;
        public Tile Tile { get; set; }

        public enum Teams {Red, Blue };

        public float X { get; set; }
        public float Y { get; set; }

        public Tile ToMoveTo { get; set; }
        
        public Character(Grid grid, Tile currentTile, Teams team)
        {
            this.grid = grid;
            this.Tile = currentTile;
            this.team = team;

            switch(team)
            {
                case Teams.Red:
                    grid.TeamRed.Add(this);
                    break;
                case Teams.Blue:
                    grid.TeamBlue.Add(this);
                    break;
            }
        }

        public override void draw(Graphics graphics) {
            graphics.FillRectangle(Brushes.HotPink, X, Y, 25, 25);
        }

        public void findPath()
        {
            path = new PathFinding(grid).findPathToClosestEnemy(Tile, team);
        }

        public bool tick() {
            if(ToMoveTo != null) {
                X = ToMoveTo.X;
                Y = ToMoveTo.Y;

                Tile.CurrentCharacter = null;
                ToMoveTo.CurrentCharacter = this;
                ToMoveTo = null;

                return true;
            }
            return false;
        }
    }
}
