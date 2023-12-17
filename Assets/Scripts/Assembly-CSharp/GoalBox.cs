using UnityEngine;

public class GoalBox : Goal
{
	private bool collected;

	private bool disabled;

	private Material m_flagVisualization;

	[SerializeField]
	private GameObject m_flagObject;

	[SerializeField]
	private GameObject m_goalAchievement;

	[SerializeField]
	private ParticleSystem m_goalParticles;

	public bool Collected
	{
		get
		{
			return collected;
		}
	}

	public bool Disabled
	{
		get
		{
			return disabled;
		}
	}

	private void Start()
	{
		m_flagVisualization = m_flagObject.GetComponent<Renderer>().material;
	}

	private void Update()
	{
		m_flagVisualization.mainTextureOffset -= Vector2.up * Time.deltaTime * 0.25f;
		m_goalAchievement.transform.position += Vector3.up * Mathf.Sin(Time.time * 4f) * Time.deltaTime;
	}

	private void DisableGoal()
	{
		disabled = true;
		GetComponent<Collider>().enabled = false;
	}

	private void HideChildren(Transform parent)
	{
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			if ((bool)child.GetComponent<Renderer>())
			{
				child.GetComponent<Renderer>().enabled = false;
			}
			child.gameObject.active = false;
			HideChildren(child);
			Debug.Log("    " + child.name);
		}
	}

	public override void OnGoalEnter()
	{
		if (!collected)
		{
			m_flagObject.GetComponent<Animation>().Play();
			m_goalAchievement.GetComponent<Animation>().Play();
			m_goalParticles.Stop();
			WPFMonoBehaviour.levelManager.NotifyGoalReached();
			collected = true;
			PlayPigLaughter();
			DisableGoal();
		}
	}

	private void PlayPigLaughter()
	{
		Pig pig = Object.FindObjectOfType(typeof(Pig)) as Pig;
		StartCoroutine(pig.PlayLaughterAnimation());
	}
}
