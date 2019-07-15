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

                    //todo: RetracePath(startNode, targetNode);
                    return;

                }

                foreach (Tile neighbour in grid.getNeighbours(currentTile))
                {
                    if (closedSet.Contains(neighbour) || !neighbour.Walkable)
                        continue;


                }

            }
        }


        public static int  getDistance(Tile start, Tile dest)
        {
            int distance = Math.Max(Math.Max(
                        Math.Abs(dest.Y - start.Y),
                        Math.Abs(Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dest.Y / -2))) + dest.X - Convert.ToInt32(Math.Ceiling(Convert.ToDouble(start.Y / -2))) - start.X)),
                        Math.Abs(-dest.Y - Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dest.Y / -2))) - dest.X + start.X + Convert.ToInt32(Math.Ceiling(Convert.ToDouble(start.X / -2)) + start.X)));
            return distance;
                        }

    }
}
