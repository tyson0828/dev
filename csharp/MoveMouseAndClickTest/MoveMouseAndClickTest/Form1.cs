namespace MoveMouseAndClickTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            SendToBack();
            Thread.Sleep(10000);
            ClickWaitClick();
            Activate();
        }

        private void ClickWaitClick() 
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            // Cursor.Position = new Point(Cursor.Position.X - 50, Cursor.Position.Y - 50);
            LeftClick(Cursor.Position.X, Cursor.Position.Y);
            System.Threading.Thread.Sleep(2180);
            this.Cursor = new Cursor(Cursor.Current.Handle);
            LeftClick(Cursor.Position.X, Cursor.Position.Y);
        }

        private void LeftClick(int x, int y)
        {
            InputSender.SetCursorPosition(x, y);

            InputSender.SendMouseInput(new InputSender.MouseInput[]
            {
                new InputSender.MouseInput
                {
                   dwFlags = (uint)InputSender.MouseEventF.LeftDown
                }
            });

            Thread.Sleep(100);

            InputSender.SendMouseInput(new InputSender.MouseInput[]
            {
                new InputSender.MouseInput
                {
                    dwFlags = (uint)InputSender.MouseEventF.LeftUp
                }
           });
        }


    }
}
