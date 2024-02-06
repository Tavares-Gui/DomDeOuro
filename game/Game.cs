using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

public class Game : Form
{
    public Graphics G { get; set; }
    public Bitmap Bmp { get; set; }
    public Timer Tmr { get; set; }
    public static PictureBox Pb { get; set; }
    public static bool GameStart { get; set; } = false;

    private Lantern lantern = new();
    private Player player = new();
    private Maze maze = new();
    private Space crrSpace;
    bool loaded = false;

    public void Reset()
    {
        this.Controls.Clear();
        if (this.Tmr is not null)
            this.Tmr.Stop();

        maze = Maze.Prim(5, 5);
        crrSpace = maze.Spaces
            .OrderByDescending(s => GlobalSeed.Current.Random.Next())
            .FirstOrDefault();

        var timer = new Timer
        {
            Interval = 20,
        };
        this.Tmr = timer;
        this.player = new();

        Pb = new()
        {
            Dock = DockStyle.Fill,
        };

        WindowState = FormWindowState.Maximized;
        FormBorderStyle = FormBorderStyle.None;

        if (loaded)
        {
            this.Bmp = new Bitmap(
                Pb.Width,
                Pb.Height
            );

            G = Graphics.FromImage(this.Bmp);
            Pb.Image = this.Bmp;
            G.InterpolationMode = InterpolationMode.NearestNeighbor;
            G.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Point chestPosition = new(
                GlobalSeed.Current.Random.Next(Pb.Width),
                GlobalSeed.Current.Random.Next(Pb.Height)
            );
            timer.Start();
        }
        else
        {
            this.Load += (o, e) =>
            {
                loaded = true;
                this.Bmp = new Bitmap(
                    Pb.Width,
                    Pb.Height
                );

                G = Graphics.FromImage(this.Bmp);
                Pb.Image = this.Bmp;
                G.InterpolationMode = InterpolationMode.NearestNeighbor;
                G.PixelOffsetMode = PixelOffsetMode.HighQuality;
                Point chestPosition = new(
                    GlobalSeed.Current.Random.Next(Pb.Width),
                    GlobalSeed.Current.Random.Next(Pb.Height)
                );
                timer.Start();
            };
        }

        Controls.Add(Pb);
        timer.Tick += (o, e) => this.Tick();

        Pb.MouseDown += (o, e) => 
        {
            GameStart = true;
        };

        KeyDown += (o, e) =>
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;

                case Keys.Up:
                    maze.MoveUp();
                    Player.Movement = true;
                    Player.ImageIndex = 3;
                    break;

                case Keys.Left:
                    maze.MoveLeft();
                    Player.Movement = true;
                    Player.ImageIndex = 9;
                    break;

                case Keys.Down:
                    maze.MoveDown();
                    Player.Movement = true;
                    Player.ImageIndex = 0;
                    break;

                case Keys.Right:
                    maze.MoveRight();
                    Player.Movement = true;
                    Player.ImageIndex = 6;
                    break;

                case Keys.C:
                    Clipboard.SetText(GlobalSeed.Current.Seed.ToString());
                    break;

                case Keys.V:
                    GlobalSeed.Reset(int.Parse(Clipboard.GetText()));
                    Reset();
                    break;
            }
        };

        KeyUp += (o, e) =>
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    maze.StopY_axis();
                    Player.Img = Player.playerAnim[3];
                    Player.Movement = false;
                    break;

                case Keys.Left:
                    maze.StopX_axis();
                    Player.Img = Player.playerAnim[9];
                    Player.Movement = false;
                    break;

                case Keys.Down:
                    maze.StopY_axis();
                    Player.Img = Player.playerAnim[0];
                    Player.Movement = false;
                    break;

                case Keys.Right:
                    maze.StopX_axis();
                    Player.Img = Player.playerAnim[6];
                    Player.Movement = false;
                    break;
            }
        };
    }

    public Game()
    {
        Reset();
    }

    public void Draw()
    {
        maze.Draw(G, crrSpace);
        player.Draw(G, Pb);
        // lantern.Draw(G, Pb);
        player.DrawStats(G, Pb);
        G.DrawString(
            $"seed: {GlobalSeed.Current.Seed}. press C to copy seed.",
            SystemFonts.MenuFont,
            Brushes.White,
            new PointF(20, Pb.Height - 20)
        );
    }

    public void Tick()
    {
        G.Clear(Color.Black);
        Update();

        if (GameStart)
            Draw();
        else
            Menu.Draw(G);

        Pb.Refresh();
    }

    public new void Update()
    {
        maze.Move(new RectangleF(
            Pb.Width / 2 - 75,
            Pb.Height / 2 - 75,
            150, 150), crrSpace
        );
    }
}
