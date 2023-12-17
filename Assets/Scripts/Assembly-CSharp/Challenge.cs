using System;
using System.Collections.Generic;
using UnityEngine;

public class Challenge : WPFMonoBehaviour
{
	public enum ChallengeType
	{
		DontUseParts = 0,
		Time = 1,
		PerfectFlight = 2,
		Transport = 3,
		Box = 4,
		Max = 5
	}

	public class ChallengeOrder : IComparer<Challenge>
	{
		public int Compare(Challenge obj1, Challenge obj2)
		{
			return string.Compare(obj1.name, obj2.name);
		}
	}

	[Serializable]
	public class IconPlacement
	{
		public Vector3 position;

		public float scale = 1f;

		public GameObject icon;
	}

	public List<IconPlacement> m_icons;

	protected ChallengeType m_type;

	public int Type
	{
		get
		{
			return (int)m_type;
		}
	}

	public List<IconPlacement> Icons
	{
		get
		{
			return m_icons;
		}
	}

	public virtual void Initialize()
	{
	}

	public virtual bool IsCompleted()
	{
		return false;
	}

	public virtual float TimeLimit()
	{
		return 0f;
	}
}
