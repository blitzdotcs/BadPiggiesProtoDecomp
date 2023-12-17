using System.Collections;
using UnityEngine;

public class TimedHide : MonoBehaviour
{
	[SerializeField]
	private float lifeTime = 1f;

	public void Show()
	{
		base.GetComponent<Renderer>().enabled = true;
		StartCoroutine(HideCountdown());
	}

	private void Start()
	{
		StartCoroutine(HideCountdown());
	}

	private IEnumerator HideCountdown()
	{
		yield return new WaitForSeconds(lifeTime);
		base.GetComponent<Renderer>().enabled = false;
	}
}
