using System.Collections.Generic;
using UnityEngine;

public class ButtonList : MonoBehaviour
{
	public class NameComparer : IComparer<GameObject>
	{
		public int Compare(GameObject obj1, GameObject obj2)
		{
			return string.Compare(obj1.name, obj2.name);
		}
	}

	public float m_spacing = 1f;

	public List<GameObject> m_buttons;

	public bool m_refreshPlacementInEditor;

	private Vector2 m_spriteSize;

	private float m_screenWidth;

	private float m_screenHeight;

	public List<GameObject> Buttons
	{
		get
		{
			return m_buttons;
		}
	}

	private void Awake()
	{
		m_buttons = new List<GameObject>();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			GameObject item = base.transform.GetChild(i).gameObject;
			m_buttons.Add(item);
		}
		m_buttons.Sort(new NameComparer());
		if (m_buttons.Count > 0)
		{
			m_spriteSize = m_buttons[0].GetComponent<Sprite>().Size;
		}
	}

	public void Sort(IComparer<GameObject> comparer)
	{
		m_buttons.Sort(comparer);
		PlaceButtons(Screen.width, Screen.height);
	}

	private void Update()
	{
		if (m_screenWidth != (float)Screen.width || m_screenHeight != (float)Screen.height)
		{
			PlaceButtons(Screen.width, Screen.height);
			m_screenWidth = Screen.width;
			m_screenHeight = Screen.height;
		}
	}

	private void PlaceButtons(float screenWidth, float screenHeight)
	{
		int num = 0;
		foreach (GameObject button in m_buttons)
		{
			if (button.GetComponent<Renderer>().enabled)
			{
				num++;
			}
		}
		float num2 = m_spriteSize.x + m_spacing;
		Vector3 vector = base.transform.position - 0.5f * ((float)(num - 1) * num2) * Vector3.right;
		float num3 = 1f;
		float num4 = vector.x - base.transform.position.x;
		float num5 = 10f * screenWidth / screenHeight - 0.5f * num2;
		if (num4 < 0f - num5)
		{
			num2 = m_spriteSize.x + 0.25f * m_spacing;
			num4 = (base.transform.position - 0.5f * ((float)(num - 1) * num2) * Vector3.right).x - base.transform.position.x;
			num5 = 10f * screenWidth / screenHeight - 0.5f * num2;
		}
		if (num4 < 0f - num5)
		{
			num3 = (0f - num5) / num4;
			base.transform.localScale = new Vector3(num3, 1f, 1f);
		}
		else
		{
			base.transform.localScale = Vector3.one;
		}
		vector = base.transform.position - num3 * (0.5f * ((float)(num - 1) * num2) * Vector3.right);
		num2 *= num3;
		foreach (GameObject button2 in m_buttons)
		{
			if (button2.GetComponent<Renderer>().enabled)
			{
				button2.transform.position = vector;
				vector += num2 * Vector3.right;
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (m_refreshPlacementInEditor)
		{
			Awake();
			PlaceButtons(1024f, 768f);
			m_refreshPlacementInEditor = false;
		}
	}
}
