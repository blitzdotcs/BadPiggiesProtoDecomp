using System.Collections.Generic;
using UnityEngine;

public class SandboxSelector : MonoBehaviour
{
	public EpisodeLevels m_episodeLevels;

	private List<string> m_levels = new List<string>();

	public List<string> Levels
	{
		get
		{
			return m_levels;
		}
		set
		{
			m_levels = value;
		}
	}

	private void Awake()
	{
		Levels = m_episodeLevels.Levels;
		GameManager.Instance.OpenSandboxEpisode(this);
	}

	public void LoadSandboxLevel(string index)
	{
		GameManager.Instance.LoadLevel(int.Parse(index));
	}

	public void GoToEpisodeSelection()
	{
		Loader.Instance.LoadLevel("EpisodeSelection", false);
	}
}
