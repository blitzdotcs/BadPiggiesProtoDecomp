using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenSequence : MonoBehaviour
{
	[Serializable]
	public class SplashFrame
	{
		public GameObject m_splash;

		public float m_time;
	}

	public List<SplashFrame> m_splashes = new List<SplashFrame>();

	private List<GameObject> m_splashObjs = new List<GameObject>();

	private void Awake()
	{
		for (int i = 0; i < m_splashes.Count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_splashes[i].m_splash, Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.SetActiveRecursively(false);
			m_splashObjs.Add(gameObject);
		}
		StartCoroutine(PlaySplashSequence());
	}

	private IEnumerator PlaySplashSequence()
	{
		while (m_splashObjs.Count > 0)
		{
			m_splashObjs[0].SetActiveRecursively(true);
			yield return new WaitForSeconds(m_splashes[0].m_time);
			if (m_splashObjs.Count > 1)
			{
				UnityEngine.Object.Destroy(m_splashObjs[0]);
			}
			m_splashes.RemoveAt(0);
			m_splashObjs.RemoveAt(0);
		}
		UnityEngine.Object.Destroy(this);
		Loader.Instance.LoadLevel("MainMenu", true);
	}
}
