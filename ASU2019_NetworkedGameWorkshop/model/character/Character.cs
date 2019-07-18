using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using ASU2019_NetworkedGameWorkshop.model.grid;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model.character {
    public class Character : GraphicsObject {
        public enum Teams { Red, Blue };

        private readonly Grid grid;
        private readonly Teams team;

        private Character currentTarget;
        private Tile toMoveTo;

        public Tile CurrentTile { get; set; }//public set ?
        public CharacterType CharacterType { get; private set; }

        public Character(Grid grid, Tile currentTile, Teams team, CharacterType characterType) {
            this.grid = grid;
            currentTile.CurrentCharacter = this;
            this.team = team;

            switch(team) {
                case Teams.Red:
                    grid.TeamRed.Add(this);
                    break;
                case Teams.Blue:
                    grid.TeamBlue.Add(this);
                    break;
            }

            CharacterType = characterType;
        }

        public override void draw(Graphics graphics) {
            graphics.FillRectangle(team == Teams.Blue ? Brushes.BlueViolet : Brushes.Red,
                CurrentTile.centerX - CharacterType.WidthHalf,
                CurrentTile.centerY - CharacterType.HeightHalf,
                CharacterType.Width, CharacterType.Height);
        }

        public bool tick() {
            if(toMoveTo != null) {
                CurrentTile.CurrentCharacter = null;
                CurrentTile.Walkable = true;
                toMoveTo.CurrentCharacter = this;
                toMoveTo = null;

                return true;
            }
            return false;
        }

        public bool update() {
            if(toMoveTo == null) {
                List<Tile> path = null;
                if(currentTarget == null) { // or dead
                    try {
                        (path, currentTarget) = PathFinding.findPathToClosestEnemy(CurrentTile, team, grid);//temp
                    } catch(PathFinding.PathNotFoundException) {
                        return false;
                    }
                }
                if(PathFinding.getDistance(CurrentTile, currentTarget.CurrentTile) <= CharacterType.Range) {
                    //Console.Write(CurrentTile + ": ");
                    //CharacterType.attack(currentTarget.CurrentTile);
                } else {
                    if(path == null) {
                        try {
                            path = PathFinding.findPath(CurrentTile, currentTarget.CurrentTile, grid, (Tile[,])grid.Tiles.Clone());
                        } catch(PathFinding.PathNotFoundException) {
                            return false;
                        }
                    }
                    toMoveTo = path[0];
                }
            }
            return false;
        }
    }
}
