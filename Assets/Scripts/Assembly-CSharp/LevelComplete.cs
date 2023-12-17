using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelComplete : WPFMonoBehaviour
{
	private GameObject m_starOne;

	private GameObject m_starTwo;

	private GameObject m_starThree;

	private List<Challenge> m_challenges = new List<Challenge>();

	private ObjectiveSlot m_objectiveOne;

	private ObjectiveSlot m_objectiveTwo;

	private ObjectiveSlot m_objectiveThree;

	private GameObject m_controlsPanel;

	private Vector3 m_controlsPosition;

	private Vector3 m_controlsHidePosition;

	private GameObject m_starPanel;

	private Vector3 m_starPanelPosition;

	private Vector3 m_starPanelHidePosition;

	private GameObject m_background;

	private Vector3 m_backgroundPosition;

	private Vector3 m_backgroundHidePosition;

	private BasePart.PartType m_unlockedPart;

	private string m_unlockedSandbox = string.Empty;

	public void SetChallenges(List<Challenge> challenges)
	{
		m_challenges = challenges;
	}

	private void OnEnable()
	{
		if (BuildCustomizationLoader.Instance.IAPEnabled)
		{
			IapManager.onPurchaseSucceeded += HandleIapManageronPurchaseSucceeded;
		}
	}

	private void OnDisable()
	{
		if (BuildCustomizationLoader.Instance.IAPEnabled)
		{
			IapManager.onPurchaseSucceeded -= HandleIapManageronPurchaseSucceeded;
		}
	}

	private void HandleIapManageronPurchaseSucceeded(IapManager.InAppPurchaseItemType type)
	{
		if (type == IapManager.InAppPurchaseItemType.UnlockFullVersion)
		{
			GameManager.Instance.LoadNextLevel();
		}
	}

	private void Start()
	{
		m_starPanel = base.transform.Find("StarPanel").gameObject;
		m_starOne = m_starPanel.transform.Find("StarOne").gameObject;
		m_starTwo = m_starPanel.transform.Find("StarTwo").gameObject;
		m_starThree = m_starPanel.transform.Find("StarThree").gameObject;
		m_objectiveOne = m_starPanel.transform.Find("ObjectiveSlot1").GetComponent<ObjectiveSlot>();
		m_objectiveTwo = m_starPanel.transform.Find("ObjectiveSlot2").GetComponent<ObjectiveSlot>();
		m_objectiveThree = m_starPanel.transform.Find("ObjectiveSlot3").GetComponent<ObjectiveSlot>();
		m_controlsPanel = base.transform.Find("ControlsPanel").gameObject;
		m_controlsPosition = m_controlsPanel.transform.position;
		m_controlsHidePosition = m_controlsPosition + new Vector3(0f, -5f, 0f);
		m_controlsPanel.transform.position = m_controlsHidePosition;
		m_starPanelPosition = m_starPanel.transform.position;
		m_starPanelHidePosition = m_starPanelPosition + new Vector3(0f, 12f, 0f);
		m_starPanel.transform.position = m_starPanelHidePosition;
		m_background = base.transform.Find("Background").gameObject;
		m_backgroundPosition = m_background.transform.position;
		m_backgroundHidePosition = m_backgroundPosition + new Vector3(0f, 12f, 0f);
		m_background.transform.position = m_backgroundHidePosition;
		m_starOne.active = false;
		m_starTwo.active = false;
		m_starThree.active = false;
		AudioManager instance = AudioManager.Instance;
		if (m_challenges.Count >= 2 && m_challenges[1].IsCompleted() && !m_challenges[0].IsCompleted())
		{
			Challenge value = m_challenges[0];
			m_challenges[0] = m_challenges[1];
			m_challenges[1] = value;
		}
		if (m_challenges.Count >= 1)
		{
			m_objectiveTwo.SetChallenge(m_challenges[0]);
		}
		if (m_challenges.Count >= 2)
		{
			m_objectiveThree.SetChallenge(m_challenges[1]);
		}
		StartCoroutine(ShowStarPanel(1.5f));
		float num = 2f;
		int num2 = GameProgress.GetInt(Application.loadedLevelName + "_stars");
		string text = "{";
		string text2 = "{";
		if (GameProgress.IsLevelCompleted(Application.loadedLevelName))
		{
			StartCoroutine(PlayOldStarEffects(m_objectiveOne, m_starOne, num, true));
			text += "1, ";
		}
		else
		{
			LevelCompletedForFirstTime();
			num += 0.2f;
			StartCoroutine(PlayStarEffects(m_objectiveOne, m_starOne, null, num, instance.CommonAudioCollection.oneStar));
			GameProgress.SetLevelCompleted(Application.loadedLevelName);
			text2 += "1, ";
			num2++;
		}
		num += 0.7f;
		if (GameProgress.GetInt(Application.loadedLevelName + "_challenge_" + m_challenges[0].Type) == 1)
		{
			StartCoroutine(PlayOldStarEffects(m_objectiveTwo, m_starTwo, num, m_challenges[0].IsCompleted()));
			text += "2, ";
		}
		else if (m_challenges.Count >= 1 && m_challenges[0].IsCompleted())
		{
			StartCoroutine(PlayStarEffects(m_objectiveTwo, m_starTwo, m_challenges[0], num, instance.CommonAudioCollection.twoStar));
			GameProgress.SetInt(Application.loadedLevelName + "_challenge_" + m_challenges[0].Type, 1);
			text2 += "2 ";
			num2++;
		}
		num += 0.7f;
		if (GameProgress.GetInt(Application.loadedLevelName + "_challenge_" + m_challenges[1].Type) == 1)
		{
			StartCoroutine(PlayOldStarEffects(m_objectiveThree, m_starThree, num, m_challenges[1].IsCompleted()));
			text += "3}";
		}
		else if (m_challenges.Count >= 2 && m_challenges[1].IsCompleted())
		{
			StartCoroutine(PlayStarEffects(m_objectiveThree, m_starThree, m_challenges[1], num, instance.CommonAudioCollection.threeStar));
			GameProgress.SetInt(Application.loadedLevelName + "_challenge_" + m_challenges[1].Type, 1);
			text2 += "3}";
			num2++;
		}
		bool flag = GameManager.Instance.CurrentLevelRowThreeStarred();
		GameProgress.SetInt(Application.loadedLevelName + "_stars", num2);
		bool jokerLevelUnlocked = GameManager.Instance.CurrentLevelRowThreeStarred() && !flag;
		StartCoroutine(ShowBackground(1.5f, num - 1.5f));
		if (GameManager.Instance.IsLastLevelInEpisode())
		{
			SetCutsceneButton();
		}
		ShowRewards(ref num, jokerLevelUnlocked);
		StartCoroutine(ShowControls(num));
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ID", Application.loadedLevelName);
			dictionary.Add("PREVIOUS_STARS", text);
			dictionary.Add("NEW_STARS", text2);
			dictionary.Add("TIME", string.Empty + WPFMonoBehaviour.levelManager.TimeElapsed);
			dictionary.Add("CONTRAPTION", string.Empty + WPFMonoBehaviour.levelManager.contraptionProto.GetContraptionID());
			FlurryManager.Instance.FlurryDataHolder.Add("NEW_STARS", text2);
			FlurryManager.Instance.FlurryDataHolder.Add("PREVIOUS_STARS", text);
			FlurryManager.Instance.LogEventWithParameters("Complete Level", dictionary);
		}
		if (DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios && (bool)WPFMonoBehaviour.levelManager)
		{
			CheckLevelEndAchievements();
		}
	}

	private void ShowRewards(ref float delay, bool jokerLevelUnlocked)
	{
		if (m_unlockedPart != 0)
		{
			delay += 0.5f;
			StartCoroutine(ShowUnlockedPart(delay));
			delay += 0.5f;
		}
		if (m_unlockedSandbox != string.Empty)
		{
			Button component = base.transform.Find("Rewards").Find("SandboxUnlock").Find("Open")
				.Find("Sandbox")
				.GetComponent<Button>();
			component.MessageTargetObject = base.gameObject;
			component.TargetComponent = "LevelComplete";
			component.MethodToInvoke = "LoadSandboxLevel";
			component.MessageParameter = m_unlockedSandbox;
			component.EventToSend = UIEvent.Type.None;
			Sprite sandboxTitle = WPFMonoBehaviour.gameData.GetSandboxTitle(m_unlockedSandbox);
			if ((bool)sandboxTitle)
			{
				GameObject gameObject = (GameObject)Object.Instantiate(sandboxTitle.gameObject);
				gameObject.GetComponent<Renderer>().enabled = false;
				gameObject.transform.parent = base.transform.Find("Rewards").Find("SandboxUnlock").Find("Open");
				gameObject.transform.localPosition = new Vector3(1.2f, 2.1f, -1f);
				gameObject.transform.localScale = Vector3.one;
				GameObject gameObject2 = (GameObject)Object.Instantiate(sandboxTitle.gameObject);
				gameObject2.GetComponent<Renderer>().enabled = false;
				gameObject2.transform.parent = base.transform.Find("Rewards").Find("SandboxUnlock").Find("Locked");
				gameObject2.transform.localPosition = new Vector3(1.2f, 2.1f, -1f);
				gameObject2.transform.localScale = Vector3.one;
			}
			StartCoroutine(ShowUnlockedSandbox(delay));
		}
		else if (jokerLevelUnlocked)
		{
			string currentRowJokerLevel = GameManager.Instance.GetCurrentRowJokerLevel();
			Button component2 = base.transform.Find("Rewards").Find("BonusLevelUnlock").Find("Open")
				.Find("JokerLevelButton")
				.GetComponent<Button>();
			component2.MessageTargetObject = base.gameObject;
			component2.TargetComponent = "LevelComplete";
			component2.MethodToInvoke = "LoadJokerLevel";
			component2.MessageParameter = currentRowJokerLevel;
			component2.EventToSend = UIEvent.Type.None;
			component2.transform.Find("LevelNumber").GetComponent<TextMesh>().text = GameManager.Instance.GetCurrentRowJokerLevelNumber();
			SendUnlockedBonusLevelFlurryEvent(currentRowJokerLevel);
			StartCoroutine(ShowUnlockedLevel(delay));
		}
	}

	private void CheckLevelEndAchievements()
	{
		if (GameManager.Instance.CurrentEpisode.CompareTo("Episode1LevelSelection") == 0 && GameManager.Instance.CurrentLevel == GameManager.Instance.LevelCount - 1)
		{
			SocialGameManager.Instance.ReportAchievementProgress("grp.GROUND_HOG_DAY", 100.0);
		}
		if (GameManager.Instance.CurrentEpisode.CompareTo("Episode2LevelSelection") == 0 && GameManager.Instance.CurrentLevel == GameManager.Instance.LevelCount - 1)
		{
			SocialGameManager.Instance.ReportAchievementProgress("grp.WHEN_PIGS_FLY", 100.0);
		}
		if (GameManager.Instance.CurrentEpisodeThreeStarredNormalLevels())
		{
			if (GameManager.Instance.CurrentEpisode.CompareTo("Episode1LevelSelection") == 0)
			{
				SocialGameManager.Instance.ReportAchievementProgress("grp.REALLY_LOW_TO_THE_GROUND", 100.0);
			}
			else if (GameManager.Instance.CurrentEpisode.CompareTo("Episode2LevelSelection") == 0)
			{
				SocialGameManager.Instance.ReportAchievementProgress("grp.BOY_DO_THEY_FLY", 100.0);
			}
		}
		if (GameManager.Instance.CurrentEpisodeThreeStarredSpecialLevels())
		{
			if (GameManager.Instance.CurrentEpisode.CompareTo("Episode1LevelSelection") == 0)
			{
				SocialGameManager.Instance.ReportAchievementProgress("grp.GROUND_BREAKER", 100.0);
			}
			else if (GameManager.Instance.CurrentEpisode.CompareTo("Episode2LevelSelection") == 0)
			{
				SocialGameManager.Instance.ReportAchievementProgress("grp.PRETTY_FLY", 100.0);
			}
		}
		if (WPFMonoBehaviour.levelManager.IsPartTransported(BasePart.PartType.KingPig))
		{
			int @int = GameProgress.GetInt("Transported_Kings");
			@int++;
			GameProgress.SetInt("Transported_Kings", @int);
			if (@int >= 10)
			{
				SocialGameManager.Instance.ReportAchievementProgress("grp.HOGFFEUR", 100.0);
			}
			else if (@int >= 5)
			{
				SocialGameManager.Instance.ReportAchievementProgress("PIGSHAW", 100.0);
			}
		}
		if (WPFMonoBehaviour.levelManager.contraptionRunning.Parts.Count == WPFMonoBehaviour.levelManager.contraptionProto.Parts.Count)
		{
			int int2 = GameProgress.GetInt("Perfect_Completions");
			int2++;
			GameProgress.SetInt("Perfect_Completions", int2);
			if (int2 >= AchievementData.Instance.GetAchievementLimit("grp.SKILLED_PILOT"))
			{
				SocialGameManager.Instance.ReportAchievementProgress("grp.SKILLED_PILOT", 100.0);
			}
		}
		int num = WPFMonoBehaviour.levelManager.TimesPartIsInContraptionProto(BasePart.PartType.EngineBig);
		if (num > 5)
		{
			SocialGameManager.Instance.ReportAchievementProgress("grp.CRASH_COURSE", 100.0);
		}
		int num2 = WPFMonoBehaviour.levelManager.TimesPartIsInContraptionProto(BasePart.PartType.TNT);
		int num3 = WPFMonoBehaviour.levelManager.TimesPartIsInContraptionProto(BasePart.PartType.Pig);
		int num4 = WPFMonoBehaviour.levelManager.TimesPartIsInContraptionProto(BasePart.PartType.KingPig);
		if (num2 + num3 + num4 >= WPFMonoBehaviour.levelManager.contraptionProto.Parts.Count)
		{
			SocialGameManager.Instance.ReportAchievementProgress("grp.PORCINE_CANNONBALL", 100.0);
		}
		if (WPFMonoBehaviour.levelManager.contraptionProto.FindPig().enclosedPart == null)
		{
			SocialGameManager.Instance.ReportAchievementProgress("grp.THINK_OUTSIDE_THE_BOX", 100.0);
		}
		Pig pig = WPFMonoBehaviour.levelManager.contraptionRunning.FindPig() as Pig;
		float traveledDistance = pig.traveledDistance;
		float rolledDistance = pig.rolledDistance;
		GameProgress.SetFloat("traveledDistance", traveledDistance);
		GameProgress.SetFloat("rolledDistance", rolledDistance);
		if (traveledDistance > (float)AchievementData.Instance.GetAchievementLimit("grp.TOURIST_1"))
		{
			SocialGameManager.Instance.ReportAchievementProgress("grp.TOURIST_1", 100.0);
		}
		else if (traveledDistance > (float)AchievementData.Instance.GetAchievementLimit("grp.TOURIST_2"))
		{
			SocialGameManager.Instance.ReportAchievementProgress("grp.TOURIST_2", 100.0);
		}
		else if (traveledDistance > (float)AchievementData.Instance.GetAchievementLimit("grp.TOURIST_3"))
		{
			SocialGameManager.Instance.ReportAchievementProgress("grp.TOURIST_3", 100.0);
		}
		if (rolledDistance > (float)AchievementData.Instance.GetAchievementLimit("grp.ROLLING_LOW_1"))
		{
			SocialGameManager.Instance.ReportAchievementProgress("grp.ROLLING_LOW_1", 100.0);
		}
		else if (rolledDistance > (float)AchievementData.Instance.GetAchievementLimit("grp.ROLLING_LOW_2"))
		{
			SocialGameManager.Instance.ReportAchievementProgress("grp.ROLLING_LOW_2", 100.0);
		}
		else if (rolledDistance > (float)AchievementData.Instance.GetAchievementLimit("grp.ROLLING_LOW_3"))
		{
			SocialGameManager.Instance.ReportAchievementProgress("grp.ROLLING_LOW_3", 100.0);
		}
	}

	private void LevelCompletedForFirstTime()
	{
		if ((bool)WPFMonoBehaviour.levelManager)
		{
			foreach (LevelManager.PartCount item in WPFMonoBehaviour.levelManager.m_partsToUnlockOnCompletion)
			{
				GameProgress.AddSandboxParts(item.type, item.count);
				m_unlockedPart = item.type;
			}
			if (WPFMonoBehaviour.levelManager.unlockSandboxLevelOnCompletion != string.Empty)
			{
				GameProgress.SetBool(WPFMonoBehaviour.levelManager.unlockSandboxLevelOnCompletion + "_sandbox_unlocked", true);
				m_unlockedSandbox = WPFMonoBehaviour.levelManager.unlockSandboxLevelOnCompletion;
			}
		}
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string value = GameManager.Instance.CurrentEpisode + "-" + Application.loadedLevel;
			dictionary.Add("ID", Application.loadedLevelName);
			dictionary.Add("POSITION", value);
			FlurryManager.Instance.LogEventWithParameters("Level End Reached", dictionary);
		}
	}

	private void OnDestroy()
	{
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			if (FlurryManager.Instance.FlurryDataHolder.ContainsKey("NEW_STARS"))
			{
				FlurryManager.Instance.FlurryDataHolder.Remove("NEW_STARS");
			}
			if (FlurryManager.Instance.FlurryDataHolder.ContainsKey("PREVIOUS_STARS"))
			{
				FlurryManager.Instance.FlurryDataHolder.Remove("PREVIOUS_STARS");
			}
		}
	}

	private void SetCutsceneButton()
	{
		m_controlsPanel.transform.Find("NextLevelButton").gameObject.active = false;
		m_controlsPanel.transform.Find("CutsceneButton").position -= Vector3.forward * 8f;
	}

	private IEnumerator ShowUnlockedLevel(float delay)
	{
		yield return new WaitForSeconds(delay);
		RewardView part = GameObject.Find("Rewards").transform.Find("UnlockPart").GetComponent<RewardView>();
		part.Hide();
		RewardView reward = GameObject.Find("Rewards").transform.Find("BonusLevelUnlock").GetComponent<RewardView>();
		if (reward.HasLocked())
		{
			reward.ShowLocked();
			yield return new WaitForSeconds(0.75f);
		}
		reward.ShowOpen();
	}

	private IEnumerator ShowUnlockedSandbox(float delay)
	{
		yield return new WaitForSeconds(delay);
		RewardView part = GameObject.Find("Rewards").transform.Find("UnlockPart").GetComponent<RewardView>();
		part.Hide();
		RewardView reward = GameObject.Find("Rewards").transform.Find("SandboxUnlock").GetComponent<RewardView>();
		if (reward.HasLocked())
		{
			reward.ShowLocked();
			yield return new WaitForSeconds(0.75f);
		}
		reward.ShowOpen();
	}

	private IEnumerator ShowUnlockedPart(float delay)
	{
		yield return new WaitForSeconds(delay);
		RewardView reward = GameObject.Find("Rewards").transform.Find("UnlockPart").GetComponent<RewardView>();
		reward.ShowOpen();
		reward.SetPart(m_unlockedPart);
	}

	private IEnumerator ShowStarPanel(float delay)
	{
		yield return new WaitForSeconds(delay);
		float t = 0f;
		while (t < 1f)
		{
			t = Mathf.Min(t + Time.deltaTime / 0.3f, 1f);
			m_starPanel.transform.position = Vector3.Slerp(m_starPanelHidePosition, m_starPanelPosition, t);
			yield return 0;
		}
	}

	private IEnumerator ShowBackground(float delay1, float delay2)
	{
		yield return new WaitForSeconds(delay1);
		float t2 = 0f;
		while (t2 < 1f)
		{
			t2 = Mathf.Min(t2 + Time.deltaTime / 0.3f, 1f);
			m_background.transform.position = Vector3.Slerp(m_backgroundHidePosition, m_backgroundPosition, t2);
			yield return 0;
		}
		yield return new WaitForSeconds(delay2);
		m_backgroundHidePosition = m_backgroundPosition;
		m_backgroundPosition.y -= 12.5f;
		t2 = 0f;
		while (t2 < 1f)
		{
			t2 = Mathf.Min(t2 + Time.deltaTime / 0.3f, 1f);
			m_background.transform.position = Vector3.Slerp(m_backgroundHidePosition, m_backgroundPosition, t2);
			yield return 0;
		}
	}

	private IEnumerator ShowControls(float delay)
	{
		if ((GameManager.Instance.CurrentLevel + 1) % 5 == 0)
		{
			m_controlsPanel.transform.Find("NextLevelButton").gameObject.active = false;
			m_controlsPanel.transform.Find("CutsceneButton").gameObject.active = false;
		}
		if (GameManager.Instance.CurrentLevel + 1 >= 29 && BuildCustomizationLoader.Instance.IsFreeVersion)
		{
			Button nextLevelButton = m_controlsPanel.transform.Find("NextLevelButton").GetComponent<Button>();
			nextLevelButton.MessageTargetObject = base.gameObject;
			nextLevelButton.TargetComponent = "LevelComplete";
			nextLevelButton.MethodToInvoke = "OpenUnlockFullVersionPurchasePage";
			nextLevelButton.MessageParameter = null;
			nextLevelButton.EventToSend = UIEvent.Type.None;
		}
		yield return new WaitForSeconds(delay);
		float t = 0f;
		while (t < 1f)
		{
			t = Mathf.Min(t + Time.deltaTime / 0.25f, 1f);
			m_controlsPanel.transform.position = Vector3.Slerp(m_controlsHidePosition, m_controlsPosition, t);
			yield return 0;
		}
	}

	private IEnumerator PlayStarEffects(ObjectiveSlot objective, GameObject star, Challenge challenge, float delay, AudioClip starAudio)
	{
		yield return new WaitForSeconds(delay);
		objective.SetSucceeded();
		StartCoroutine(PlayStarEffects(star, 0.25f, starAudio));
	}

	private IEnumerator PlayStarEffects(GameObject star, float delay, AudioClip starAudio)
	{
		yield return new WaitForSeconds(delay);
		star.active = true;
		ParticleSystem[] componentsInChildren = star.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem starEffect in componentsInChildren)
		{
			starEffect.Play();
		}
		AudioManager.Instance.Play2dEffect(starAudio);
	}

	private IEnumerator PlayOldStarEffects(ObjectiveSlot objective, GameObject star, float delay, bool completedInThisRun)
	{
		star.active = true;
		yield return new WaitForSeconds(delay);
		if (completedInThisRun)
		{
			objective.SetSucceeded();
		}
		star.GetComponent<Animation>().Play();
	}

	public void LoadJokerLevel(string levelName)
	{
		Loader.Instance.LoadLevel(levelName, true);
	}

	public void LoadSandboxLevel(string levelName)
	{
		Loader.Instance.LoadLevel(levelName, true);
	}

	public void LoadEndingCutscene()
	{
		GameManager.Instance.LoadEndingCutscene();
	}

	public void OpenUnlockFullVersionPurchasePage()
	{
		if (BuildCustomizationLoader.Instance.IAPEnabled)
		{
			IapManager.Instance.EnableUnlockFullVersionPurchasePage();
		}
	}

	public void SendUnlockedBonusLevelFlurryEvent(string id)
	{
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ID", id);
			dictionary.Add("PLAYED_ID", Application.loadedLevelName);
			FlurryManager.Instance.LogEventWithParameters("Pause Flight", dictionary);
		}
	}
}
