using System.Collections.Generic;
using UnityEngine;

public class PausePage : WebviewDelegate
{
	private bool m_rovioNewsLoaded;

	private void OnEnable()
	{
		if (BuildCustomizationLoader.Instance.RovioNews)
		{
			WebViewManager instance = WebViewManager.Instance;
			if (!instance.Created)
			{
				instance.Create(0f, (float)Screen.width * 0.3f, (float)Screen.width * 0.7f, Screen.height);
				instance.RegisterListener(this);
			}
			Debug.Log(Screen.width + "-" + Screen.height);
			if (!m_rovioNewsLoaded)
			{
				instance.LoadURL("http://smoke.rovio.com/content/embed/pauseMenu/?d=iphone&p=abp");
				m_rovioNewsLoaded = true;
			}
			else
			{
				SendStandardFlurryEvent("Rovio News Loaded And Displayed", Application.loadedLevelName);
				instance.Show();
			}
		}
	}

	private void OnDisable()
	{
		if (BuildCustomizationLoader.Instance.RovioNews)
		{
			WebViewManager instance = WebViewManager.Instance;
			instance.Hide();
		}
	}

	public void SendStandardFlurryEvent(string eventName, string id)
	{
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ID", id);
			FlurryManager.Instance.LogEventWithParameters(eventName, dictionary);
		}
	}
}
