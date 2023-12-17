using UnityEngine;
using UnityEngine.UI;

public class GUITextureAspectFix : MonoBehaviour
{
	protected Vector3 m_origScale;

	public void Awake()
	{
		m_origScale = base.transform.localScale;
		FixAspect();
	}

	public void LateUpdate()
	{
		FixAspect();
	}

	private void FixAspect()
	{
		Image component = GetComponent<Image>();
		Vector3 origScale = m_origScale;
		float num = (float)Screen.width / (float)Screen.height;
	//	float num2 = (float)component.texture.width / (float)component.texture.height;
		origScale.y *= num;
		base.transform.localScale = origScale;
	}
}
