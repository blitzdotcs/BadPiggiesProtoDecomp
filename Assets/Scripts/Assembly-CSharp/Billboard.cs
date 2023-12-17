using UnityEngine;

public class Billboard : MonoBehaviour
{
	public Transform m_upFrom;

	public void LateUpdate()
	{
		Vector3 normalized = (base.transform.position - Camera.main.transform.position).normalized;
		Vector3 vector = base.transform.right;
		if ((bool)m_upFrom)
		{
			vector = Vector3.Cross(m_upFrom.up, normalized);
		}
		Vector3 normalized2 = Vector3.Cross(normalized, vector).normalized;
		Debug.DrawRay(base.transform.position, vector, Color.red);
		Debug.DrawRay(base.transform.position, normalized2, Color.green);
		base.transform.rotation = Quaternion.LookRotation(normalized, normalized2);
	}
}
