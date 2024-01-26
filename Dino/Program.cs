using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dino
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

   



   

    public static class GameController
    {
        public static Image spritesheet;
        public static List<Road> roads;
        public static List<Cactus> cactuses;
        public static List<Bird> birds;

        public static int dangerSpawn = 10;
        public static int countDangerSpawn = 0;


        public static void Init()
        {
            roads = new List<Road>();
            birds = new List<Bird>();
            cactuses = new List<Cactus>();
            spritesheet = Properties.Resources.sprite;
            GenerateRoad();
        }

        public static void MoveMap()
        {
            for (int i = 0; i < roads.Count; i++)
            {
                roads[i].transform.position.X -= 4;
                if (roads[i].transform.position.X + roads[i].transform.size.Width < 0)
                {
                    roads.RemoveAt(i);
                    GetNewRoad();
                }
            }
            for (int i = 0; i < cactuses.Count; i++)
            {
                cactuses[i].transform.position.X -= 4;
                if (cactuses[i].transform.position.X + cactuses[i].transform.size.Width < 0)
                {
                    cactuses.RemoveAt(i);
                }
            }
            for (int i = 0; i < birds.Count; i++)
            {
                birds[i].transform.position.X -= 4;
                if (birds[i].transform.position.X + birds[i].transform.size.Width < 0)
                {
                    birds.RemoveAt(i);
                }
            }
        }

        public static void GetNewRoad()
        {
            Road road = new Road(new PointF(0 + 100 * 9, 200), new Size(100, 17));
            roads.Add(road);
            countDangerSpawn++;

            if (countDangerSpawn >= dangerSpawn)
            {
                Random r = new Random();
                dangerSpawn = r.Next(5, 9);
                countDangerSpawn = 0;
                int obj = r.Next(0, 2);
                switch (obj)
                {
                    case 0:
                        Cactus cactus = new Cactus(new PointF(0 + 100 * 9, 150), new Size(50, 50));
                        cactuses.Add(cactus);
                        break;
                    case 1:
                        Bird bird = new Bird(new PointF(0 + 100 * 9, 110), new Size(50, 50));
                        birds.Add(bird);
                        break;
                }
            }
        }
        public static void GenerateRoad()
        {
            for (int i = 0; i < 10; i++)
            {
                Road road = new Road(new PointF(0 + 100 * i, 200), new Size(100, 17));
                roads.Add(road);
                countDangerSpawn++;
            }
        }

        public static void DrawObjets(Graphics g)
        {
            for (int i = 0; i < roads.Count; i++)
            {
                roads[i].DrawSprite(g);
            }
            for (int i = 0; i < cactuses.Count; i++)
            {
                cactuses[i].DrawSprite(g);
            }
            for (int i = 0; i < birds.Count; i++)
            {
                birds[i].DrawSprite(g);
            }
        }
    }



    public class Physics
    {
        public Transform transform;
        float gravity;
        float a;

        public bool isJumping;
        public bool isCrouching;

        public Physics(PointF position, Size size)
        {
            transform = new Transform(position, size);
            gravity = 0;
            a = 0.4f;
            isJumping = false;
            isCrouching = false;
        }

        public void ApplyPhysics()
        {
            CalculatePhysics();
        }

        public void CalculatePhysics()
        {
            if (transform.position.Y < 150 || isJumping)
            {
                transform.position.Y += gravity;
                gravity += a;
            }
            if (transform.position.Y > 150)
                isJumping = false;
        }

        public bool Collide()
        {
            for (int i = 0; i < GameController.cactuses.Count; i++)
            {
                var cactus = GameController.cactuses[i];
                PointF delta = new PointF();
                delta.X = (transform.position.X + transform.size.Width / 2) - (cactus.transform.position.X + cactus.transform.size.Width / 2);
                delta.Y = (transform.position.Y + transform.size.Height / 2) - (cactus.transform.position.Y + cactus.transform.size.Height / 2);
                if (Math.Abs(delta.X) <= transform.size.Width / 2 + cactus.transform.size.Width / 2)
                {
                    if (Math.Abs(delta.Y) <= transform.size.Height / 2 + cactus.transform.size.Height / 2)
                    {
                        return true;
                    }
                }
            }
            for (int i = 0; i < GameController.birds.Count; i++)
            {
                var bird = GameController.birds[i];
                PointF delta = new PointF();
                delta.X = (transform.position.X + transform.size.Width / 2) - (bird.transform.position.X + bird.transform.size.Width / 2);
                delta.Y = (transform.position.Y + transform.size.Height / 2) - (bird.transform.position.Y + bird.transform.size.Height / 2);
                if (Math.Abs(delta.X) <= transform.size.Width / 2 + bird.transform.size.Width / 2)
                {
                    if (Math.Abs(delta.Y) <= transform.size.Height / 2 + bird.transform.size.Height / 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void AddForce()
        {
            if (!isJumping)
            {
                isJumping = true;
                gravity = -10;
            }
        }
    }


    public class Player
    {
        public Physics physics;
        public int framesCount = 0;
        public int animationCount = 0;
        public int score = 0;

        public Player(PointF position, Size size)
        {
            physics = new Physics(position, size);
            framesCount = 0;
            score = 0;
        }

        public void DrawSprite(Graphics g)
        {
            if (physics.isCrouching)
            {
                DrawNeededSprite(g, 1870, 40, 109, 51, 118, 1.35f);
            }
            else
            {
                DrawNeededSprite(g, 1518, 0, 79, 91, 88, 1);
            }
        }

        public void DrawNeededSprite(Graphics g, int srcX, int srcY, int width, int height, int delta, float multiplier)
        {
            framesCount++;
            if (framesCount <= 10)
                animationCount = 0;
            else if (framesCount > 10 && framesCount <= 20)
                animationCount = 1;
            else if (framesCount > 20)
                framesCount = 0;

            g.DrawImage(GameController.spritesheet, new Rectangle(new Point((int)physics.transform.position.X, (int)physics.transform.position.Y), new Size((int)(physics.transform.size.Width * multiplier), physics.transform.size.Height)), srcX + delta * animationCount, srcY, width, height, GraphicsUnit.Pixel);
        }
    }



    public class Road
    {
        public Transform transform;

        public Road(PointF pos, Size size)
        {
            transform = new Transform(pos, size);
        }

        public void DrawSprite(Graphics g)
        {
            g.DrawImage(GameController.spritesheet, new Rectangle(new Point((int)transform.position.X, (int)transform.position.Y), new Size(transform.size.Width, transform.size.Height)), 2300, 112, 100, 17, GraphicsUnit.Pixel);
        }
    }


    public class Transform
    {
        public PointF position;
        public Size size;

        public Transform(PointF pos, Size size)
        {
            this.position = pos;
            this.size = size;
        }
    }
}
