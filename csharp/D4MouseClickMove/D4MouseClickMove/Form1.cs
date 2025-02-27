using Gma.System.MouseKeyHook;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace D4MouseClickMove
{
    public partial class Form1 : Form
    {
        private IKeyboardMouseEvents m_Events;

        public Main()
        {
            InitializeComponent();
            radioGlobal.Checked = true;
            SubscribeGlobal();
            FormClosing += Main_Closing;
        }

        private void Main_Closing(object sender, CancelEventArgs e)
        {
            Unsubscribe();
        }

        private void SubscribeApplication()
        {
            Unsubscribe();
            Subscribe(Hook.AppEvents());
        }

        private void SubscribeGlobal()
        {
            Unsubscribe();
            Subscribe(Hook.GlobalEvents());
        }

        private void Subscribe(IKeyboardMouseEvents events)
        {
            m_Events = events;
            m_Events.KeyDown += OnKeyDown;
            m_Events.KeyUp += OnKeyUp;
            m_Events.KeyPress += HookManager_KeyPress;

            m_Events.MouseUp += OnMouseUp;
            m_Events.MouseClick += OnMouseClick;
            m_Events.MouseDoubleClick += OnMouseDoubleClick;

            m_Events.MouseMove += HookManager_MouseMove;

            m_Events.MouseDragStarted += OnMouseDragStarted;
            m_Events.MouseDragFinished += OnMouseDragFinished;

            if (checkBoxSupressMouseWheel.Checked)
            {
                m_Events.MouseWheelExt += HookManager_MouseWheelExt;
                m_Events.MouseHWheelExt += HookManager_MouseHWheelExt;
            }
            else
            {
                m_Events.MouseWheel += HookManager_MouseWheel;
                m_Events.MouseHWheel += HookManager_MouseHWheel;
            }

            if (checkBoxSuppressMouse.Checked)
                m_Events.MouseDownExt += HookManager_Supress;
            else
                m_Events.MouseDown += OnMouseDown;
        }

        private void Unsubscribe()
        {
            if (m_Events == null) return;
            m_Events.KeyDown -= OnKeyDown;
            m_Events.KeyUp -= OnKeyUp;
            m_Events.KeyPress -= HookManager_KeyPress;

            m_Events.MouseUp -= OnMouseUp;
            m_Events.MouseClick -= OnMouseClick;
            m_Events.MouseDoubleClick -= OnMouseDoubleClick;

            m_Events.MouseMove -= HookManager_MouseMove;

            m_Events.MouseDragStarted -= OnMouseDragStarted;
            m_Events.MouseDragFinished -= OnMouseDragFinished;

            if (checkBoxSupressMouseWheel.Checked)
            {
                m_Events.MouseWheelExt -= HookManager_MouseWheelExt;
                m_Events.MouseHWheelExt -= HookManager_MouseHWheelExt;
            }
            else
            {
                m_Events.MouseWheel -= HookManager_MouseWheel;
                m_Events.MouseHWheel -= HookManager_MouseHWheel;
            }

            if (checkBoxSuppressMouse.Checked)
                m_Events.MouseDownExt -= HookManager_Supress;
            else
                m_Events.MouseDown -= OnMouseDown;

            m_Events.Dispose();
            m_Events = null;
        }

        private void HookManager_Supress(object sender, MouseEventExtArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                Log(string.Format("MouseDown \t\t {0}\n", e.Button));
                return;
            }

            Log(string.Format("MouseDown \t\t {0} Suppressed\n", e.Button));
            e.Handled = true;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Log(string.Format("KeyDown  \t\t {0}\n", e.KeyCode));
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            Log(string.Format("KeyUp  \t\t\t {0}\n", e.KeyCode));
        }

        private void HookManager_KeyPress(object sender, KeyPressEventArgs e)
        {
            Log(string.Format("KeyPress \t\t\t {0}\n", e.KeyChar));
        }

        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            labelMousePosition.Text = string.Format("x={0:0000}; y={1:0000}", e.X, e.Y);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Log(string.Format("MouseDown \t\t {0}\n", e.Button));
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            Log(string.Format("MouseUp \t\t {0}\n", e.Button));
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            Log(string.Format("MouseClick \t\t {0}\n", e.Button));
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            Log(string.Format("MouseDoubleClick \t\t {0}\n", e.Button));
        }

        private void OnMouseDragStarted(object sender, MouseEventArgs e)
        {
            Log("MouseDragStarted\n");
        }

        private void OnMouseDragFinished(object sender, MouseEventArgs e)
        {
            Log("MouseDragFinished\n");
        }

        private void HookManager_MouseWheel(object sender, MouseEventArgs e)
        {
            labelWheel.Text = string.Format("Wheel={0:000}", e.Delta);
        }

        private void HookManager_MouseWheelExt(object sender, MouseEventExtArgs e)
        {
            labelWheel.Text = string.Format("Wheel={0:000}", e.Delta);
            Log("Mouse Wheel Move Suppressed.\n");
            e.Handled = true;
        }

        private void HookManager_MouseHWheel(object sender, MouseEventArgs e)
        {
            labelHWheel.Text = string.Format("HWheel={0:000}", e.Delta);
        }

        private void HookManager_MouseHWheelExt(object sender, MouseEventExtArgs e)
        {
            labelHWheel.Text = string.Format("HWheel={0:000}", e.Delta);
            Log("Horizontal Mouse Wheel Move Suppressed.\n");
            e.Handled = true;
        }

        private void Log(string text)
        {
            if (IsDisposed) return;
            textBoxLog.AppendText(text);
            textBoxLog.AppendText(Environment.NewLine);
            textBoxLog.ScrollToCaret();
        }

        private void radioApplication_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) SubscribeApplication();
        }

        private void radioGlobal_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) SubscribeGlobal();
        }

        private void radioNone_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Unsubscribe();
        }

        private void checkBoxSupressMouseWheel_CheckedChanged(object sender, EventArgs e)
        {
            if (m_Events == null) return;

            if (((CheckBox)sender).Checked)
            {
                m_Events.MouseWheel -= HookManager_MouseWheel;
                m_Events.MouseWheelExt += HookManager_MouseWheelExt;
                m_Events.MouseHWheel -= HookManager_MouseHWheel;
                m_Events.MouseHWheelExt += HookManager_MouseHWheelExt;
            }
            else
            {
                m_Events.MouseWheelExt -= HookManager_MouseWheelExt;
                m_Events.MouseWheel += HookManager_MouseWheel;
                m_Events.MouseHWheelExt -= HookManager_MouseHWheelExt;
                m_Events.MouseHWheel += HookManager_MouseHWheel;
            }
        }

        private void checkBoxSuppressMouse_CheckedChanged(object sender, EventArgs e)
        {
            if (m_Events == null) return;

            if (((CheckBox)sender).Checked)
            {
                m_Events.MouseDown -= OnMouseDown;
                m_Events.MouseDownExt += HookManager_Supress;
            }
            else
            {
                m_Events.MouseDownExt -= HookManager_Supress;
                m_Events.MouseDown += OnMouseDown;
            }
        }

        private void clearLog_Click(object sender, EventArgs e)
        {
            textBoxLog.Clear();
        }
    }

    /*
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
    */
}
