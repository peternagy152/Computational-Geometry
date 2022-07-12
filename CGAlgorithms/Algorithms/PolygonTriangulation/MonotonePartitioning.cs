using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;
using CGAlgorithms;
using System.Collections;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotonePartitioning : Algorithm
    {
        public static List<Point> SortLineToY_Axis(List<Point> UnsortedList)
        {
            Point temp;
            for (int i = 0; i < UnsortedList.Count - 1; i++)
            {
                for (int j = i + 1; j < UnsortedList.Count; j++)
                {
                    if (UnsortedList[i].Y < UnsortedList[j].Y)
                    {
                        temp = UnsortedList[i];
                        UnsortedList[i] = UnsortedList[j];
                        UnsortedList[j] = temp;
                    }
                }
            }
            return UnsortedList;
        }
        public List<Point> Sorting(List<Point> UnsortedList)
        {

            Point temp;
            for (int i = 0; i < UnsortedList.Count - 1; i++)
            {
                for (int j = i + 1; j < UnsortedList.Count; j++)
                {
                    if (UnsortedList[i].X < UnsortedList[j].X)
                    {
                        temp = UnsortedList[i];
                        UnsortedList[i] = UnsortedList[j];
                        UnsortedList[j] = temp;
                    }



                }
            }
            return UnsortedList;
        }
        public static List<Point> ConvertTopoints(List<Line> Pol)
        {
            List<Point> outPoints = new List<Point>();
            for (int i = 0; i < Pol.Count; i++)
            {
                if (!outPoints.Contains(Pol[i].Start))
                    outPoints.Add(Pol[i].Start);
                if (!outPoints.Contains(Pol[i].End))
                    outPoints.Add(Pol[i].End);

            }
            return outPoints;
        }

        public override void Run(System.Collections.Generic.List<CGUtilities.Point> points, System.Collections.Generic.List<CGUtilities.Line> lines, System.Collections.Generic.List<CGUtilities.Polygon> polygons, ref System.Collections.Generic.List<CGUtilities.Point> outPoints, ref System.Collections.Generic.List<CGUtilities.Line> outLines, ref System.Collections.Generic.List<CGUtilities.Polygon> outPolygons)
        {
            CGAlgorithms.Algorithms.ConvexHull.GrahamScan G1 = new CGAlgorithms.Algorithms.ConvexHull.GrahamScan();
            List<Line> LL = new List<Line>();
            List<Polygon> PP = new List<Polygon>();

            List<Point> ConvexHull = new List<Point>();


            List<Point> PolgonPoints = new List<Point>();
            PolgonPoints = ConvertTopoints(lines);


            //Sort Along X-Axis for the Median 
            PolgonPoints = Sorting(PolgonPoints);

            Point Median = PolgonPoints[PolgonPoints.Count / 2 + 1];

            PolgonPoints = SortLineToY_Axis(PolgonPoints); // Sorted For Sweepline 

            G1.Run(PolgonPoints, LL, polygons, ref ConvexHull, ref LL, ref polygons);

            List<Line> T = new List<Line>();
            Hashtable helper = new Hashtable();

            Hashtable VertexType = new Hashtable();

            List<Line> Diagonals = new List<Line>();


            //Get the X-Axis Boundary leftmost , Rightmost for SweepLine
            int  left = 0, right = 0;
            for (int i = 0; i < PolgonPoints.Count; i++)
            {
                if (PolgonPoints[i].X < PolgonPoints[left].X)
                    left = i;
                if (PolgonPoints[i].X > PolgonPoints[right].X)
                    right = i;
            }

            //Get the Median of X-Axis Points 




            for (int i = 0; i < PolgonPoints.Count; i++)
            {
                List<Point> N = new List<Point>();
                //Find Neighbours 
                for (int j = 0; j < lines.Count; j++)
                {
                    if (lines[j].Start == PolgonPoints[i])
                        N.Add(lines[j].End);
                    else if (lines[j].End == PolgonPoints[i])
                        N.Add(lines[j].Start);
                }

                // Define the Type of Vertex

                if (N[0].Y < PolgonPoints[i].Y && N[1].Y < PolgonPoints[i].Y)
                {
                    // It may be Start or Split
                    if (ConvexHull.Contains(PolgonPoints[i]))
                    {
                        // Start Vertex
                        VertexType.Add(PolgonPoints[i], "Start");
                        Line Edge;
                        if (N[0].X < N[1].X)
                            Edge = new Line(PolgonPoints[i], N[0]);
                        else
                            Edge = new Line(PolgonPoints[i], N[1]);
                        T.Add(Edge);
                        helper.Add(Edge, PolgonPoints[i]);

                    }
                    else
                    {
                        //Split Vertex
                        VertexType.Add(PolgonPoints[i], "Split");


                        Line Edge;
                        if (N[0].X > N[1].X)
                            Edge = new Line(PolgonPoints[i], N[0]);
                        else
                            Edge = new Line(PolgonPoints[i], N[1]);


                        // Get the Left Edge 
                        Point X1, X2;
                        Line SweepLine = new Line(X1 = new Point(left, PolgonPoints[i].Y), X2 = new Point(right, PolgonPoints[i].Y));
                        Line LeftEdge = new Line(points[0], points[0]);
                        for (int j = 0; j < T.Count; j++)
                        {
                            if (CGAlgorithms.Algorithms.SegmentIntersection.SweepLine.CheckIntersection(SweepLine.Start, SweepLine.End, T[j].Start, T[j].End) && HelperMethods.CheckTurn(T[j], PolgonPoints[i]) == Enums.TurnType.Left)
                            {
                                LeftEdge = T[j];
                                break;
                            }
                        }
                        Line Dia = new Line(PolgonPoints[i], (Point)helper[LeftEdge]);
                        Diagonals.Add(Dia);

                        T.Add(Edge);
                        helper.Add(Edge, PolgonPoints[i]);
                        helper[LeftEdge] = PolgonPoints[i];


                    }
                }
                else if (N[0].Y > PolgonPoints[i].Y && N[1].Y > PolgonPoints[i].Y)
                {
                    // It May be Merge or End
                    if (ConvexHull.Contains(PolgonPoints[i]))
                    {
                        // End Vertex
                        VertexType.Add(PolgonPoints[i], "End");

                        Line PrevEdge = new Line(points[0], points[0]);
                        for (int j = 0; j < T.Count; j++)
                            if (T[j].Start == PolgonPoints[i] || T[j].End == PolgonPoints[i])
                            {
                                PrevEdge = T[j];
                                break;
                            }
                        if (VertexType[helper[PrevEdge]] == "Merge")
                        {
                            Line Dia = new Line(PolgonPoints[i], (Point)helper[PrevEdge]);
                            Diagonals.Add(Dia);

                        }
                        T.Remove(PrevEdge);
                    }
                    else
                    {
                        // Merge
                        VertexType.Add(PolgonPoints[i], "Merge");

                        Line PrevEdge = new Line(points[0], points[0]);
                        for (int j = 0; j < T.Count; j++)
                            if (T[j].Start == PolgonPoints[i] || T[j].End == PolgonPoints[i])
                            {
                                PrevEdge = T[j];
                                break;
                            }
                        if (VertexType[helper[PrevEdge]] == "Merge")
                        {
                            Line Dia = new Line(PolgonPoints[i], (Point)helper[PrevEdge]);
                            Diagonals.Add(Dia);

                        }
                        T.Remove(PrevEdge);

                        //Search for the Left Edge 

                        Point X1, X2;
                        Line SweepLine = new Line(X1 = new Point(left, PolgonPoints[i].Y), X2 = new Point(right, PolgonPoints[i].Y));
                        Line LeftEdge = new Line(points[0], points[0]);
                        for (int j = 0; j < T.Count; j++)
                        {
                            if (CGAlgorithms.Algorithms.SegmentIntersection.SweepLine.CheckIntersection(SweepLine.Start, SweepLine.End, T[j].Start, T[j].End) && HelperMethods.CheckTurn(T[j], PolgonPoints[i]) == Enums.TurnType.Left)
                            {
                                LeftEdge = T[j];
                                break;
                            }
                        }

                        if (VertexType[helper[LeftEdge]] == "Merge")
                        {
                            Line Dia = new Line(PolgonPoints[i], (Point)helper[LeftEdge]);
                            Diagonals.Add(Dia);
                            
                        }
                        helper[LeftEdge] = PolgonPoints[i];


                    }


                }
                else
                {
                    // Regular Vertex 

                    VertexType.Add(PolgonPoints[i], "Regular");
                    Line Edge;


                    if (N[0].Y > PolgonPoints[i].Y)
                        Edge = new Line(PolgonPoints[i], N[0]);
                    else
                        Edge = new Line(PolgonPoints[i], N[0]);


                    // if Left 
                    if(PolgonPoints[i].X < Median.X)
                    {
                        // search for Ej-1 and then Get the Helper
                        Line PrevEdge = new Line(points[0], points[0]);
                        for (int j = 0; j < T.Count; j++)
                            if (T[j].Start == PolgonPoints[i] || T[j].End == PolgonPoints[i])
                            {
                                PrevEdge = T[j];
                                break;
                            }
                        if (VertexType[helper[PrevEdge]] == "Merge")
                        {
                            Line Dia = new Line(PolgonPoints[i], (Point)helper[PrevEdge]);
                            Diagonals.Add(Dia);

                        }
                        // Delete the Previos Edge
                        T.Remove(PrevEdge);
                        // Add new Egde to T 
                        T.Add(Edge);
                        helper.Add(Edge, PolgonPoints[i]);
                    }
                    else
                    {
                        //If right 

                        // Search in T for Edge Directly to the Left of Vertex 
                        Point X1, X2;
                        Line SweepLine = new Line(X1 = new Point(left, PolgonPoints[i].Y), X2 = new Point(right, PolgonPoints[i].Y));
                        Line LeftEdge = new Line(points[0], points[0]);
                        for (int j = 0; j < T.Count; j++)
                        {
                            if (CGAlgorithms.Algorithms.SegmentIntersection.SweepLine.CheckIntersection(SweepLine.Start, SweepLine.End, T[j].Start, T[j].End) && HelperMethods.CheckTurn(T[j], PolgonPoints[i]) == Enums.TurnType.Left)
                            {
                                LeftEdge = T[j];
                                break;
                            }
                        }

                        if (VertexType[helper[LeftEdge]] == "Merge")
                        {
                            Line Dia = new Line(PolgonPoints[i], (Point)helper[LeftEdge]);
                            Diagonals.Add(Dia);
                            helper[LeftEdge] = PolgonPoints[i];
                        }
                    }
                   


                    



                }





            }


        }

        public override string ToString()
        {
            return "Monotone Partitioning";
        }
    }
}