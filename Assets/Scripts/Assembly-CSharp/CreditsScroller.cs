using System.Collections.Generic;
using UnityEngine;

public class CreditsScroller : MonoBehaviour
{
	private const float clusterOffset = -2f;

	private const float WrapLimitUp = 86f;

	private const float WrapLimitDown = 42f;

	private const float TouchScrollSpeedAdjustment = 0.43f;

	private const float AutoScrollSpeed = 1f;

	private bool scroll;

	private bool touchScroll;

	private float scrollSpeed;

	public void InstantiateCredits()
	{
		float y = 0f;
		List<CreditsCluster> list = new List<CreditsCluster>(GetComponentsInChildren<CreditsCluster>());
		list.Sort((CreditsCluster c1, CreditsCluster c2) => c1.gameObject.name.CompareTo(c2.gameObject.name));
		CreditsCluster[] array = list.ToArray();
		foreach (CreditsCluster creditsCluster in array)
		{
			y = creditsCluster.CreateCreditsCluster(new Vector3(0f, y, 0f)) + -2f;
		}
	}

	public void CleanCredits()
	{
		CreditsCluster[] componentsInChildren = GetComponentsInChildren<CreditsCluster>();
		foreach (CreditsCluster creditsCluster in componentsInChildren)
		{
			TextMesh[] componentsInChildren2 = creditsCluster.GetComponentsInChildren<TextMesh>();
			TextMesh[] array = componentsInChildren2;
			foreach (TextMesh textMesh in array)
			{
				Object.DestroyImmediate(textMesh.gameObject);
			}
		}
	}

	private void Start()
	{
		base.transform.position = new Vector3(base.transform.position.x, 42f, base.transform.position.z);
	}

	private void OnEnable()
	{
		scroll = true;
		touchScroll = false;
	}

	private void OnDisable()
	{
		scroll = false;
		touchScroll = false;
	}

	private void Update()
	{
		if (touchScroll)
		{
			scrollSpeed = Input.GetAxis("Mouse Y") * 0.43f;
			Vector3 position = base.gameObject.transform.position;
			base.gameObject.transform.position = new Vector3(position.x, position.y + scrollSpeed, position.z);
		}
		else if (scroll)
		{
			Vector3 position2 = base.gameObject.transform.position;
			base.gameObject.transform.position = new Vector3(position2.x, position2.y + 1f * Time.deltaTime, position2.z);
		}
		if (Input.GetMouseButtonDown(0))
		{
			touchScroll = true;
		}
		if (Input.GetMouseButtonUp(0))
		{
			touchScroll = false;
		}
		if (base.transform.position.y < 42f)
		{
			base.transform.position = new Vector3(base.transform.position.x, 86f, base.transform.position.z);
		}
		else if (base.transform.position.y > 86f)
		{
			base.transform.position = new Vector3(base.transform.position.x, 42f, base.transform.position.z);
		}
	}
}
