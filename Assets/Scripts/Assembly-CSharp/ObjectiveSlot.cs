using System;
using UnityEngine;

public class ObjectiveSlot : MonoBehaviour
{
	private GameObject m_succeededImage;

	private void Awake()
	{
		m_succeededImage = base.transform.Find("Succeeded").gameObject;
		m_succeededImage.GetComponent<Renderer>().enabled = false;
	}

	public void SetSucceeded()
	{
		base.GetComponent<Renderer>().enabled = false;
		m_succeededImage.GetComponent<Renderer>().enabled = true;
	}

	public void SetChallenge(Challenge challenge)
	{
		foreach (Challenge.IconPlacement icon in challenge.Icons)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(icon.icon);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = icon.position;
			gameObject.transform.localScale = new Vector3(icon.scale, icon.scale, 1f);
			TimeChallenge timeChallenge = challenge as TimeChallenge;
			if ((bool)timeChallenge)
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds(timeChallenge.TimeLimit());
				string text = string.Format("{0:D2}:{1:D2}", timeSpan.Seconds, timeSpan.Milliseconds / 10);
				base.transform.Find("ObjectiveText").GetComponent<TextMesh>().text = text;
				base.transform.Find("ObjectiveText").Find("Shadow").GetComponent<TextMesh>()
					.text = text;
			}
		}
	}
}
