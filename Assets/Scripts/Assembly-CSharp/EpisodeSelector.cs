using System.Collections.Generic;
using UnityEngine;

public class EpisodeSelector : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> m_episodes = new List<GameObject>();

	private List<Transform> m_episodesLayoutList = new List<Transform>();

	private int m_screenWidth;

	private int m_screenHeight;

	private Camera m_hudCamera;

	private Vector3 m_initialInputPos;

	private Vector3 m_lastInputPos;

	private int CurrentEpisode
	{
		get
		{
			return Mathf.Clamp(Mathf.FloorToInt(base.transform.localPosition.x / (0f - m_hudCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f)).x)), 0, m_episodesLayoutList.Count);
		}
	}

	private void Awake()
	{
		if (DeviceInfo.Instance.ActiveDeviceFamily != 0)
		{
			KeyListener.keyPressed += HandleKeyListenerkeyPressed;
		}
		GameManager.Instance.CreateMenuBackground();
		m_hudCamera = GameObject.FindGameObjectWithTag("HUDCamera").GetComponent<Camera>();
		foreach (GameObject episode in m_episodes)
		{
			GameObject gameObject = Object.Instantiate(episode, Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.transform.parent = base.transform;
			m_episodesLayoutList.Add(gameObject.transform);
		}
		m_screenWidth = Screen.width;
		m_screenHeight = Screen.height;
		Layout();
		if (BuildCustomizationLoader.Instance.AdsEnabled && BurstlyManager.Instance.BannerAdReady && !BurstlyManager.Instance.BannerAdShown)
		{
			BurstlyManager.Instance.ShowBanner(BurstlyManager.AdType.Banner);
		}
	}

	private void OnDisable()
	{
		if (DeviceInfo.Instance.ActiveDeviceFamily != 0)
		{
			KeyListener.keyPressed -= HandleKeyListenerkeyPressed;
		}
	}

	private void Layout()
	{
		float num = m_hudCamera.orthographicSize * (float)m_screenWidth / (float)m_screenHeight;
		float num2 = Mathf.Clamp(num / (float)Mathf.Clamp(m_episodesLayoutList.Count - 1, 0, 5), 6f, 9f);
		Vector3 vector = ((m_episodesLayoutList.Count % 2 != 0) ? new Vector3((float)(-m_episodesLayoutList.Count / 2) * num2, 0f, 0f) : new Vector3((float)(-m_episodesLayoutList.Count / 2) * num2 + num2 / 2f, 0f, 0f));
		vector.x = Mathf.Clamp(vector.x, (0f - num) / 1.5f, 0f);
		for (int i = 0; i < m_episodesLayoutList.Count; i++)
		{
			m_episodesLayoutList[i].position = vector + num2 * Vector3.right * i;
		}
	}

	private void Update()
	{
		if (m_screenWidth != Screen.width)
		{
			Layout();
			m_screenWidth = Screen.width;
		}
	}

	private bool isInInteractiveArea(Vector2 touchPos)
	{
		return touchPos.y > (float)Screen.height * 0.1f && touchPos.y < (float)Screen.height * 0.8f;
	}

	public void GoToMainMenu()
	{
		SendExitEpisodeSelectionFlurryEvent();
		Loader.Instance.LoadLevel("MainMenu", false);
	}

	private void HandleKeyListenerkeyPressed(KeyCode obj)
	{
		if (obj == KeyCode.Escape)
		{
			GoToMainMenu();
		}
	}

	public void SendExitEpisodeSelectionFlurryEvent()
	{
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			FlurryManager.Instance.LogEvent("Quit Episode Selection");
		}
	}
}
