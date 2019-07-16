using ASU2019_NetworkedGameWorkshop.model.collection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Tile startTile = grid.Tiles[startX, startY];
            Tile endTile = grid.Tiles[endX, endY];
            Heap<Tile> openSet = new Heap<Tile>(grid.MaxSize); 
            HashSet<Tile> closedSet = new HashSet<Tile>();
            openSet.Add(startTile);

            while (openSet.Count > 0)
            {
                Tile currentTile = openSet.RemoveFirst();

                closedSet.Add(currentTile);
                if (currentTile == endTile)
                {
                    stopWatch.Stop();
                    Console.WriteLine("path found in: {0}", stopWatch.ElapsedMilliseconds);
                    retracePath(startTile, endTile);
                    return;

                }
                Console.WriteLine("neighbours of  {0}, {1}", currentTile.X, currentTile.Y);
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
                    Console.WriteLine("neighbour: {0}, {1}, hcost: {2}, gcost: {3}, fcost: {4}", neighbour.X, neighbour.Y, neighbour.Hcost, neighbour.Gcost, neighbour.Fcost);

                }
                Console.WriteLine("finished neighbours");
            }
        }

        private void retracePath(Tile startTile, Tile endTile)
        {

            List<Tile> path = new List<Tile>();
            Tile currentTile = endTile;
            
            while (currentTile != startTile)
            {
                path.Add(currentTile);
                currentTile = currentTile.Parent;
            }

            path.Reverse();
            grid.path = path;
            foreach (var tile in path)
            {
                Console.WriteLine("cord: {0}, {1}, hcost: {2}, gcost: {3}, fcost: {4}", tile.X, tile.Y, tile.Hcost, tile.Gcost, tile.Fcost);
            }
        }

        public static int getDistance(Tile start, Tile dest)
        {
            Cube startCube = oddrToCube(start);
            Cube destCube = oddrToCube(dest);
            return Math.Max(
                        Math.Abs(startCube.X - destCube.X),
                        Math.Max(
                            Math.Abs(startCube.Y - destCube.Y),
                            Math.Abs(startCube.Z - destCube.Z)));
        }

        private static Cube oddrToCube(Tile tile)
        {
            var x = tile.X - (tile.Y - (tile.Y & 1)) / 2;
            var z = tile.Y;
            var y = -x - z;
            return new Cube(x, y, z);
        }
        private class Cube
        {
            public int X;
            public int Y;
            public int Z;

            public Cube(int x, int y, int z)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
            }
        }
    }



}
