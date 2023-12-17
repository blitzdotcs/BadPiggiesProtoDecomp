using UnityEngine;

public class RopeVisualization : MonoBehaviour
{
	public Material m_stringMaterial;

	public Vector3 m_pos1Anchor;

	public Vector3 m_pos2Anchor;

	public Transform m_pos2Transform;

	public void Start()
	{
		LineRenderer lineRenderer = base.gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = m_stringMaterial;
		lineRenderer.SetVertexCount(2);
		lineRenderer.SetWidth(0.05f, 0.05f);
		lineRenderer.SetColors(Color.black, Color.black);
	}

	public void LateUpdate()
	{
		LineRenderer component = GetComponent<LineRenderer>();
		Vector3 position = base.transform.TransformPoint(m_pos1Anchor);
		Vector3 position2 = m_pos2Transform.TransformPoint(m_pos2Anchor);
		component.SetPosition(0, position);
		component.SetPosition(1, position2);
	}
}
