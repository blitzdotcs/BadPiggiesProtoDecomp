public class PerfectFlightChallenge : Challenge
{
	public override bool IsCompleted()
	{
		return !WPFMonoBehaviour.levelManager.contraptionRunning.IsBroken();
	}

	private void Awake()
	{
		m_type = ChallengeType.PerfectFlight;
	}
}
