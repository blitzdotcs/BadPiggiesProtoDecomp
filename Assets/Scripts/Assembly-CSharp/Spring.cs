using UnityEngine;

public class Spring : BasePart
{
	public GameObject m_endPointPrefab;

	private Rigidbody m_connectedBody;

	private GameObject m_visualization;

	private Vector3 m_localConnectionPoint;

	private Vector3 m_remoteConnectionPoint;

	public override void Awake()
	{
		base.Awake();
		m_visualization = base.transform.Find("SpringVisualization").gameObject;
	}

	private void Update()
	{
		if ((bool)base.GetComponent<Rigidbody>() && (bool)m_connectedBody)
		{
			Vector3 vector = base.transform.TransformPoint(m_localConnectionPoint);
			Vector3 vector2 = m_connectedBody.transform.TransformPoint(m_remoteConnectionPoint);
			Debug.DrawRay(vector, vector2 - vector);
			Vector3 localScale = m_visualization.transform.localScale;
			localScale.y = Vector3.Distance(vector, vector2);
			m_visualization.transform.localScale = localScale;
			m_visualization.transform.position = 0.5f * (vector + vector2);
		}
	}

	public override Joint CustomConnectToPart(BasePart part)
	{
		ConfigurableJoint configurableJoint = base.gameObject.AddComponent<ConfigurableJoint>();
		configurableJoint.connectedBody = part.GetComponent<Rigidbody>();
		configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Limited;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		SoftJointLimit linearLimit = configurableJoint.linearLimit;
		linearLimit.limit = 0f;
	//	linearLimit.spring = 30f;
		linearLimit.bounciness = 0f;
		configurableJoint.linearLimit = linearLimit;
		m_connectedBody = part.GetComponent<Rigidbody>();
		m_localConnectionPoint = new Vector3(0f, 0.5f, 0f);
		m_remoteConnectionPoint = part.transform.InverseTransformPoint(base.transform.position - 0.5f * base.transform.up);
		return configurableJoint;
	}

	public void CreateSpringBody(Direction direction)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(m_endPointPrefab, base.transform.position, base.transform.rotation);
		ConfigurableJoint configurableJoint = base.gameObject.AddComponent<ConfigurableJoint>();
		configurableJoint.connectedBody = gameObject.GetComponent<Rigidbody>();
		configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Limited;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		SoftJointLimit linearLimit = configurableJoint.linearLimit;
		linearLimit.limit = 0f;
	//	linearLimit.spring = 30f;
		linearLimit.bounciness = 0f;
		configurableJoint.linearLimit = linearLimit;
		gameObject.transform.parent = base.transform;
		m_connectedBody = gameObject.GetComponent<Rigidbody>();
		m_localConnectionPoint = new Vector3(0f, 0.5f, 0f);
		m_remoteConnectionPoint = gameObject.transform.InverseTransformPoint(base.transform.position - 0.5f * base.transform.up);
	}
}
