using System.Collections;
using UnityEngine;

public class PigMechanic : MonoBehaviour
{
	public float m_minWrenchAnimationDelay;

	public float m_maxWrenchAnimationDelay;

	public float m_minBlinkAnimationDelay;

	public float m_maxBlinkAnimationDelay;

	private float m_wrenchAnimationTimer;

	private float m_blinkAnimationTimer;

	private GameObject m_pig;

	private Sprite m_pigSprite;

	private GameObject m_wrench;

	private void Start()
	{
		m_wrenchAnimationTimer = Random.Range(m_minWrenchAnimationDelay, m_minWrenchAnimationDelay);
		m_blinkAnimationTimer = Random.Range(m_minBlinkAnimationDelay, m_maxBlinkAnimationDelay);
		m_pig = base.transform.Find("Pig").gameObject;
		m_pigSprite = m_pig.GetComponent<Sprite>();
		m_wrench = base.transform.Find("Wrench").gameObject;
	}

	private void Update()
	{
		m_wrenchAnimationTimer -= Time.deltaTime;
		m_blinkAnimationTimer -= Time.deltaTime;
		if (m_wrenchAnimationTimer <= 0f)
		{
			m_wrench.GetComponent<Animation>().Play();
			m_pig.GetComponent<Animation>().Play();
			m_wrenchAnimationTimer = Random.Range(m_minWrenchAnimationDelay, m_minWrenchAnimationDelay);
		}
		if (m_blinkAnimationTimer <= 0f)
		{
			m_blinkAnimationTimer = Random.Range(m_minBlinkAnimationDelay, m_maxBlinkAnimationDelay);
			StartCoroutine(Blink());
		}
	}

	private IEnumerator Blink()
	{
		m_pigSprite.m_UVx = 2;
		m_pigSprite.RebuildMesh();
		yield return new WaitForSeconds(0.2f);
		m_pigSprite.m_UVx = 0;
		m_pigSprite.RebuildMesh();
	}
}
