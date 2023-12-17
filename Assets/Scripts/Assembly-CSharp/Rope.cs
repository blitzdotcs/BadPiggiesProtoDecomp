using System.Collections.Generic;
using UnityEngine;

public class Rope : BasePart
{
	public GameObject m_segment;

	private List<HingeJoint> m_joints = new List<HingeJoint>();

	private void Start()
	{
		if ((bool)base.GetComponent<Rigidbody>())
		{
			base.GetComponent<Rigidbody>().isKinematic = true;
		}
	}

	private void Update()
	{
	}

	public void Create(BasePart leftPart, BasePart rightPart)
	{
		Vector3 position = base.transform.position - 0.5f * base.transform.right;
		Quaternion rotation = base.transform.rotation;
		float num = 75f;
		rotation = Quaternion.AngleAxis(0f - num, Vector3.forward);
		int num2 = 8;
		HingeJoint hingeJoint = null;
		if ((bool)leftPart)
		{
			hingeJoint = leftPart.gameObject.AddComponent<HingeJoint>();
			hingeJoint.anchor = 0.5f * Vector3.right;
			hingeJoint.axis = Vector3.forward;
		}
		for (int i = 0; i < num2; i++)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(m_segment, position, rotation);
			if (hingeJoint != null)
			{
				hingeJoint.connectedBody = gameObject.GetComponent<Rigidbody>();
				m_joints.Add(hingeJoint);
			}
			if (i < num2 - 1 || (bool)rightPart)
			{
				hingeJoint = gameObject.AddComponent<HingeJoint>();
				hingeJoint.anchor = 0.5f * Vector3.right;
				hingeJoint.axis = Vector3.forward;
				if (i == num2 - 1)
				{
					hingeJoint.connectedBody = rightPart.GetComponent<Rigidbody>();
					m_joints.Add(hingeJoint);
				}
			}
			position += 0.5f * (rotation * Vector3.right);
			rotation = ((i % 2 != 0) ? Quaternion.AngleAxis(0f - num, Vector3.forward) : Quaternion.AngleAxis(num, Vector3.forward));
			gameObject.transform.parent = base.transform;
		}
		base.GetComponent<Renderer>().enabled = false;
	}

	public void FixedUpdate()
	{
		float num = 0.5f * (float)(m_joints.Count + 1);
		float num2 = 0f;
		for (int i = 0; i < m_joints.Count; i++)
		{
			Rigidbody rigidbody = m_joints[i].GetComponent<Rigidbody>();
			Rigidbody connectedBody = m_joints[i].connectedBody;
			if ((bool)rigidbody && (bool)connectedBody)
			{
				num2 += Vector3.Distance(rigidbody.position, connectedBody.position);
			}
		}
		Debug.Log(num2 + " " + num);
		if (num2 > 1.05f * num)
		{
			Rigidbody rigidbody2 = m_joints[0].GetComponent<Rigidbody>();
			Rigidbody connectedBody2 = m_joints[0].connectedBody;
			Vector3 vector = (1f + num2 - num) * 100f * (connectedBody2.position - rigidbody2.position);
			rigidbody2.AddForce(vector, ForceMode.Force);
			Debug.DrawRay(rigidbody2.position, 0.1f * vector);
			rigidbody2 = m_joints[m_joints.Count - 1].connectedBody;
			connectedBody2 = m_joints[m_joints.Count - 1].GetComponent<Rigidbody>();
			vector = (1f + num2 - num) * 100f * (connectedBody2.position - rigidbody2.position);
			rigidbody2.AddForce(vector, ForceMode.Force);
			Debug.DrawRay(rigidbody2.position, 0.1f * vector);
		}
	}
}
