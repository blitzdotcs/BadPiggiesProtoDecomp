using System.Collections.Generic;
using UnityEngine;

public class ConstructionUI : WPFMonoBehaviour
{
	public class PartDesc
	{
		public BasePart part;

		public Texture tex;

		public Rect texCoords;

		public int coordX;

		public int coordY;

		public int useCount;

		public int maxCount;

		public int sortKey;

		public int CurrentCount
		{
			get
			{
				return maxCount - useCount;
			}
		}
	}

	public List<Transform> m_purchasableParts = new List<Transform>();

	public Transform m_gridPrefab;

	public Transform m_cellPrefab;

	public int m_itemsPerRow = 14;

	public float m_offsetX = 0.2f;

	public float m_offsetY = 0.2f;

	public float m_spacingX = 0.06f;

	public float m_scale = 0.1f;

	public Font m_font;

	public Texture2D m_cellTextureValid;

	public Texture2D m_cellTextureInvalid;

	public Texture2D m_textureSelected;

	protected Transform m_grid;

	protected Dictionary<int, Transform> m_cellMap = new Dictionary<int, Transform>();

	protected List<Transform> m_partInstances = new List<Transform>();

	protected List<PartDesc> m_partDescs = new List<PartDesc>();

	protected List<PartDesc> m_purchasablePartDescs = new List<PartDesc>();

	protected Contraption m_contraption;

	protected int m_draggedElement = -1;

	protected int m_flipCount;

	protected int m_selectedElement = -1;

	protected BasePart.GridRotation draggedPartRotation;

	protected bool draggedPartFlipped;

	protected GUIStyle m_textStyle;

	private GameObject clearButton;

	private GameObject playButton;

	private GameObject moveButtons;

	private GameObject moveLeftButton;

	private GameObject moveRightButton;

	private GameObject moveUpButton;

	private GameObject moveDownButton;

	private List<Transform> m_parts = new List<Transform>();

	private PartSelector partSelector;

	private GameObject m_dragIcon;

	private bool m_useDragOffset = true;

	private Vector3 m_dragOffset;

	private Transform m_mouseOverCell;

	private bool m_dragStarted;

	private Vector3 m_dragStartPosition;

	private Vector3 m_rightDragStartPosition;

	private int m_moveCounter;

	private int m_rotationCounter;

	public List<PartDesc> PartDescriptors
	{
		get
		{
			return m_partDescs;
		}
	}

	public int RotationCount
	{
		get
		{
			return m_rotationCounter;
		}
	}

	public int MoveCount
	{
		get
		{
			return m_moveCounter;
		}
	}

	private void AddMove()
	{
		m_moveCounter++;
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<UIEvent>(ReceiveUIEvent);
	}

	public PartDesc FindPartDesc(BasePart.PartType partType)
	{
		foreach (PartDesc partDesc in m_partDescs)
		{
			if (partDesc.part.m_partType == partType)
			{
				return partDesc;
			}
		}
		return null;
	}

	private void UpdatePartCount(int newCount)
	{
		foreach (PartDesc purchasablePartDesc in m_purchasablePartDescs)
		{
			if (purchasablePartDesc.part.m_partType == BasePart.PartType.Rocket)
			{
				Debug.Log("part count updated");
				purchasablePartDesc.maxCount = newCount;
			}
		}
	}

	private void Awake()
	{
		EventManager.Connect<UIEvent>(ReceiveUIEvent);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		m_useDragOffset = DeviceInfo.Instance.UsesTouchInput && !BuildCustomizationLoader.Instance.IsHDVersion;
		foreach (GameObject part in WPFMonoBehaviour.gameData.m_parts)
		{
			m_parts.Add(part.transform);
		}
		foreach (Transform part2 in m_parts)
		{
			Transform transform = Object.Instantiate(part2) as Transform;
			BasePart component = transform.GetComponent<BasePart>();
			int partTypeCount = WPFMonoBehaviour.levelManager.GetPartTypeCount(component.m_partType);
			if (partTypeCount == 0 && !m_purchasableParts.Contains(part2))
			{
				Object.Destroy(transform.gameObject);
				continue;
			}
			transform.gameObject.SetActiveRecursively(false);
			transform.parent = base.transform;
			Rect texCoords = new Rect(0f, 0f, 1f, 1f);
			MeshRenderer component2 = transform.GetComponent<MeshRenderer>();
			if ((bool)transform.GetComponent<BasePart>().m_constructionIconSprite)
			{
				component2 = transform.GetComponent<BasePart>().m_constructionIconSprite.GetComponent<MeshRenderer>();
				texCoords = transform.GetComponent<BasePart>().m_constructionIconSprite.m_uvRect;
			}
			if (!component2 || !component2.sharedMaterial)
			{
				continue;
			}
			Texture mainTexture = component2.sharedMaterial.mainTexture;
			if ((bool)mainTexture)
			{
				PartDesc partDesc = new PartDesc();
				partDesc.part = component;
				partDesc.tex = mainTexture;
				partDesc.texCoords = texCoords;
				partDesc.coordX = num;
				partDesc.coordY = num2;
				partDesc.useCount = 0;
				partDesc.maxCount = partTypeCount;
				m_partDescs.Add(partDesc);
				WPFMonoBehaviour.levelManager.m_totalAvailableParts += partTypeCount;
				num++;
				if (num >= m_itemsPerRow)
				{
					num = 0;
					num2++;
				}
				num3++;
			}
		}
		if ((bool)WPFMonoBehaviour.levelManager)
		{
			m_contraption = WPFMonoBehaviour.levelManager.contraptionProto;
		}
		if (!m_contraption)
		{
			GameObject gameObject = new GameObject("Contraption");
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			m_contraption = gameObject.AddComponent<Contraption>();
		}
		if ((bool)m_cellPrefab)
		{
			GameObject gameObject2 = new GameObject();
			gameObject2.transform.parent = base.transform;
			gameObject2.transform.localPosition = Vector3.zero;
			for (int i = 0; i < WPFMonoBehaviour.levelManager.gridHeight; i++)
			{
				for (int j = WPFMonoBehaviour.levelManager.gridXmin; j <= WPFMonoBehaviour.levelManager.gridXmax; j++)
				{
					if (WPFMonoBehaviour.levelManager.CanPlacePartAtGridCell(j, i))
					{
						Transform transform2 = Object.Instantiate(m_cellPrefab) as Transform;
						transform2.transform.parent = gameObject2.transform;
						transform2.localPosition = new Vector3(j, i, 1f);
						int key = i * 1000 + j;
						m_cellMap[key] = transform2;
					}
				}
			}
			m_grid = gameObject2.transform;
		}
		GameObject gameObject3 = GameObject.Find("InGameGUI");
		if ((bool)gameObject3)
		{
			clearButton = gameObject3.transform.Find("InGameBuildMenu").Find("ClearButton").gameObject;
			playButton = gameObject3.transform.Find("InGameBuildMenu").Find("PlayButton").gameObject;
			moveButtons = gameObject3.transform.Find("InGameBuildMenu").Find("MoveButtons").gameObject;
			moveLeftButton = moveButtons.transform.Find("MoveLeftButton").gameObject;
			moveRightButton = moveButtons.transform.Find("MoveRightButton").gameObject;
			moveUpButton = moveButtons.transform.Find("MoveUpButton").gameObject;
			moveDownButton = moveButtons.transform.Find("MoveDownButton").gameObject;
			partSelector = gameObject3.transform.Find("InGameBuildMenu").Find("PartSelector").GetComponent<PartSelector>();
			partSelector.SetParts(m_partDescs);
			partSelector.gameObject.SetActiveRecursively(false);
		}
	}

	private void Update()
	{
		if (m_draggedElement != -1 && (bool)m_dragIcon)
		{
			Vector3 position = WPFMonoBehaviour.hudCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
			position.z = WPFMonoBehaviour.hudCamera.transform.position.z + 1f;
			position += m_dragOffset;
			m_dragIcon.transform.position = position;
		}
		SetHudPositionFromRelativeLevelPosition(clearButton, new Vector3((float)WPFMonoBehaviour.levelManager.gridXmin - 1f, 0.5f * (float)(WPFMonoBehaviour.levelManager.gridHeight - 1), 0f), new Vector3(-0.7f, 0f, 0f));
		SetHudPositionFromRelativeLevelPosition(playButton, new Vector3((float)WPFMonoBehaviour.levelManager.gridXmax + 1f, 0.5f * (float)(WPFMonoBehaviour.levelManager.gridHeight - 1), 0f), new Vector3(0.7f, 0f, 0f));
		SetHudPositionFromRelativeLevelPosition(moveButtons, new Vector3(0.5f * ((float)WPFMonoBehaviour.levelManager.gridXmin + (float)WPFMonoBehaviour.levelManager.gridXmax), WPFMonoBehaviour.levelManager.gridHeight, 0f), new Vector3(0f, 0.37f, 0f));
		if (moveButtons.transform.localPosition.y < 8.5f)
		{
			Vector3 localPosition = moveButtons.transform.localPosition;
			localPosition.y = 8.5f;
			moveButtons.transform.localPosition = localPosition;
		}
		HandleDragging();
	}

	private void SetHudPositionFromRelativeLevelPosition(GameObject obj, Vector3 levelOffset, Vector3 hudOffset)
	{
		float z = obj.transform.position.z;
		Vector3 position = base.transform.position + levelOffset;
		Vector3 position2 = Camera.main.WorldToScreenPoint(position);
		Vector3 position3 = WPFMonoBehaviour.hudCamera.GetComponent<Camera>().ScreenToWorldPoint(position2);
		position3 += hudOffset;
		position3.z = z;
		obj.transform.position = position3;
	}

	private void ReceiveUIEvent(UIEvent data)
	{
		switch (data.type)
		{
		case UIEvent.Type.MoveContraptionLeft:
			MoveContraption(-1, 0);
			break;
		case UIEvent.Type.MoveContraptionRight:
			MoveContraption(1, 0);
			break;
		case UIEvent.Type.MoveContraptionUp:
			MoveContraption(0, 1);
			break;
		case UIEvent.Type.MoveContraptionDown:
			MoveContraption(0, -1);
			break;
		}
	}

	private void ContraptionPartChanged(int x, int y)
	{
		if (m_contraption.Parts.Count < 2)
		{
			SetMoveButtonStates();
			return;
		}
		if (x == WPFMonoBehaviour.levelManager.gridXmin)
		{
			SetMoveButtonState(-1, 0);
		}
		if (x == WPFMonoBehaviour.levelManager.gridXmax)
		{
			SetMoveButtonState(1, 0);
		}
		if (y == 0)
		{
			SetMoveButtonState(0, -1);
		}
		if (y == WPFMonoBehaviour.levelManager.gridHeight - 1)
		{
			SetMoveButtonState(0, 1);
		}
	}

	public void SetMoveButtonStates()
	{
		SetMoveButtonState(1, 0);
		SetMoveButtonState(-1, 0);
		SetMoveButtonState(0, 1);
		SetMoveButtonState(0, -1);
	}

	private void SetMoveButtonState(int dx, int dy)
	{
		bool flag = m_contraption.CanMoveOnGrid(dx, dy);
		switch (dx)
		{
		case -1:
			moveLeftButton.GetComponent<Renderer>().enabled = flag;
			moveLeftButton.GetComponent<Collider>().enabled = flag;
			return;
		case 1:
			moveRightButton.GetComponent<Renderer>().enabled = flag;
			moveRightButton.GetComponent<Collider>().enabled = flag;
			return;
		}
		switch (dy)
		{
		case 1:
			moveUpButton.GetComponent<Renderer>().enabled = flag;
			moveUpButton.GetComponent<Collider>().enabled = flag;
			break;
		case -1:
			moveDownButton.GetComponent<Renderer>().enabled = flag;
			moveDownButton.GetComponent<Collider>().enabled = flag;
			break;
		}
	}

	private void MoveContraption(int dx, int dy)
	{
		AddMove();
		m_contraption.MoveOnGrid(dx, dy);
		if (dx != 0)
		{
			SetMoveButtonState(1, 0);
			SetMoveButtonState(-1, 0);
		}
		if (dy != 0)
		{
			SetMoveButtonState(0, 1);
			SetMoveButtonState(0, -1);
		}
	}

	public void SelectPart(PartDesc partDesc)
	{
		m_selectedElement = -1;
		for (int i = 0; i < m_partDescs.Count; i++)
		{
			if (m_partDescs[i] == partDesc)
			{
				m_selectedElement = i;
				break;
			}
		}
	}

	public void StartDrag(PartDesc partDesc)
	{
		int draggedElement = -1;
		m_draggedElement = -1;
		for (int i = 0; i < m_partDescs.Count; i++)
		{
			if (m_partDescs[i] == partDesc)
			{
				if (m_partDescs[i].useCount < m_partDescs[i].maxCount)
				{
					m_partDescs[i].useCount++;
					EventManager.Send(new PartCountChanged(partDesc.part.m_partType, m_partDescs[i].CurrentCount));
					draggedElement = i;
				}
				break;
			}
		}
		SetDraggedElement(draggedElement);
	}

	public bool IsDragging()
	{
		return m_draggedElement != -1;
	}

	public void CancelDrag(PartDesc partDesc)
	{
		SetDraggedElement(-1);
	}

	private void SetDraggedElement(int element)
	{
		SetDraggedElement(element, false);
	}

	private void SetDraggedElement(int element, bool fromContraption)
	{
		if (m_useDragOffset)
		{
			if (fromContraption)
			{
				m_dragOffset = new Vector3(0f, 0.75f, 0f);
			}
			else
			{
				m_dragOffset = new Vector3(0f, 3f, 0f);
			}
		}
		m_draggedElement = element;
		if (element == -1)
		{
			if ((bool)m_dragIcon)
			{
				Object.Destroy(m_dragIcon);
				m_dragIcon = null;
			}
		}
		else
		{
			m_dragIcon = (GameObject)Object.Instantiate(m_partDescs[element].part.m_constructionIconSprite.gameObject);
			if (m_useDragOffset)
			{
				m_dragIcon.transform.localScale = new Vector3(1.75f, 1.75f, 1f);
			}
			else
			{
				m_dragIcon.transform.localScale = new Vector3(1.75f, 1.75f, 1f);
			}
		}
	}

	private int FindPartIndex(BasePart part)
	{
		for (int i = 0; i < m_partDescs.Count; i++)
		{
			if (m_partDescs[i].part.m_partType == part.m_partType)
			{
				return i;
			}
		}
		return -1;
	}

	private void HandleDragging()
	{
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.PausedWhileBuilding || WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.IngamePurchase)
		{
			return;
		}
		int constructionUiRows = m_partDescs.Count / m_itemsPerRow + ((m_partDescs.Count % m_itemsPerRow != 0) ? 1 : 0);
		WPFMonoBehaviour.levelManager.m_constructionUiRows = constructionUiRows;
		if (Input.GetMouseButtonDown(0) && m_draggedElement == -1 && !m_dragStarted)
		{
			Vector3 vector = WPFMonoBehaviour.ScreenToZ0(Input.mousePosition);
			Vector3 vector2 = vector - base.transform.position;
			int coordX = Mathf.RoundToInt(vector2.x);
			int coordY = Mathf.RoundToInt(vector2.y);
			ChangeCoordinatesToSelectBigPart(ref coordX, ref coordY);
			BasePart basePart = m_contraption.FindPartAt(coordX, coordY);
			if ((bool)basePart)
			{
				if ((bool)basePart.enclosedPart)
				{
					basePart = basePart.enclosedPart;
				}
				if (!basePart.m_static)
				{
					m_dragStartPosition = Input.mousePosition;
					m_dragStarted = true;
				}
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			if (m_dragStarted && m_draggedElement == -1)
			{
				Vector3 vector3 = WPFMonoBehaviour.ScreenToZ0(m_dragStartPosition);
				Vector3 vector4 = vector3 - base.transform.position;
				int x = Mathf.RoundToInt(vector4.x);
				int y = Mathf.RoundToInt(vector4.y);
				BasePart basePart2 = m_contraption.FindPartAt(x, y);
				if ((bool)basePart2 && m_contraption.Flip(basePart2))
				{
					AddMove();
					m_rotationCounter++;
					AudioManager.Instance.Play2dEffect(AudioManager.Instance.CommonAudioCollection.rotatePart);
				}
			}
			m_dragStarted = false;
		}
		float num = ((!m_useDragOffset) ? 1f : 20f);
		if (m_dragStarted && m_draggedElement == -1 && Vector3.Distance(Input.mousePosition, m_dragStartPosition) >= num)
		{
			Vector3 vector5 = WPFMonoBehaviour.ScreenToZ0(m_dragStartPosition);
			Vector3 vector6 = vector5 - base.transform.position;
			int coordX2 = Mathf.RoundToInt(vector6.x);
			int coordY2 = Mathf.RoundToInt(vector6.y);
			ChangeCoordinatesToSelectBigPart(ref coordX2, ref coordY2);
			BasePart basePart3 = m_contraption.RemovePartAt(coordX2, coordY2);
			if ((bool)basePart3)
			{
				Debug.Log("old part was " + basePart3.name);
				draggedPartRotation = basePart3.m_gridRotation;
				draggedPartFlipped = basePart3.m_flipped;
				for (int i = 0; i < m_partDescs.Count; i++)
				{
					if (m_partDescs[i].part.m_partType == basePart3.m_partType)
					{
						SetDraggedElement(i, true);
						m_selectedElement = i;
						partSelector.SetSelection(m_partDescs[m_selectedElement]);
						break;
					}
				}
				Object.Destroy(basePart3.gameObject);
				ContraptionPartChanged(coordX2, coordY2);
				WPFMonoBehaviour.levelManager.PlayDragSound();
			}
		}
		if (m_draggedElement != -1)
		{
			PartDesc partDesc = m_partDescs[m_draggedElement];
			Vector3 position = WPFMonoBehaviour.hudCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition) + m_dragOffset;
			Vector3 position2 = WPFMonoBehaviour.hudCamera.GetComponent<Camera>().WorldToScreenPoint(position);
			Vector3 vector7 = Camera.main.ScreenToWorldPoint(position2);
			Vector3 vector8 = vector7 - base.transform.position;
			int num2 = Mathf.RoundToInt(vector8.x);
			int num3 = Mathf.RoundToInt(vector8.y);
			if (m_useDragOffset)
			{
				int key = num3 * 1000 + num2;
				Transform value;
				if (m_cellMap.TryGetValue(key, out value))
				{
					if ((bool)m_mouseOverCell)
					{
						m_mouseOverCell.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
					}
					m_mouseOverCell = value;
					value.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f);
				}
				else if ((bool)m_mouseOverCell)
				{
					m_mouseOverCell.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
				}
			}
			if (Input.GetMouseButtonUp(0))
			{
				AddMove();
				if (WPFMonoBehaviour.levelManager.CanPlacePartAtGridCell(num2, num3) && m_contraption.CanPlaceSpecificPartAt(num2, num3, partDesc.part))
				{
					SetPartAt(num2, num3, partDesc.part);
					ContraptionPartChanged(num2, num3);
					WPFMonoBehaviour.levelManager.PlayPartPlacedSound();
				}
				else
				{
					partDesc.useCount--;
					WPFMonoBehaviour.levelManager.PlayRemoveSound();
					EventManager.Send(new PartCountChanged(partDesc.part.m_partType, partDesc.CurrentCount));
				}
				SetDraggedElement(-1);
			}
		}
		else if (m_selectedElement != -1 && m_draggedElement == -1)
		{
			PartDesc partDesc2 = m_partDescs[m_selectedElement];
			Vector3 vector9 = WPFMonoBehaviour.ScreenToZ0(Input.mousePosition);
			Vector3 vector10 = vector9 - base.transform.position;
			int num4 = Mathf.RoundToInt(vector10.x);
			int num5 = Mathf.RoundToInt(vector10.y);
			BasePart basePart4 = m_contraption.FindPartAt(num4, num5);
			if (!basePart4 && Input.GetMouseButtonDown(0) && partDesc2.useCount < partDesc2.maxCount)
			{
				if (TryPlacePartAtGridCell(num4, num5, partDesc2))
				{
					AddMove();
				}
			}
			else if ((bool)basePart4 && basePart4.CanEncloseParts() && partDesc2.part.CanBeEnclosed() && Input.GetMouseButtonUp(0) && partDesc2.useCount < partDesc2.maxCount && TryPlacePartAtGridCell(num4, num5, partDesc2))
			{
				AddMove();
			}
		}
		if (Input.GetMouseButtonDown(1))
		{
			m_rightDragStartPosition = Input.mousePosition;
		}
		if (m_draggedElement == -1 && (Input.GetMouseButtonDown(1) || (Input.GetMouseButton(1) && Input.mousePosition != m_rightDragStartPosition)))
		{
			Vector3 vector11 = WPFMonoBehaviour.ScreenToZ0(Input.mousePosition);
			Vector3 vector12 = vector11 - base.transform.position;
			int x2 = Mathf.RoundToInt(vector12.x);
			int y2 = Mathf.RoundToInt(vector12.y);
			BasePart basePart5 = m_contraption.FindPartAt(x2, y2);
			if ((bool)basePart5 && (bool)basePart5.enclosedPart)
			{
				basePart5 = basePart5.enclosedPart;
			}
			if ((bool)basePart5 && !basePart5.m_static)
			{
				BasePart basePart6 = m_contraption.RemovePartAt(x2, y2);
				if ((bool)basePart6)
				{
					for (int j = 0; j < m_partDescs.Count; j++)
					{
						if (m_partDescs[j].part.m_partType == basePart6.m_partType)
						{
							PartDesc partDesc3 = m_partDescs[j];
							partDesc3.useCount--;
							EventManager.Send(new PartCountChanged(partDesc3.part.m_partType, partDesc3.CurrentCount));
							AddMove();
							break;
						}
					}
					Object.Destroy(basePart6.gameObject);
					ContraptionPartChanged(x2, y2);
				}
			}
		}
		if (m_draggedElement == -1 && (bool)m_mouseOverCell)
		{
			m_mouseOverCell.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
			m_mouseOverCell = null;
		}
	}

	private void ChangeCoordinatesToSelectBigPart(ref int coordX, ref int coordY)
	{
		for (int i = -1; i <= 0; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (j == 0 && i == 0)
				{
					continue;
				}
				int num = coordX + j;
				int num2 = coordY + i;
				BasePart basePart = m_contraption.FindPartAt(num, num2);
				if ((bool)basePart)
				{
					if ((bool)basePart.enclosedPart)
					{
						basePart = basePart.enclosedPart;
					}
					if (num + basePart.m_gridXmin <= coordX && num + basePart.m_gridXmax >= coordX && num2 + basePart.m_gridYmin <= coordY && num2 + basePart.m_gridYmax >= coordY)
					{
						coordX = num;
						coordY = num2;
						return;
					}
				}
			}
		}
	}

	private bool TryPlacePartAtGridCell(int coordX, int coordY, PartDesc partDesc)
	{
		if (WPFMonoBehaviour.levelManager.CanPlacePartAtGridCell(coordX, coordY) && m_contraption.CanPlaceSpecificPartAt(coordX, coordY, partDesc.part))
		{
			partDesc.useCount++;
			EventManager.Send(new PartCountChanged(partDesc.part.m_partType, partDesc.CurrentCount));
			SetPartAt(coordX, coordY, partDesc.part);
			ContraptionPartChanged(coordX, coordY);
			WPFMonoBehaviour.levelManager.PlayPartPlacedSound();
			return true;
		}
		return false;
	}

	private void ClearNonChassisPart(int coordX, int coordY)
	{
		BasePart basePart = m_contraption.FindPartAt(coordX, coordY);
		if (!basePart || (basePart.IsPartOfChassis() && !basePart.enclosedPart))
		{
			return;
		}
		basePart = m_contraption.RemovePartAt(coordX, coordY);
		for (int i = 0; i < m_partDescs.Count; i++)
		{
			if (m_partDescs[i].part.m_partType == basePart.m_partType)
			{
				PartDesc partDesc = m_partDescs[i];
				partDesc.useCount--;
				EventManager.Send(new PartCountChanged(partDesc.part.m_partType, partDesc.CurrentCount));
				break;
			}
		}
		Object.Destroy(basePart.gameObject);
		ContraptionPartChanged(coordX, coordY);
	}

	public Vector3 GridPositionToGuiPosition(int x, int y)
	{
		Vector3 position = m_contraption.transform.position;
		Vector3 position2 = position + Vector3.right * x + Vector3.up * y;
		Vector3 position3 = Camera.main.WorldToScreenPoint(position2);
		return WPFMonoBehaviour.hudCamera.GetComponent<Camera>().ScreenToWorldPoint(position3);
	}

	public Vector3 GridPositionToWorldPosition(int x, int y)
	{
		Vector3 position = m_contraption.transform.position;
		return position + Vector3.right * x + Vector3.up * y;
	}

	private void ClearCollidingParts(int coordX, int coordY, BasePart part)
	{
		for (int i = part.m_gridYmin; i <= part.m_gridYmax; i++)
		{
			for (int j = part.m_gridXmin; j <= part.m_gridXmax; j++)
			{
				if (j != 0 || i != 0)
				{
					ClearNonChassisPart(coordX + j, coordY + i);
				}
			}
		}
		if (part.IsPartOfChassis())
		{
			return;
		}
		for (int k = -1; k <= 0; k++)
		{
			for (int l = -1; l <= 1; l++)
			{
				BasePart basePart = m_contraption.FindPartAt(coordX + l, coordY + k);
				if ((bool)basePart)
				{
					if ((bool)basePart.enclosedPart)
					{
						basePart = basePart.enclosedPart;
					}
					if (basePart.m_partType == BasePart.PartType.KingPig)
					{
						ClearNonChassisPart(coordX + l, coordY + k);
					}
				}
			}
		}
	}

	public BasePart SetPartAt(int coordX, int coordY, BasePart part, bool autoalign = true)
	{
		GameObject gameObject = Object.Instantiate(part.gameObject) as GameObject;
		BasePart component = gameObject.GetComponent<BasePart>();
		BasePart basePart = m_contraption.FindPartAt(coordX, coordY);
		if (component.m_partType == BasePart.PartType.Pig && (bool)basePart && basePart.m_partType == BasePart.PartType.WoodenFrame && DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios)
		{
			SocialGameManager.Instance.ReportAchievementProgress("grp.THINK_INSIDE_THE_BOX", 100.0);
		}
		ClearCollidingParts(coordX, coordY, component);
		component.PrePlaced();
		BasePart part2 = m_contraption.SetPartAt(coordX, coordY, component, autoalign);
		if (autoalign)
		{
			m_contraption.AutoAlign(component);
		}
		CollectPart(part2);
		return component;
	}

	protected void CollectPart(BasePart part)
	{
		if (!part)
		{
			return;
		}
		Debug.Log("collecting " + part.name + ", enc " + part.enclosedPart);
		foreach (PartDesc partDesc in m_partDescs)
		{
			if (partDesc.part.m_partType == part.m_partType)
			{
				partDesc.useCount--;
				EventManager.Send(new PartCountChanged(partDesc.part.m_partType, partDesc.CurrentCount));
			}
		}
		if ((bool)part.enclosedPart)
		{
			CollectPart(part.enclosedPart);
		}
		Object.Destroy(part.gameObject);
	}

	public void SetEnabled(bool enableUI, bool enableGrid)
	{
		if ((bool)m_grid)
		{
			m_grid.gameObject.SetActiveRecursively(enableGrid);
		}
		m_contraption.RefreshConnections();
		base.gameObject.active = enableUI;
	}

	public void ClearContraption()
	{
		m_contraption.RemoveAllDynamicParts();
		foreach (PartDesc partDesc in m_partDescs)
		{
			partDesc.useCount = 0;
			EventManager.Send(new PartCountChanged(partDesc.part.m_partType, partDesc.CurrentCount));
		}
		SetMoveButtonStates();
		AddMove();
	}

	public void LateUpdate()
	{
		for (int i = 0; i < WPFMonoBehaviour.levelManager.gridHeight; i++)
		{
			for (int j = WPFMonoBehaviour.levelManager.gridXmin; j <= WPFMonoBehaviour.levelManager.gridXmax; j++)
			{
				if (!WPFMonoBehaviour.levelManager.CanPlacePartAtGridCell(j, i))
				{
					continue;
				}
				int key = i * 1000 + j;
				Transform value;
				if (m_cellMap.TryGetValue(key, out value))
				{
					Renderer renderer = value.GetComponent<Renderer>();
					if ((bool)renderer)
					{
					}
				}
			}
		}
	}
}
