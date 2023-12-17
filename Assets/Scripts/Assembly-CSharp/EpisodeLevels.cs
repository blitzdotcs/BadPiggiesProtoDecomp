using System.Collections.Generic;
using UnityEngine;

public class EpisodeLevels : MonoBehaviour
{
	public List<string> m_levels = new List<string>();

	public List<string> Levels
	{
		get
		{
			return m_levels;
		}
	}
}
