using System.Collections.Generic;
using UnityEngine;

public class Goal : WPFMonoBehaviour
{
	[SerializeField]
	[HideInInspector]
	private int m_goalId;

	public int GoalId
	{
		get
		{
			return m_goalId;
		}
		set
		{
			m_goalId = value;
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		BasePart basePart = FindPart(col);
		if (!basePart)
		{
			return;
		}
		BasePart basePart2 = WPFMonoBehaviour.levelManager.contraptionRunning.FindPig();
		if (basePart.ConnectedComponent == basePart2.ConnectedComponent)
		{
			OnGoalEnter();
			return;
		}
		List<BasePart> parts = WPFMonoBehaviour.levelManager.contraptionRunning.Parts;
		for (int i = 0; i < parts.Count; i++)
		{
			BasePart basePart3 = parts[i];
			if ((bool)basePart3 && basePart3.ConnectedComponent == basePart.ConnectedComponent && Vector3.Distance(basePart3.transform.position, basePart2.transform.position) < 2.5f)
			{
				OnGoalEnter();
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

	public virtual void OnGoalEnter()
	{
	}
}
