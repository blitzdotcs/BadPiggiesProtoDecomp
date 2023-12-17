using UnityEngine;

public class DontUsePartChallenge : Challenge
{
	public BasePart.PartType m_partType;

	public override bool IsCompleted()
	{
		return !WPFMonoBehaviour.levelManager.IsPartUsed(m_partType);
	}

	private void Awake()
	{
		if (m_icons.Count >= 2)
		{
			Debug.Log(WPFMonoBehaviour.gameData.name);
			Debug.Log(WPFMonoBehaviour.gameData.GetPart(m_partType).name);
			m_icons[1].icon = WPFMonoBehaviour.gameData.GetPart(m_partType).GetComponent<BasePart>().m_constructionIconSprite.gameObject;
		}
		m_type = ChallengeType.DontUseParts;
	}
}
