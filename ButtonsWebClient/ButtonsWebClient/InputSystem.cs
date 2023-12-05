namespace ButtonsWebClient
{
    public class InputSystem
    {
        public class ClickEventArgs : EventArgs
        {
            public MouseButtons Button;
            public Point Position;
            public ClickEventArgs(MouseButtons button)
            {
                Button = button;
                Position = Instance.MouseCoords;
            }
        }

        private static readonly Lazy<InputSystem> _instance = new Lazy<InputSystem>(new InputSystem());
        public static InputSystem Instance => _instance.Value;

        private readonly IDictionary<MouseButtons, ButtonStates> _buttonStates;

        private InputSystem()
        {
            _buttonStates = new Dictionary<MouseButtons, ButtonStates>()
            {
                {MouseButtons.Left, ButtonStates.Up},
                {MouseButtons.Middle, ButtonStates.Up},
                {MouseButtons.Right, ButtonStates.Up},
            };
        }

        public Point MouseCoords;

        public event EventHandler<ClickEventArgs> LeftClick;
        public event EventHandler<ClickEventArgs> RightClick;
        public event EventHandler<ClickEventArgs> MiddleClick;

        public void SetButtonState(MouseButtons button, ButtonStates state)
        {
            _buttonStates[button] = state;
            if (state == ButtonStates.Down)
            {
                switch (button)
                {
                    case MouseButtons.Left:
                        LeftClick?.Invoke(this, new ClickEventArgs(button));
                        break;
                    case MouseButtons.Middle:
                        MiddleClick?.Invoke(this, new ClickEventArgs(button));
                        break;
                    case MouseButtons.Right:
                        RightClick?.Invoke(this, new ClickEventArgs(button));
                        break;
                }
            }
        }

        public ButtonStates GetButtonState(MouseButtons button) => _buttonStates[button];
    }

    public enum MouseButtons
    {
        Left = 0,
        Middle = 1,
        Right = 2
    }

    public enum ButtonStates
    {
        Up = 0,
        Down = 1
    }
}
