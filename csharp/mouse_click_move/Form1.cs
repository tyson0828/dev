namespace mouse_click_move;

 public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static implicit operator System.Drawing.Point(POINT p)
        {
            return new System.Drawing.Point(p.X, p.Y);
        }

        public static implicit operator POINT(System.Drawing.Point p)
        {
            return new POINT(p.X, p.Y);
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetCursorPos(out POINT lpPoint);
    private void Form1_Load(object sender, EventArgs e)
    {
        Hook.GlobalEvents().MouseClick += MouseClickAll;

    }

    private void MouseClickAll(object sender, MouseEventArgs e)
    {
        POINT p;
        if (GetCursorPos(out p))
        {
            label1.Text = Convert.ToString(p.X) + ";" + Convert.ToString(p.Y);
        }
    }
}