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
				AudioManager.SetApplicationVolume(HotkeyVolumeChanger.getPID(), HotkeyVolumeChanger.getVol1());
			}
			else if (key.ToString() == HotkeyVolumeChanger.get_key(2))
			{
				AudioManager.SetApplicationVolume(HotkeyVolumeChanger.getPID(), HotkeyVolumeChanger.getVol2());
			}
            else if (key.ToString() == HotkeyVolumeChanger.get_key(3))
            {
                AudioManager.SetApplicationVolume(HotkeyVolumeChanger.getPID(), HotkeyVolumeChanger.getVol3());
            }
            else if (key.ToString() == HotkeyVolumeChanger.get_key(4))
            {
                AudioManager.SetApplicationVolume(HotkeyVolumeChanger.getPID(), HotkeyVolumeChanger.getVol4());
            }
        }
	}
}
