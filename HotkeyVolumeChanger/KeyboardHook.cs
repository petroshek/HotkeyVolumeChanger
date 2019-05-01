using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HotkeyVolumeChanger
{
	public sealed class KeyboardHook : IDisposable
	{
        private class BoundKey
        {
            public BoundKeys Key;
            public int Id;
        }

		private class Window : NativeWindow, IDisposable
		{
			private static int WM_HOTKEY = 786;

			public event EventHandler<KeyPressedEventArgs> KeyPressed;

            public Window()
            {
                CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
			{
				base.WndProc(ref m);
				if (m.Msg == WM_HOTKEY)
				{
					Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
					if (this.KeyPressed != null)
					{
						this.KeyPressed(this, new KeyPressedEventArgs(key));
					}
				}
			}

			public void Dispose()
			{
				DestroyHandle();
			}
		}

		private Window _window = new Window();

		private int _currentId;
        private List<BoundKey> keyList;

		public event EventHandler<KeyPressedEventArgs> KeyPressed;

		[DllImport("user32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		public KeyboardHook()
		{
            keyList = new List<BoundKey>();
            _window.KeyPressed += delegate(object sender, KeyPressedEventArgs args)
			{
				if (this.KeyPressed != null)
				{
					this.KeyPressed(this, args);
				}
			};
		}

		public void RegisterHotKey(BoundKeys BK)
		{
			_currentId++;
            BoundKey temp = new BoundKey();
            temp.Key = BK;
            temp.Id = _currentId;
            keyList.Add(temp);
            if (!RegisterHotKey(_window.Handle, _currentId, 0u, (uint)BK.Key))
			{
				throw new InvalidOperationException("Couldn’t register the hot key.");
			}
		}

        public void DisposeKey(Keys key)
        {
            var temp = keyList.ToArray();
            foreach(BoundKey BK in temp)
            {
                if(BK.Key.Key == key)
                {
                    UnregisterHotKey(_window.Handle, BK.Id);
                    keyList.Remove(BK);
                }
            }

            if (keyList.Count == 0)
                Dispose();
        }

        public void Dispose()
		{
			_window.Dispose();
		}
	}
}
