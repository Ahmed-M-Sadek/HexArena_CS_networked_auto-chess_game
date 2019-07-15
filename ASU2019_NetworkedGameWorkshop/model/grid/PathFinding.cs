using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASU2019_NetworkedGameWorkshop.model.grid
{
    class PathFinding
    {
        Grid grid;

        public PathFinding(Grid grid)
        {
            this.grid = grid;
        }

        public void findPath(int startX, int startY, int endX, int endY)
        {
            Tile startTile = grid.Tiles[startX, startY];
            Tile endTile = grid.Tiles[startX, startY];

            List<Tile> openSet = new List<Tile>(); //use a heap for a way better optimaization
            HashSet<Tile> closedSet = new HashSet<Tile>();
            openSet.Add(startTile);

            while (openSet.Count > 0)
            {
                Tile currentTile = openSet[0];

                //v.bad optimization
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].Fcost < currentTile.Fcost ||
                        openSet[i].Fcost == currentTile.Fcost)
                    {
                        if (openSet[i].Hcost < currentTile.Hcost)
                            currentTile = openSet[i];
                    }

                }
                openSet.Remove(currentTile);

                closedSet.Add(currentTile);
                if (currentTile == endTile)
                {

                    retracePath(startTile, endTile);
                    return;

                }

                foreach (Tile neighbour in grid.getNeighbours(currentTile))
                {
                    if (closedSet.Contains(neighbour) || !neighbour.Walkable)
                        continue;

                    int newNeighbourCost = currentTile.Gcost + getDistance(currentTile, neighbour);
                    if (newNeighbourCost < neighbour.Gcost || !openSet.Contains(neighbour))
                    {

                        neighbour.Gcost = newNeighbourCost;
                        neighbour.Hcost = getDistance(neighbour, endTile);
                        neighbour.Parent = currentTile;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);

                    }
                }

            }
        }

        private void retracePath(Tile startTile, Tile endTile)
        {

            List<Tile> path = new List<Tile>();
            Tile currentNode = endTile;

            while (currentNode != startTile)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            grid.path = path;
        }

        public static int getDistance(Tile start, Tile dest)
        {
            return Math.Max(Math.Max(
                        Math.Abs(dest.Y - start.Y),
                        Math.Abs((int)Math.Ceiling(dest.Y / -2.0) + dest.X - (int)(Math.Ceiling(start.Y / -2.0)) - start.X)),
                        Math.Abs(-dest.Y - (int)Math.Ceiling((dest.Y / -2.0)) - dest.X + start.X + (int)(Math.Ceiling((start.X / -2.0)) + start.X)));
        }

    }
}
