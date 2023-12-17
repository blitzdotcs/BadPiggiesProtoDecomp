using System.Collections;
using UnityEngine;

public class Blueprint : WPFMonoBehaviour
{
	protected bool m_isTriggered;

	private void Start()
	{
		if (GameProgress.GetInt(Application.loadedLevelName + "_blueprint") == 1)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		CheckTrigger(col.transform);
	}

	private void CheckTrigger(Transform t)
	{
		if (t.tag == "Contraption" && !m_isTriggered)
		{
			BasePart component = t.GetComponent<BasePart>();
			if ((!(component != null) || (component.m_partType != BasePart.PartType.Balloon && component.m_partType != BasePart.PartType.Balloons2 && component.m_partType != BasePart.PartType.Balloons3)) && !(Vector3.Distance(t.position, WPFMonoBehaviour.levelManager.contraptionRunning.m_cameraTarget.transform.position) > 2f))
			{
				m_isTriggered = true;
				StartCoroutine(PlayAnimation());
			}
		}
		else if ((bool)t.parent)
		{
			CheckTrigger(t.parent);
		}
	}

	public IEnumerator PlayAnimation()
	{
		AudioManager.Instance.Play2dEffect(AudioManager.Instance.CommonAudioCollection.goalBoxCollected);
		float timer = 1f;
		while (timer > 0f)
		{
			base.transform.localScale += Vector3.one * 0.1f * Mathf.Pow(timer, 2f);
			base.transform.position += Vector3.up * 0.1f * Mathf.Pow(timer, 2f);
			timer -= 0.03f;
			yield return new WaitForSeconds(0.03f);
		}
		base.gameObject.GetComponent<Renderer>().enabled = false;
		GameObject particles = Object.Instantiate(WPFMonoBehaviour.gameData.m_particles, base.transform.position, Quaternion.identity) as GameObject;
	//	particles.GetComponent<ParticleEmitter>().emit = true;
		yield return new WaitForSeconds(1.5f);
		WPFMonoBehaviour.levelManager.NotifyBlueprintCollected(this);
		Object.Destroy(particles);
		Object.Destroy(base.gameObject);
	}
}
