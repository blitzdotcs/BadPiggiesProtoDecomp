using System.Collections.Generic;
using UnityEngine;

public class BuildUI : WPFMonoBehaviour
{
	public GameObject m_partButtonPrefab;

	public GameData m_gameData;

	private ScrollList m_scrollList;

	private void Start()
	{
		m_scrollList = base.transform.Find("ScrollList").GetComponent<ScrollList>();
		List<GameObject> parts = m_gameData.m_parts;
		foreach (GameObject item in parts)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(m_partButtonPrefab);
			gameObject.transform.parent = m_scrollList.transform;
			GameObject gameObject2 = item.GetComponent<BasePart>().m_constructionIconSprite.gameObject;
			gameObject.GetComponent<DraggableButton>().DragIconPrefab = gameObject2;
			gameObject.GetComponent<DraggableButton>().DragIconScale = 1.75f;
			gameObject.transform.Find("PartCount").GetComponent<TextMesh>().text = Random.Range(1, 20).ToString();
			GameObject gameObject3 = (GameObject)Object.Instantiate(gameObject2);
			gameObject3.transform.parent = gameObject.transform;
			gameObject3.transform.localScale = new Vector3(1.75f, 1.75f, 1f);
			gameObject3.transform.localPosition = new Vector3(0f, 0f, -0.1f);
			m_scrollList.AddButton(gameObject.GetComponent<Widget>());
		}
	}
}
