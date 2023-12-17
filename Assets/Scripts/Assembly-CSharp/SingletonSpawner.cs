using System;
using System.Collections.Generic;
using UnityEngine;

public class SingletonSpawner : MonoBehaviour
{
	[Serializable]
	public class PlatformSingleton
	{
		public List<DeviceInfo.DeviceFamily> platforms = new List<DeviceInfo.DeviceFamily>();

		public GameObject singleton;
	}

	[SerializeField]
	private List<PlatformSingleton> m_platformSingletons;

	[SerializeField]
	private List<GameObject> m_commonSingletons;

	private static bool spawnDone;

	public static bool SpawnDone
	{
		get
		{
			return spawnDone;
		}
	}

	private void Awake()
	{
		if (spawnDone)
		{
			return;
		}
		Application.targetFrameRate = 60;
		Screen.sleepTimeout = -1;
		foreach (GameObject commonSingleton in m_commonSingletons)
		{
			if (!GameObject.Find(commonSingleton.name))
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(commonSingleton);
				gameObject.name = commonSingleton.name;
				gameObject.active = true;
			}
			else
			{
				Debug.LogError("Singleton already instantiated: " + commonSingleton.name);
			}
		}
		SpawnPlatformSingletons();
		spawnDone = true;
	}

	private void SpawnPlatformSingletons()
	{
		foreach (PlatformSingleton platformSingleton in m_platformSingletons)
		{
			if (platformSingleton.platforms.Contains(DeviceInfo.Instance.ActiveDeviceFamily))
			{
				if (!GameObject.Find(platformSingleton.singleton.name))
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(platformSingleton.singleton);
					gameObject.name = platformSingleton.singleton.name;
					gameObject.active = true;
				}
				else
				{
					Debug.LogError("Singleton already instantiated: " + platformSingleton.singleton.name);
				}
			}
		}
	}
}
