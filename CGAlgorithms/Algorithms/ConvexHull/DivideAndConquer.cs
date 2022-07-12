using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {
        public List<Point> SortAlongXAxis(List<Point> UnsortedList)
        {
            Point temp;
            for (int i = 0; i < UnsortedList.Count; i++)
            {
                for (int j = i + 1; j < UnsortedList.Count; j++)
                {
                    if (UnsortedList[i].X > UnsortedList[j].X)
                    {
                        temp = UnsortedList[i];
                        UnsortedList[i] = UnsortedList[j];
                        UnsortedList[j] = temp;
                    }
                }
            }
            return UnsortedList;
        }
        public static List<Point> GrahamAlgorithm(List<Point> points)
        {
            points = points.OrderBy(p => p.Y).ToList();
            points = points.OrderBy(p => Math.Atan2(p.Y - points[0].Y, p.X - points[0].X)).ToList();
            List<Point> stack = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                if (i == 0 || i == 1 || i == 2)
                    stack.Add(points[i]);
                else
                {
                    stack.Add(points[i]);
                    Point p1, p2, p3;
                    while (true)
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
            List<Line> outlines = new List<Line>();
            for (int i = 0; i < stack.Count; i++)
            {

                if (i == stack.Count - 1)
                {
                    Line l2 = new Line(stack[i], stack[0]);
                    outlines.Add(l2);
                    break;
                }
                Line l1 = new Line(stack[i], stack[i + 1]);
                outlines.Add(l1);

            }
            return stack;
        }
        public static int orientation(Point p1, Point p2, Point p3)
        {

            double exp = (p2.Y - p1.Y) * (p3.X - p2.X) - (p2.X - p1.X) * (p3.Y - p2.Y);

            if (exp == 0) return 0;  // collinear

            return (exp > 0) ? 1 : -1; // clock or counterclock wise
        }

        public static Line LowerTangent(List<Point> LeftPolygon, List<Point> RightPolygon)
        {
            int rightmost = 0;
            for (int i = 0; i < LeftPolygon.Count; i++)
            {
                if (LeftPolygon[i].X > LeftPolygon[rightmost].X)
                    rightmost = i;

            }

            Point X = LeftPolygon[rightmost], PreX, NextX;

            //Loop Variabls  
            List<Point> SelectedPolygon = new List<Point>();
            SelectedPolygon = RightPolygon;
            Point SelectedPoint = LeftPolygon[rightmost];
            Point PerviosPoint = LeftPolygon[rightmost];

            Boolean LowerTangent = false;
            int r = 0;
            while (true)
            {
                for (int i = 0; i < SelectedPolygon.Count; i++)
                {
                    if (i == 0)
                    {
                        X = SelectedPolygon[i];
                        PreX = SelectedPolygon[SelectedPolygon.Count - 1];
                        NextX = SelectedPolygon[i + 1];
                    }
                    else if (i == SelectedPolygon.Count - 1)
                    {
                        X = SelectedPolygon[i];
                        PreX = SelectedPolygon[i - 1];
                        NextX = SelectedPolygon[0];
                    }
                    else
                    {
                        X = SelectedPolygon[i];
                        PreX = SelectedPolygon[i - 1];
                        NextX = SelectedPolygon[i + 1];
                    }
                    Line TempLine1 = new Line(PreX, X);
                    Line TempLine2 = new Line(X, NextX);
                    if (orientation(PreX, X, SelectedPoint) != orientation(X, NextX, SelectedPoint))
                    {
                        if (X == PerviosPoint)
                        {
                            LowerTangent = true;
                        }
                        PerviosPoint = SelectedPoint;
                        SelectedPoint = X;

                        if (SelectedPolygon == RightPolygon)
                            SelectedPolygon = LeftPolygon;
                        else
                            SelectedPolygon = RightPolygon;
                        break;
                    }



                }
                if (LowerTangent == true)
                    break;
                //r++;
            }

            //outPoints.Add(X);
            // outPoints.Add(PerviosPoint);
            Line LowerTan = new Line(PerviosPoint, X);
            List<Point> output = new List<Point>();
            output.Add(X);
            output.Add(PerviosPoint);
            return LowerTan;

        }
        public static Line UpperTangent(List<Point> LeftPolygon, List<Point> RightPolygon)
        {
            int rightmost = 0;
            for (int i = 0; i < LeftPolygon.Count; i++)
            {
                if (LeftPolygon[i].X > LeftPolygon[rightmost].X)
                    rightmost = i;

            }
            // Default for LeftPolygon is Left 
            // Default for RightPolygon is Right 
            RightPolygon.Reverse();

            int leftmost = 0;
            for (int i = 0; i < RightPolygon.Count; i++)
            {
                if (RightPolygon[i].X < RightPolygon[leftmost].X)
                    leftmost = i;

            }

            // outPoints.Add(RightPolygon[leftmost]);
            //outPoints.Add(LeftPolygon[rightmost]);

            //Declaration
            List<Point> SelectedPolygon = new List<Point>();
            int DefaultOrientation;
            Point SelectedPoint;
            Point X, NextX, Pervious = null;
            int Start;


            //Intialize 
            SelectedPolygon = LeftPolygon;
            DefaultOrientation = -1;
            SelectedPoint = RightPolygon[leftmost];
            Start = rightmost;

            //Start Loop 
            int z = 0;
            Boolean Found = false;
            while (true)
            {
                for (int i = Start; i < SelectedPolygon.Count; i++)
                {
                    // Handelling 
                    if (i == SelectedPolygon.Count - 1)
                    {
                        X = SelectedPolygon[i];
                        NextX = SelectedPolygon[0];
                    }
                    else
                    {
                        X = SelectedPolygon[i];
                        NextX = SelectedPolygon[i + 1];
                    }
                    if (orientation(SelectedPoint, X, NextX) == DefaultOrientation)
                    {
                        if (X == Pervious)
                            Found = true;
                        Pervious = SelectedPoint;
                        SelectedPoint = X;
                        if (SelectedPolygon == LeftPolygon)
                        {
                            SelectedPolygon = RightPolygon;
                            DefaultOrientation = 1;
                            Start = leftmost;

                        }
                        else
                        {
                            SelectedPolygon = LeftPolygon;
                            DefaultOrientation = -1;
                            Start = rightmost;
                        }

                        break;
                    }

                    z++;
                }
                if (Found == true)
                    break;
            }
            Line L1 = new Line(SelectedPoint, Pervious);
            List<Point> output = new List<Point>();
            output.Add(SelectedPoint);
            output.Add(Pervious);
            return L1; ;


        }
        public static Line Tangent(List<Point> a, List<Point> b)
        {
            int n1 = a.Count(), n2 = b.Count();

            int ia = 0, ib = 0;
            for (int i = 1; i < n1; i++)
                if (a[i].X > a[ia].X)
                    ia = i;

            // ib -> leftmost point of b
            for (int i = 1; i < n2; i++)
                if (b[i].X < b[ib].X)
                    ib = i;

            // finding the upper tangent
            int inda = ia, indb = ib;
            bool done = false;

            int uppera = inda, upperb = indb;
            inda = ia;
            indb = ib;
            done = false;
            int g = 0;
            while (!done)//finding the lower tangent
            {
                done = true;
                while (orientation(a[inda], b[indb], b[(indb + 1) % n2]) >= 0)
                    indb = (indb + 1) % n2;

                while (orientation(b[indb], a[inda], a[(n1 + inda - 1) % n1]) <= 0)
                {
                    inda = (n1 + inda - 1) % n1;
                    done = false;
                }
            }

            int lowera = inda, lowerb = indb;

            Line L1 = new Line(a[lowera], b[lowerb]);
            return L1;
        }
        public static List<Point> Merge(List<Point> LeftPolygon, List<Point> RightPolygon)
        {

            //Line L1 = LowerTangent(LeftPolygon, RightPolygon);
            Line L1 = Tangent(LeftPolygon, RightPolygon); // Lower 
            Line L2 = UpperTangent(LeftPolygon, RightPolygon); // Upper 
            List<Point> Unwanted = new List<Point>();
            List<Point> Lower = new List<Point>();


            //return Lower;
            LeftPolygon.Reverse();
            int tempo = LeftPolygon.IndexOf(L1.Start);
            if (LeftPolygon.Contains(L2.Start))
            {
                for(int i = LeftPolygon.IndexOf(L2.Start) + 1 ; i < LeftPolygon.IndexOf(L1.Start);i++)
                    Unwanted.Add(LeftPolygon[i]);
            }
            else
            {
                for (int i = LeftPolygon.IndexOf(L1.Start) + 1; i < LeftPolygon.IndexOf(L2.Start); i++)
                    Unwanted.Add(LeftPolygon[i]);
            }
            //return Unwanted;
            for(int i = 0; i < Unwanted.Count; i++)
                LeftPolygon.Remove(Unwanted[i]);
            // return LeftPolygon;

            Unwanted.Clear();
            // Right Polygon 
            //RightPolygon.Reverse();
            //if(RightPolygon.Contains(L2.End))
            //{
            //    for (int i = RightPolygon.IndexOf(L2.End) + 1; i < LeftPolygon.IndexOf(L1.End); i++)
            //        Unwanted.Add(LeftPolygon[i]);
            //}
            //else
            //{
            //    for (int i = RightPolygon.IndexOf(L1.End) + 1; i < LeftPolygon.IndexOf(L2.End); i++)
            //        Unwanted.Add(LeftPolygon[i]);
            //} 
            //    return Unwanted;

            List<Point> Wanted = new List<Point>();

            RightPolygon.Reverse();
            if (RightPolygon.Contains(L1.Start))
            {
                if (RightPolygon.Contains(L2.Start))
                    for (int i = RightPolygon.IndexOf(L1.Start); i <= RightPolygon.IndexOf(L2.Start); i++)
                        Wanted.Add(RightPolygon[i]);
                else
                    for (int i = RightPolygon.IndexOf(L1.Start); i <= RightPolygon.IndexOf(L2.End); i++)
                        Wanted.Add(RightPolygon[i]);
            }
            else
            {
                if (RightPolygon.Contains(L2.Start))
                    for (int i = RightPolygon.IndexOf(L1.End); i <= RightPolygon.IndexOf(L2.Start); i++)
                        Wanted.Add(RightPolygon[i]);
                else
                    for (int i = RightPolygon.IndexOf(L1.End); i <= RightPolygon.IndexOf(L2.End); i++)
                        Wanted.Add(RightPolygon[i]);
            }


            List<Point> outPoints = new List<Point>();
            outPoints = LeftPolygon;
            outPoints.AddRange(Wanted);
            return outPoints;


        }
        public static List<Point> divideAndConquer(List<Point> points)
        {
            if (points.Count < 7)
                return GrahamAlgorithm(points);
            else
            {
                int middle = (points.Count / 2) - 1;
                List<Point> LeftPolygon = new List<Point>();
                List<Point> RightPolygon = new List<Point>();
                for (int i = 0; i < middle; i++)
                    LeftPolygon.Add(points[i]);
                for (int i = middle; i < points.Count; i++)
                    RightPolygon.Add(points[i]);
                LeftPolygon = GrahamAlgorithm(LeftPolygon);
                RightPolygon = GrahamAlgorithm(RightPolygon);

                return Merge(LeftPolygon, RightPolygon);

            }

        }



        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            //points = CGAlgorithms.Algorithms.ConvexHull.Class1.Case1000Points(points);
            points = SortAlongXAxis(points);
            // Divide And Conqar Function 

            outPoints = divideAndConquer(points);

            //// Return 2 Convex Hulls 
            //for(int i = 0; i< LeftPolygon.Count;i++)
            //{
            //    //outPoints.Add(LeftPolygon[i]);
            //    if (i == LeftPolygon.Count - 1)
            //    {
            //        Line temo1 = new Line(LeftPolygon[i], LeftPolygon[0]);
            //       // outLines.Add(temo1);
            //        break;
            //    }
            //    Line temo = new Line(LeftPolygon[i], LeftPolygon[i + 1]);

            //    //outLines.Add(temo);
            //}

            //for (int i = 0; i < RightPolygon.Count; i++)
            //{
            //    //outPoints.Add(RightPolygon[i]);
            //    if (i == RightPolygon.Count - 1)
            //    {
            //        Line temo1 = new Line(RightPolygon[i], RightPolygon[0]);
            //        //outLines.Add(temo1);
            //        break;
            //    }
            //    Line temo = new Line(RightPolygon[i], RightPolygon[i + 1]);
            //    //outLines.Add(temo);
            //}




        }

        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }

    }
}
