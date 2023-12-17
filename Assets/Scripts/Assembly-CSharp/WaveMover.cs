using System;
using UnityEngine;

public class WaveMover : MonoBehaviour
{
	public float m_rangeX;

	public float m_speedX;

	public float m_rangeY;

	public float m_speedY;

	protected Vector3 m_origPos;

	protected float m_periodX;

	protected float m_periodY;

	private Transform cachedTransform;

	private void Start()
	{
		m_origPos = base.transform.position;
		cachedTransform = base.transform;
	}

	private void Update()
	{
		float num = Time.deltaTime * m_speedX;
		m_periodX += num;
		if (m_periodX > (float)Math.PI)
		{
			m_periodX -= (float)Math.PI * 2f;
		}
		m_periodY += Time.deltaTime * m_speedY;
		if (m_periodY > (float)Math.PI)
		{
			m_periodY -= (float)Math.PI * 2f;
		}
		Vector3 position = m_origPos + cachedTransform.up * Mathf.Sin(m_periodX) * m_rangeX + cachedTransform.right * Mathf.Sin(m_periodY) * m_rangeY;
		float to = ((!(cachedTransform.position.y - position.y <= 0f)) ? 50 : 0);
		cachedTransform.position = position;
		foreach (Transform item in cachedTransform)
		{
			float x = item.eulerAngles.x;
			item.eulerAngles = new Vector3(Mathf.Lerp(x, to, num), 0f, 0f);
		}
	}
}
