using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : WPFMonoBehaviour
{
	public enum GameState
	{
		Undefined = 0,
		Building = 1,
		Preview = 2,
		PreviewMoving = 3,
		PreviewWhileBuilding = 4,
		PreviewWhileRunning = 5,
		Running = 6,
		Continue = 7,
		Completed = 8,
		PausedWhileRunning = 9,
		PausedWhileBuilding = 10,
		IngamePurchase = 11,
		AutoBuilding = 12
	}

	[Serializable]
	public class CameraLimits
	{
		public Vector2 topLeft;

		public Vector2 size;
	}

	[Serializable]
	public class ScoreLimits
	{
		public int silverScore;

		public int goldScore;
	}

	[Serializable]
	public class PartCount
	{
		public BasePart.PartType type;

		public int count;
	}

	public GameObject m_inGameGuiPrefab;

	public Vector3 m_cameraOffset = new Vector3(0f, 0f, -10f);

	public float m_zoomOffset = -2f;

	public float m_predictionOffset = 2f;

	[HideInInspector]
	public float m_cameraMaxZoom;

	[HideInInspector]
	public float m_cameraMinZoom;

	public CameraLimits m_cameraLimits;

	public Vector3 m_constructionOffset = new Vector3(0f, 0f, -7f);

	public Vector3 m_previewOffset = new Vector3(0f, 0f, -10f);

	public float m_previewZoomOut = 15f;

	public float m_previewMoveTime = 5f;

	public float m_previewWaitTime = 5f;

	public Texture2D m_blueprintTexture;

	public ScoreLimits m_scoreLimits;

	private bool m_requireConnectedContraption;

	[HideInInspector]
	public int m_constructionUiRows = 1;

	[HideInInspector]
	public List<int> m_constructionGridRows = new List<int>();

	[HideInInspector]
	public List<int> m_secondStarConstructionGridRows = new List<int>();

	[HideInInspector]
	public List<int> m_currentConstructionGridRows;

	[HideInInspector]
	public List<PartCount> m_partTypeCounts = new List<PartCount>();

	public float m_tutorialDelayBeforeHint = 3f;

	public float m_tutorialDelayAfterHint = 1.5f;

	public GameData m_gameData;

	protected GameState m_gameState;

	protected float m_timeStateChanged;

	protected Contraption m_contraptionProto;

	protected Contraption m_contraptionRunning;

	protected ConstructionUI m_constructionUI;

	protected Transform m_collectibleBackup;

	protected int m_maxFood;

	protected int m_maxGoal;

	protected int m_foodCollected;

	protected int m_eggsCollected;

	protected int m_starCollected;

	protected float m_timeElapsed;

	protected float m_completionTime;

	private int m_totalScore;

	private AudioManager audioManager;

	[HideInInspector]
	public bool m_newHighscore;

	protected int m_gridHeight;

	protected int m_gridWidth;

	protected int m_gridXmin;

	protected int m_gridXmax;

	protected float m_previewAlpha;

	public GUIStyle m_buttonStyle;

	public float m_previewSpeed;

	public float m_previewTime;

	protected Vector3 m_previewCenter;

	public bool m_previewDragging;

	public Vector2 m_previewLastMousePos;

	protected float m_lastTimePlayedCollisionSound;

	public TextAsset m_prebuiltContraption;

	public TextAsset m_oneStarContraption;

	public List<PartCount> m_partsToUnlockOnCompletion = new List<PartCount>();

	public bool m_showOnlyEngineButton;

	public bool m_disablePigCollisions;

	public List<PartCount> m_extraPartsForSecondStar = new List<PartCount>();

	public bool m_sandbox;

	public string unlockSandboxLevelOnCompletion = string.Empty;

	[HideInInspector]
	public bool m_gridForSecondStar;

	public static string kDataPath;

	protected LevelStart m_levelStart;

	[HideInInspector]
	public int m_totalAvailableParts;

	[HideInInspector]
	public int m_totalDestroyedParts;

	public bool m_autoBuildUnlocked;

	private GameObject m_inGameGui;

	private List<Challenge> m_challenges = new List<Challenge>();

	private float m_timeLimit;

	private List<Goal> m_goals = new List<Goal>();

	private bool m_useSecondStarParts;

	private float m_autoBuildTimer;

	private int m_autoBuildIndex;

	private ContraptionDataset m_autoBuildData;

	public GameState gameState
	{
		get
		{
			return m_gameState;
		}
	}

	public bool RequireConnectedContraption
	{
		get
		{
			return m_requireConnectedContraption;
		}
	}

	public Contraption contraptionProto
	{
		get
		{
			return m_contraptionProto;
		}
	}

	public Contraption contraptionRunning
	{
		get
		{
			return m_contraptionRunning;
		}
	}

	public ConstructionUI constructionUI
	{
		get
		{
			return m_constructionUI;
		}
	}

	public float TimeLimit
	{
		get
		{
			return m_timeLimit;
		}
	}

	public int EggsCollected
	{
		get
		{
			return m_eggsCollected;
		}
	}

	public int TotalScore
	{
		get
		{
			return m_totalScore;
		}
	}

	public float TimeElapsed
	{
		get
		{
			return m_timeElapsed;
		}
		set
		{
			m_timeElapsed = value;
		}
	}

	public float CompletionTime
	{
		get
		{
			return m_completionTime;
		}
	}

	public int FoodCollected
	{
		get
		{
			return m_foodCollected;
		}
	}

	public int gridHeight
	{
		get
		{
			return m_gridHeight;
		}
	}

	public int gridWidth
	{
		get
		{
			return m_gridWidth;
		}
	}

	public int gridXmin
	{
		get
		{
			return m_gridXmin;
		}
	}

	public int gridXmax
	{
		get
		{
			return m_gridXmax;
		}
	}

	public float previewAlpha
	{
		get
		{
			return m_previewAlpha;
		}
	}

	public Vector3 previewCenter
	{
		get
		{
			return m_previewCenter;
		}
		set
		{
			m_previewCenter = value;
		}
	}

	public LevelStart StartingPosition
	{
		get
		{
			return m_levelStart;
		}
	}

	public Transform GoalPosition
	{
		get
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("Goal");
			if ((bool)gameObject)
			{
				return gameObject.transform;
			}
			return null;
		}
	}

	public bool IsPartUsed(BasePart.PartType type)
	{
		return m_contraptionProto.HasPart(type);
	}

	private void Awake()
	{
		if (!SingletonSpawner.SpawnDone)
		{
			UnityEngine.Object.Instantiate(m_gameData.singletonSpawner);
		}
		UnityEngine.Object.Instantiate(m_gameData.effectManager);
		Debug.Log("Current level: " + Application.loadedLevelName);
		if ((bool)m_inGameGuiPrefab)
		{
			m_inGameGui = (GameObject)UnityEngine.Object.Instantiate(m_inGameGuiPrefab);
			m_inGameGui.name = m_inGameGuiPrefab.name;
			GameObject gameObject = GameObject.FindWithTag("HUDCamera");
			Vector3 position = gameObject.transform.position;
			m_inGameGui.transform.position = new Vector3(position.x, position.y, m_inGameGui.transform.position.z);
		}
		WPFMonoBehaviour.gameData.m_useTouchControls = ((Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) ? true : false);
		m_levelStart = WPFMonoBehaviour.FindSceneObjectOfType<LevelStart>();
		kDataPath = DeviceInfo.Instance.PersistentDataPath();
		m_gridWidth = 1;
		m_useSecondStarParts = GameProgress.IsLevelCompleted(Application.loadedLevelName);
		if (m_useSecondStarParts && m_gridForSecondStar)
		{
			m_currentConstructionGridRows = m_secondStarConstructionGridRows;
		}
		else
		{
			m_currentConstructionGridRows = m_constructionGridRows;
		}
		for (int i = 0; i < m_currentConstructionGridRows.Count; i++)
		{
			if (m_currentConstructionGridRows[i] != 0)
			{
				int numberOfHighestBit = WPFMonoBehaviour.GetNumberOfHighestBit(m_currentConstructionGridRows[i]);
				if (numberOfHighestBit + 1 > m_gridWidth)
				{
					m_gridWidth = numberOfHighestBit + 1;
				}
				m_gridHeight = i + 1;
			}
		}
		m_gridXmin = -(m_gridWidth - 1) / 2;
		m_gridXmax = m_gridWidth / 2;
		Vector3 position2 = ((!m_levelStart) ? Vector3.zero : m_levelStart.transform.position);
		if ((bool)WPFMonoBehaviour.gameData.m_contraptionPrefab)
		{
			Transform transform = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_contraptionPrefab, position2, Quaternion.identity) as Transform;
			m_contraptionProto = transform.GetComponent<Contraption>();
		}
		if ((bool)WPFMonoBehaviour.gameData.m_hudPrefab)
		{
			Transform transform2 = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_hudPrefab, position2, Quaternion.identity) as Transform;
			transform2.parent = base.transform;
		}
		if ((bool)WPFMonoBehaviour.gameData.m_constructionUIPrefab)
		{
			Transform transform3 = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_constructionUIPrefab) as Transform;
			transform3.gameObject.name = WPFMonoBehaviour.gameData.m_constructionUIPrefab.name;
			if ((bool)transform3)
			{
				m_constructionUI = transform3.GetComponent<ConstructionUI>();
				transform3.position = position2;
			}
			bool flag = false;
			ContraptionDataset cds;
			if (!m_prebuiltContraption)
			{
				cds = ((GameProgress.GetInt(Application.loadedLevelName + "_contraption") == 0) ? new ContraptionDataset() : WPFPrefs.LoadContraptionDataset(Application.loadedLevelName));
			}
			else
			{
				cds = WPFPrefs.LoadContraptionDataset(m_prebuiltContraption);
				flag = true;
			}
			if (flag)
			{
				PreBuildContraption(cds);
			}
			else
			{
				BuildContraption(cds);
			}
		}
		foreach (ConstructionUI.PartDesc partDescriptor in m_constructionUI.PartDescriptors)
		{
			EventManager.Send(new PartCountChanged(partDescriptor.part.m_partType, partDescriptor.CurrentCount));
		}
		m_constructionUI.SetMoveButtonStates();
		if (!m_contraptionProto)
		{
			m_contraptionProto = WPFMonoBehaviour.FindSceneObjectOfType<Contraption>();
		}
		GameObject gameObject2 = new GameObject("CollectibleStash");
		gameObject2.transform.parent = base.transform;
		FindChallenges();
		CheckStarBoxes();
		Goal[] array = UnityEngine.Object.FindSceneObjectsOfType(typeof(Goal)) as Goal[];
		int num = 1;
		Goal[] array2 = array;
		foreach (Goal goal in array2)
		{
			goal.transform.parent = null;
			goal.GoalId = num;
			num++;
			Transform transform4 = UnityEngine.Object.Instantiate(goal.transform) as Transform;
			transform4.parent = gameObject2.transform;
			transform4.gameObject.SetActiveRecursively(false);
		}
		m_maxGoal = array.Length;
		m_collectibleBackup = gameObject2.transform;
		InitializeChallenges();
		SetGameState(GameState.Preview);
		if (base.GetComponent<AudioSource>() == null)
		{
			base.gameObject.AddComponent<AudioSource>();
		}
		string value = GameObject.FindGameObjectWithTag("World").name;
		GameProgress.SetString("MenuBackground", value);
		if (BuildCustomizationLoader.Instance.AdsEnabled && BurstlyManager.Instance.InterstitialAdReady)
		{
			BurstlyManager.Instance.ShowBanner(BurstlyManager.AdType.Interstitial);
		}
		SendStartLevelFlurryEvent();
		CheckForLevelStartAchievements();
		m_autoBuildUnlocked = ((m_oneStarContraption != null) ? true : false);
		if (DeviceInfo.Instance.ActiveDeviceFamily != 0)
		{
			KeyListener.keyPressed += HandleKeyListenerkeyPressed;
		}
	}

	private void HandleKeyListenerkeyPressed(KeyCode obj)
	{
		switch (obj)
		{
		case KeyCode.Escape:
			if (gameState != GameState.PausedWhileBuilding && gameState != GameState.PausedWhileRunning)
			{
				EventManager.Send(new UIEvent(UIEvent.Type.Pause));
			}
			else
			{
				EventManager.Send(new UIEvent(UIEvent.Type.LevelSelection));
			}
			break;
		case KeyCode.Menu:
			if (gameState != GameState.PausedWhileBuilding && gameState != GameState.PausedWhileRunning)
			{
				EventManager.Send(new UIEvent(UIEvent.Type.Pause));
			}
			break;
		}
	}

	private void BuildContraption(ContraptionDataset cds)
	{
		foreach (ContraptionDataset.ContraptionDatasetUnit contraptionDataset in cds.ContraptionDatasetList)
		{
			ConstructionUI.PartDesc partDesc = m_constructionUI.FindPartDesc((BasePart.PartType)contraptionDataset.partType);
			if (partDesc != null)
			{
				BuildPart(contraptionDataset, partDesc.part);
				partDesc.useCount++;
			}
		}
	}

	private void PreBuildContraption(ContraptionDataset cds)
	{
		foreach (ContraptionDataset.ContraptionDatasetUnit contraptionDataset in cds.ContraptionDatasetList)
		{
			GameObject part = WPFMonoBehaviour.gameData.GetPart((BasePart.PartType)contraptionDataset.partType);
			if ((bool)part)
			{
				BasePart component = part.GetComponent<BasePart>();
				BasePart basePart = BuildPart(contraptionDataset, component);
				basePart.m_static = true;
				contraptionProto.IncreaseStaticPartCount();
			}
		}
	}

	private BasePart BuildPart(ContraptionDataset.ContraptionDatasetUnit cdu, BasePart partPrefab)
	{
		BasePart basePart = m_constructionUI.SetPartAt(cdu.x, cdu.y, partPrefab, false);
		if (cdu.flipped)
		{
			basePart.SetFlipped(true);
		}
		else
		{
			basePart.SetRotation((BasePart.GridRotation)cdu.rot);
		}
		return basePart;
	}

	private void CheckStarBoxes()
	{
		List<StarBox> list = new List<StarBox>();
		GameObject gameObject = GameObject.Find("StarBoxes");
		if ((bool)gameObject)
		{
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				list.Add(gameObject.transform.GetChild(i).GetComponent<StarBox>());
			}
		}
		foreach (StarBox item in list)
		{
			foreach (StarBox item2 in list)
			{
				if (item != item2)
				{
					Assert.Check(item.name != item2.name, "StarBox objects must have unique names: " + item.name);
				}
			}
		}
	}

	private void FindChallenges()
	{
		GameObject gameObject = GameObject.Find("Challenges");
		if ((bool)gameObject)
		{
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				m_challenges.Add(gameObject.transform.GetChild(i).GetComponent<Challenge>());
			}
		}
		else
		{
			Debug.Log("Challenges not found");
		}
		foreach (Challenge challenge in m_challenges)
		{
			foreach (Challenge challenge2 in m_challenges)
			{
				if (challenge != challenge2)
				{
					Assert.Check(challenge.name != challenge2.name, "Challenge objects must have unique names: " + challenge.name);
				}
			}
		}
		m_challenges.Sort(new Challenge.ChallengeOrder());
	}

	private void InitializeChallenges()
	{
		for (int i = 0; i < m_challenges.Count; i++)
		{
			Challenge challenge = m_challenges[i];
			challenge.Initialize();
			if ((bool)challenge.GetComponent<WaypointChallenge>())
			{
				GameObject gameObject = new GameObject(challenge.name);
				WaypointChallenge waypointChallenge = gameObject.AddComponent<WaypointChallenge>();
				waypointChallenge.m_goalId = challenge.GetComponent<WaypointChallenge>().m_goalId;
				waypointChallenge.m_icons = challenge.m_icons;
				waypointChallenge.transform.parent = GameObject.Find("Challenges").transform;
				m_challenges[i] = waypointChallenge;
			}
			if (challenge.TimeLimit() > 0f && (m_timeLimit == 0f || challenge.TimeLimit() > m_timeLimit))
			{
				m_timeLimit = challenge.TimeLimit();
			}
		}
	}

	private void Start()
	{
		audioManager = AudioManager.Instance;
	}

	private void OnEnable()
	{
		EventManager.Connect<UIEvent>(ReceiveUIEvent);
		EventManager.Connect<GadgetControlEvent>(ReceiveGadgetControlEvent);
	}

	private void OnDisable()
	{
		EventManager.Disconnect<GadgetControlEvent>(ReceiveGadgetControlEvent);
		EventManager.Disconnect<UIEvent>(ReceiveUIEvent);
		if (DeviceInfo.Instance.ActiveDeviceFamily != 0)
		{
			KeyListener.keyPressed -= HandleKeyListenerkeyPressed;
		}
	}

	public void ReceiveGadgetControlEvent(GadgetControlEvent data)
	{
		contraptionRunning.ActivatePartType(data.partType, data.direction);
	}

	public void CheckForLevelStartAchievements()
	{
		if (DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios && contraptionProto.Parts.Count >= AchievementData.Instance.GetAchievementLimit("grp.COMPLEX_COMPLEX"))
		{
			SocialGameManager.Instance.ReportAchievementProgress("grp.COMPLEX_COMPLEX", 100.0);
		}
	}

	public void SendStartLevelFlurryEvent()
	{
		if (!BuildCustomizationLoader.Instance.Flurry)
		{
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("ID", Application.loadedLevelName);
		string text = "{";
		for (Challenge.ChallengeType challengeType = Challenge.ChallengeType.DontUseParts; challengeType < Challenge.ChallengeType.Max; challengeType++)
		{
			int @int = GameProgress.GetInt(Application.loadedLevelName + "_challenge_" + challengeType);
			if (@int > 0)
			{
				string text2 = text;
				text = string.Concat(text2, string.Empty, challengeType, ",");
			}
			text = "}";
		}
		if (Loader.Instance.LastLoadedString.CompareTo(Application.loadedLevelName) == 0)
		{
			dictionary.Add("SOURCE", "REPLAY");
		}
		else
		{
			dictionary.Add("SOURCE", "LEVEL_SELECTION");
		}
		FlurryManager.Instance.LogEventWithParameters("StartLevel", dictionary);
	}

	public void SendContraptionStartedFlurryEvent()
	{
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ID", Application.loadedLevelName);
			dictionary.Add("PARTS", string.Empty + m_contraptionProto.Parts.Count);
			dictionary.Add("MOVES", string.Empty + m_constructionUI.MoveCount);
			dictionary.Add("CONTRAPTION", m_contraptionProto.GetContraptionID());
			FlurryManager.Instance.LogEventWithParameters("StartContraption", dictionary);
		}
	}

	public void SendStandardFlurryEventWithTime(string eventName, string id)
	{
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ID", id);
			dictionary.Add("TIME", string.Empty + m_timeElapsed);
			FlurryManager.Instance.LogEventWithParameters(eventName, dictionary);
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

	public void SendClearContraptionFlurryEvent()
	{
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ID", Application.loadedLevelName);
			dictionary.Add("PARTS", string.Empty + m_contraptionProto.Parts.Count);
			FlurryManager.Instance.LogEventWithParameters("Pause Flight", dictionary);
		}
	}

	public void SendPauseWhileFlyingFlurryEvent()
	{
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ID", Application.loadedLevelName);
			dictionary.Add("TIME", string.Empty + m_timeElapsed);
			FlurryManager.Instance.LogEventWithParameters("Pause Flight", dictionary);
		}
	}

	public void SendNextLevelFlurryEvent()
	{
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ID", Application.loadedLevelName);
			string value = string.Empty;
			FlurryManager.Instance.FlurryDataHolder.TryGetValue("PREVIOUS_STARS", out value);
			dictionary.Add("PREVIOUS_STARS", value);
			FlurryManager.Instance.FlurryDataHolder.TryGetValue("PREVIOUS_STARS", out value);
			dictionary.Add("NEW_STARS", value);
			dictionary.Add("TIME", string.Empty + m_timeElapsed);
			FlurryManager.Instance.LogEventWithParameters("Next On Level End", dictionary);
		}
	}

	public void SendReplayFromCompleteFlurryEvent()
	{
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ID", Application.loadedLevelName);
			string value = string.Empty;
			FlurryManager.Instance.FlurryDataHolder.TryGetValue("PREVIOUS_STARS", out value);
			dictionary.Add("PREVIOUS_STARS", value);
			FlurryManager.Instance.FlurryDataHolder.TryGetValue("PREVIOUS_STARS", out value);
			dictionary.Add("NEW_STARS", value);
			dictionary.Add("TIME", string.Empty + m_timeElapsed);
			FlurryManager.Instance.LogEventWithParameters("Next On Level End", dictionary);
		}
	}

	public void SendExitFromCompleteFlurryEvent()
	{
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ID", Application.loadedLevelName);
			string value = string.Empty;
			FlurryManager.Instance.FlurryDataHolder.TryGetValue("PREVIOUS_STARS", out value);
			dictionary.Add("PREVIOUS_STARS", value);
			FlurryManager.Instance.FlurryDataHolder.TryGetValue("PREVIOUS_STARS", out value);
			dictionary.Add("NEW_STARS", value);
			dictionary.Add("TIME", string.Empty + m_timeElapsed);
			FlurryManager.Instance.LogEventWithParameters("Next On Level End", dictionary);
		}
	}

	public void ReceiveUIEvent(UIEvent data)
	{
		switch (data.type)
		{
		case UIEvent.Type.Play:
		{
			if (WPFMonoBehaviour.levelManager.gameState == GameState.Building)
			{
				SendContraptionStartedFlurryEvent();
			}
			GameState gameState = ((WPFMonoBehaviour.levelManager.gameState != GameState.Building) ? GameState.Continue : GameState.Running);
			if (WPFMonoBehaviour.levelManager.gameState == GameState.Building)
			{
				AudioManager.Instance.Play2dEffect(AudioManager.Instance.CommonAudioCollection.buildContraption);
			}
			SetGameState(gameState);
			break;
		}
		case UIEvent.Type.Clear:
			SendClearContraptionFlurryEvent();
			AudioManager.Instance.Play2dEffect(AudioManager.Instance.CommonAudioCollection.clearBuildGrid);
			constructionUI.ClearContraption();
			break;
		case UIEvent.Type.LevelSelection:
		{
			if (GameTime.IsPaused())
			{
				GameTime.Pause(false);
			}
			if (WPFMonoBehaviour.levelManager.gameState == GameState.PausedWhileRunning)
			{
				SendStandardFlurryEventWithTime("Exit From Flight Paused", Application.loadedLevelName);
			}
			else if (this.gameState == GameState.PausedWhileBuilding)
			{
				SendStandardFlurryEventWithTime("Exit From Build Paused", Application.loadedLevelName);
			}
			else if (this.gameState == GameState.Completed)
			{
				SendExitFromCompleteFlurryEvent();
			}
			string levelName = ((!(GameManager.Instance.CurrentEpisode != string.Empty)) ? "EpisodeSelection" : GameManager.Instance.CurrentEpisode);
			Loader.Instance.LoadLevel(levelName, true);
			break;
		}
		case UIEvent.Type.Preview:
		{
			GameState gameState2 = ((this.gameState != GameState.Running) ? GameState.PreviewWhileBuilding : GameState.PreviewWhileRunning);
			SetGameState(gameState2);
			break;
		}
		case UIEvent.Type.Building:
		{
			if (WPFMonoBehaviour.levelManager.gameState == GameState.PausedWhileRunning)
			{
				SendStandardFlurryEvent("Restart From Flight Paused", Application.loadedLevelName);
			}
			else if (this.gameState == GameState.PausedWhileBuilding)
			{
				SendStandardFlurryEvent("Exit From Build Paused", Application.loadedLevelName);
			}
			Vector3 position = ((!m_levelStart) ? Vector3.zero : m_levelStart.transform.position);
			m_constructionUI.transform.position = position;
			m_contraptionProto.transform.position = position;
			SetGameState(GameState.Building);
			break;
		}
		case UIEvent.Type.ActivateRockets:
		{
			Rocket[] componentsInChildren = contraptionRunning.GetComponentsInChildren<Rocket>();
			Rocket[] array = componentsInChildren;
			foreach (Rocket rocket in array)
			{
				rocket.ProcessTouch();
			}
			break;
		}
		case UIEvent.Type.ActivateEngines:
		{
			Engine[] componentsInChildren2 = contraptionRunning.GetComponentsInChildren<Engine>();
			if (componentsInChildren2.Length > 0)
			{
				componentsInChildren2[0].ProcessTouch();
			}
			else
			{
				contraptionRunning.m_pig.ProcessTouch();
			}
			break;
		}
		case UIEvent.Type.BackFromPreview:
		{
			GameState gameState3 = ((this.gameState != GameState.PreviewWhileRunning) ? GameState.Building : GameState.Continue);
			WPFMonoBehaviour.levelManager.SetGameState(gameState3);
			break;
		}
		case UIEvent.Type.Pause:
		{
			if (WPFMonoBehaviour.levelManager.gameState == GameState.Building)
			{
				SendStandardFlurryEvent("Pause Build", Application.loadedLevelName);
			}
			else if (this.gameState == GameState.Running)
			{
				SendPauseWhileFlyingFlurryEvent();
			}
			GameState gameState4 = ((this.gameState != GameState.Running) ? GameState.PausedWhileBuilding : GameState.PausedWhileRunning);
			WPFMonoBehaviour.levelManager.SetGameState(gameState4);
			break;
		}
		case UIEvent.Type.ContinueFromPause:
		{
			if (this.gameState == GameState.PausedWhileRunning)
			{
				SendStandardFlurryEvent("Continue From Flight Paused", Application.loadedLevelName);
			}
			else if (this.gameState == GameState.PausedWhileBuilding)
			{
				SendStandardFlurryEvent("Continue From Build Paused", Application.loadedLevelName);
			}
			GameState gameState5 = ((this.gameState != GameState.PausedWhileRunning) ? GameState.Building : GameState.Continue);
			WPFMonoBehaviour.levelManager.SetGameState(gameState5);
			break;
		}
		case UIEvent.Type.OpenIapMenu:
			WPFMonoBehaviour.levelManager.SetGameState(GameState.IngamePurchase);
			break;
		case UIEvent.Type.CloseIapMenu:
			WPFMonoBehaviour.levelManager.SetGameState(GameState.Building);
			break;
		case UIEvent.Type.ReplayLevel:
			if (this.gameState == GameState.Completed)
			{
				SendReplayFromCompleteFlurryEvent();
			}
			Loader.Instance.LoadLevel(Application.loadedLevelName, true);
			break;
		case UIEvent.Type.ReplayFlight:
			StopRunningContraption();
			SetGameState(GameState.Running);
			break;
		case UIEvent.Type.NextLevel:
			SendNextLevelFlurryEvent();
			GameManager.Instance.LoadNextLevel();
			break;
		case UIEvent.Type.QuestModeBuild:
			PlaceBuildArea();
			SetGameState(GameState.Building);
			break;
		case UIEvent.Type.Blueprint:
		{
			if (!m_oneStarContraption)
			{
				break;
			}
			int @int = GameProgress.GetInt("Blueprints_Available");
			if (@int == 0 && !GameProgress.GetBool(Application.loadedLevelName + "_autobuild_available"))
			{
				IapManager.Instance.EnableBlueprintsPurchasePage(true);
				break;
			}
			if (!GameProgress.GetBool(Application.loadedLevelName + "_autobuild_available"))
			{
				@int--;
				GameProgress.SetInt("Blueprints_Available", @int);
				GameProgress.SetBool(Application.loadedLevelName + "_autobuild_available", true);
				GameProgress.Save();
				EventManager.Send(new InGameBuildMenu.AutoBuildEvent(@int));
			}
			SetGameState(GameState.AutoBuilding);
			break;
		}
		case UIEvent.Type.Home:
		case UIEvent.Type.IapPurchaseCurrency:
		case UIEvent.Type.IapPurchaseRocket:
		case UIEvent.Type.IapPurchaseEngine:
			break;
		}
	}

	private void PlaceBuildArea()
	{
		float x = m_contraptionProto.FindPig().transform.localPosition.x;
		Vector3 position = m_contraptionRunning.FindPig().transform.position;
		Vector3 vector = position;
		int layerMask = 1 << LayerMask.NameToLayer("Ground");
		RaycastHit hitInfo;
		if (Physics.Raycast(new Ray(position, new Vector3(0f, -1f, 0f)), out hitInfo, 100f, layerMask))
		{
			vector.y = position.y - hitInfo.distance + 1.1f;
		}
		vector.x = position.x - x;
		int num = 0;
		int num2 = 0;
		for (int i = gridXmin; i <= gridXmax; i++)
		{
			int num3 = 0;
			for (int j = 0; j < gridHeight; j++)
			{
				for (int k = gridXmin - 1; k <= gridXmax + 1; k++)
				{
					if (!Physics.CheckSphere(vector + new Vector3(k + i, j, 0f), 0.55f, layerMask))
					{
						num3++;
					}
				}
			}
			int num4 = num3;
			if (i == 0)
			{
				num4++;
			}
			if (num4 > num)
			{
				num = num4;
				num2 = i;
			}
		}
		vector.x += num2;
		vector.z = 0f;
		m_constructionUI.transform.position = vector;
		m_contraptionProto.transform.position = vector;
	}

	public void SetGameState(GameState newState)
	{
		Debug.Log("SetGameState: " + newState);
		switch (newState)
		{
		case GameState.Building:
			if (GameTime.IsPaused())
			{
				GameTime.Pause(false);
			}
			if (m_gameState == GameState.Running || m_gameState == GameState.PausedWhileRunning)
			{
				StopRunningContraption();
			}
			SetupCollectibles();
			m_contraptionProto.SetVisible(true);
			m_contraptionProto.GetComponent<Rigidbody>().isKinematic = true;
			if ((bool)m_constructionUI)
			{
				m_constructionUI.SetEnabled(true, true);
			}
			break;
		case GameState.Running:
			if (GameTime.IsPaused())
			{
				GameTime.Pause(false);
			}
			m_foodCollected = 0;
			m_eggsCollected = 0;
			m_timeElapsed = 0f;
			if ((bool)m_constructionUI)
			{
				m_constructionUI.SetEnabled(false, false);
			}
			m_contraptionRunning = m_contraptionProto.Clone();
			m_contraptionProto.SetVisible(false);
			m_contraptionRunning.StartContraption();
			m_contraptionRunning.SaveContraption();
			break;
		case GameState.Continue:
			if (GameTime.IsPaused())
			{
				GameTime.Pause(false);
			}
			newState = GameState.Running;
			break;
		case GameState.Preview:
			m_previewSpeed = 1f;
			m_previewTime = 0f;
			m_contraptionProto.SetVisible(false);
			if ((bool)m_constructionUI)
			{
				m_constructionUI.SetEnabled(false, true);
			}
			break;
		case GameState.PreviewMoving:
			m_previewAlpha = 0f;
			m_previewTime = 0f;
			m_contraptionProto.SetVisible(false);
			if ((bool)m_constructionUI)
			{
				m_constructionUI.SetEnabled(false, true);
			}
			break;
		case GameState.PreviewWhileRunning:
			m_previewCenter = m_contraptionRunning.transform.position;
			GameTime.Pause(true);
			m_previewDragging = false;
			break;
		case GameState.PreviewWhileBuilding:
			if ((bool)m_constructionUI)
			{
				m_constructionUI.SetEnabled(false, true);
			}
			m_previewCenter = m_contraptionProto.transform.position;
			m_previewDragging = false;
			break;
		case GameState.PausedWhileRunning:
			GameTime.Pause(true);
			break;
		case GameState.AutoBuilding:
			constructionUI.ClearContraption();
			m_autoBuildData = WPFPrefs.LoadContraptionDataset(m_oneStarContraption);
			SetAutoBuildOrder(m_autoBuildData);
			m_autoBuildTimer = 0f;
			m_autoBuildIndex = 0;
			break;
		}
		m_timeStateChanged = Time.time;
		m_gameState = newState;
		EventManager.Send(new GameStateChanged(newState));
	}

	private void StopRunningContraption()
	{
		if ((bool)m_contraptionRunning)
		{
			m_contraptionRunning.StopContraption();
			UnityEngine.Object.Destroy(m_contraptionRunning.gameObject);
			m_contraptionRunning = null;
		}
		List<GameObject> list = new List<GameObject>(GameObject.FindGameObjectsWithTag("ParticleEmitter"));
		foreach (GameObject item in list)
		{
			UnityEngine.Object.Destroy(item);
		}
	}

	private void SetupCollectibles()
	{
		List<Goal> list = new List<Goal>(UnityEngine.Object.FindSceneObjectsOfType(typeof(Goal)) as Goal[]);
		foreach (Transform item in m_collectibleBackup)
		{
			Goal component = item.GetComponent<Goal>();
			if ((bool)component)
			{
				list.Remove(component);
			}
		}
		foreach (Goal item2 in list)
		{
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		m_goals.Clear();
		foreach (Transform item3 in m_collectibleBackup)
		{
			Transform transform2 = UnityEngine.Object.Instantiate(item3) as Transform;
			transform2.transform.parent = null;
			m_goals.Add(transform2.GetComponent<Goal>());
		}
	}

	public GameObject GetGoal(int goalId)
	{
		foreach (Goal goal in m_goals)
		{
			if (goal.GoalId == goalId)
			{
				return goal.gameObject;
			}
		}
		return null;
	}

	public void NotifyGoalReached()
	{
		m_completionTime = m_timeElapsed;
		CheckTransportedParts();
		StartCoroutine(LevelCompleted());
	}

	public bool IsPartTransported(BasePart.PartType partType)
	{
		int connectedComponent = m_contraptionRunning.FindPig().ConnectedComponent;
		foreach (BasePart part in contraptionRunning.Parts)
		{
			if (part.m_partType != partType)
			{
				continue;
			}
			if (part.ConnectedComponent == connectedComponent)
			{
				return true;
			}
			foreach (BasePart part2 in contraptionRunning.Parts)
			{
				if (part2.ConnectedComponent == connectedComponent && Vector3.Distance(part.transform.position, part2.transform.position) < 2.5f)
				{
					return true;
				}
			}
		}
		return false;
	}

	public int TimesPartIsInContraptionProto(BasePart.PartType partType)
	{
		int connectedComponent = contraptionProto.FindPig().ConnectedComponent;
		int num = 0;
		foreach (BasePart part in contraptionRunning.Parts)
		{
			if (part == null || part.m_partType != partType)
			{
				continue;
			}
			if (part.ConnectedComponent == connectedComponent)
			{
				num++;
				continue;
			}
			foreach (BasePart part2 in contraptionRunning.Parts)
			{
				if (part2.ConnectedComponent == connectedComponent && !(part2 == null) && Vector3.Distance(part.transform.position, part2.transform.position) < 2.5f)
				{
					num++;
				}
			}
		}
		return num;
	}

	private void CheckTransportedParts()
	{
	}

	public void NotifyStarCollected(Star star)
	{
		m_starCollected++;
	}

	public void NotifyBlueprintCollected(Blueprint blueprint)
	{
		GameProgress.SetInt(Application.loadedLevelName + "_blueprint", 1);
	}

	public IEnumerator LevelCompleted()
	{
		GameObject.Find("InGameGUI").GetComponent<InGameGUI>().HideCurrentMenu();
		SetGameState(GameState.Completed);
		PlayVictorySound();
		LevelComplete levelComplete = GameObject.Find("InGameGUI").transform.Find("InGameLevelCompleteMenu").GetComponent<LevelComplete>();
		levelComplete.SetChallenges(m_challenges);
		int bonusStyle = ((!((float)(m_totalDestroyedParts / m_totalAvailableParts) > 0.25f)) ? (500 * (m_totalAvailableParts - m_totalDestroyedParts)) : (500 * m_totalDestroyedParts));
		m_totalScore = bonusStyle;
		int highscore = GameProgress.GetInt(Application.loadedLevelName + "_score");
		if (m_totalScore > highscore)
		{
			GameProgress.SetInt(Application.loadedLevelName + "_score", m_totalScore);
			m_newHighscore = true;
			Debug.Log("new highscore");
		}
		yield return new WaitForEndOfFrame();
	}

	public bool CanPlacePartAtGridCell(int x, int y)
	{
		if (x < m_gridXmin || x > m_gridXmax || y < 0 || y >= m_gridHeight)
		{
			return false;
		}
		int index = m_gridHeight - y - 1;
		int num = x - m_gridXmin;
		int num2 = m_currentConstructionGridRows[index];
		return (num2 & (1 << num)) != 0;
	}

	public int GetPartTypeCount(BasePart.PartType type)
	{
		int num = 0;
		foreach (PartCount partTypeCount in m_partTypeCounts)
		{
			if (partTypeCount.type == type)
			{
				num += partTypeCount.count;
				break;
			}
		}
		if (m_useSecondStarParts)
		{
			foreach (PartCount item in m_extraPartsForSecondStar)
			{
				if (item.type == type)
				{
					num += item.count;
					break;
				}
			}
		}
		if (m_sandbox)
		{
			num += GameProgress.GetSandboxPartCount(type);
		}
		return num;
	}

	public void SetPartTypeCount(BasePart.PartType type, int count)
	{
		for (int i = 0; i < m_partTypeCounts.Count; i++)
		{
			PartCount partCount = m_partTypeCounts[i];
			if (partCount.type == type)
			{
				partCount.count = count;
				m_partTypeCounts[i] = partCount;
				return;
			}
		}
		PartCount partCount2 = new PartCount();
		partCount2.type = type;
		partCount2.count = count;
		m_partTypeCounts.Add(partCount2);
	}

	public void OnDrawGizmosSelected()
	{
		LevelStart levelStart = WPFMonoBehaviour.FindSceneObjectOfType<LevelStart>();
		Transform goalPosition = GoalPosition;
		if ((bool)levelStart && (bool)goalPosition)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(levelStart.transform.position, goalPosition.transform.position);
		}
		if ((bool)levelStart)
		{
			Gizmos.color = Color.green;
			Vector3 constructionOffset = m_constructionOffset;
			constructionOffset.z = 0f;
			Vector3 center = levelStart.transform.position + constructionOffset;
			float num = 1.3333334f;
			Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
			float num2 = Mathf.Tan(camera.fieldOfView * ((float)Math.PI / 180f));
			float num3 = num2 * Mathf.Abs(m_constructionOffset.z);
			Vector3 size = new Vector3(num3, num3 / num, 0f);
			Gizmos.DrawWireCube(center, size);
		}
		if ((bool)goalPosition)
		{
			Gizmos.color = Color.green;
			Vector3 previewOffset = m_previewOffset;
			previewOffset.z = 0f;
			Vector3 center2 = goalPosition.transform.position + previewOffset;
			float num4 = 1.3333334f;
			Camera camera2 = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
			float num5 = Mathf.Tan(camera2.fieldOfView * ((float)Math.PI / 180f));
			float num6 = num5 * Mathf.Abs(m_previewOffset.z);
			Vector3 size2 = new Vector3(num6, num6 / num4, 0f);
			Gizmos.DrawWireCube(center2, size2);
		}
	}

	public void Update()
	{
		switch (gameState)
		{
		case GameState.Preview:
		{
			m_previewTime += Time.deltaTime * m_previewSpeed;
			bool flag = ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) ? true : false);
			if (m_previewTime > m_previewWaitTime || flag)
			{
				SetGameState(GameState.PreviewMoving);
			}
			break;
		}
		case GameState.PreviewMoving:
		{
			m_previewTime += Time.deltaTime * m_previewSpeed;
			float num = m_previewTime / m_previewMoveTime;
			m_previewAlpha = num;
			break;
		}
		case GameState.Running:
			m_timeElapsed += Time.deltaTime;
			break;
		case GameState.AutoBuilding:
			AutoBuild();
			break;
		}
	}

	private void AutoBuild()
	{
		m_autoBuildTimer += Time.deltaTime;
		if (!(m_autoBuildTimer > 0.3f))
		{
			return;
		}
		m_autoBuildTimer = 0f;
		if (m_autoBuildIndex < m_autoBuildData.ContraptionDatasetList.Count)
		{
			ContraptionDataset.ContraptionDatasetUnit contraptionDatasetUnit = m_autoBuildData.ContraptionDatasetList[m_autoBuildIndex];
			ConstructionUI.PartDesc partDesc = m_constructionUI.FindPartDesc((BasePart.PartType)contraptionDatasetUnit.partType);
			if (partDesc != null)
			{
				BasePart basePart = BuildPart(contraptionDatasetUnit, partDesc.part);
				basePart.GetComponent<BasePart>().ChangeVisualConnections();
				contraptionProto.RefreshNeighbours(basePart.m_coordX, basePart.m_coordY);
				partDesc.useCount++;
				EventManager.Send(new PartCountChanged(partDesc.part.m_partType, partDesc.CurrentCount));
				Vector3 position = m_constructionUI.GridPositionToWorldPosition(contraptionDatasetUnit.x, contraptionDatasetUnit.y);
				WPFMonoBehaviour.effectManager.CreateParticles(WPFMonoBehaviour.gameData.m_dustParticles, position);
			}
			m_autoBuildIndex++;
		}
		else
		{
			SetGameState(GameState.Building);
		}
	}

	public void SetAutoBuildOrder(ContraptionDataset data)
	{
		int num = -1;
		List<ContraptionDataset.ContraptionDatasetUnit> contraptionDatasetList = data.ContraptionDatasetList;
		for (int i = 0; i < contraptionDatasetList.Count; i++)
		{
			if (contraptionDatasetList[i].partType == 10)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			ContraptionDataset.ContraptionDatasetUnit value = contraptionDatasetList[contraptionDatasetList.Count - 1];
			contraptionDatasetList[contraptionDatasetList.Count - 1] = contraptionDatasetList[num];
			contraptionDatasetList[num] = value;
		}
	}

	public void PlayVictorySound()
	{
		audioManager.Play2dEffect(audioManager.CommonAudioCollection.victory);
	}

	public void PlayPartPlacedSound()
	{
		audioManager.Play2dEffect(audioManager.CommonAudioCollection.placePart);
	}

	public void PlayDragSound()
	{
		audioManager.Play2dEffect(audioManager.CommonAudioCollection.dragPart);
	}

	public void PlayRemoveSound()
	{
		audioManager.Play2dEffect(audioManager.CommonAudioCollection.removePart);
	}
}
