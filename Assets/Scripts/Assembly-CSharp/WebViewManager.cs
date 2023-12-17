using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WebViewManager : MonoBehaviour
{
	private List<WebviewDelegate> m_listeners = new List<WebviewDelegate>();

	private static WebViewManager instance;

	private bool m_created;

	public bool Created
	{
		get
		{
			return m_created;
		}
	}

	public static WebViewManager Instance
	{
		get
		{
			return instance;
		}
	}

	[DllImport("__Internal")]
	private static extern void _Create(float top, float left, float width, float height, string delegateObject);

	[DllImport("__Internal")]
	private static extern void _Destroy();

	[DllImport("__Internal")]
	private static extern void _Show();

	[DllImport("__Internal")]
	private static extern void _Hide();

	[DllImport("__Internal")]
	private static extern void _LoadURL(string url);

	public void Create(float top, float left, float width, float height)
	{
		if (!Application.isEditor)
		{
			string delegateObject = base.name.Replace("(Clone)", string.Empty);
			_Create(top, left, width, height, delegateObject);
			m_created = true;
		}
	}

	public void Destroy()
	{
		if (!Application.isEditor)
		{
			_Destroy();
			m_created = false;
		}
	}

	public void Show()
	{
		if (!Application.isEditor)
		{
			_Show();
		}
	}

	public void Hide()
	{
		if (!Application.isEditor)
		{
			_Hide();
		}
	}

	public void LoadURL(string url)
	{
		if (!Application.isEditor)
		{
			_LoadURL(url);
		}
	}

	public void RegisterListener(WebviewDelegate obj)
	{
		m_listeners.Add(obj);
	}

	public void RemoveListener(WebviewDelegate obj)
	{
		m_listeners.Remove(obj);
	}

	public static bool IsInstantiated()
	{
		return instance;
	}

	private void Awake()
	{
		Assert.Check(instance == null, "Singleton " + base.name + " spawned twice");
		instance = this;
		Object.DontDestroyOnLoad(this);
	}

	protected void webViewDidFinishLoad(string pageTitle)
	{
		foreach (WebviewDelegate listener in m_listeners)
		{
			listener.webViewDidFinishLoad(pageTitle);
		}
		Debug.Log("webViewDidFinishLoad: " + pageTitle);
	}

	protected void webViewDidFail(string errorCode)
	{
		foreach (WebviewDelegate listener in m_listeners)
		{
			listener.webViewDidFail(errorCode);
		}
		Debug.Log("webViewDidFail: " + errorCode);
	}
}
