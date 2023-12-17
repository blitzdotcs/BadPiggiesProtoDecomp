using System.Collections.Generic;
using UnityEngine;

public class PartSelector : WPFMonoBehaviour, WidgetListener
{
	public class PartDescOrder : IComparer<ConstructionUI.PartDesc>
	{
		public int Compare(ConstructionUI.PartDesc obj1, ConstructionUI.PartDesc obj2)
		{
			if (obj1.sortKey < obj2.sortKey)
			{
				return -1;
			}
			if (obj1.sortKey > obj2.sortKey)
			{
				return 1;
			}
			return 0;
		}
	}

	public GameObject m_partButtonPrefab;

	public GameData m_gameData;

	private ScrollList m_scrollList;

	private List<ConstructionUI.PartDesc> m_partDescs;

	private ConstructionUI m_constructionUI;

	private Dictionary<BasePart.PartType, int> m_partOrder = new Dictionary<BasePart.PartType, int>();

	public GameObject FindPartButton(ConstructionUI.PartDesc partDesc)
	{
		return m_scrollList.FindButton(partDesc);
	}

	public void SetSelection(ConstructionUI.PartDesc targetObject)
	{
		m_scrollList.SetSelection(targetObject);
	}

	public void Select(Widget widget, object targetObject)
	{
		if ((bool)m_constructionUI)
		{
			m_constructionUI.SelectPart((ConstructionUI.PartDesc)targetObject);
		}
	}

	public void StartDrag(Widget widget, object targetObject)
	{
		if ((bool)m_constructionUI)
		{
			m_constructionUI.StartDrag((ConstructionUI.PartDesc)targetObject);
		}
	}

	public void CancelDrag(Widget widget, object targetObject)
	{
		if ((bool)m_constructionUI)
		{
			m_constructionUI.CancelDrag((ConstructionUI.PartDesc)targetObject);
		}
	}

	public void Drop(Widget widget, Vector3 dropPosition, object targetObject)
	{
	}

	public void SetParts(List<ConstructionUI.PartDesc> partDescs)
	{
		m_partDescs = new List<ConstructionUI.PartDesc>(partDescs);
		foreach (ConstructionUI.PartDesc partDesc in m_partDescs)
		{
			Assert.Check(m_partOrder.ContainsKey(partDesc.part.m_partType), "Part not found in order list: " + partDesc.part.m_partType);
			partDesc.sortKey = m_partOrder[partDesc.part.m_partType];
		}
		m_partDescs.Sort(new PartDescOrder());
		CreatePartList(false);
	}

	private void Awake()
	{
		m_scrollList = base.transform.Find("ScrollList").GetComponent<ScrollList>();
		m_scrollList.SetListener(this);
		if (DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Pc || DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Osx)
		{
			m_scrollList.SetMaxRows(2);
		}
		ReadPartOrder();
		if (!WPFMonoBehaviour.levelManager)
		{
			CreateTestPartList();
		}
	}

	private void Start()
	{
		m_constructionUI = WPFMonoBehaviour.FindObjectComponent<ConstructionUI>("ConstructionUI");
	}

	private void ReadPartOrder()
	{
		m_partOrder.Clear();
		string text = m_gameData.m_partOrderList.text;
		char[] separator = new char[1] { '\n' };
		string[] array = text.Split(separator);
		int num = 0;
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			string text3 = text2.Trim();
			if (!(text3 != string.Empty))
			{
				continue;
			}
			BasePart.PartType key = BasePart.PartType.Unknown;
			foreach (GameObject part in m_gameData.m_parts)
			{
				BasePart.PartType partType = part.GetComponent<BasePart>().m_partType;
				if (partType.ToString() == text3)
				{
					key = partType;
					break;
				}
			}
			m_partOrder[key] = num;
			num++;
		}
	}

	private void CreatePartList(bool handleDragIcons)
	{
		foreach (ConstructionUI.PartDesc partDesc in m_partDescs)
		{
			BasePart part = partDesc.part;
			GameObject gameObject = (GameObject)Object.Instantiate(m_partButtonPrefab);
			gameObject.transform.parent = m_scrollList.transform;
			GameObject gameObject2 = part.m_constructionIconSprite.gameObject;
			gameObject.GetComponent<DraggableButton>().DragObject = partDesc;
			if (handleDragIcons)
			{
				gameObject.GetComponent<DraggableButton>().DragIconPrefab = gameObject2;
			}
			gameObject.GetComponent<DraggableButton>().DragIconScale = 1.75f;
			int renderQueue = 3001;
			gameObject.transform.Find("PartCount").GetComponent<TextMesh>().text = partDesc.maxCount.ToString();
			gameObject.transform.Find("PartCount").GetComponent<PartCounter>().m_partType = partDesc.part.m_partType;
			gameObject.GetComponent<Renderer>().sharedMaterial.renderQueue = renderQueue;
			gameObject.transform.Find("PartCount").GetComponent<Renderer>().sharedMaterial.renderQueue = renderQueue;
			GameObject gameObject3 = (GameObject)Object.Instantiate(gameObject2);
			gameObject3.transform.parent = gameObject.transform;
			gameObject3.transform.localScale = new Vector3(1.75f, 1.75f, 1f);
			gameObject3.transform.localPosition = new Vector3(0f, 0f, -0.1f);
			gameObject3.GetComponent<Renderer>().sharedMaterial.renderQueue = renderQueue;
			gameObject.GetComponent<DraggableButton>().Icon = gameObject3;
			m_scrollList.AddButton(gameObject.GetComponent<Widget>());
		}
	}

	private void CreateTestPartList()
	{
		m_partDescs = new List<ConstructionUI.PartDesc>();
		foreach (GameObject part in m_gameData.m_parts)
		{
			ConstructionUI.PartDesc partDesc = new ConstructionUI.PartDesc();
			partDesc.part = part.GetComponent<BasePart>();
			partDesc.sortKey = m_partOrder[part.GetComponent<BasePart>().m_partType];
			partDesc.maxCount = Random.Range(0, 20);
			m_partDescs.Add(partDesc);
		}
		m_partDescs.Sort(new PartDescOrder());
		CreatePartList(true);
	}
}
