using System.Collections.Generic;
using UnityEngine;

public class OneTimeCollectable : WPFMonoBehaviour
{
	[SerializeField]
	protected ParticleSystem collectedEffect;

	public bool collected;

	private bool disabled;

	private void OnTriggerEnter(Collider col)
	{
		if (disabled)
		{
			return;
		}
		BasePart basePart = FindPart(col);
		if (!basePart)
		{
			return;
		}
		BasePart basePart2 = WPFMonoBehaviour.levelManager.contraptionRunning.FindPig();
		if (basePart.ConnectedComponent == basePart2.ConnectedComponent)
		{
			Collect();
			return;
		}
		List<BasePart> parts = WPFMonoBehaviour.levelManager.contraptionRunning.Parts;
		for (int i = 0; i < parts.Count; i++)
		{
			BasePart basePart3 = parts[i];
			if (basePart3.ConnectedComponent == basePart.ConnectedComponent && Vector3.Distance(basePart3.transform.position, basePart2.transform.position) < 2.5f)
			{
				Collect();
				break;
			}
		}
	}

	private BasePart FindPart(Collider collider)
	{
		Transform parent = collider.transform;
		while (parent != null)
		{
			BasePart component = parent.GetComponent<BasePart>();
			if ((bool)component)
			{
				return component;
			}
			parent = parent.parent;
		}
		return null;
	}

	protected void DisableGoal()
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

	public virtual void Collect()
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
			DisableGoal();
			OnCollected();
		}
	}

	public virtual void OnCollected()
	{
		Debug.Log("Collected: " + base.name);
	}
}
