using System;
using System.Drawing;
namespace Snake
{
    public struct Point
    {
        public readonly int x;
        public readonly int y;
       
        public Point(int x, int y)
        { this.x = x; this.y = y; }

        public static bool operator !=(Point a, Point b) =>
            a.x != b.x || a.y != b.y;

        public static bool operator ==(Point a, Point b) =>
            !(a != b);

        public override string ToString() =>
            $"[<{x}>|<{y}>]";
	}
    /// <summary>
    /// Пост для обхода Змейкой-ИИ
    /// </summary>
    public class Post
    {
        public readonly Point l;
        public bool available;
        public Post(int x, int y)
        { l = new Point(x, y); available = true; }

    }
}
