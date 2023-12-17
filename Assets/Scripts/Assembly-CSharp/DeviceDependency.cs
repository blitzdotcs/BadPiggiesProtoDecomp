using UnityEngine;

public class DeviceDependency : MonoBehaviour
{
	[SerializeField]
	private DeviceInfo.DeviceFamily[] enabledOnDevices;

	private void Start()
	{
		DeviceInfo.DeviceFamily activeDeviceFamily = DeviceInfo.Instance.ActiveDeviceFamily;
		bool flag = false;
		DeviceInfo.DeviceFamily[] array = enabledOnDevices;
		foreach (DeviceInfo.DeviceFamily deviceFamily in array)
		{
			if (activeDeviceFamily == deviceFamily)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
