using UnityEngine;

public class BalloonBalancer : MonoBehaviour
{
	private int m_balloonCount;

	private ConfigurableJoint m_joint;

	public void AddBalloon()
	{
		m_balloonCount++;
		if (!m_joint)
		{
			ConfigurableJoint configurableJoint = base.gameObject.AddComponent<ConfigurableJoint>();
			configurableJoint.configuredInWorldSpace = true;
			configurableJoint.anchor = Vector3.zero;
			configurableJoint.angularZMotion = ConfigurableJointMotion.Limited;
			SoftJointLimit angularZLimit = configurableJoint.angularZLimit;
			angularZLimit.limit = 0f;
			angularZLimit.bounciness = 10f;
		//	angularZLimit.spring = 0f;
			configurableJoint.angularZLimit = angularZLimit;
			angularZLimit = configurableJoint.linearLimit;
			angularZLimit.limit = 0.1f;
		//	angularZLimit.spring = 30f;
		//	angularZLimit.damper = 10f;
			configurableJoint.linearLimit = angularZLimit;
			configurableJoint.breakForce = 2f;
			configurableJoint.breakTorque = 5f;
			m_joint = configurableJoint;
		}
	}

	public void RemoveBalloon()
	{
		m_balloonCount--;
		if ((bool)m_joint && m_balloonCount == 0)
		{
			Object.Destroy(m_joint);
			m_joint = null;
		}
	}

	public void Configure(float powerFactor)
	{
		if ((bool)m_joint)
		{
			SoftJointLimit angularZLimit = m_joint.angularZLimit;
		//	angularZLimit.spring = (float)m_balloonCount * 20f * powerFactor;
			m_joint.angularZLimit = angularZLimit;
		}
	}
}
