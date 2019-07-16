using ASU2019_NetworkedGameWorkshop.model.collection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
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

        /// <summary>
        /// raises exception if no path found
        /// </summary>
        /// <param name="currentTile"></param>
        /// <returns></returns>
        internal List<Tile> findPathToClosestEnemy(Tile currentTile, Character.Teams team)
        {
            List<Character> enemyList = (team == Character.Teams.Red) ? grid.TeamBlue : grid.TeamRed;
            List<Tile> shortestPath = null;

            foreach (Character enemy in enemyList)
            {
                List<Tile> path;

                try
                {
                    path = findPath(currentTile, enemy.Tile);
                }
                catch (PathNotFoundException)
                {
                    continue;
                }

                if (shortestPath == null)
                {
                    shortestPath = path;
                }
                else
                {
                    shortestPath = shortestPath.Count <= path.Count ? shortestPath : path;
                }
            }

            if (shortestPath == null)
            {
                throw new PathNotFoundException();
            }
            return shortestPath;
        }

        public List<Tile> findPath(Tile startTile, Tile endTile)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Heap<Tile> openSet = new Heap<Tile>(grid.MaxSize);
            HashSet<Tile> closedSet = new HashSet<Tile>();
            openSet.Add((Tile)startTile.Clone());

            while (openSet.Count > 0)
            {
                Tile currentTile = openSet.RemoveFirst();

                closedSet.Add(currentTile);
                if (currentTile == endTile)
                {
                    stopWatch.Stop();
                    Console.WriteLine("path found in: {0}", stopWatch.ElapsedMilliseconds);
                    return retracePath(startTile, endTile);
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

            //no path was returned in the while loop
            throw new PathNotFoundException();
        }

        private List<Tile> retracePath(Tile startTile, Tile endTile)
        {

            List<Tile> path = new List<Tile>();
            Tile currentTile = endTile;

            while (currentTile != startTile)
            {
                currentTile.InPath = true;
                path.Add(currentTile);
                currentTile = currentTile.Parent;
            }

            path.Reverse();
            foreach (var tile in path)
            {
                Console.WriteLine("cord: {0}, {1}, hcost: {2}, gcost: {3}, fcost: {4}", tile.X, tile.Y, tile.Hcost, tile.Gcost, tile.Fcost);
            }

            return path;
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

        [Serializable]
        private class PathNotFoundException : Exception
        {
            public PathNotFoundException()
            {
            }

            public PathNotFoundException(string message) : base(message)
            {
            }

            public PathNotFoundException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected PathNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }



}
