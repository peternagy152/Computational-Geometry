using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGAlgorithms;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            //computing Extreme Points 
            // input -> Polygon 
            //output -> List of Points 
            //points = CGAlgorithms.Algorithms.ConvexHull.Class1.SpecialCaseTriangle(points);

            List<Point> Neglected = new List<Point>();

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    if (i == j)
                        continue;
                    for(int k = 0; k < points.Count; k++)
                    {
                        if (j == k)
                            continue;
                      for (int l = 0; l < points.Count; l++)
                        {
                            if (l == k)
                                continue;
                            if (HelperMethods.PointInTriangle(points[l], points[i], points[j], points[k]) == Enums.PointInPolygon.Inside)
                            {
                                if(!Neglected.Contains(points[l]))
                                Neglected.Add(points[l]);
                            }
                        }
         
                       
                    }
                }
            }
            
            for (int i = 0; i < points.Count ; i++)
            {
                if(Neglected.Contains(points[i]))
                {
                    continue;
                }
                else
                {
                    if(!outPoints.Contains(points[i]))
                    outPoints.Add(points[i]);
                }
            }

            int   left = 0;
            for (int i = 0; i < outPoints.Count; i++)
            {
                if (outPoints[i].X < outPoints[left].X)
                    left = i;
            }


            List<Point> tempPoint = new List<Point>();
            tempPoint = outPoints;
            tempPoint = tempPoint.OrderBy(p => p.Y).ToList();
            tempPoint = tempPoint.OrderBy(p => Math.Atan2(p.Y - tempPoint[left].Y, p.X - tempPoint[left].X)).ToList();

            List<Point> Collinear = new List<Point>();

            for (int i = 0; i <= tempPoint.Count - 2; i++)
            {
                if (i == tempPoint.Count - 2)
                {
                    Line temp = new Line(tempPoint[tempPoint.Count - 2], tempPoint[0]);
                    if (HelperMethods.CheckTurn(temp, tempPoint[tempPoint.Count-1]) == Enums.TurnType.Colinear)
                    {
                        Collinear.Add(tempPoint[tempPoint.Count-1]);
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

            //    if (CGAlgorithms.Algorithms.ConvexHull.ExtremeSegments.collinear(outPoints[i].X, outPoints[i].Y, outPoints[i + 1].X, outPoints[i + 1].Y, outPoints[i + 2].X, outPoints[i + 2].Y) == 1)
            //        Collinear.Add(outPoints[i + 1]);
            //}
            //Point Origin = new Point(0, 0);
            //if (outPoints.Contains(Origin))
            //    outPoints.Remove(Origin);
            //Origin = new Point(100, 0);
            //if (outPoints.Contains(Origin))
            //    outPoints.Remove(Origin);


            for (int i = 0; i < Collinear.Count; i++)
                outPoints.Remove(Collinear[i]);
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
