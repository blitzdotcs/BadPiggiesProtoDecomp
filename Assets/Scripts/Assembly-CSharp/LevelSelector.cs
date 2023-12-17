using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : WPFMonoBehaviour
{
	public GameObject m_levelButtonPrefab;

	public GameObject m_levelJokerButtonPrefab;

	public int m_levelsPerPage = 15;

	[SerializeField]
	private int m_episodeLevelsGameDataIndex;

	[SerializeField]
	private Transform m_startingCutsceneButton;

	[SerializeField]
	private Transform m_endingCutsceneButton;

	[SerializeField]
	private GameObject m_leftScroll;

	[SerializeField]
	private GameObject m_rightScroll;

	[SerializeField]
	private GameObject m_pageDot;

	private float m_buttonOffset;

	private List<string> m_levels = new List<string>();

	private List<PageDot> m_dotsList = new List<PageDot>();

	private int m_page;

	private int m_pageCount;

	private Vector2 m_initialInputPos;

	private Vector2 m_lastInputPos;

	private ButtonGrid m_buttonGrid;

	private float m_leftDragLimit;

	private float m_rightDragLimit;

	private bool m_interacting;

	private int m_currentScreenWidth;

	private Camera m_hudCamera;

	private bool m_isIapOpen;

	public List<string> Levels
	{
		get
		{
			return m_levels;
		}
		set
		{
			m_levels = value;
		}
	}

	public string OpeningCutscene
	{
		get
		{
			return m_startingCutsceneButton.GetComponent<Button>().MessageParameter;
		}
	}

	public string EndingCutscene
	{
		get
		{
			return m_endingCutsceneButton.GetComponent<Button>().MessageParameter;
		}
	}

	private int CurrentPage
	{
		get
		{
			return Mathf.Clamp(Mathf.RoundToInt(m_buttonGrid.transform.localPosition.x / (0f - m_hudCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f)).x)), 0, m_pageCount);
		}
	}

	private void OnEnable()
	{
		IapManager.onPurchaseSucceeded += HandleIapManageronPurchaseSucceeded;
		InGameInAppPurchaseMenu.onViewDismissed += HandleOnViewDismissed;
	}

	private void HandleKeyListenerkeyPressed(KeyCode obj)
	{
		if (obj == KeyCode.Escape)
		{
			GoToEpisodeSelection();
		}
	}

	private void OnDisable()
	{
		IapManager.onPurchaseSucceeded -= HandleIapManageronPurchaseSucceeded;
		InGameInAppPurchaseMenu.onViewDismissed -= HandleOnViewDismissed;
		if (DeviceInfo.Instance.ActiveDeviceFamily != 0)
		{
			KeyListener.keyPressed -= HandleKeyListenerkeyPressed;
		}
	}

	private void Start()
	{
		if (DeviceInfo.Instance.ActiveDeviceFamily != 0)
		{
			KeyListener.keyPressed += HandleKeyListenerkeyPressed;
		}
		m_hudCamera = GameObject.FindGameObjectWithTag("HUDCamera").GetComponent<Camera>();
		m_buttonOffset = Mathf.Clamp((float)Screen.width / 7f, (float)(80 * Screen.width) / 1024f, (float)(160 * Screen.height) / 768f);
		Levels = WPFMonoBehaviour.gameData.m_episodeLevels[m_episodeLevelsGameDataIndex].Levels;
		m_page = UserSettings.GetInt(Application.loadedLevelName + "_active_page");
		m_pageCount = Mathf.RoundToInt(m_levels.Count / m_levelsPerPage);
		m_buttonGrid = base.transform.Find("ButtonGrid").GetComponent<ButtonGrid>();
		m_currentScreenWidth = Screen.width;
		GameManager.Instance.OpenEpisode(this);
		CreateButtons();
		CreatePageDots();
		if (BuildCustomizationLoader.Instance.AdsEnabled && BurstlyManager.Instance.BannerAdReady && !BurstlyManager.Instance.BannerAdShown)
		{
			BurstlyManager.Instance.ShowBanner(BurstlyManager.AdType.Banner);
		}
		if (DeviceInfo.Instance.UsesTouchInput)
		{
			m_leftScroll.active = false;
			m_rightScroll.active = false;
		}
	}

	public void SendExitLevelSelectionFlurryEvent()
	{
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ID", Application.loadedLevelName);
			FlurryManager.Instance.LogEventWithParameters("Quit Level Selection", dictionary);
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

	public void GoToEpisodeSelection()
	{
		Loader.Instance.LoadLevel("EpisodeSelection", false);
	}

	public void LoadLevel(string levelIndex)
	{
		SendStandardFlurryEvent("Select Level", levelIndex);
		int index = int.Parse(levelIndex);
		GameManager.Instance.LoadLevel(index);
		if (BuildCustomizationLoader.Instance.AdsEnabled)
		{
			BurstlyManager.Instance.HideBanner(BurstlyManager.AdType.Banner);
		}
	}

	public void LoadOpeningCutscene(string cutscene)
	{
		GameManager.Instance.LoadOpeningCutscene();
	}

	public void LoadEndingCutscene(string cutscene)
	{
		GameManager.Instance.LoadEndingCutscene();
	}

	public void NextPage()
	{
		m_page = Mathf.Clamp(m_page + 1, 0, m_pageCount);
	}

	public void PreviousPage()
	{
		m_page = Mathf.Clamp(m_page - 1, 0, m_pageCount);
	}

	private void CreateButtons()
	{
		bool @bool = GameProgress.GetBool("UnlockAllLevels");
		bool isFreeVersion = BuildCustomizationLoader.Instance.IsFreeVersion;
		m_buttonGrid.Clear();
		int num = 0;
		int count = m_levels.Count;
		int num2 = 0;
		int num3 = 0;
		Vector2 vector = new Vector2(m_buttonOffset, (float)Screen.height * 0.22f);
		Vector2 vector2 = new Vector2(((float)Screen.width - m_buttonOffset * (float)(m_buttonGrid.horizontalCount - 1)) / 2f, (float)Screen.height * 0.75f);
		m_rightDragLimit = 0f - m_hudCamera.ScreenToWorldPoint(new Vector3(Screen.width * m_pageCount, 0f, 0f)).x;
		m_leftDragLimit = 0f - m_hudCamera.ScreenToWorldPoint(new Vector3(Screen.width / 4, 0f, 0f)).x;
		for (int i = num; i < count; i++)
		{
			int num4 = i / m_levelsPerPage;
			bool flag = (((i + 1) % 5 == 0) ? true : false);
			bool flag2 = !GameProgress.IsLevelCompleted(Levels[i]);
			Button button = ((!flag) ? ((GameObject)Object.Instantiate(m_levelButtonPrefab)).GetComponent<Button>() : ((GameObject)Object.Instantiate(m_levelJokerButtonPrefab)).GetComponent<Button>());
			button.transform.parent = m_buttonGrid.transform;
			button.name = "LevelButton";
			Vector3 position = new Vector3(vector2.x + (float)(i % 5) * vector.x + (float)(Screen.width * num4), vector2.y - (float)(i % m_levelsPerPage / 5) * vector.y, 10f);
			button.transform.position = m_hudCamera.ScreenToWorldPoint(position);
			m_buttonGrid.AddButton(button);
			if (isFreeVersion && num4 + 1 == m_pageCount)
			{
				LockLevel(button, flag);
				continue;
			}
			if (flag)
			{
				if (i == num2)
				{
					num2++;
				}
				if (num3 >= 12 || @bool)
				{
					UnlockLevel(button, i, true);
				}
				else
				{
					LockLevel(button, true);
					SetJokerStars(button, num3);
				}
				num3 = 0;
			}
			else
			{
				if (i <= num2 || i == 0)
				{
					UnlockLevel(button, i, false);
					num3 += GameProgress.GetInt(Levels[i] + "_stars");
				}
				else
				{
					LockLevel(button, false);
				}
				if (!flag2 || @bool)
				{
					num2++;
				}
			}
			if (i == 0 && GameProgress.GetInt(OpeningCutscene + "_played") == 0)
			{
				button.MethodToInvoke = "LoadOpeningCutscene";
				button.MessageParameter = null;
			}
		}
		m_startingCutsceneButton.gameObject.active = GameProgress.GetInt(m_startingCutsceneButton.GetComponent<Button>().MessageParameter + "_played") == 1;
		m_endingCutsceneButton.gameObject.active = GameProgress.GetInt(m_endingCutsceneButton.GetComponent<Button>().MessageParameter + "_played") == 1;
		if (@bool || !BuildCustomizationLoader.Instance.IAPEnabled || isFreeVersion)
		{
			base.transform.Find("UnlockAllLevelsButton").gameObject.active = false;
		}
		Vector3 position2 = new Vector3(0f - m_hudCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2 + Screen.width * m_page, 0f, 0f)).x, m_buttonGrid.transform.localPosition.y, m_buttonGrid.transform.localPosition.z);
		m_buttonGrid.transform.position = position2;
	}

	private void CreatePageDots()
	{
		Vector3 position = -Vector3.up * m_hudCamera.orthographicSize / 1.25f;
		GameObject gameObject = Object.Instantiate(new GameObject(), position, Quaternion.identity) as GameObject;
		gameObject.name = "PageDots";
		gameObject.transform.parent = base.transform;
		float num = (0f - (float)m_pageCount) / 2f * 1.2f;
		for (int i = 0; i < m_pageCount; i++)
		{
			GameObject gameObject2 = Object.Instantiate(m_pageDot) as GameObject;
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = new Vector3(num + (float)i * 1.2f, 0f, 0f);
			gameObject2.name = "Dot" + i + 1;
			PageDot component = gameObject2.GetComponent<PageDot>();
			m_dotsList.Add(component);
		}
	}

	private void LayoutButtons()
	{
		m_buttonOffset = Mathf.Clamp((float)Screen.width / 7f, (float)(80 * Screen.width) / 1024f, (float)(160 * Screen.height) / 768f);
		Vector2 vector = new Vector2(m_buttonOffset, (float)Screen.height * 0.22f);
		Vector2 vector2 = new Vector2(((float)Screen.width - m_buttonOffset * (float)(m_buttonGrid.horizontalCount - 1)) / 2f, (float)Screen.height * 0.75f);
		m_rightDragLimit = 0f - m_hudCamera.ScreenToWorldPoint(new Vector3(Screen.width * m_pageCount, 0f, 0f)).x;
		m_leftDragLimit = 0f - m_hudCamera.ScreenToWorldPoint(new Vector3(Screen.width / 4, 0f, 0f)).x;
		m_buttonGrid.transform.position = Vector3.zero;
		m_page = 0;
		for (int num = m_buttonGrid.transform.childCount - 1; num >= 0; num--)
		{
			int num2 = num / m_levelsPerPage;
			Button component = m_buttonGrid.transform.GetChild(num).GetComponent<Button>();
			Vector3 position = new Vector3(vector2.x + (float)(num % 5) * vector.x + (float)(Screen.width * num2), vector2.y - (float)(num % m_levelsPerPage / 5) * vector.y, 10f);
			component.transform.position = m_hudCamera.ScreenToWorldPoint(position);
		}
	}

	private void Update()
	{
		if (m_currentScreenWidth != Screen.width)
		{
			LayoutButtons();
			m_currentScreenWidth = Screen.width;
		}
		if (!m_interacting)
		{
			Vector3 vector = new Vector3(0f - m_hudCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2 + Screen.width * m_page, 0f, 0f)).x, m_buttonGrid.transform.localPosition.y, m_buttonGrid.transform.localPosition.z);
			m_buttonGrid.transform.position += (vector - m_buttonGrid.transform.position) * Time.deltaTime * 4f;
			float magnitude = (vector - m_buttonGrid.transform.position).magnitude;
			if (magnitude < 1f)
			{
				UserSettings.SetInt(Application.loadedLevelName + "_active_page", m_page);
				if (!DeviceInfo.Instance.UsesTouchInput)
				{
					m_rightScroll.active = true;
					m_leftScroll.active = true;
				}
				for (int i = 0; i < m_dotsList.Count; i++)
				{
					if (i == m_page)
					{
						m_dotsList[i].Enable();
					}
					else
					{
						m_dotsList[i].Disable();
					}
				}
				if (CurrentPage == m_pageCount && BuildCustomizationLoader.Instance.IsFreeVersion && !m_isIapOpen)
				{
					OpenUnlockFullVersionPurchasePage();
				}
			}
			else if (!DeviceInfo.Instance.UsesTouchInput)
			{
				m_rightScroll.active = false;
				m_leftScroll.active = false;
			}
			if (!DeviceInfo.Instance.UsesTouchInput)
			{
				if (CurrentPage == 0)
				{
					m_leftScroll.active = false;
				}
				if (CurrentPage == m_pageCount)
				{
					m_rightScroll.active = false;
				}
			}
		}
		if (m_isIapOpen)
		{
			return;
		}
		if (DeviceInfo.Instance.UsesTouchInput)
		{
			if (Input.touchCount > 0)
			{
				if (Input.touches[0].phase == TouchPhase.Began)
				{
					m_initialInputPos = Input.touches[0].position;
					if (isInInteractiveArea(m_initialInputPos))
					{
						m_interacting = true;
					}
				}
				if (Input.touches[0].phase == TouchPhase.Moved)
				{
					float num = Input.touches[0].deltaPosition.x * Time.deltaTime;
					m_buttonGrid.transform.localPosition = new Vector3(Mathf.Clamp(m_buttonGrid.transform.localPosition.x + num, m_rightDragLimit, m_leftDragLimit), m_buttonGrid.transform.localPosition.y, m_buttonGrid.transform.localPosition.z);
				}
				if (Input.touches[0].phase == TouchPhase.Ended)
				{
					float num2 = Input.touches[0].position.x - m_initialInputPos.x;
					float num3 = Input.touches[0].position.x - m_lastInputPos.x;
					if ((num2 < (float)(-Screen.width / 8) && !(num2 > (float)(-Screen.width / 2))) || num3 < -50f)
					{
						m_page++;
					}
					else if ((num2 > (float)(Screen.width / 8) && !(num2 < (float)(Screen.width / 2))) || num3 > 50f)
					{
						m_page--;
					}
					m_page = Mathf.Clamp(m_page, 0, m_pageCount - 1);
					m_interacting = false;
				}
				m_lastInputPos = Input.touches[0].position;
			}
		}
		else
		{
			if (Input.GetMouseButtonDown(0))
			{
				m_initialInputPos = Input.mousePosition;
				if (isInInteractiveArea(m_initialInputPos))
				{
					m_interacting = true;
				}
			}
			if (Input.GetMouseButton(0) && m_interacting)
			{
				float num4 = (Input.mousePosition.x - m_lastInputPos.x) * Time.deltaTime * 2f;
				m_buttonGrid.transform.localPosition = new Vector3(Mathf.Clamp(m_buttonGrid.transform.localPosition.x + num4, m_rightDragLimit, m_leftDragLimit), m_buttonGrid.transform.localPosition.y, m_buttonGrid.transform.localPosition.z);
				float f = Input.mousePosition.x - m_initialInputPos.x;
				if (Mathf.Abs(f) > 1f)
				{
					m_rightScroll.active = false;
					m_leftScroll.active = false;
				}
			}
			if (Input.GetMouseButtonUp(0))
			{
				float num5 = Input.mousePosition.x - m_initialInputPos.x;
				float num6 = Input.mousePosition.x - m_lastInputPos.x;
				if ((num5 < (float)(-Screen.width / 8) && !(num5 > (float)(-Screen.width / 2))) || num6 < -50f)
				{
					m_page++;
				}
				else if ((num5 > (float)(Screen.width / 8) && !(num5 < (float)(Screen.width / 2))) || num6 > 50f)
				{
					m_page--;
				}
				m_page = Mathf.Clamp(m_page, 0, m_pageCount - 1);
				m_interacting = false;
			}
			m_lastInputPos = Input.mousePosition;
		}
		if (m_startingCutsceneButton.gameObject.active)
		{
			m_startingCutsceneButton.position = m_buttonGrid.transform.GetChild(0).position - Vector3.right * 4f;
		}
		if (m_endingCutsceneButton.gameObject.active)
		{
			m_endingCutsceneButton.position = m_buttonGrid.transform.GetChild(m_buttonGrid.transform.childCount - 1).position + Vector3.right * 4f;
		}
	}

	private bool isInInteractiveArea(Vector2 touchPos)
	{
		return touchPos.y > (float)Screen.height * 0.1f && touchPos.y < (float)Screen.height * 0.8f;
	}

	private void UnlockLevel(Button button, int index, bool isJoker)
	{
		button.transform.Find("LevelNumber").GetComponent<TextMesh>().text = (index + 1).ToString();
		button.transform.Find("Lock").gameObject.active = false;
		button.MessageTargetObject = base.gameObject;
		button.TargetComponent = "LevelSelector";
		button.MethodToInvoke = "LoadLevel";
		button.MessageParameter = index.ToString();
		int @int = GameProgress.GetInt(Levels[index] + "_stars");
		GameObject gameObject = button.transform.Find("StarSets/Star1").gameObject;
		GameObject gameObject2 = button.transform.Find("StarSets/Star2").gameObject;
		GameObject gameObject3 = button.transform.Find("StarSets/Star3").gameObject;
		switch (@int)
		{
		case 1:
			gameObject2.SetActiveRecursively(false);
			gameObject3.SetActiveRecursively(false);
			break;
		case 2:
			gameObject.SetActiveRecursively(false);
			gameObject3.SetActiveRecursively(false);
			break;
		case 3:
			gameObject.SetActiveRecursively(false);
			gameObject2.SetActiveRecursively(false);
			break;
		default:
			gameObject.SetActiveRecursively(false);
			gameObject2.SetActiveRecursively(false);
			gameObject3.SetActiveRecursively(false);
			break;
		}
		if (isJoker)
		{
			GameObject gameObject4 = button.transform.Find("StarSetsLocked").gameObject;
			gameObject4.SetActiveRecursively(false);
		}
	}

	private void LockLevel(Button button, bool isJoker)
	{
		button.transform.Find("LevelNumber").gameObject.active = false;
		GameObject gameObject = button.transform.Find("StarSets").gameObject;
		gameObject.SetActiveRecursively(false);
	}

	private void SetJokerStars(Button jokerButton, int stars)
	{
		TextMesh component = jokerButton.transform.Find("StarSetsLocked/StarsCollected").GetComponent<TextMesh>();
		component.text = string.Empty + stars + "/12";
	}

	public void OpenUnlockAllLevelsPurchasePage()
	{
		if (BuildCustomizationLoader.Instance.IAPEnabled)
		{
			m_isIapOpen = true;
			IapManager.Instance.EnableUnlockAllLevelsPurchasePage(true);
		}
	}

	public void OpenUnlockFullVersionPurchasePage()
	{
		if (BuildCustomizationLoader.Instance.IAPEnabled)
		{
			m_isIapOpen = true;
			IapManager.Instance.EnableUnlockFullVersionPurchasePage();
		}
	}

	private void HandleIapManageronPurchaseSucceeded(IapManager.InAppPurchaseItemType type)
	{
		Loader.Instance.LoadLevel(Application.loadedLevelName, false);
	}

	private void HandleOnViewDismissed(string viewName)
	{
		m_isIapOpen = false;
		if (viewName.Contains("InGameInAppPurchaseMenuUnlockFullVersion") && m_buttonGrid != null)
		{
			m_buttonGrid.transform.position += Vector3.right * 3f;
			m_page = 1;
		}
	}
}
