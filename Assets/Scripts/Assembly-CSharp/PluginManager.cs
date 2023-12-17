using UnityEngine;

public class PluginManager : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			base.gameObject.AddComponent<FlurryManager>();
			FlurryManager.Instance.StartSession();
			FlurryManager.Instance.LogEvent("Game Started");
		}
		if (BuildCustomizationLoader.Instance.AdsEnabled)
		{
			base.gameObject.AddComponent<BurstlyManager>();
		}
		if (BuildCustomizationLoader.Instance.RovioNews)
		{
			base.gameObject.AddComponent<WebViewManager>();
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			if (focus)
			{
				FlurryManager.Instance.StartSession();
			}
			else
			{
				FlurryManager.Instance.EndSession();
			}
		}
	}
}
