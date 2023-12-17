using System;
using UnityEngine;

public class CheatsUtility : WPFMonoBehaviour
{
	private float m_buttonHeight;

	private float m_buttonWidth;

	private void Start()
	{
		m_buttonHeight = (float)Screen.height * 0.15f;
		m_buttonWidth = (float)Screen.width * 0.2f;
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(0f, 0f, m_buttonWidth, m_buttonHeight), "Main Menu"))
		{
			Loader.Instance.LoadLevel("MainMenu", false);
		}
		if (GUI.Button(new Rect(0f, m_buttonHeight, m_buttonWidth, m_buttonHeight), "Reset progress"))
		{
			GameProgress.DeleteAll();
			GameProgress.InitializeGameProgressData();
			GameProgress.Save();
			UserSettings.DeleteAll();
			if (DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios)
			{
				SocialGameManager.Instance.ResetAchievementData();
			}
		}
		if (GUI.Button(new Rect(0f, 2f * m_buttonHeight, m_buttonWidth, m_buttonHeight), "3-stars all but one"))
		{
			foreach (EpisodeLevels episodeLevel in WPFMonoBehaviour.gameData.m_episodeLevels)
			{
				int num = UnityEngine.Random.Range(0, episodeLevel.Levels.Count - 3);
				for (int i = 0; i < episodeLevel.Levels.Count - 2; i++)
				{
					if (i != num)
					{
						SetThreeStarsCompletion(episodeLevel.Levels[i]);
					}
				}
			}
		}
		if (GUI.Button(new Rect(0f, 3f * m_buttonHeight, m_buttonWidth, m_buttonHeight), "3-stars all"))
		{
			foreach (EpisodeLevels episodeLevel2 in WPFMonoBehaviour.gameData.m_episodeLevels)
			{
				for (int j = 0; j < episodeLevel2.Levels.Count; j++)
				{
					SetThreeStarsCompletion(episodeLevel2.Levels[j]);
				}
			}
		}
		if (GUI.Button(new Rect(0f, 4f * m_buttonHeight, m_buttonWidth, m_buttonHeight), "Sandbox all starboxes"))
		{
			foreach (GameData.SandBoxInfo sandboxTitle in WPFMonoBehaviour.gameData.m_sandboxTitles)
			{
				for (int k = 0; k < 20; k++)
				{
					if (k < 10)
					{
						GameProgress.AddSandboxStar(sandboxTitle.name, "StarBox0" + k);
					}
					else
					{
						GameProgress.AddSandboxStar(sandboxTitle.name, "StarBox" + k);
					}
				}
			}
		}
		if (GUI.Button(new Rect(m_buttonWidth, 0f, m_buttonWidth, m_buttonHeight), "Unlimited Sandbox Parts"))
		{
			foreach (int value in Enum.GetValues(typeof(BasePart.PartType)))
			{
				int sandboxPartCount = GameProgress.GetSandboxPartCount((BasePart.PartType)value);
				GameProgress.AddSandboxParts((BasePart.PartType)value, 99 - sandboxPartCount);
			}
		}
		if (GUI.Button(new Rect(m_buttonWidth, m_buttonHeight, m_buttonWidth, m_buttonHeight), "Set low FPS"))
		{
			Application.targetFrameRate = 25;
		}
		if (GUI.Button(new Rect(m_buttonWidth, 2f * m_buttonHeight, m_buttonWidth, m_buttonHeight), "Set high FPS"))
		{
			Application.targetFrameRate = 60;
		}
		if (GUI.Button(new Rect(m_buttonWidth, 3f * m_buttonHeight, m_buttonWidth, m_buttonHeight), "Unlock all levels"))
		{
			GameProgress.SetBool("UnlockAllLevels", true);
			GameProgress.SetBool("FullVersionUnlocked", true);
		}
		if (GUI.Button(new Rect(m_buttonWidth, 4f * m_buttonHeight, m_buttonWidth, m_buttonHeight), "Restore IAPs"))
		{
			IapManager.Instance.RestorePurchasedItems();
		}
		GUI.Label(new Rect((float)Screen.width * 0.9f, (float)Screen.height * 0.9f, (float)Screen.width * 0.1f, (float)Screen.height * 0.1f), "Debug \n(v" + BuildCustomizationLoader.Instance.ApplicationVersion + " - " + BuildCustomizationLoader.Instance.SVNRevisionNumber + ")");
	}

	private void SetThreeStarsCompletion(string level)
	{
		GameProgress.SetInt(level + "_stars", 3);
		foreach (int value in Enum.GetValues(typeof(Challenge.ChallengeType)))
		{
			GameProgress.SetLevelCompleted(level);
			GameProgress.SetInt(level + "_challenge_" + value, 1);
		}
	}
}
