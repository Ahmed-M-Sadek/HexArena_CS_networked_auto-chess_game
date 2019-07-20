using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.collection;
using ASU2019_NetworkedGameWorkshop.model.grid;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ASU2019_NetworkedGameWorkshop.controller {
    public class PathFinding {
        private PathFinding() {
        }

        /// <summary>
        /// raises exception if no path found
        /// </summary>
        /// <param name="currentTile"></param>
        /// <returns></returns>

        
        internal static Tuple<List<Tile>, Character> findPathToClosestEnemy(Tile currentTile, Character.Teams team, Grid grid) {
            List<Character> enemyList = (team == Character.Teams.Red) ? grid.TeamBlue : grid.TeamRed;
            Character closestEnemy = null;
            List<Tile> shortestPath = null;

            Tile[,] tilesClone = (Tile[,]) grid.Tiles.Clone();
            foreach(Character enemy in enemyList) {
                List<Tile> path;

                try {
                    path = findPath(currentTile, enemy.CurrentTile, grid, tilesClone);
                } catch(PathNotFoundException) {
                    continue;
                }

                if(shortestPath == null) {
                    shortestPath = path;
                    closestEnemy = enemy;
                } else {
                    shortestPath = shortestPath.Count <= path.Count ? shortestPath : path;
                    closestEnemy = enemy;
                }
            }

            if(shortestPath == null) {
                throw new PathNotFoundException();
            }
            return Tuple.Create(shortestPath, closestEnemy);
        }
        internal static Tuple<List<Tile>, Character> findClosestEnemy(Tile currentTile, Character.Teams team, Grid grid)
        {
            List<Character> enemyList = (team == Character.Teams.Red) ? grid.TeamBlue : grid.TeamRed;
            Character closestEnemy = null;
            List<Tile> shortestPath = null;

            Tile[,] tilesClone = (Tile[,])grid.Tiles.Clone();
            foreach (Character enemy in enemyList)
            {
                if (enemy.CurrentTile == currentTile)
                {
                    continue;
                }
                List<Tile> path;
                    
                try
                {
                    path = findPath(currentTile, enemy.CurrentTile, grid, tilesClone);
                }
                catch (PathNotFoundException)
                {
                    continue;
                }

                if (shortestPath == null)
                {
                    shortestPath = path;
                    closestEnemy = enemy;
                }
                else
                {
                    shortestPath = shortestPath.Count <= path.Count ? shortestPath : path;
                    closestEnemy = enemy;
                }
            }

            if (shortestPath == null)
            {
                throw new PathNotFoundException();
            }
            return Tuple.Create(shortestPath, closestEnemy);
        }
        public static List<Tile> findPath(Tile startTile, Tile endTile, Grid grid, Tile[,] tilesClone) {
            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();
            Heap<Tile> openSet = new Heap<Tile>(grid.MaxSize);
            HashSet<Tile> closedSet = new HashSet<Tile>();
            openSet.Add(tilesClone[startTile.X, startTile.Y]);

            while(openSet.Count > 0) {
                Tile currentTile = openSet.RemoveFirst();

                closedSet.Add(currentTile);
                if(currentTile.Equals(endTile)) {
                    //stopWatch.Stop();
                    //Console.WriteLine("path found in: {0}", stopWatch.ElapsedMilliseconds);
                    return retracePath(startTile, currentTile, grid);
                }
                //Console.WriteLine("neighbours of  {0}, {1}", currentTile.X, currentTile.Y);
                foreach(Tile neighbour in grid.getNeighbours(currentTile, tilesClone)) {
                    if(closedSet.Contains(neighbour) || !(neighbour.Walkable || neighbour.Equals(endTile)))
                        continue;

                    int newNeighbourCost = currentTile.Gcost + getDistance(currentTile, neighbour);
                    if(newNeighbourCost < neighbour.Gcost || !openSet.Contains(neighbour)) {

                        neighbour.Gcost = newNeighbourCost;
                        neighbour.Hcost = getDistance(neighbour, endTile);
                        neighbour.Parent = currentTile;

                        if(!openSet.Contains(neighbour))
                            openSet.Add(neighbour);

                    }
                    //Console.WriteLine("neighbour: {0}, {1}, hcost: {2}, gcost: {3}, fcost: {4}", neighbour.X, neighbour.Y, neighbour.Hcost, neighbour.Gcost, neighbour.Fcost);

                }
                //Console.WriteLine("finished neighbours");
            }

            //no path was returned in the while loop
            throw new PathNotFoundException();
        }

        private static List<Tile> retracePath(Tile startTile, Tile endTile, Grid grid) {

            List<Tile> path = new List<Tile>();
            Tile currentTile = endTile;

            while(!currentTile.Equals(startTile)) {
                Tile currentGridTile = grid.Tiles[currentTile.X, currentTile.Y];
                currentGridTile.InPath = true;
                path.Add(currentGridTile);
                currentTile = currentTile.Parent;
            }

            path.Reverse();
            foreach(var tile in path) {
                //Console.WriteLine("cord: {0}, {1}, hcost: {2}, gcost: {3}, fcost: {4}", tile.X, tile.Y, tile.Hcost, tile.Gcost, tile.Fcost);
            }

            return path;
        }

        public static int getDistance(Tile start, Tile dest) {
            Cube startCube = oddrToCube(start);
            Cube destCube = oddrToCube(dest);
            return Math.Max(
                        Math.Abs(startCube.X - destCube.X),
                        Math.Max(
                            Math.Abs(startCube.Y - destCube.Y),
                            Math.Abs(startCube.Z - destCube.Z)));
        }

        private static Cube oddrToCube(Tile tile) {
            var x = tile.X - (tile.Y - (tile.Y & 1)) / 2;
            var z = tile.Y;
            var y = -x - z;
            return new Cube(x, y, z);
        }
        private class Cube {
            public int X;
            public int Y;
            public int Z;

            public Cube(int x, int y, int z) {
                this.X = x;
                this.Y = y;
                this.Z = z;
            }
        }

        [Serializable]
        public class PathNotFoundException : Exception {
            public PathNotFoundException() {
            }

            public PathNotFoundException(string message) : base(message) {
            }

            public PathNotFoundException(string message, Exception innerException) : base(message, innerException) {
            }

            protected PathNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {
            }
        }
    }



}
