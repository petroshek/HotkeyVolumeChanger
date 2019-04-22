using System;
using System.Runtime.InteropServices;

namespace HotkeyVolumeChanger
{
	public static class AudioManager
	{
		public static float GetMasterVolume()
		{
			IAudioEndpointVolume audioEndpointVolume = null;
			try
			{
				audioEndpointVolume = GetMasterVolumeObject();
				if (audioEndpointVolume == null)
				{
					return -1f;
				}
				audioEndpointVolume.GetMasterVolumeLevelScalar(out float level);
				return level * 100f;
			}
			finally
			{
				if (audioEndpointVolume != null)
				{
					Marshal.ReleaseComObject(audioEndpointVolume);
				}
			}
		}

		public static bool GetMasterVolumeMute()
		{
			IAudioEndpointVolume audioEndpointVolume = null;
			try
			{
				audioEndpointVolume = GetMasterVolumeObject();
				if (audioEndpointVolume == null)
				{
					return false;
				}
				audioEndpointVolume.GetMute(out bool isMuted);
				return isMuted;
			}
			finally
			{
				if (audioEndpointVolume != null)
				{
					Marshal.ReleaseComObject(audioEndpointVolume);
				}
			}
		}

		public static void SetMasterVolume(float newLevel)
		{
			IAudioEndpointVolume audioEndpointVolume = null;
			try
			{
				audioEndpointVolume = GetMasterVolumeObject();
				audioEndpointVolume?.SetMasterVolumeLevelScalar(newLevel / 100f, Guid.Empty);
			}
			finally
			{
				if (audioEndpointVolume != null)
				{
					Marshal.ReleaseComObject(audioEndpointVolume);
				}
			}
		}

		public static float StepMasterVolume(float stepAmount)
		{
			IAudioEndpointVolume audioEndpointVolume = null;
			try
			{
				audioEndpointVolume = GetMasterVolumeObject();
				if (audioEndpointVolume == null)
				{
					return -1f;
				}
				float num = stepAmount / 100f;
				audioEndpointVolume.GetMasterVolumeLevelScalar(out float level);
				float val = level + num;
				val = Math.Min(1f, val);
				val = Math.Max(0f, val);
				audioEndpointVolume.SetMasterVolumeLevelScalar(val, Guid.Empty);
				return val * 100f;
			}
			finally
			{
				if (audioEndpointVolume != null)
				{
					Marshal.ReleaseComObject(audioEndpointVolume);
				}
			}
		}

		public static void SetMasterVolumeMute(bool isMuted)
		{
			IAudioEndpointVolume audioEndpointVolume = null;
			try
			{
				audioEndpointVolume = GetMasterVolumeObject();
				audioEndpointVolume?.SetMute(isMuted, Guid.Empty);
			}
			finally
			{
				if (audioEndpointVolume != null)
				{
					Marshal.ReleaseComObject(audioEndpointVolume);
				}
			}
		}

		public static bool ToggleMasterVolumeMute()
		{
			IAudioEndpointVolume audioEndpointVolume = null;
			try
			{
				audioEndpointVolume = GetMasterVolumeObject();
				if (audioEndpointVolume == null)
				{
					return false;
				}
				audioEndpointVolume.GetMute(out bool isMuted);
				audioEndpointVolume.SetMute(!isMuted, Guid.Empty);
				return !isMuted;
			}
			finally
			{
				if (audioEndpointVolume != null)
				{
					Marshal.ReleaseComObject(audioEndpointVolume);
				}
			}
		}

		private static IAudioEndpointVolume GetMasterVolumeObject()
		{
			IMMDeviceEnumerator iMMDeviceEnumerator = null;
			IMMDevice ppDevice = null;
			try
			{
				iMMDeviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
				iMMDeviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out ppDevice);
				Guid iid = typeof(IAudioEndpointVolume).GUID;
				ppDevice.Activate(ref iid, 0, IntPtr.Zero, out object ppInterface);
				return (IAudioEndpointVolume)ppInterface;
			}
			finally
			{
				if (ppDevice != null)
				{
					Marshal.ReleaseComObject(ppDevice);
				}
				if (iMMDeviceEnumerator != null)
				{
					Marshal.ReleaseComObject(iMMDeviceEnumerator);
				}
			}
		}

		public static float? GetApplicationVolume(int pid)
		{
			ISimpleAudioVolume volumeObject = GetVolumeObject(pid);
			if (volumeObject == null)
			{
				return null;
			}
			volumeObject.GetMasterVolume(out float pfLevel);
			Marshal.ReleaseComObject(volumeObject);
			return pfLevel * 100f;
		}

		public static bool? GetApplicationMute(int pid)
		{
			ISimpleAudioVolume volumeObject = GetVolumeObject(pid);
			if (volumeObject == null)
			{
				return null;
			}
			volumeObject.GetMute(out bool pbMute);
			Marshal.ReleaseComObject(volumeObject);
			return pbMute;
		}

		public static void SetApplicationVolume(int pid, float level)
		{
			ISimpleAudioVolume volumeObject = GetVolumeObject(pid);
			if (volumeObject != null)
			{
				Guid EventContext = Guid.Empty;
				volumeObject.SetMasterVolume(level / 100f, ref EventContext);
				Marshal.ReleaseComObject(volumeObject);
			}
		}

		public static void SetApplicationMute(int pid, bool mute)
		{
			ISimpleAudioVolume volumeObject = GetVolumeObject(pid);
			if (volumeObject != null)
			{
				Guid EventContext = Guid.Empty;
				volumeObject.SetMute(mute, ref EventContext);
				Marshal.ReleaseComObject(volumeObject);
			}
		}

		private static ISimpleAudioVolume GetVolumeObject(int pid)
		{
			IMMDeviceEnumerator iMMDeviceEnumerator = null;
			IAudioSessionEnumerator SessionEnum = null;
			IAudioSessionManager2 audioSessionManager = null;
			IMMDevice ppDevice = null;
			try
			{
				iMMDeviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
				iMMDeviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out ppDevice);
				Guid iid = typeof(IAudioSessionManager2).GUID;
				ppDevice.Activate(ref iid, 0, IntPtr.Zero, out object ppInterface);
				audioSessionManager = (IAudioSessionManager2)ppInterface;
				audioSessionManager.GetSessionEnumerator(out SessionEnum);
				SessionEnum.GetCount(out int SessionCount);
				ISimpleAudioVolume result = null;
				for (int i = 0; i < SessionCount; i++)
				{
					IAudioSessionControl2 Session = null;
					SessionEnum.GetSession(i, out Session);
					Session.GetProcessId(out int pRetVal);
					if (pRetVal == pid)
					{
						result = (Session as ISimpleAudioVolume);
						break;
					}
				}
				return result;
			}
			finally
			{
				if (SessionEnum != null)
				{
					Marshal.ReleaseComObject(SessionEnum);
				}
				if (audioSessionManager != null)
				{
					Marshal.ReleaseComObject(audioSessionManager);
				}
				if (ppDevice != null)
				{
					Marshal.ReleaseComObject(ppDevice);
				}
				if (iMMDeviceEnumerator != null)
				{
					Marshal.ReleaseComObject(iMMDeviceEnumerator);
				}
			}
		}
	}
}
