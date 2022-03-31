using System;
using System.Drawing;
using System.Collections.Generic;
namespace Snake
{
    /// <summary>
    /// Базовый класс для сущностей карты
    /// </summary>
    public abstract class Entity
    {
        protected Random r;
        public List<Point> points { get; protected set; }
        public Color color { get; protected set; }
    }

    /// <summary>
    /// Еда, дающая очки
    /// </summary>
    public class Apple : Entity
    {
        public int time;
	Point mapSize = Form1.Size;
        public Apple()
        {
            r = new Random();
            points = new List<Point>();
            points.Add(new Point(r.Next(1, mapSize.x - 1), r.Next(1, mapSize.y - 1)));
            color = Color.FromArgb(220, 0, 0); Log();
        }
        public virtual int Chrum()
        {
            points[0] = new Point(r.Next(1, mapSize.x - 1), r.Next(1, mapSize.y - 1));
            Log(); return 1;
        }
        protected virtual void Log() => Console.WriteLine($"{points[0]}:  rodzą tybłoko"); 
    }

    /// <summary>
    /// Бонусная еда, появляющаяся после 5 очков
    /// </summary>
    public class Bonus : Apple
    {
        public Bonus() : base() { time = 120; color = Color.FromArgb(240, 240, 0); }
        public override int Chrum()
        { return time / 10; }
    }

    
    /// <summary>
    /// Сущность со спиралевидным поведением для сброса длины змеек
    /// </summary>
    public class Klisan : Entity
    {
        public bool alive { get; protected set; }
        protected Point c;
        public Point location { get; protected set; }
        private double i;
        private double _r;
        public Klisan()
        {
            r = new Random();
            color = Color.FromArgb(0, 0, 210);
            points = new List<Point>();
            c = new Point(r.Next(10, 65), r.Next(10, 65));
            location = c; alive = true; i = 0.0; _r = 0.0;
            Console.WriteLine($"{c}:  szewbroda powstała");
        }
        public void Move()
        {
            if (alive)
            {
                if (true)
                {
                    int x = Convert.ToInt32(Math.Cos(2 * Math.PI * i / 360) * _r + c.x);
                    int y = Convert.ToInt32(Math.Sin(2 * Math.PI * i / 360) * _r + c.y);
                    location = new Point(x, y);
                    _r += 0.08; i += 1;
                }
                if (location.x > 0 & location.x < 75 & location.y > 0 & location.y < 75)
                { points.Add(location); points = points.GetRange(points.Count - 1, 1); }
                else { points.Clear(); alive = false; }
            }
        }
    }

    /// <summary>
    /// Стены на карте
    /// </summary>
    public class Walls : Entity
    {
        public Walls()
        {
            var S = Form1.Size;
            color = Color.FromArgb(192, 192, 192);
            r = new Random();

            int density = (S.x * S.y) / 700;
            
            points = new List<Point>();
            int q = r.Next(density, density + 4);
            for (int i = 0; i < q; i++)
            {
                var p0 = new Point(r.Next(2, S.x - 5), r.Next(2, S.y - 5));
                Point p1;
                switch (r.Next(0, 2))
                {
                    case 0:
                        int y = p0.y;
                        int x = p0.x + r.Next(8, 24);
                        x = x > S.x - 1 ? S.x - 1 : x;
                        p1 = new Point(x, y); break;
                    default:
                        int y0 = p0.y + r.Next(8, 24);
                        y0 = y0 > S.y - 1 ? S.y - 1 : y0;
                        p1 = new Point(p0.x, y0); break;
                }
                if (p0.x != p1.x)
                {
                    for (int j = p0.x; j <= p1.x; j++)
                    { points.Add(new Point(j, p0.y)); }
                    for (int j = p0.x; j <= p1.x; j++)
                    { points.Add(new Point(j, p0.y - 1)); }
                }
                else
                {
                    for (int j = p0.y; j <= p1.y; j++)
                    { points.Add(new Point(p0.x, j)); }
                    for (int j = p0.y; j <= p1.y; j++)
                    { points.Add(new Point(p0.x + 1, j)); }
                }
                Console.WriteLine($"{p0}+{p1}:  stawiamy ścianę");
            }
        }
    }

    /// <summary>
    /// Змейка игрока
    /// </summary>
    public class Snake : Entity
    {
        protected Point mapSize;
        public int length { get; protected set; }
        public Point location { get; protected set; }
        public Snake()
        {
            mapSize = Form1.Size;
            r = new Random();
            color = Color.FromArgb(0, 170, 0);
            length = 1;
            points = new List<Point>();
            location = new Point(r.Next(0, mapSize.x), r.Next(0, mapSize.y));
            Console.WriteLine($"{location}:  rodzą węża");
        }
        public virtual void Move(int dx, int dy)
        {
            int x = location.x + dx;
            int y = location.y + dy;
            location = new Point(x, y);
            TryTeleport();
            foreach (Point p in points)
            { if (location == p) { throw new Exception("Kurwa!"); } }
            Stable();
        }
        protected virtual void TryTeleport()
        {
            int x = location.x;
            int y = location.y;
            if (x >= mapSize.x) { location = new Point(0, location.y); }
            if (y >= mapSize.y) { location = new Point(location.x, 0); }
            if (x < 0) { location = new Point(mapSize.x -1, location.y); }
            if (y < 0) { location = new Point(location.x, mapSize.y - 1); }
        }
        protected void Stable()
        {
            points.Add(location);
            int s = points.Count - length;
            points = points.GetRange(s, length);
        }
        public virtual int TryEat(Apple a)
        {
            if (a.points[0] == location)
            { length++; return a.Chrum(); }
            return 0;
        }
        public void Erase()
        { this.length = 1; Console.WriteLine($"{location}:  :c"); }
    }
    #region говнокод
    /// <summary>
    /// Змейка-ИИ
    /// </summary>
    public class Kafar : Snake
    {
        protected int stack;
        protected int mem;
        protected bool[,] map;
        protected bool star;
        protected Point target;
        protected Point apple;
        protected List<Post> posts;
        protected bool farit;
        protected Point[] line;
        protected int l;

        /// <summary>
        /// Посещенные точки на карте
        /// </summary>
        public bool[,] visited { get; protected set; }

        public Kafar(Walls walls) : base()
        {
            color = Color.FromArgb(180, 0, 200);
            map = new bool[mapSize.x, mapSize.y];
            visited = new bool[mapSize.x, mapSize.y];
            posts = new List<Post>();
            foreach (Point p in walls.points)
            { map[p.x, p.y] = true; }
            for (int y = 1; y < mapSize.y - 1; y+= 5)
            {
                for (int x = 1; x < mapSize.x - 1; x+= 5)
                { if (!map[x, y]) { posts.Add(new Post(x, y)); } }
            }
            star = true;  target = apple;

        }

        public override void Move(int ax, int ay)
        {
            try
            {
                apple = new Point(ax, ay);
                star = Visible(apple);
                if (star)
                {
                    if (l == 0) { line = new Line(location, apple).points; l = line.Length; }
                    if (l > 0) { target = line[line.Length - l]; l--; }
                    if (mem == 0)
                    { Console.WriteLine($"{line[line.Length - 1]}:  widoczność"); }
                    mem++; stack = 0; ; farit = true;
                }
                else
                {
                    try
                    {
                        if (target == location | stack > 64 | (farit & stack > 32))
                        {
                            if (farit & stack > 32) { farit = false; }
                            target = Scan(); ClearVisit(); mem = 0; stack = 0; l = 0;
                            Console.WriteLine($"{target}:  czekpost");
                        }
                    }
                    catch
                    { TryTeleport(); }
                }
                if (visited[location.x, location.y]) { stack++; }
                if (stack > 128)
                {
                    if (true)
                    {
                        int x = r.Next(1, mapSize.x - 1);
                        int y = r.Next(1, mapSize.y - 1);
                        while (map[x, y] | !Visible(new Point(x, y)))
                        {
                            x = r.Next(1, mapSize.x - 1);
                            y = r.Next(1, mapSize.y - 1);
                        }
                        target = new Point(x, y);
                        Console.WriteLine($"{target}:  retargeting");
                        ClearVisit(); stack = 0; mem = 0; l = 0;
                    }
                }
                location = Star(target);
                Stable(); visited[location.x, location.y] = true;
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); TryTeleport(); }
        }
        protected virtual bool Visible(Point t)
        {
            var v = new Line(location, t);
            foreach (Point p in v.points)
            { if (map[p.x, p.y]) { return false; } }
            return true;
        }

        protected override void TryTeleport()
        {
            int x = r.Next(1, mapSize.x - 1);
            int y = r.Next(1, mapSize.y - 1);
            while (map[x, y])
            {
                x = r.Next(1, mapSize.x - 1);
                y = r.Next(1, mapSize.y - 1);
            }
            location = new Point(x, y);
            Console.WriteLine($"{location}:  kewerborliert");
            AllClear();
        }

		public override int TryEat(Apple a)
        {
            int b = base.TryEat(a);
            if (b > 0) { AllClear(); }
            return b;
        }

        protected void ClearVisit() { stack = 0; visited = new bool[mapSize.x, mapSize.y]; }

        protected virtual Point Star(Point target)
        {
            var path = new List<Vector>();
            for (int i = 0; i < 4; i++)
            {
                int x = Convert.ToInt32(Math.Cos(2 * Math.PI * 90 * i / 360) * 1 + location.x);
                int y = Convert.ToInt32(Math.Sin(2 * Math.PI * 90 * i / 360) * 1 + location.y);
                try
                {
                    if (!map[x, y])
                    { path.Add(new Vector(new Point(x, y), target)); }
                }
                catch (Exception ex)
                { Console.WriteLine(ex.Message); return new Point(r.Next(2, mapSize.x -2 ), r.Next(2, mapSize.y - 2)); }
            }
            return VOps.Less(path).p0;
        }

        public void ClearPosts()
        {
            foreach (Post p in posts)
            { p.available = true; }
            mem = 0;
        }

        protected virtual Point Scan()
        {
            var distance = new List<Vector[]>();
            int j = 0; int jinhsha = 0; bool b = true;
            foreach (Post p in posts)
            {
                if (p.available & Visible(p.l))
                {
                    var d = new Vector[2] { new Vector(location, p.l), new Vector(p.l, apple) };
                    distance.Add(d); if (b) { jinhsha = j; b = false; }
                }
                j++;
            }
            var l= new int[2]; int index = -1;
            l[0] = distance[0][0].length;
            l[1] = distance[0][1].length;
            for (int i = 0; i < distance.Count; i++)
            {
                if (distance[i][0].length <= l[0] & distance[i][1].length <= l[1])
                { l[0] = distance[i][0].length; l[1] = distance[i][1].length; index = i; }
            }
            int kurwa = index < 0 ? r.Next(0, distance.Count) : index;
            var post = distance[kurwa][0].p1;
            for (int i = 0; i < posts.Count; i++)
            { if (posts[i].l == post) { posts[i].available = false; }}
            return post;
        }

        public void AllClear()
        { mem = 0; ClearPosts(); ClearVisit(); l = 0; }

    }
    public class Fake : Apple
    {
        public Fake(Point l) { points[0] = l; }
        protected override void Log() { }
    }
#endregion
}
