using System;
using UnityEngine;

public class SocialGameManager : MonoBehaviour
{
	private static SocialGameManager instance;

	public static SocialGameManager Instance
	{
		get
		{
			return instance;
		}
	}

	public virtual void Authenticate()
	{
	}

	public virtual void ShowAchievementsView()
	{
	}

	public virtual void ShowLeaderboardsView()
	{
	}

	public virtual void LoadAchievements()
	{
	}

	public virtual void LoadLeaderboardScores()
	{
	}

	public virtual void ReportAchievementProgress(string achievementId, double progress)
	{
	}

	public virtual void ReportLeaderboardScore(string leaderboardId, long score, Action<bool> handler)
	{
	}

	public virtual void SyncAllAchievementsNow()
	{
	}

	public virtual void ResetAchievementData()
	{
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
	}
}
