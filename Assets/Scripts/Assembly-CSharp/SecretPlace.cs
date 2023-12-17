using UnityEngine;

public class SecretPlace : OneTimeCollectable
{
	private bool m_disablingGoal;

	private float m_animationTimer;

	private void Awake()
	{
		m_disablingGoal = false;
		if (!GameProgress.GetBool("SECRET_DISCOVERED_" + Application.loadedLevelName))
		{
		}
	}

	public override void Collect()
	{
		if (!collected)
		{
			if ((bool)collectedEffect)
			{
				Object.Instantiate(collectedEffect, base.transform.position, base.transform.rotation);
			}
			AudioManager instance = AudioManager.Instance;
			instance.Play2dEffect(instance.CommonAudioCollection.bonusBoxCollected);
			collected = true;
			m_disablingGoal = true;
			m_animationTimer = 0f;
			OnCollected();
		}
	}

	private void Update()
	{
		if (m_disablingGoal)
		{
			m_animationTimer += Time.deltaTime;
			if (m_animationTimer < 0.2f)
			{
				base.transform.localScale += Vector3.one * Time.deltaTime;
				return;
			}
			if (m_animationTimer < 1f)
			{
				base.transform.localScale -= Vector3.one * Time.deltaTime;
				return;
			}
			DisableGoal();
			m_disablingGoal = false;
		}
	}

	public override void OnCollected()
	{
		GameProgress.SetBool("SECRET_DISCOVERED_" + Application.loadedLevelName, true);
		int @int = GameProgress.GetInt("SECRETS_DISCOVERED_" + GameManager.Instance.CurrentEpisode);
		GameProgress.SetInt("SECRETS_DISCOVERED_" + GameManager.Instance.CurrentEpisode, @int);
		if (GameManager.Instance.CurrentEpisode.CompareTo("Episode1LevelSelection") == 0)
		{
			if (DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios && @int >= AchievementData.Instance.GetAchievementLimit("grp.IS_IT_SECRET"))
			{
				SocialGameManager.Instance.ReportAchievementProgress("grp.IS_IT_SECRET", 100.0);
			}
		}
		else if (GameManager.Instance.CurrentEpisode.CompareTo("Episode2LevelSelection") == 0 && DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios && @int >= AchievementData.Instance.GetAchievementLimit("grp.SECRET_ADMIRER"))
		{
			SocialGameManager.Instance.ReportAchievementProgress("grp.SECRET_ADMIRER", 100.0);
		}
	}
}
