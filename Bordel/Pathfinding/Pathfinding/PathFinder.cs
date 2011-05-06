using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Pathfinding
{
    public class PathFinder
    {
        public int[,] map = null;
        private int width = 0;
        private int height = 0;

        public List<Point> closedset = new List<Point>();
        public List<Point> openset = new List<Point>();

        public Point start = new Point(5, 5);
        public Point finish = new Point(50, 40);

        public bool useHeuristic = false;

        public bool diagonals = false;

        public List<Point> lastPath = new List<Point>();

        public byte costEst = 0;

        public PathFinder(int w, int h)
        {
            map = new int[w, h];
            width = w;
            height = h;
        }

        public void Clear()
        {
            lastPath.Clear();
            closedset.Clear();
            openset.Clear();
        }

        public void FindPath()
        {
            lastPath.Clear();

            Point[,] pointMap = new Point[width, height];
            closedset = new List<Point>();    // The set of nodes already evaluated.
            openset = new List<Point>();    // The set of tentative nodes to be evaluated.
            bool tentative_is_better = false;

            openset.Add(start);

            Dictionary<Point, float> gScore = new Dictionary<Point, float>();
            Dictionary<Point, float> hScore = new Dictionary<Point, float>();
            Dictionary<Point, float> fScore = new Dictionary<Point, float>();
            Dictionary<Point, Point> came_from = new Dictionary<Point, Point>();

            gScore[start] = 0;    // Cost from start along best known path.
            hScore[start] = heuristic(start, finish);
            fScore[start] = hScore[start];    // Estimated total cost from start to goal through y.

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    pointMap[i, j] = new Point(i, j);
                }

            while (openset.Count > 0)
            {
                Point x = new Point(0, 0);

                float fs = float.PositiveInfinity;

                foreach (Point p in openset)
                {
                    if (fs == float.PositiveInfinity || fScore[p] < fs)
                    {
                        x = p;
                        fs = fScore[p];
                    }
                }

                if (x == finish) // jsme na konci
                {
                    if (came_from.Count == 0) lastPath.Clear();
                    else
                        lastPath = reconstructPath(came_from, came_from[finish]);
                    return;
                }

                openset.Remove(x);
                closedset.Add(x);

                // projit sousedy
                for (int i = -1; i < 2; i++)
                    for (int j = -1; j < 2; j++)
                        if (!(i == 0 && j == 0)) // nepridavat sebe
                        {
                            if (!diagonals && j * i != 0) continue;
                            try
                            {
                                tentative_is_better = false;
                                Point y = pointMap[x.X + i, x.Y + j];
                                if (map[x.X + i, x.Y + j] <= 0) continue; // nepruchozi
                                if (map[x.X + i, x.Y + j] > 255) continue; // nepruchozi

                                if (closedset.Contains(y)) continue;
                                float tentative_g_score = gScore[x] + dist(x, y);

                                if (!openset.Contains(y))
                                {
                                    openset.Add(y);
                                    tentative_is_better = true;
                                }
                                else if (tentative_g_score < gScore[y]) tentative_is_better = true;
                                else tentative_is_better = false;

                                if (tentative_is_better)
                                {
                                    came_from[y] = x;
                                    gScore[y] = tentative_g_score;
                                    hScore[y] = heuristic(y, finish);
                                    fScore[y] = gScore[y] + hScore[y];
                                }
                            }
                            catch
                            {

                            }
                        }
            }

            MessageBox.Show("Path not found!");
        }

        private List<Point> reconstructPath(Dictionary<Point, Point> came_from, Point current_node)
        {
            List<Point> p = new List<Point>();

            if (came_from.ContainsKey(current_node))
            {
                p = reconstructPath(came_from, came_from[current_node]);
                p.Add(current_node);
                return p;
            }
            else
            {
                p.Add(current_node);
                return p;
            }
        }

        private float dist(Point p1, Point p2)
        {
            float f = 0;
            float d = 1;
            if ((p1.X - p2.X) * (p1.Y - p2.Y) != 0) d *= 1.4142f; //diagonal
            float h = map[p2.X, p2.Y] - map[p1.X, p1.Y]; // vyskovy rozdil
            h = Math.Abs(h);

            switch (costEst)
            {
                case 0: // Combined
                    {
                        f = d + h;
                    }
                    break;
                case 1: // Elevation
                    {
                        f = h;
                    }
                    break;
                case 2: // Shortest
                    {
                        f = d;
                    }
                    break;
            }
            
            return f;
        }

        // distance based
        private float heuristic(Point p1, Point p2)
        {
            if (useHeuristic) return Heuristic1(p1, p2);
            return Heuristic2(p1, p2);
        }

        private float Heuristic1(Point p1, Point p2)
        {
            return (float)Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        private float Heuristic2(Point p1, Point p2)
        {
            return 0;
        }
    }
}
