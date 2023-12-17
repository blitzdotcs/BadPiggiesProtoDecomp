public class TransportChallenge : Challenge
{
	public BasePart.PartType partToTransport;

	public override bool IsCompleted()
	{
		return WPFMonoBehaviour.levelManager.IsPartTransported(partToTransport);
	}

	private void Awake()
	{
		m_type = ChallengeType.Transport;
	}
}
