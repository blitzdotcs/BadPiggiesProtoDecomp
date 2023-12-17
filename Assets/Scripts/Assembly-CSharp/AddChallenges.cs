using System.Collections.Generic;
using UnityEngine;

public class AddChallenges : MonoBehaviour
{
	public List<Challenge> m_challenges;

	private void Awake()
	{
		LevelComplete component = GameObject.Find("InGameLevelCompleteMenu").GetComponent<LevelComplete>();
		component.SetChallenges(m_challenges);
	}
}
