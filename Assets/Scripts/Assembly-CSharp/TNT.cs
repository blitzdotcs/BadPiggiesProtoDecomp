using UnityEngine;

public class TNT : BasePart
{
	public float m_explosionImpulse;

	public float m_explosionRadius;

	public float m_triggerSpeed;

	protected bool m_triggered;

	public GameObject smokeCloud;

	public override bool CanBeEnclosed()
	{
		return true;
	}

	public override void Initialize()
	{
		base.contraption.ChangeOneShotPartAmount(m_partType, EffectDirection(), 1);
	}

	private void OnCollisionEnter(Collision c)
	{
		float magnitude = c.relativeVelocity.magnitude;
		if (magnitude > m_triggerSpeed)
		{
			Explode();
		}
	}

	private void FixedUpdate()
	{
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, m_explosionRadius);
	}

	public override void ProcessTouch()
	{
		Explode();
	}

	public void Explode()
	{
		if (m_triggered)
		{
			return;
		}
		m_triggered = true;
		base.contraption.ChangeOneShotPartAmount(m_partType, EffectDirection(), -1);
		Collider[] array = Physics.OverlapSphere(base.transform.position, m_explosionRadius);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if ((bool)collider.GetComponent<Rigidbody>())
			{
				Vector3 vector = collider.transform.position - base.transform.position;
				float f = Mathf.Max(vector.magnitude, 1f);
				collider.GetComponent<Rigidbody>().AddForce(vector.normalized * m_explosionImpulse / Mathf.Pow(f, 1.5f), ForceMode.Impulse);
			}
			TNT component = collider.GetComponent<TNT>();
			if ((bool)component)
			{
				component.Explode();
			}
		}
		AudioManager.Instance.SpawnOneShotEffect(AudioManager.Instance.CommonAudioCollection.tntExplosion, base.transform.position);
		Object.Instantiate(smokeCloud, base.transform.position, Quaternion.identity);
		CheckForTNTAchievement();
		Object.Destroy(base.gameObject);
	}

	public void CheckForTNTAchievement()
	{
		if (DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios)
		{
			int num = GameProgress.GetInt("Broken_TNTs") + 1;
			GameProgress.SetInt("Broken_TNTs", num);
			if (num > AchievementData.Instance.GetAchievementLimit("grp.BOOM_BOOM_3"))
			{
				SocialGameManager.Instance.ReportAchievementProgress("grp.BOOM_BOOM_3", 100.0);
			}
			else if (num > AchievementData.Instance.GetAchievementLimit("grp.BOOM_BOOM_2"))
			{
				SocialGameManager.Instance.ReportAchievementProgress("grp.BOOM_BOOM_2", 100.0);
			}
			else if (num > AchievementData.Instance.GetAchievementLimit("grp.BOOM_BOOM_1"))
			{
				SocialGameManager.Instance.ReportAchievementProgress("grp.BOOM_BOOM_1", 100.0);
			}
		}
	}
}
