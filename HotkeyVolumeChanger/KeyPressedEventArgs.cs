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
			if (key.ToString() == HotkeyVolumeChanger.get_key(1))
			{
				AudioManager.SetApplicationVolume(HotkeyVolumeChanger.GetPID(1), HotkeyVolumeChanger.GetVol1());
			}
			else if (key.ToString() == HotkeyVolumeChanger.get_key(2))
			{
				AudioManager.SetApplicationVolume(HotkeyVolumeChanger.GetPID(2), HotkeyVolumeChanger.GetVol2());
			}
            else if (key.ToString() == HotkeyVolumeChanger.get_key(3))
            {
                AudioManager.SetApplicationVolume(HotkeyVolumeChanger.GetPID(3), HotkeyVolumeChanger.GetVol3());
            }
            else if (key.ToString() == HotkeyVolumeChanger.get_key(4))
            {
                AudioManager.SetApplicationVolume(HotkeyVolumeChanger.GetPID(4), HotkeyVolumeChanger.GetVol4());
            }
        }
	}
}
