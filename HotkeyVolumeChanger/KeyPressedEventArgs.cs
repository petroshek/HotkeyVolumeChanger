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
            foreach(BoundKeys BK in HotkeyVolumeChanger.ListBoundKeys)
            {
                if(_key == BK.Key)
                {
                    if (BK.ToggleVol)
                    {
                        if (BK.Vol == 0.0F)
                            BK.Vol = BK.Vol1;
                        AudioManager.SetApplicationVolume(BK.PID, BK.Vol);
                        if (BK.Vol == BK.Vol1)
                            BK.Vol = BK.Vol2;
                        else
                            BK.Vol = BK.Vol1;
                    }
                    else
                        AudioManager.SetApplicationVolume(BK.PID, BK.Vol);
                }
            }
        }
	}
}
