using System;
using System.Windows.Forms;

namespace HotkeyVolumeChanger
{
	public class KeyPressedEventArgs : EventArgs
	{
		private Keys _key;

		public Keys Key => _key;

		internal KeyPressedEventArgs(Keys key)
		{
			_key = key;
			if (key.ToString() == Form1.get_key(1))
			{
				AudioManager.SetApplicationVolume(Form1.getPID(), Form1.getVol1());
			}
			else if (key.ToString() == Form1.get_key(2))
			{
				AudioManager.SetApplicationVolume(Form1.getPID(), Form1.getVol2());
			}
            else if (key.ToString() == Form1.get_key(3))
            {
                AudioManager.SetApplicationVolume(Form1.getPID(), Form1.getVol3());
            }
            else if (key.ToString() == Form1.get_key(4))
            {
                AudioManager.SetApplicationVolume(Form1.getPID(), Form1.getVol4());
            }
        }
	}
}
