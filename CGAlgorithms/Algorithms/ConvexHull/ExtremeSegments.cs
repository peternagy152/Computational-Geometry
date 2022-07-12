using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGAlgorithms;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        public static int collinear(double x1, double y1, double x2,
                          double y2, double x3, double y3)
        {

            /* Calculation the area of 
            triangle. We have skipped
            multiplication with 0.5 to
            avoid floating point computations */
            double a = x1 * (y2 - y3) +
                    x2 * (y3 - y1) +
                    x3 * (y1 - y2);

            if (a == 0)
                return 1;
            else
                return 0;
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count < 4)
            {
                outPoints = points;
            }
            else
            {
                //points = CGAlgorithms.Algorithms.ConvexHull.Class1.SpecialCaseTriangle(points);
                for (int i = 0; i < points.Count; i++)
                {
                    for (int j = 0; j < points.Count; j++)
                    {
                        if (i == j)
                            continue;
                        Line LineTest = new Line(points[i], points[j]);
                        List<bool> Orientation = new List<bool>();
                        for (int k = 0; k < points.Count; k++)
                        {
                            if (k == j)
                                continue;
                            if (HelperMethods.CheckTurn(LineTest, points[k]) == Enums.TurnType.Right)
                                Orientation.Add(true);
                            else
                                Orientation.Add(false);
                        }
                        bool ExtremeSegment = true;
                        for (int l = 1; l < Orientation.Count; l++)
                        {
                            if (Orientation[l] != Orientation[0])
                            {
                                ExtremeSegment = false;
                            }
                        }
                        if (ExtremeSegment == true)
                        {
                            if (!outPoints.Contains(points[i]))
                                outPoints.Add(points[i]);
                            if (!outPoints.Contains(points[j]))
                                outPoints.Add(points[j]);

                        }
                        ExtremeSegment = true;


                       

                    }

                }

                int upper = 0;
                for (int i = 0; i < outPoints.Count; i++)
                {
                    if (points[i].Y > points[upper].Y)
                        upper = i;
                }


                List<Point> tempPoint = new List<Point>();
                tempPoint = outPoints;
                tempPoint = tempPoint.OrderBy(p => p.Y).ToList();
                tempPoint = tempPoint.OrderBy(p => Math.Atan2(p.Y - tempPoint[upper].Y, p.X - tempPoint[upper].X)).ToList();

                List<Point> Collinear = new List<Point>();

                for (int i = 0; i <= tempPoint.Count - 2; i++)
                {
                    if (i == tempPoint.Count - 2)
                    {
                        Line temp = new Line(tempPoint[tempPoint.Count - 2], tempPoint[0]);
                        if (HelperMethods.CheckTurn(temp, tempPoint[tempPoint.Count - 1]) == Enums.TurnType.Colinear)
                        {
                            Collinear.Add(tempPoint[tempPoint.Count - 1]);
                        }
                    }
                    else
                    {
                        Line temp = new Line(tempPoint[i], tempPoint[i + 2]);
                        if (HelperMethods.CheckTurn(temp, tempPoint[i + 1]) == Enums.TurnType.Colinear)
                        {
                            Collinear.Add(tempPoint[i + 1]);
                        }
                    }
                }
                for (int i = 0; i < Collinear.Count; i++)
                    outPoints.Remove(Collinear[i]);

            }

          

        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
