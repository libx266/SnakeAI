using System;
using System.Collections.Generic;

namespace Snake
{
    public class Vector
    {
        public readonly Point p0;
        public readonly Point p1;
        public int length => VOps.GetLength(p0, p1);
        public Vector(Point _0, Point _1)
        { this.p0 = _0; this.p1 = _1; }
    }

    public class Line : Vector
    {
        public Line(Point _0, Point _1) : base(_0, _1) { }
        public Point[] points => VOps.GetPoints((Vector)this);
    }

    public static class VOps
    {
        public static List<Point> GetPoints(params Line[] lines)
        {
            var result = new List<Point>();
            foreach (Line l in lines)
            {
                foreach (Point p in l.points)
                { result.Add(p); }
            }
            return result;
        }
        public static int GetLength(Point P0, Point P1)
        {
            double x = Math.Pow(P1.x - P0.x, 2);
            double y = Math.Pow(P1.y - P0.y, 2);
            return Convert.ToInt32(Math.Sqrt(x + y));
        }
        public static Point[] GetPoints(Vector v)
        {
            int l = v.length;
            var result = new Point[l];
            for (int i = 0; i < l; i++)
            {
                var d = i + 1;
                int x1 = v.p0.x;
                int x2 = v.p1.x;
                int y1 = v.p0.y;
                int y2 = v.p1.y;
                int x = Convert.ToInt32(x1 + d * (x2 - x1) / Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)));
                int y = Convert.ToInt32(y1 + d * (y2 - y1) / Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)));
                result[i] = new Point(x, y);
            }
            return result;
        }
        public static Vector Less(List<Vector> vectors)
        {
            var result = vectors[0];
            for (int i = 1; i < vectors.Count; i++)
            {
                int l = result.length;
                if (vectors[i].length < l)
                { result= vectors[i]; }
            }
            return result;
        }
    }
}
