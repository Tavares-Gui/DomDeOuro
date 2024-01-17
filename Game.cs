using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class Game : Form
{
    public Graphics G { get; set; }
    public Bitmap Bmp { get; set; }
    public Timer Tmr { get; set; }
    public PictureBox Pb { get; set; }
    public int TickCounter { get; set; }
    public Player Player { get; set; }
    public Enemy Enemy { get; set; }


    public int X { get; set; }
    public int Y { get; set; }
    public Space Top { get; set; } = null;
    public Space Left { get; set; } = null;
    public Space Right { get; set; } = null;
    public Space Bottom { get; set; } = null;
    public bool Visited { get; set; } = false;
    public bool IsSolution { get; set; } = false;
    public bool Exit { get; set; } = false;

    public void Reset()
    {
        IsSolution = false;
        Visited = false;
    }

    private Maze maze;
    private Space crrSpace;

    private Image floor = Image.FromFile("./assets/blocks/floor.png");
    private Image wall = Image.FromFile("./assets/blocks/wall.png");
    private float baseX = 0;
    private float baseY = 0;
    public Game()
    {
        maze = Maze.Prim(
            Random.Shared.Next(48),
            Random.Shared.Next(27)
        );
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
        this.Pb.Refresh();
        DrawFloor();
        DrawMaze(crrSpace, baseX, baseY);
        TickCounter++;
    }

    private void DrawMaze(Space space, float x, float y)
    {
        if (space == null || space.Visited)
            return;

        space.Visited = true;

        
    }

    private void DrawFloor()
    {
        var cols = Bmp.Width / floor.Width;
        var lins = Bmp.Height / floor.Height;
        for (int i = -1; i < cols + 1; i++)
        {
            for (int j = -1; j < lins + 1; j++)
            {
                var x = i * floor.Width + baseX % floor.Height;
                var y = j * floor.Height + baseY % floor.Width;
                G.DrawImage(floor, new PointF(x, y));
            }
        }
    }

    private void DrawWall(float x, float y, Space space)
    {
    }
}