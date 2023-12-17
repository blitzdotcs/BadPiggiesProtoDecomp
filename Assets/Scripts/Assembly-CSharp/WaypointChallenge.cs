using UnityEngine;

public class WaypointChallenge : Challenge
{
	public Collectable m_target;

	public int m_goalId;

	public override void Initialize()
	{
		m_goalId = m_target.GoalId;
	}

	public override bool IsCompleted()
	{
		GameObject goal = WPFMonoBehaviour.levelManager.GetGoal(m_goalId);
		if ((bool)goal)
		{
			return goal.GetComponent<Collectable>().Collected;
		}
		return false;
	}

	private void Awake()
	{
		m_type = ChallengeType.Box;
	}
}
