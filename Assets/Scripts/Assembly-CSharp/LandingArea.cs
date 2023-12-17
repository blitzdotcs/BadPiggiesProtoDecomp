using System.Collections;
using UnityEngine;

public class LandingArea : Goal
{
	protected bool m_landed;

	public Texture2D m_activeTexture;

	public Texture2D m_inactiveTexture;

	public override void OnGoalEnter()
	{
		if (!m_landed)
		{
			m_landed = true;
			StartCoroutine(ContraptionHasLanded());
		}
	}

	private IEnumerator ContraptionHasLanded()
	{
		if (WPFMonoBehaviour.levelManager.EggsCollected > 0)
		{
			base.GetComponent<Renderer>().material.mainTexture = m_activeTexture;
			yield return new WaitForSeconds(0.5f);
			base.GetComponent<Renderer>().material.mainTexture = m_inactiveTexture;
			yield return new WaitForSeconds(0.8f);
			base.GetComponent<Renderer>().material.mainTexture = m_activeTexture;
			yield return new WaitForSeconds(0.5f);
			base.GetComponent<Renderer>().material.mainTexture = m_inactiveTexture;
			yield return new WaitForSeconds(0.8f);
			Pig pig = Object.FindObjectOfType(typeof(Pig)) as Pig;
			Debug.Log(Vector3.Distance(pig.transform.position, base.transform.position));
			if (Vector3.Distance(pig.transform.position, base.transform.position) < 4f)
			{
				base.GetComponent<Renderer>().material.mainTexture = m_activeTexture;
				StartCoroutine(WPFMonoBehaviour.levelManager.LevelCompleted());
				StartCoroutine(pig.PlayLaughterAnimation());
			}
			else
			{
				m_landed = false;
				base.GetComponent<Renderer>().material.mainTexture = m_inactiveTexture;
			}
		}
		else
		{
			m_landed = false;
		}
	}
}
