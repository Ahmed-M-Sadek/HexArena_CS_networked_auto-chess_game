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

        private int pathPosition;
        //temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp
        private bool sfdf = true;

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

            graphics.FillRectangle(team == Teams.Blue ? Brushes.BlueViolet : Brushes.Red, X, Y, 25, 25);
        }

        public void findPath()
        {
            //todo add try catcj PathNotFoundException
            path = new PathFinding(grid).findPathToClosestEnemy(Tile, team);
            ToMoveTo = path[0];
            pathPosition = 0;
        }

        public bool tick() {
            //add unit behaviour logic
            //temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp
            if (sfdf)
            findPath();

            //implement ranged units (put range - 1 in place of 0)
            if (path[0].Hcost == 0)
            {
                //temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp//temp
                sfdf = false;
                return true;
            }
            if (ToMoveTo != null) {
                X = ToMoveTo.X;
                Y = ToMoveTo.Y;

                System.Console.WriteLine("tile.x : {0}, tile.y : {1}, next.x : {2}, next.y : {3}", Tile.X, Tile.Y, ToMoveTo.X, ToMoveTo.Y);
                //System.Console.WriteLine("current tile: {}",ToMoveTo);
                Tile.CurrentCharacter = null;
                Tile.Walkable = true;
                ToMoveTo.CurrentCharacter = this;
                ToMoveTo = path[++pathPosition];


                return true;
            }
            return false;
        }
    }
}
