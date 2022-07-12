using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGAlgorithms;

namespace CGAlgorithms.Algorithms.ConvexHull
{
   

    public class Incremental : Algorithm
    {

        public static int orientation(Point p1, Point p2, Point p3)
        {

            double exp = (p2.Y - p1.Y) * (p3.X - p2.X) - (p2.X - p1.X) * (p3.Y - p2.Y);

            if (exp == 0) return 0;  // collinear

            return (exp > 0) ? 1 : 2; // clock or counterclock wise
        }
        public static List<Point> Tangent(List<Point> Polygon , Point SelectedPoint)
        {
            List<Point> twoTangents = new List<Point>();
            Point X, NextX, PreX;
            int Counter = 0;
            for (int i = 0; i < Polygon.Count; i++)
            {
                if (i == 0)
                {
                    X = Polygon[i];
                    PreX = Polygon[Polygon.Count - 1];
                    NextX = Polygon[i + 1];
                }
                else if (i == Polygon.Count - 1)
                {
                    X = Polygon[i];
                    PreX = Polygon[i - 1];
                    NextX = Polygon[0];
                }
                else
                {
                    X = Polygon[i];
                    PreX = Polygon[i - 1];
                    NextX = Polygon[i + 1];
                }

                if (orientation(PreX, X, SelectedPoint) != orientation(X, NextX, SelectedPoint))
                {
                    Counter++;
                    twoTangents.Add(X);
          
                }
                if (Counter == 2)
                    break;

            }
            return twoTangents;
        }
        //public static List<Point> NewTangent(List<Point> Polygon, Point SelectedPoint)
        //{

        //}
        public static bool onSegment(Point p, Point q, Point r)
        {
            if (q.X <= Math.Max(p.X, r.X) &&
                q.X >= Math.Min(p.X, r.X) &&
                q.Y <= Math.Max(p.Y, r.Y) &&
                q.Y >= Math.Min(p.Y, r.Y))
            {
                return true;
            }
            return false;
        }
       
        public static List<Point> ConvertLineToPoint(List<Line> LinePolygon)
        {
            List<Point> PointPolygon = new List<Point>();
            for(int i = 0;i < LinePolygon.Count;i++)
            {
                PointPolygon.Add(LinePolygon[i].Start);
            }
            //while (LinePolygon.Count != 0)
            //{
            //    Line temp = LinePolygon.Pop();
            //    PointPolygon.Add(temp.Start);
            //}
                
            return PointPolygon;
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count < 4)
                outPoints = points;
            else
            {


                // points =CGAlgorithms.Algorithms.ConvexHull.Class1.Case5000Points(points);
                //Incremental Algorithm
                int upper = 0, left = 0, right = 0;
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i].X < points[left].X)
                        left = i;
                    if (points[i].X > points[right].X)
                        right = i;
                    if (points[i].Y > points[upper].Y)
                        upper = i;

                }

                // Final Polygon
                List<Point> Polygon = new List<Point>();
                Polygon.Add(points[upper]);
                Polygon.Add(points[left]);
                Polygon.Add(points[right]);


                // Lines To Add
                List<Line> PolyLines = new List<Line>();
                Line temp = new Line(points[upper], points[left]);
                PolyLines.Add(temp);
                temp = new Line(points[left], points[right]);
                PolyLines.Add(temp);
                temp = new Line(points[right], points[upper]);
                PolyLines.Add(temp);

                for (int j = 0; j < PolyLines.Count; j++)
                {
                    List<Point> AllTangents = new List<Point>();
                    Line SelectedLine = PolyLines[j];
                    List<Point> PosiblePoints = new List<Point>();
                    for (int i = 0; i < points.Count; i++)
                    {
                        if (HelperMethods.CheckTurn(SelectedLine, points[i]) == Enums.TurnType.Right)
                            PosiblePoints.Add(points[i]);
                    }
                    if (PosiblePoints.Count == 0)
                        continue;
                    double maxDistance = 0;
                    int maxElement = 0;
                    for (int i = 0; i < PosiblePoints.Count; i++)
                    {
                        if (CGAlgorithms.Algorithms.ConvexHull.QuickHull.DistanacePointLine(PosiblePoints[i], SelectedLine) > maxDistance)
                        {
                            maxDistance = CGAlgorithms.Algorithms.ConvexHull.QuickHull.DistanacePointLine(PosiblePoints[i], SelectedLine);
                            maxElement = i;
                        }

                    }
                    //Polygon.Add(PosiblePoints[maxElement]);

                    AllTangents = CGAlgorithms.Algorithms.ConvexHull.Incremental.Tangent(Polygon, PosiblePoints[maxElement]);
                    if (orientation(AllTangents[0], PosiblePoints[maxElement], AllTangents[1]) == 2)
                    {
                        for (int i = Polygon.IndexOf(AllTangents[0]) + 1; i < Polygon.IndexOf(AllTangents[1]); i++)
                        {
                            Polygon.RemoveAt(i);
                        }
                        Polygon.Insert(Polygon.IndexOf(AllTangents[0]) + 1, PosiblePoints[maxElement]);

                        Line Line1 = new Line(AllTangents[0], PosiblePoints[maxElement]);
                        Line Line2 = new Line(PosiblePoints[maxElement], AllTangents[1]);
                        PolyLines.Add(Line1);
                        PolyLines.Add(Line2);
                    }
                    else
                    {
                        for (int i = Polygon.IndexOf(AllTangents[1]) + 1; i < Polygon.IndexOf(AllTangents[0]); i++)
                        {
                            Polygon.RemoveAt(i);
                        }
                        Polygon.Insert(Polygon.IndexOf(AllTangents[1]) + 1, PosiblePoints[maxElement]);
                        Line Line1 = new Line(AllTangents[1], PosiblePoints[maxElement]);
                        Line Line2 = new Line(PosiblePoints[maxElement], AllTangents[0]);
                        PolyLines.Add(Line1);
                        PolyLines.Add(Line2);
                    }


                }

                outPoints = Polygon;
                for (int i = 0; i < outPoints.Count - 2; i++)
                    if (outPoints[i] == outPoints[outPoints.Count-1])
                    {
                        outPoints.RemoveAt(i);
                        break;
                    }
                        
            }

            //if (points.Count < 4)
            //    outPoints = points;
            //else
            //{
            //    // points = CGAlgorithms.Algorithms.ConvexHull.Class1.Case5000Points(points);

            //    int upper = 0, lower = 0, left = 0, right = 0;
            //    for (int i = 0; i < points.Count; i++)
            //    {
            //        if (points[i].X < points[left].X)
            //            left = i;
            //        if (points[i].X > points[right].X)
            //            right = i;
            //        if (points[i].Y > points[upper].Y)
            //            upper = i;
            //        if (points[i].Y < points[lower].Y)
            //            lower = i;

            //    }

            //    List<Line> Polygon = new List<Line>();
            //    Line temp = new Line(points[upper], points[left]);
            //    Polygon.Add(temp);
            //    temp = new Line(points[left], points[right]);
            //    Polygon.Add(temp);
            //    temp = new Line( points[right] , points[upper]);
            //    Polygon.Add(temp);

            //outPoints.Add(points[upper]);
            //outPoints.Add(points[left]);
            //outPoints.Add(points[right]);
            //int t = 0;
            //List<Point> PPP = new List<Point>();
            //for(int j = 0; j < Polygon.Count;j++)
            //    {
            //        List<Point> AllTangents = new List<Point>();
            //        Line SelectedLine = Polygon[j];
            //        List<Point> PosiblePoints = new List<Point>();
            //        for (int i = 0; i < points.Count; i++)
            //        {
            //            if (HelperMethods.CheckTurn(SelectedLine, points[i]) == Enums.TurnType.Right)
            //                PosiblePoints.Add(points[i]);
            //        }

            //    //outPoints.AddRange(PosiblePoints);
            //    if (PosiblePoints.Count == 0)
            //        continue;
            //    double maxDistance = 0;
            //    int maxElement = 0;
            //    for (int i = 0; i < PosiblePoints.Count; i++)
            //    {
            //        if (CGAlgorithms.Algorithms.ConvexHull.QuickHull.DistanacePointLine(PosiblePoints[i], SelectedLine) > maxDistance)
            //        {
            //            maxDistance = CGAlgorithms.Algorithms.ConvexHull.QuickHull.DistanacePointLine(PosiblePoints[i], SelectedLine);
            //            maxElement = i;
            //        }

            //    }
            //    outPoints.Add(PosiblePoints[maxElement]);
            //    AllTangents = Tangent(ConvertLineToPoint(Polygon), PosiblePoints[maxElement]);

            //    // Merge Tangents 
            //    if (orientation(AllTangents[0], PosiblePoints[maxElement], AllTangents[1]) == 2)
            //    {
            //        Line LowerTangent = new Line(points[0],points[1]);
            //        Line UpperTangent = new Line(points[0], points[1]); ;
            //        for (int i = 0; i < Polygon.Count; i++)
            //        {
            //            if (Polygon[i].End == AllTangents[0])
            //                LowerTangent = Polygon[i];
            //            if(Polygon[i].Start == AllTangents[1])
            //                UpperTangent = Polygon[i];
            //        }
            //        Line Line1 = new Line(AllTangents[0], PosiblePoints[maxElement]);
            //        Line Line2 = new Line(PosiblePoints[maxElement], AllTangents[1]);
            //        //for (int i = Polygon.IndexOf(LowerTangent) + 1; i<=Polygon.IndexOf(UpperTangent);i++)
            //        //{
            //        //    Polygon.Remove(Polygon[i]);
            //        //}
            //        Polygon.Add(Line1);
            //        Polygon.Add(Line2);
            //        //Polygon.Insert(Polygon.IndexOf(LowerTangent) + 1, Line1);
            //        //Polygon.Insert(Polygon.IndexOf(LowerTangent) + 2, Line2);


            //    }

            //    else
            //    {

            //        Line LowerTangent = new Line(points[0], points[1]);
            //        Line UpperTangent = new Line(points[0], points[1]); ;
            //        for (int i = 0; i < Polygon.Count; i++)
            //        {
            //            if (Polygon[i].End == AllTangents[1])
            //                LowerTangent = Polygon[i];
            //            if (Polygon[i].Start == AllTangents[0])
            //                UpperTangent = Polygon[i];
            //        }
            //        Line Line1 = new Line(AllTangents[1], PosiblePoints[maxElement]);
            //        Line Line2 = new Line(PosiblePoints[maxElement], AllTangents[0]);
            //       // for (int i = Polygon.IndexOf(LowerTangent)  ; i < Polygon.IndexOf(UpperTangent); i++)
            //           // Polygon.Remove(Polygon[i]);
            //        Polygon.Add( Line1);
            //        Polygon.Add( Line2);


            //    }

            //    AllTangents.Clear();


            //}


            //for(int i = 0; i < Polygon.Count;i++)
            //{
            //    if(!outPoints.Contains(Polygon[i].Start))
            //        outPoints.Add(Polygon[i].Start);
            //}
            //for (int i = 0; i < outPoints.Count - 2; i++)
            //    if (outPoints[i] == outPoints[i + 2])
            //        outPoints.RemoveAt(i);
            //    Polygon.Add(points[upper]);
            //    Polygon.Add(points[left]);
            //    //Polygon.Add(points[lower]);
            //    Polygon.Add(points[right]);


            //outPoints = Polygon;
            //    //outPoints = Polygon;





            //for (int i = 0; i < points.Count; i++)
            //{
            //    if (i == right || i == left || i == upper)
            //        continue;

            //    if (isInside(Polygon, points[i]))
            //        continue;
            //    List<Point> Tangents = Tangent(Polygon, points[i]);
            //    if (orientation(Tangents[0], points[i], Tangents[1]) == 2)
            //    {
            //        for (int j = Polygon.IndexOf(Tangents[0]) + 1; j < Polygon.IndexOf(Tangents[1]); j++)
            //            Polygon.Remove(Polygon[j]);
            //        Polygon.Insert(Polygon.IndexOf(Tangents[0]) + 1, points[i]);
            //    }

            //    else
            //    {
            //        for (int j = Polygon.IndexOf(Tangents[1]) + 1; j < Polygon.IndexOf(Tangents[0]); j++)
            //            Polygon.Remove(Polygon[j]);
            //        Polygon.Insert(Polygon.IndexOf(Tangents[1]) + 1, points[i]);

            //    }

            //    Tangents.Clear();
            //}

            //outPoints = Polygon;


            // }



        }

        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}
