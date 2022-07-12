using System;
using CGUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities.DataStructures;
using CGAlgorithms;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{
    class EventPoint
    {
        public CGUtilities.Point P;
       public string EventType;
        public int index;
    }
    
    class SweepLine:Algorithm
    {
        public static List<Line> Handel(List<Line> UnHandeled)
        {
            for(int i = 0; i<UnHandeled.Count; i++)
            {
                if(UnHandeled[i].Start.X > UnHandeled[i].End.X)
                {
                    Point temp;
                    temp = UnHandeled[i].Start;
                    UnHandeled[i].Start = UnHandeled[i].End;
                    UnHandeled[i].End = temp;
                    
                }
            }
            return UnHandeled;
        }
        public List<EventPoint> Sorting(List<EventPoint> UnsortedList)
        {

            EventPoint temp;
            for (int i = 0; i < UnsortedList.Count - 1; i++)
            {
                for (int j = i + 1; j < UnsortedList.Count; j++)
                {
                    if (UnsortedList[i].P.X < UnsortedList[j].P.X)
                    {
                        temp = UnsortedList[i];
                        UnsortedList[i] = UnsortedList[j];
                        UnsortedList[j] = temp;
                    }



                }
            }
            return UnsortedList;
        }
        public static EventPoint MinY(List<EventPoint> Above)
        {
            EventPoint A = Above[0];
            for(int i = 1; i < Above.Count;i++)
            {
                if (A.P.Y > Above[i].P.Y)
                    A = Above[i];
            }
            return A;
        }
        public static EventPoint MaxY(List<EventPoint> Above)
        {
            EventPoint A = Above[0];
            for (int i = 1; i < Above.Count; i++)
            {
                if (A.P.Y < Above[i].P.Y)
                    A = Above[i];
            }
            return A;
        }
        public static int orientation(Point p1, Point p2, Point p3)
        {

            double exp = (p2.Y - p1.Y) * (p3.X - p2.X) - (p2.X - p1.X) * (p3.Y - p2.Y);

            if (exp == 0) return 0;  // collinear

            return (exp > 0) ? 1 : 2; // clock or counterclock wise
        }

        public static Boolean CheckIntersection(Point a1, Point a2, Point b1, Point b2)
        {
            int o1 = orientation(a1, a2, b1);
            int o2 = orientation(a1, a2, b2);
            int o3 = orientation(b1, b2, a1);
            int o4 = orientation(b1, b2, a2);

            // general case
            if (o1 != o2 && o3 != o4)
                return true;
            else
                return false;
        }
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            lines = Handel(lines);
            List<EventPoint> PeroiretyQueue = new List<EventPoint>();

            for (int i = 0; i < lines.Count; i++)
            {
                EventPoint temppoint = new EventPoint();

                temppoint.EventType = "StartPoint";
                temppoint.index = i;
                temppoint.P = lines[i].Start;
                PeroiretyQueue.Add(temppoint);

            }
            for (int i = 0; i < lines.Count; i++)
            {
                EventPoint temppoint = new EventPoint();

                temppoint.EventType = "EndPoint";
                temppoint.index = i;
                temppoint.P = lines[i].End;
                PeroiretyQueue.Add(temppoint);

            }
            // Sort Queue according to X-axis 
            PeroiretyQueue = Sorting(PeroiretyQueue);
            PeroiretyQueue.Reverse();

            // OrderList of Sweep Line Sorted Along with Y-axis 
            List<EventPoint> ActivitySet = new List<EventPoint>();
            List<EventPoint> AllBelow = new List<EventPoint>();
            List<EventPoint> AllAbove = new List<EventPoint>();
            EventPoint Above, Below;
            for (int i = 0; i < PeroiretyQueue.Count; i++)
            {
                AllBelow.Clear();
                AllAbove.Clear();
                if (PeroiretyQueue[i].EventType == "StartPoint")
                {
                    ActivitySet.Add(PeroiretyQueue[i]);
                    
                    for (int j = 0; j < ActivitySet.Count - 1; j++)
                    {
                        if(ActivitySet[j].P.Y > PeroiretyQueue[i].P.Y)
                            AllAbove.Add(ActivitySet[j]);
                        else
                            AllBelow.Add(ActivitySet[j]);

                    }

                    if(AllAbove.Count != 0)
                    {
                        Above = MinY(AllAbove);
                        if (CheckIntersection(lines[PeroiretyQueue[i].index].Start , lines[PeroiretyQueue[i].index].End, lines[Above.index].Start , lines[Above.index].End) == true)
                        {
                            if (!outLines.Contains(lines[Above.index]))
                                outLines.Add(lines[Above.index]);
                            if (!outLines.Contains(lines[PeroiretyQueue[i].index]))
                                outLines.Add(lines[PeroiretyQueue[i].index]);

                        }
                    }

                    if(AllBelow.Count != 0)
                    {
                        Below = MaxY(AllBelow);

                        // Check InterSection

                        if (CheckIntersection(lines[PeroiretyQueue[i].index].Start, lines[PeroiretyQueue[i].index].End, lines[Below.index].Start, lines[Below.index].End) == true)
                        {
                            if (!outLines.Contains(lines[Below.index]))
                                outLines.Add(lines[Below.index]);
                            if (!outLines.Contains(lines[PeroiretyQueue[i].index]))
                                outLines.Add(lines[PeroiretyQueue[i].index]);

                        }
                    }
                }
                else //EndPoint
                {
                    int StartIndex = 0 ;
                    for (int j = 0; j < ActivitySet.Count ; j++)
                    {
                        if(ActivitySet[j].index == PeroiretyQueue[i].index && ActivitySet[j].EventType != PeroiretyQueue[i].EventType)
                        {
                            StartIndex = j;
                            break;
                        }
                           
                    }
                    ActivitySet.RemoveAt(StartIndex);

                    for (int j = 0; j < ActivitySet.Count ; j++)
                    {
                        if (ActivitySet[j].P.Y > PeroiretyQueue[i].P.Y)
                            AllAbove.Add(ActivitySet[j]);
                        else
                            AllBelow.Add(ActivitySet[j]);

                    }

                    if (AllAbove.Count != 0)
                    {
                        Above = MinY(AllAbove);
                        if (CheckIntersection(lines[PeroiretyQueue[i].index].Start, lines[PeroiretyQueue[i].index].End, lines[Above.index].Start, lines[Above.index].End) == true)
                        {
                            if(!outLines.Contains(lines[Above.index]))
                            outLines.Add(lines[Above.index]);
                            if(!outLines.Contains(lines[PeroiretyQueue[i].index]))
                            outLines.Add(lines[PeroiretyQueue[i].index]);

                        }
                    }

                    if (AllBelow.Count != 0)
                    {
                        Below = MaxY(AllBelow);

                        // Check InterSection

                        if (CheckIntersection(lines[PeroiretyQueue[i].index].Start, lines[PeroiretyQueue[i].index].End, lines[Below.index].Start, lines[Below.index].End) == true)
                        {
                            if (!outLines.Contains(lines[Below.index]))
                                outLines.Add(lines[Below.index]);
                            if (!outLines.Contains(lines[PeroiretyQueue[i].index]))
                                outLines.Add(lines[PeroiretyQueue[i].index]);

                        }
                    }

                }
                    

            }


           

        }

        public override string ToString()
        {
            return "Sweep Line";
        }
    }
}
