using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FlurryManager : MonoBehaviour
{
	private const string FLURRY_KEY = "Z8V6EYB8AKEGVUBYW3ND";

	private static FlurryManager instance;

	private Dictionary<string, string> m_flurryDataHolder = new Dictionary<string, string>();

	private DateTime m_lastSessionStart;

	public Dictionary<string, string> FlurryDataHolder
	{
		get
		{
			return m_flurryDataHolder;
		}
	}

	public static FlurryManager Instance
	{
		get
		{
			return instance;
		}
	}

	[DllImport("__Internal")]
	private static extern void _StartSession(string key);

	[DllImport("__Internal")]
	private static extern void _EndSession();

	[DllImport("__Internal")]
	private static extern void _LogEvent(string eventName);

	[DllImport("__Internal")]
	private static extern void _LogEventWithParameters(string eventName, string dictionary2string);

	public void StartSession()
	{
		if (!Application.isEditor)
		{
			_StartSession("Z8V6EYB8AKEGVUBYW3ND");
			LogSessionStart();
		}
	}

	public void EndSession()
	{
		if (!Application.isEditor)
		{
			_EndSession();
		}
	}

	public void LogEvent(string eventName)
	{
		if (!Application.isEditor)
		{
			_LogEvent(eventName);
		}
	}

	public void LogEventWithParameters(string eventName, Dictionary<string, string> parameters)
	{
		if (Application.isEditor)
		{
			return;
		}
		string text = string.Empty;
		foreach (KeyValuePair<string, string> parameter in parameters)
		{
			string text2 = text;
			text = text2 + parameter.Key + "::" + parameter.Value + "##";
		}
		Debug.Log(text);
		_LogEventWithParameters(eventName, text);
	}

	public static bool IsInstantiated()
	{
		return instance;
	}

	private void Awake()
	{
		Assert.Check(instance == null, "Singleton " + base.name + " spawned twice");
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
		m_lastSessionStart = DateTime.Now;
	}

	private void LogSessionStart()
	{
		if (!Application.isEditor)
		{
			DateTime now = DateTime.Now;
			int hours = (now - m_lastSessionStart).Hours;
			if (hours > 0)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("DAYS", hours.ToString());
				LogEventWithParameters("Session Start", dictionary);
			}
			m_lastSessionStart = DateTime.Now;
		}
	}
}
