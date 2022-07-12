using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGAlgorithms;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    
    public class GrahamScan : Algorithm
    {
        
        public List<Point> SwapPoints( Point p1 , Point p2)
        {
            Point temp = p1;
            p1 = p2;
            p2 = temp;
            List<Point> NewPoints = new List<Point>();
            NewPoints.Add(p1);
            NewPoints.Add(p2);
            return NewPoints;
        }
        public int orientation(Point p, Point q, Point r)
        {
             double val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);

            if (val == 0) return 0;  // collinear
            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            //points = CGAlgorithms.Algorithms.ConvexHull.Class1.SpecialCaseTriangle(points);

            int lower = 0;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Y < points[lower].Y)
                    lower = i;

            }
            List<Point> NewPoints = new List<Point>();
            NewPoints = SwapPoints(points[0], points[lower]);
            points[0] = NewPoints[0];
            Point testp = new Point(100, 0);
            points[lower] = NewPoints[1];

            //Sort  Point Angularly 
            points = points.OrderBy(p => p.Y).ToList();
            points = points.OrderBy(p => Math.Atan2(p.Y - points[0].Y , p.X - points[0].X)).ToList();

           
           List<Point> stack = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                if(i == 0 || i == 1 || i == 2)
                    stack.Add(points[i]);
                else
                {
                    stack.Add(points[i]);
                    Point p1, p2, p3;
                   

                    // P3 -> P2 -> P1 
                    while(true)
                    {
                        p1 = stack[stack.Count - 1];
                        p2 = stack[stack.Count - 2];
                        p3 = stack[stack.Count - 3];
                        Line tempLine = new Line(p3, p2);
                        if (HelperMethods.CheckTurn(tempLine, p1) == Enums.TurnType.Left)
                            break;
                            stack.Remove(p2);

                    }
                    
                }
               

            }
            outPoints = stack;
         





            if(outPoints.Contains(testp))
                outPoints.Remove(testp);
          


        }

        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}
