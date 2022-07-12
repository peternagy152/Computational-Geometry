using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGAlgorithms;
using CGUtilities;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotoneTriangulation  :Algorithm
    {
        public static Boolean CheckValidity(List<Point> Chain , Line SelectedLine , Point StoppingElement)
        {
            Boolean flag = false;
            for(int i = 0; i < Chain.IndexOf(StoppingElement) - 1;i++)
            {
                Line templine = new Line(Chain[i], Chain[i + 1]);
                if(HelperMethods.CheckTurn(SelectedLine , templine.Start) != HelperMethods.CheckTurn(SelectedLine,templine.End))
                {
                    flag = true;
                    break;
                }

            }
            return flag;
        }
        
        public override void Run(System.Collections.Generic.List<CGUtilities.Point> points, System.Collections.Generic.List<CGUtilities.Line> lines, System.Collections.Generic.List<CGUtilities.Polygon> polygons, ref System.Collections.Generic.List<CGUtilities.Point> outPoints, ref System.Collections.Generic.List<CGUtilities.Line> outLines, ref System.Collections.Generic.List<CGUtilities.Polygon> outPolygons)
        {

            // Handel Environemnt 
            List<Point> PolygonPoints = new List<Point>();


            PolygonPoints = CGAlgorithms.Algorithms.PolygonTriangulation.MonotonePartitioning.ConvertTopoints(lines);


           // Getting the Right Chain .
          
            int upper = 0, lower = 0;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Y > points[upper].Y)
                    upper = i;
                if (points[i].Y < points[lower].Y)
                    lower = i;
            }
            List<Point> RightChain = new List<Point>();
            List<Point> LeftChain = new List<Point>();
            Point LowerMost = PolygonPoints[lower];
            Point UpperMost = PolygonPoints[upper];
            points = points.OrderBy(p => p.Y).ToList();
            points = points.OrderBy(p => Math.Atan2(p.Y - points[lower].Y, p.X - points[lower].X)).ToList();

            for (int i = PolygonPoints.IndexOf(LowerMost); i <= PolygonPoints.IndexOf(UpperMost);i++)
                RightChain.Add(PolygonPoints[i]);

            for(int i = 0; i < PolygonPoints.Count;i++)
                if (!RightChain.Contains(PolygonPoints[i]))
                    LeftChain.Add(PolygonPoints[i]);
            //Sort For Y-Axis
            PolygonPoints = CGAlgorithms.Algorithms.PolygonTriangulation.MonotonePartitioning.SortLineToY_Axis(PolygonPoints);


            Stack<Point> stack = new Stack<Point>();
            Stack<Point> PartialStack = new Stack<Point>();
            List<Line> Diagonals = new List<Line>();
            List<Line>PartialDiagonals = new List<Line>();

            stack.Push(PolygonPoints[0]);
            stack.Push(PolygonPoints[1]);

            for (int i =2; i< PolygonPoints.Count;i++)
            {
                if( (RightChain.Contains(PolygonPoints[i]) && !RightChain.Contains(stack.Peek()) ) || (!RightChain.Contains(PolygonPoints[i]) && RightChain.Contains(stack.Peek())) )
                {
                    //Opposite Chain
                    Point Top = stack.Peek();

                    while (stack.Count > 0)
                    {
                       
                        Point SelectedPoint = stack.Pop();
                        Line SelectedLine1 = new Line(PolygonPoints[i], SelectedPoint);
                        Line SelectedLine2 = new Line(SelectedPoint, PolygonPoints[i]);


                        //if (!lines.Contains(SelectedLine1) && !lines.Contains(SelectedLine2)) //Also Check if Line outSide the Polygon.
                        //    Diagonals.Add(SelectedLine1);

                        Diagonals.Add(SelectedLine1);

                        if (stack.Count == 0)
                        {
                            stack.Push(Top);
                            stack.Push(PolygonPoints[i]);
                        }

                    }




                }
                else
                {
                    //Same Chain 
                    while(stack.Count > 0)
                    {
                        bool flag = false;
                        Point SelectedPoint = stack.Pop();
                        Line SelectedLine1 = new Line(PolygonPoints[i], SelectedPoint);
                        Line SelectedLine2 = new Line(SelectedPoint, PolygonPoints[i]);

                        Diagonals.Add(SelectedLine1);
                        if(lines.Contains(SelectedLine1) || lines.Contains(SelectedLine2))
                            flag = true;
                        if(flag == false)
                        {
                            if(CheckValidity(LeftChain, SelectedLine1, SelectedPoint))
                            {
                                stack.Push(SelectedPoint);
                                stack.Push(SelectedLine1.End);
                                stack.Push(PolygonPoints[i]);
                                break;
                            }
                        }
                        
                        if(stack.Count == 0)
                        {
                            stack.Push(SelectedLine1.End);
                            stack.Push(PolygonPoints[i]);
                        }


                    }
                   



                }
            }

                

            }

        public override string ToString()
        {
            return "Monotone Triangulation";
        }
    }
}
