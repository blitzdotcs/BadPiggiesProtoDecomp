using UnityEngine;

public class PageDot : MonoBehaviour
{
	private GameObject m_spriteOn;

	private GameObject m_spriteOff;

	private void Awake()
	{
		m_spriteOn = base.transform.Find("SpriteOn").gameObject;
		m_spriteOff = base.transform.Find("SpriteOff").gameObject;
	}

	public void Enable()
	{
		m_spriteOn.active = true;
		m_spriteOff.active = false;
	}

	public void Disable()
	{
		m_spriteOn.active = false;
		m_spriteOff.active = true;
	}
}
