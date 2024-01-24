using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

public class Game : Form
{
    public Graphics G { get; set; }
    public Bitmap Bmp { get; set; }
    public Timer Tmr { get; set; }
    public PictureBox Pb { get; set; }
    public int TickCounter { get; set; }
    public Player Player { get; set; }
    public Enemy Enemy { get; set; }
    public Chest Chest { get; set; }
    public int Index { get; set; } = 0;
    private Random random = new Random();

    public int X { get; set; }
    public int Y { get; set; }
    public bool Exit { get; set; } = false;
    public float CurrentX { get; set; }
    public float CurrentY { get; set; }
    public float TargetX { get; set; } = 0;
    public float TargetY { get; set; } = 0;

    private Maze maze;
    private Space crrSpace;

    private Image floor1 = Image.FromFile("./assets/blocks/floor1.png");
    private Image floor2 = Image.FromFile("./assets/blocks/floor2.png");
    private Image floor3 = Image.FromFile("./assets/blocks/floor3.png");
    private Image floor4 = Image.FromFile("./assets/blocks/floor4.png");
    private Image floor5 = Image.FromFile("./assets/blocks/floor5.png");
    private Image floor6 = Image.FromFile("./assets/blocks/floor6.png");
    private Image floor7 = Image.FromFile("./assets/blocks/floor7.png");
    private Image floor8 = Image.FromFile("./assets/blocks/floor8.png");
    private Image floor9 = Image.FromFile("./assets/blocks/floor9.png");
    private Image floor10 = Image.FromFile("./assets/blocks/floor10.png");
    private Image floor11 = Image.FromFile("./assets/blocks/floor11.png");
    private Image floor12 = Image.FromFile("./assets/blocks/floor12.png");
    private Image floor13 = Image.FromFile("./assets/blocks/floor13.png");
    private Image floor14 = Image.FromFile("./assets/blocks/floor14.png");
    private Image floor15 = Image.FromFile("./assets/blocks/floor15.png");
    
    private Image wall = Image.FromFile("./assets/blocks/wall.png");

    private Image heart = Image.FromFile("./assets/objects/heart.png");
    private Image seed = Image.FromFile("./assets/objects/seed.png");

    private Image chestClosed = Image.FromFile("./assets/objects/chestClosed.png");
    private Image chestOpened = Image.FromFile("./assets/objects/chestOpened.png");

    public Image[] playerAnim = 
    {
        Bitmap.FromFile("./assets/player/1down.png")
    };

    public Image[] enemyAnim = 
    {
        Bitmap.FromFile("./assets/enemy/enemy1.png"),
        Bitmap.FromFile("./assets/enemy/enemy2.png")
    };

    public Image[] chestAnim = 
    {
        Bitmap.FromFile("./assets/objects/chestClosed.png"),
        Bitmap.FromFile("./assets/objects/chestOpened.png")
    };

    private float baseX = 400;
    private float baseY = 400;

    float lanternX = 960;
    float lanternY = 540;
    float radius = 1100;
    float borderWidth = 800;

    public Game()
    {
        maze = Maze.Prim(50, 50);
        crrSpace = maze.Spaces
            .OrderByDescending(s => Random.Shared.Next())
            .FirstOrDefault();

        var timer = new Timer
        {
            Interval = 20,
        };
        this.Tmr = timer;
        this.Player = new();
        this.Enemy = new();
        this.Chest = new();

        this.Pb = new()
        {
            Dock = DockStyle.Fill,
        };

        WindowState = FormWindowState.Maximized;
        FormBorderStyle = FormBorderStyle.None;

        this.Load += (o, e) =>
        {
            this.Bmp = new Bitmap(
                Pb.Width,
                Pb.Height
            );

            G = Graphics.FromImage(this.Bmp);
            Pb.Image = this.Bmp;
            G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            timer.Start();
        };

        Controls.Add(Pb);
        timer.Tick += (o, e) => this.Tick();

        KeyDown += (o, e) =>
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;
            }
        };

        // KeyUp += (o, e) =>
        // {
        //     switch (e.KeyCode)
        //     {

        //     }
        // };
    }

    public void Tick()
    {
        G.Clear(Color.FromArgb(0xFF, 0x41, 0x98, 0x0A));
        DrawMaze(baseX, baseY, crrSpace);
        // DrawLantern(lanternX, lanternY, radius, borderWidth);
        DrawPlayer();
        DrawStats();
        this.Pb.Refresh();
        // DrawChest();
        // DrawEnemies();
        TickCounter++;
    }

    private void DrawMaze(float x, float y, Space space)
    {
        if (space == null)
            return;
        
        DrawWall(space, x, y);
    }

    private void DrawWall(Space space, float x, float y, List<Space> visited = null)
    {
        const float wallSize = 35;

        if (visited is null)
            visited = new();
        
        if (visited.Contains(space))
            return;
        visited.Add(space);

        var imgFloor = (space.Left, space.Top, space.Right, space.Bottom) switch {
            (null, null, null, _) => floor11,
            (null, null, _, null) => floor10,
            (null, _, null, null) => floor9,
            (_, null, null, null) => floor8,
            (null, null, _, _) => floor7,
            (null, _, _, null) => floor6,
            (_, _, null, null) => floor5,
            (_, null, null, _) => floor4,
            (_, null, _, null) => floor2,
            (null, _, null, _) => floor1,
            (_, null, _, _) => floor15,
            (null, _, _, _) => floor14,
            (_, _, _, null) => floor13,
            (_, _, null, _) => floor12,
            _ => floor3
        };

        G.DrawImage(imgFloor, x, y, wallSize, wallSize);

        if (space.Top == null)
            G.DrawImage(wall, x, y - 5, wallSize, 10);
        else DrawWall(space.Top, x, y - wallSize, visited);

        if (space.Bottom == null)
            G.DrawImage(wall, x, y + wallSize - 5, wallSize, 10);
        else DrawWall(space.Bottom, x, y + wallSize, visited);

        if (space.Left == null)
            G.DrawImage(wall, x - 5, y, 10, wallSize);
        else DrawWall(space.Left, x - wallSize, y, visited);

        if (space.Right == null)
            G.DrawImage(wall, x + wallSize - 5, y, 10, wallSize);
        else DrawWall(space.Right, x + wallSize, y, visited);
    }

    private void DrawStats()
    {
        Color textColor = Color.White;
        SolidBrush textBrush = new(textColor);

        Font font = new("Arial", 12, FontStyle.Bold);

        G.DrawImage(heart, Pb.Width * 0.01f, Pb.Height * 0.01f);
        G.DrawImage(seed, Pb.Width * 0.06f, Pb.Height * 0.01f);
        G.DrawString(Player.PlayerLife.ToString(), font, textBrush, new PointF(Pb.Width * 0.05f, Pb.Height * 0.05f));
        G.DrawString(Player.Seeds.ToString(), font, textBrush, new PointF(Pb.Width * 0.10f, Pb.Height * 0.05f));
    }

    private void DrawPlayer()
    {
        G.DrawImage(playerAnim[0], Pb.Width / 2, Pb.Height / 2, 150, 150);      
    }

    private void DrawEnemies()
    {
        const int speedAnimEnemy = 6;

        if (Enemy.EnemyLife > 0)
        {
            if (Index < speedAnimEnemy)
            {
                G.DrawImage(enemyAnim[0], 500, 500);
                Index++;
            }
            else
            {
                G.DrawImage(enemyAnim[1], 500, 500);
                Index++;
                if (Index > 2 * speedAnimEnemy)
                    Index = 0;
            }
        }
    }

    private void DrawChest()
    {
        if (Chest.Open == true)
            G.DrawImage(chestAnim[1], 100, 100);
        else G.DrawImage(chestAnim[0], 100, 100);
    }

    private void DrawLantern(float x, float y, float radius, float borderWidth)
    {
        float width = radius * 2;
        float height = radius * 2;

        RectangleF rect = new RectangleF(x - radius, y - radius, width, height);

        for (float i = 0; i <= borderWidth; i += 1)
        {
            float alpha = Math.Max(0, 1 - (i / radius));
            Color color = Color.FromArgb((int)(alpha * 255), 0, 0, 0);
            RectangleF borderRect = new RectangleF(rect.X + i, rect.Y + i, rect.Width - 2 * i, rect.Height - 2 * i);
            G.DrawEllipse(new Pen(color, 4), borderRect);
        }
    }
}





//INTERPOLAÇÃO