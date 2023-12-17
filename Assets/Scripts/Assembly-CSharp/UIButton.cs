using UnityEngine;

public class UIButton : WPFMonoBehaviour
{
	protected enum ButtonState
	{
		Idle = 0,
		Over = 1,
		Exit = 2,
		Down = 3,
		Released = 4
	}

	protected Vector3 m_originalScale;

	protected Vector3 m_originalPosition;

	protected Quaternion m_originalRotation;

	protected Transform m_transform;

	protected ButtonState m_state;

	protected void Start()
	{
		m_originalScale = base.transform.localScale;
		m_originalPosition = base.transform.position;
		m_originalRotation = base.transform.rotation;
		m_transform = base.transform;
	}

	protected virtual void OnTouchOver()
	{
		if (m_state != ButtonState.Down)
		{
			m_transform.localScale = m_originalScale * 1.2f;
			m_state = ButtonState.Over;
		}
	}

	protected virtual void OnTouchExit()
	{
		if (m_state != ButtonState.Exit)
		{
			m_transform.localScale = m_originalScale;
			m_state = ButtonState.Exit;
		}
	}

	protected virtual void OnTouchRelease()
	{
		m_transform.localScale = m_originalScale;
		m_state = ButtonState.Released;
	}

	protected virtual void OnTouchDown()
	{
		m_transform.localScale = m_originalScale * 1.2f;
		m_state = ButtonState.Down;
	}
}
