using UnityEngine;

public class AddLevelsToSelector : MonoBehaviour
{
	public EpisodeLevels m_episodeLevels;

	private void Awake()
	{
		LevelSelector component = GameObject.Find("LevelSelector").GetComponent<LevelSelector>();
		component.Levels = m_episodeLevels.Levels;
	}
}
