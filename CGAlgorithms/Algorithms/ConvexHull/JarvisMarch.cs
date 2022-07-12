using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGAlgorithms;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public static int orientation(Point p, Point q, Point r)
        {
            int val = (int)((q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y));
            if (val == 0) return 0;      // colinear
            return (val > 0) ? 1 : 2;   // clock or counterclock wise
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // Get the Leftmost Point [L]
            // find the Right most point Respect to L 

            //points = CGAlgorithms.Algorithms.ConvexHull.Class1.SpecialCaseTriangle(points);
            Point Origin = new Point(0, 0);
            if (points.Count < 4)
            {
                outPoints = points;
            }
            else
            {
                int left = 0;
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i].X < points[left].X)
                        left = i;


                }
                int next;

                int current = left;
                do
                {
                    outPoints.Add(points[current]);
                    next = (current + 1) % points.Count;
                    for (int i = 0; i < points.Count; i++)
                    {

                        if (orientation(points[current], points[i], points[next]) == 2)
                        {
                            next = i;
                        }


                    }
                    current = next;
                } while (current != left);


                List<Point> Collinear = new List<Point>();
                for (int i = 0; i < outPoints.Count; i++)
                {
                    if (i == outPoints.Count - 2)
                        break;
                    if (CGAlgorithms.Algorithms.ConvexHull.ExtremeSegments.collinear(outPoints[i].X, outPoints[i].Y, outPoints[i + 1].X, outPoints[i + 1].Y, outPoints[i + 2].X, outPoints[i + 2].Y) == 1)
                        Collinear.Add(outPoints[i + 1]);
                }
                
                if (outPoints.Contains(Origin))
                    outPoints.Remove(Origin);

                for (int i = 0; i < Collinear.Count; i++)
                    outPoints.Remove(Collinear[i]);

            }

            #region
            //int lower = 0;
            //for (int i = 0; i < points.Count; i++)
            //    if(points[i].Y < points[lower].Y)
            //        lower = i;

            //points = points.OrderBy(p => Math.Atan2(p.Y - points[0].Y, p.X - points[0].X)).ToList();
            //for (int i = 0; i < 1; i++)
            //outPoints.Add(points[i]);

            ////Point Termnate = points[lower];
            //int tt = 0;
            //do
            //{
            //    points = points.OrderBy(p => Math.Atan2(p.Y - points[lower].Y, p.X - points[lower].X)).ToList();
            //    outPoints.Add(points[0]);
            //    lower++;
            //    tt++;
            //}
            //while (tt < 5);//points[lower] != Termnate);
            #endregion



        }

        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
