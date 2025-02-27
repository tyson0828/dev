using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace d4click
{
    public partial class Form1 : Form
    {
        private bool m_RClickMoveCursorEnabled;
        private int m_MousePointX;
        private int m_MousePointY;
        private IKeyboardMouseEvents m_Events;
        public Form1()
        {
            InitializeComponent();
            SubscribeGlobal();
            m_RClickMoveCursorEnabled = false;
            m_MousePointX = 0;
            m_MousePointY = 0;
            FormClosing += Main_Closing;
        }

        private void Main_Closing(object sender, CancelEventArgs e)
        {
            Unsubscribe();
        }

        private void SubscribeGlobal()
        {
            Unsubscribe();
            Subscribe(Hook.GlobalEvents());
        }

        private void Subscribe(IKeyboardMouseEvents events)
        {
            m_Events = events;


            m_Events.MouseClick += OnMouseClick;
            m_Events.MouseMove += HookManager_MouseMove;
        }

        private void Unsubscribe()
        {
            if (m_Events == null) return;
            m_Events.MouseClick -= OnMouseClick;
            m_Events.MouseMove -= HookManager_MouseMove;

            m_Events.Dispose();
            m_Events = null;
        }

        private static void LeftClick(int x, int y)
        {
            InputSender.SetCursorPosition(x, y);

            InputSender.SendMouseInput(new InputSender.MouseInput[]
            {
                new InputSender.MouseInput
                {
                   dwFlags = (uint)InputSender.MouseEventF.LeftDown
                }
            });

            Thread.Sleep(50);

            InputSender.SendMouseInput(new InputSender.MouseInput[]
            {
                new InputSender.MouseInput
                {
                    dwFlags = (uint)InputSender.MouseEventF.LeftUp
                }
           });
        }

        public Thread StartTheThread(int x, int y)
        {
            var t = new Thread(() => RealStart(x, y));
            t.Start();
            return t;
        }

        private static void RealStart(int x, int y)
        {
            LeftClick(x, y);
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (m_RClickMoveCursorEnabled)
            {

                //this.Cursor = new Cursor(Cursor.Current.Handle);
                //Cursor.Position = new Point(Cursor.Position.X - 50, Cursor.Position.Y - 50);
                //Cursor.Clip = new Rectangle(this.Location, this.Size);
                /*
                Win32.POINT p = new Win32.POINT(e.X - 50, e.Y - 50);

                Win32.ClientToScreen(this.Handle, ref p);
                Win32.SetCursorPos(p.x, p.y);
                */
                System.Threading.Thread.Sleep(2180);
                //Thread t = StartTheThread(m_MousePointX, m_MousePointY);
                LeftClick(m_MousePointX, m_MousePointY);
            }
        }

        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            m_MousePointX = e.X;
            m_MousePointY = e.Y;
            label1.Text = String.Format("x={0:0000}; y={1:0000}", m_MousePointX, m_MousePointY);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                m_RClickMoveCursorEnabled = true;
            }
            else
            {
                m_RClickMoveCursorEnabled = false;
            }
        }
    }
}
