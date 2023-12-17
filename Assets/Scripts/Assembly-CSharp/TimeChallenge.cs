public class TimeChallenge : Challenge
{
	public float m_targetTime;

	public override bool IsCompleted()
	{
		return WPFMonoBehaviour.levelManager.CompletionTime <= m_targetTime;
	}

	public override float TimeLimit()
	{
		return m_targetTime;
	}

	private void Awake()
	{
		m_type = ChallengeType.Time;
	}
}
