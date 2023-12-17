using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
	public struct ParallaxCustomLayer
	{
		public GameObject layer;

		public float speedX;
	}

	protected GameObject[] m_backgroundLayerFar;

	protected GameObject[] m_backgroundLayerNear;

	protected GameObject[] m_backgroundLayerSky;

	protected GameObject[] m_backgroundLayerForeground;

	protected float m_fgLimitY;

	protected List<ParallaxCustomLayer> m_miscellanousLayer = new List<ParallaxCustomLayer>();

	protected Vector3 m_oldPosition;

	private void Awake()
	{
		m_backgroundLayerFar = GameObject.FindGameObjectsWithTag("ParallaxLayerFar");
		m_backgroundLayerNear = GameObject.FindGameObjectsWithTag("ParallaxLayerNear");
		m_backgroundLayerSky = GameObject.FindGameObjectsWithTag("ParallaxLayerSky");
		int num = GameObject.FindGameObjectsWithTag("ParallaxLayerForeground").Length;
		m_backgroundLayerForeground = GameObject.FindGameObjectsWithTag("ParallaxLayerForeground");
		if (num > 0)
		{
			m_fgLimitY = m_backgroundLayerForeground[0].transform.position.y;
		}
		m_oldPosition = base.transform.position;
	}

	private void Update()
	{
		float num = base.transform.position.x - m_oldPosition.x;
		float num2 = base.transform.position.y - m_oldPosition.y;
		if (num != 0f)
		{
			GameObject[] backgroundLayerForeground = m_backgroundLayerForeground;
			foreach (GameObject gameObject in backgroundLayerForeground)
			{
				gameObject.transform.position -= Vector3.right * num * 0.2f;
			}
			GameObject[] backgroundLayerFar = m_backgroundLayerFar;
			foreach (GameObject gameObject2 in backgroundLayerFar)
			{
				gameObject2.transform.position += Vector3.right * num * 0.6f;
			}
			GameObject[] backgroundLayerNear = m_backgroundLayerNear;
			foreach (GameObject gameObject3 in backgroundLayerNear)
			{
				gameObject3.transform.position += Vector3.right * num * 0.4f;
			}
			GameObject[] backgroundLayerSky = m_backgroundLayerSky;
			foreach (GameObject gameObject4 in backgroundLayerSky)
			{
				gameObject4.transform.position += Vector3.right * num * 0.8f;
			}
			foreach (ParallaxCustomLayer item in m_miscellanousLayer)
			{
				item.layer.transform.position += Vector3.right * num * item.speedX;
			}
		}
		if (num2 != 0f)
		{
			GameObject[] backgroundLayerForeground2 = m_backgroundLayerForeground;
			foreach (GameObject gameObject5 in backgroundLayerForeground2)
			{
				Vector3 position = gameObject5.transform.position;
				if (position.y <= m_fgLimitY)
				{
					position -= Vector3.up * num2 * 0.2f;
				}
				else
				{
					position.y = m_fgLimitY;
				}
				gameObject5.transform.position = position;
			}
		}
		m_oldPosition = base.transform.position;
	}

	public void RegisterParallaxLayer(ParallaxCustomLayer pcl)
	{
		m_miscellanousLayer.Add(pcl);
	}
}
