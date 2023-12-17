using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraPreview : WPFMonoBehaviour
{
	public enum EasingAnimation
	{
		None = 0,
		EasingIn = 1,
		EasingOut = 2,
		EasingInOut = 3
	}

	[Serializable]
	public class CameraControlPoint
	{
		public Vector2 position;

		public float zoom = 2f;

		public EasingAnimation easing;

		public CameraControlPoint()
		{
			zoom = 1f;
			easing = EasingAnimation.EasingInOut;
		}
	}

	public EasingAnimation m_easing;

	public List<CameraControlPoint> m_controlPoints = new List<CameraControlPoint>();

	public float m_animationTime;

	private int m_currentControlPointIndex = 1;

	private float m_timer;

	private bool m_done;

	private int m_fastPreviewMultiplier = 1;

	public bool Done
	{
		get
		{
			return m_done;
		}
	}

	private void Awake()
	{
		if (m_controlPoints.Count >= 3)
		{
			m_controlPoints.Insert(0, m_controlPoints[0]);
			m_controlPoints.Add(m_controlPoints[m_controlPoints.Count - 1]);
		}
	}

	public void UpdateCameraPreview(ref Vector3 cameraPosition, ref float cameraOrtoSize)
	{
		if (m_done)
		{
			return;
		}
		if (Input.GetMouseButton(0) || Input.touchCount > 0)
		{
			m_fastPreviewMultiplier = 6;
		}
		Vector2 vector = cameraPosition;
		m_timer += GameTime.RealTimeDelta * (float)m_fastPreviewMultiplier;
		float num = (vector - new Vector2(m_controlPoints[m_currentControlPointIndex + 1].position.x, m_controlPoints[m_currentControlPointIndex + 1].position.y)).magnitude;
		if (m_timer > m_animationTime / (float)(m_controlPoints.Count - 3))
		{
			m_timer = m_animationTime / (float)(m_controlPoints.Count - 3);
			num = 0f;
		}
		if (num < 0.5f)
		{
			m_currentControlPointIndex++;
			m_timer = 0f;
			if (m_currentControlPointIndex == m_controlPoints.Count - 2)
			{
				m_done = true;
				return;
			}
		}
		float i;
		switch (m_easing)
		{
		case EasingAnimation.EasingIn:
			i = MathsUtil.EasingInQuad(m_timer, 0f, 1f, m_animationTime / (float)(m_controlPoints.Count - 3));
			break;
		case EasingAnimation.EasingInOut:
			i = MathsUtil.EaseInOutQuad(m_timer, 0f, 1f, m_animationTime / (float)(m_controlPoints.Count - 3));
			break;
		case EasingAnimation.EasingOut:
			i = MathsUtil.EasingOutQuad(m_timer, 0f, 1f, m_animationTime / (float)(m_controlPoints.Count - 3));
			break;
		default:
			i = m_timer / (m_animationTime / (float)(m_controlPoints.Count - 3));
			break;
		}
		float x = MathsUtil.CatmullRomInterpolate(m_controlPoints[m_currentControlPointIndex - 1].position.x, m_controlPoints[m_currentControlPointIndex].position.x, m_controlPoints[m_currentControlPointIndex + 1].position.x, m_controlPoints[m_currentControlPointIndex + 2].position.x, i);
		float y = MathsUtil.CatmullRomInterpolate(m_controlPoints[m_currentControlPointIndex - 1].position.y, m_controlPoints[m_currentControlPointIndex].position.y, m_controlPoints[m_currentControlPointIndex + 1].position.y, m_controlPoints[m_currentControlPointIndex + 2].position.y, i);
		float num2 = MathsUtil.CatmullRomInterpolate(m_controlPoints[m_currentControlPointIndex - 1].zoom, m_controlPoints[m_currentControlPointIndex].zoom, m_controlPoints[m_currentControlPointIndex + 1].zoom, m_controlPoints[m_currentControlPointIndex + 2].zoom, i);
		cameraPosition = new Vector3(x, y, cameraPosition.z);
		cameraOrtoSize = num2;
	}
}
