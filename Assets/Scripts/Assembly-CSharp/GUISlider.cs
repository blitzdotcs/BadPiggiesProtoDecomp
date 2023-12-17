using System;
using UnityEngine;
using UnityEngine.UI;

public class GUISlider : MonoBehaviour
{
	public enum MoveState
	{
		Off = 0,
		On = 1
	}

	public Vector3 m_posActive;

	public Vector3 m_posInactive;

	public float m_alphaActive = 1f;

	public float m_alphaInactive = 1f;

	public float m_activationTime = 1f;

	public float m_deactivationTime = 1f;

	public bool m_isBlueprint;

	public MoveState m_state;

	protected float m_stateValue = 1f;

	public void Awake()
	{
		m_stateValue = ((m_state != MoveState.On) ? 0f : 1f);
		LateUpdate();
	}

	private void Update()
	{
		if (((Input.GetMouseButton(0) || Input.touchCount > 0) && m_isBlueprint))
		{
			m_state = MoveState.Off;
		}
	}

	public void LateUpdate()
	{
		float num = ((m_state != MoveState.On) ? (0f - m_deactivationTime) : m_activationTime);
		m_stateValue = Mathf.Clamp01(m_stateValue + num * Time.deltaTime);
		float t = (1f - Mathf.Cos(m_stateValue * (float)Math.PI)) * 0.5f;
		Vector3 position = Vector3.Lerp(m_posInactive, m_posActive, t);
		base.transform.position = position;
	}
}
