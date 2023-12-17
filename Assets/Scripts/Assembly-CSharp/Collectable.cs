using UnityEngine;

public class Collectable : Goal
{
	[SerializeField]
	private ParticleSystem collectedEffect;

	public bool collected;

	private bool disabled;

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

	private void DisableGoal()
	{
		if ((bool)base.GetComponent<Renderer>())
		{
			base.GetComponent<Renderer>().enabled = false;
		}
		HideChildren(base.transform);
		disabled = true;
		GetComponent<BoxCollider>().enabled = false;
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
		}
	}

	public override void OnGoalEnter()
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
			Debug.Log("Collected");
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
