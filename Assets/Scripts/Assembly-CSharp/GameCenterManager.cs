using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

public class GameCenterManager : SocialGameManager
{
	public struct AchievementQueueBlock
	{
		public string id;

		public double progress;
	}

	public struct AchievementDataStruct
	{
		public string title;

		public string description;

		public double percentComplete;

		public bool completed;

		public bool hidden;
	}

	public struct LeaderboardDataStruct
	{
		public string title;

		public int rank;

		public long score;
	}

	[SerializeField]
	private AchievementPopup m_achievementPopup;

	private Dictionary<string, AchievementDataStruct> m_achievementList = new Dictionary<string, AchievementDataStruct>();

	private Dictionary<string, LeaderboardDataStruct> m_leaderboardList = new Dictionary<string, LeaderboardDataStruct>();

	private List<AchievementQueueBlock> m_achievementsQueue = new List<AchievementQueueBlock>();

	private bool m_achievementsQueueSemaphore;

	public List<string> m_leaderboardIDs = new List<string>();

	public Dictionary<string, AchievementDataStruct> Achievements
	{
		get
		{
			return m_achievementList;
		}
	}

	public static bool Authenticated
	{
		get
		{
			return Social.localUser.authenticated;
		}
	}

	public static event Action<bool> onAuthenticationSucceeded;

	private void Start()
	{
		Authenticate();
		GameObject gameObject = UnityEngine.Object.Instantiate(m_achievementPopup.gameObject) as GameObject;
		m_achievementPopup = gameObject.GetComponent<AchievementPopup>();
		StartCoroutine(SyncAchievementData());
	}

	public override void Authenticate()
	{
		Social.localUser.Authenticate(AuthenticationSucceeded);
	}

	public override void ShowAchievementsView()
	{
		if (Authenticated && !Application.isEditor)
		{
			Social.ShowAchievementsUI();
		}
		else
		{
			Debug.Log("GameCenterManager::ShowAchievementsView - User is not authenticated.");
		}
	}

	public override void ShowLeaderboardsView()
	{
		if (Authenticated && !Application.isEditor)
		{
			Social.ShowLeaderboardUI();
		}
		else
		{
			Debug.Log("GameCenterManager::ShowLeaderboardsView - User is not authenticated.");
		}
	}

	public override void LoadAchievements()
	{
		if (Authenticated && !Application.isEditor)
		{
			Social.LoadAchievements(AchievementsDidLoad);
		}
		else
		{
			Debug.Log("GameCenterManager::LoadAchievements - User is not authenticated.");
		}
	}

	public override void LoadLeaderboardScores()
	{
		if (Authenticated && !Application.isEditor)
		{
			foreach (string leaderboardID in m_leaderboardIDs)
			{
				ILeaderboard lb = Social.CreateLeaderboard();
				lb.id = leaderboardID;
				lb.LoadScores(delegate
				{
					LeaderboardDataStruct value = default(LeaderboardDataStruct);
					value.title = lb.title;
					value.rank = lb.localUserScore.rank;
					long.TryParse(lb.localUserScore.formattedValue, out value.score);
					m_leaderboardList.Add(lb.id, value);
				});
			}
			return;
		}
		Debug.Log("GameCenterManager::LoadAchievements - User is not authenticated.");
	}

	public override void ReportAchievementProgress(string achievementId, double progress)
	{
		if (Authenticated && !Application.isEditor)
		{
			AchievementQueueBlock item = default(AchievementQueueBlock);
			item.id = achievementId;
			item.progress = progress;
			AchievementData.AchievementDataHolder achievement = AchievementData.Instance.GetAchievement(achievementId);
			achievement.progress = progress;
			achievement.completed = progress >= 100.0;
			AchievementData.Instance.SetAchievement(achievementId, achievement);
			m_achievementsQueue.Add(item);
		}
		else
		{
			Debug.Log("GameCenterManager::ReportAchievementProgress - User is not authenticated.");
		}
	}

	public override void ReportLeaderboardScore(string leaderboardId, long score, Action<bool> handler)
	{
		if (Authenticated && !Application.isEditor)
		{
			Social.ReportScore(score, leaderboardId, handler);
		}
		else
		{
			Debug.Log("GameCenterManager::ReportLeaderboardScore - User is not authenticated.");
		}
	}

	public override void ResetAchievementData()
	{
		if (Authenticated && !Application.isEditor)
		{
			GameCenterPlatform.ResetAllAchievements(AchievementsDidReset);
		}
		else
		{
			Debug.Log("GameCenterManager::ResetAchievementData - User is not authenticated.");
		}
	}

	public override void SyncAllAchievementsNow()
	{
		Debug.Log("GameCenterManager::SyncAllAchievementsNow - Syncing");
		Dictionary<string, double> achievementsProgress = AchievementData.Instance.AchievementsProgress;
		foreach (KeyValuePair<string, double> item in achievementsProgress)
		{
			if (item.Value > 0.0)
			{
				ReportAchievementProgress(item.Key, item.Value);
			}
		}
	}

	private void AchievementsDidLoad(IAchievement[] achievementsList)
	{
		foreach (IAchievement achievement in achievementsList)
		{
			AchievementDataStruct value = default(AchievementDataStruct);
			value.percentComplete = achievement.percentCompleted;
			value.completed = achievement.completed;
			value.hidden = achievement.hidden;
			m_achievementList.Add(achievement.id, value);
		}
		Social.LoadAchievementDescriptions(AchievementDescriptionsDidLoad);
	}

	private void AchievementDescriptionsDidLoad(IAchievementDescription[] achievementsList)
	{
		foreach (IAchievementDescription achievementDescription in achievementsList)
		{
			AchievementDataStruct achievementDataStruct = Achievements[achievementDescription.id];
			achievementDataStruct.title = achievementDescription.title;
			achievementDataStruct.description = achievementDescription.achievedDescription;
		}
	}

	private void AchievementsDidReset(bool success)
	{
		if (success)
		{
			Debug.Log("Achievements did reset");
		}
		else
		{
			Debug.Log("Failed to reset achievements");
		}
	}

	private void AchievementReportDidComplete(bool success)
	{
		if (success)
		{
			Debug.Log("Achievement report successful");
			if (m_achievementsQueue[0].progress >= 100.0)
			{
				m_achievementPopup.Show(m_achievementsQueue[0].id);
			}
		}
		else
		{
			Debug.Log("Achievement report failed");
		}
		m_achievementsQueueSemaphore = false;
		m_achievementsQueue.RemoveAt(0);
	}

	private void AuthenticationSucceeded(bool success)
	{
		if (success)
		{
			Debug.Log("Authentication successful: " + Social.localUser.userName);
			SyncAllAchievementsNow();
		}
		else
		{
			Debug.Log("Authentication failed");
		}
		if (GameCenterManager.onAuthenticationSucceeded != null)
		{
			GameCenterManager.onAuthenticationSucceeded(success);
		}
	}

	private IEnumerator SyncAchievementData()
	{
		while (true)
		{
			if (m_achievementsQueue.Count > 0 && !m_achievementsQueueSemaphore)
			{
				Debug.Log("Syncing achievement: " + m_achievementsQueue[0].id);
				Social.ReportProgress(m_achievementsQueue[0].id, m_achievementsQueue[0].progress, AchievementReportDidComplete);
				m_achievementsQueueSemaphore = true;
			}
			else
			{
				yield return new WaitForSeconds(3f);
			}
		}
	}
}
