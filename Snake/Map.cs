using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Snake
{
    public class Map
    {
        public readonly int SizeX;
        public readonly int SizeY;
        Snake snake;
        Apple apple;
        Walls walls;
        Klisan klisan;
        Kafar kafar;
        int dx;
        int dy;
        public int score { get; protected set; }
        Random r;
        int bonus;
        int mem;
        bool bari;
        int tari;
        public Bitmap Image => Render(kafar, snake, walls, klisan, apple);
        public Map()
        {
            var S = Form1.Size;
            SizeX = S.x; SizeY = S.y;
            dx = 0;
            dy = 1;
            snake = new Snake();
            walls = new Walls();
            kafar = new Kafar(walls);
            apple = new Apple();
            klisan = new Klisan();
            r = new Random();
        }
        #region говнокод
        public bool Step()
        {
            try
            {
                snake.Move(dx, dy);
                int ax = apple.points[0].x;
                int ay = apple.points[0].y;
                kafar.Move(ax, ay);
                klisan.Move();
                if (!klisan.alive && r.Next(0, 3072) == 1488)
                { klisan = new Klisan(); }
                if (bonus == 2)
                { apple = new Apple(); bonus = 0; }
                if (snake.length % 6 == 0 & bonus == 0 & mem == 0)
                { bonus = 1; }
                if (bonus == 1)
                { apple = new Bonus(); bonus = 3; }
                if (bonus == 3)
                { apple.time--; }
                if (bonus == 3 & apple.time == 0)
                { bonus = 2; mem++; }
                foreach (Point p in walls.points)
                {
                    if (snake.location == p & snake.location != apple.points[0])
                    { throw new Exception($"{snake.location}:  kurwa rozpodrawa"); }
                    if (apple.points[0] == p) { apple = new Apple(); }
                }
                foreach (Point kp in kafar.points)
                {
                    foreach (Point sp in snake.points)
                    { if (!bari && kp == sp) { Console.WriteLine(snake.location.ToString() + ":  {#+{[|[#][[+]}#+$[%]#|+&{$#{%{}"); bari = true; } }
                }
                int s = score;
                int _s = snake.TryEat(apple);
                score += _s;
                int l = kafar.length;
                if (_s > 0) { kafar.AllClear(); }
                score -= kafar.TryEat(apple);
                if ((bonus == 2 | bonus == 0) & s != score) { mem = 0; }
                if (bonus == 3 & (s != score | l < kafar.length)) { bonus = 2; mem = 0; }
                if (klisan.alive && (klisan.location == snake.location || klisan.location == kafar.location))
                { snake.Erase(); kafar.Erase(); apple = new Apple(); bari = false; }
                //if (klisan.alive && (kafar.length > 1 | snake.length > 1)) { kafar.setTarget(klisan.location); }
                if (bari) { tari++; } if (bari && tari % 4 == 0) { snake.TryEat(new Fake(snake.location)); }
                return true;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return false;  }
        }
        #endregion
        public void SetVector(int dx, int dy)
        { this.dx = dx; this.dy = dy; }
        public Bitmap Render(params Entity[] objects)
        {
            var result = new Bitmap(SizeX, SizeY);
            foreach (Entity e in objects)
            {
                foreach (Point p in e.points)
                {
                    result.SetPixel(p.x, p.y, e.color);
                }
            }
            return result;
        }
    }
}
