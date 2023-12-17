using System.Collections.Generic;
using UnityEngine;

public class ScrollList : Widget, WidgetListener
{
	private enum Action
	{
		Place = 0,
		DrawGizmos = 1
	}

	private struct ScrollInfo
	{
		public float time;

		public float offset;

		public ScrollInfo(float time, float offset)
		{
			this.time = time;
			this.offset = offset;
		}
	}

	public GameObject leftButton;

	public GameObject rightButton;

	public GameObject scrollButtonOffset;

	public GameObject buttonPrefab;

	public int horizontalCount = 10;

	public Vector2 offset = new Vector2(10f, 10f);

	public int count = 20;

	public int maxRows = 1;

	public WidgetListener m_listener;

	private List<GameObject> buttons = new List<GameObject>();

	private bool dragging;

	private bool dragDirectionDetected;

	private bool dragLocked;

	private Vector3 dragStart;

	private float scrollOffsetAtDragStart;

	private float scrollOffset;

	private float scrollTargetOffset;

	private float scrollTargetStart;

	private bool scrollTargetSet;

	private float scrollTargetSetTime;

	private float scrollVelocity;

	private Queue<ScrollInfo> scrollHistory = new Queue<ScrollInfo>();

	private bool canScroll = true;

	private float scrollButtonWidth;

	private float scrollAreaWidth;

	private bool onLeftBorder;

	private bool onRightBorder;

	private bool updatePlacement;

	private int maxButtonsInRow;

	private int usedRows = 1;

	public void SetMaxRows(int rows)
	{
		maxRows = rows;
	}

	public void ScrollLeft()
	{
		scrollTargetOffset = scrollOffset - 0.5f * scrollAreaWidth;
		updatePlacement = true;
		onRightBorder = false;
		float num = -0.5f * (float)horizontalCount * offset.x;
		float num2 = 10f * (float)Screen.width / (float)Screen.height;
		float num3 = num - scrollTargetOffset;
		float num4 = 0f - num2 + scrollButtonWidth;
		if (Mathf.Abs(num3 - num4) < 3f)
		{
			scrollTargetOffset -= 3f;
		}
		if (num3 > num4)
		{
			scrollTargetOffset = num - num4 - 0.01f;
		}
		scrollTargetSet = true;
		scrollTargetStart = scrollOffset;
		scrollTargetSetTime = Time.time;
	}

	public void ScrollRight()
	{
		scrollTargetOffset = scrollOffset + 0.5f * scrollAreaWidth;
		updatePlacement = true;
		onLeftBorder = false;
		float num = 0.5f * (float)horizontalCount * offset.x;
		float num2 = 10f * (float)Screen.width / (float)Screen.height;
		float num3 = num - scrollTargetOffset;
		float num4 = num2 - scrollButtonWidth;
		if (Mathf.Abs(num3 - num4) < 3f)
		{
			scrollTargetOffset += 3f;
		}
		if (num3 < num4)
		{
			scrollTargetOffset = num - num4 + 0.01f;
		}
		scrollTargetSet = true;
		scrollTargetStart = scrollOffset;
		scrollTargetSetTime = Time.time;
	}

	public override void SetListener(WidgetListener listener)
	{
		m_listener = listener;
	}

	protected override void OnInput(InputEvent input)
	{
		if (input.type == InputEvent.EventType.Press && canScroll)
		{
			dragging = true;
			dragStart = GuiManager.Instance.FindCamera().ScreenToWorldPoint(Input.mousePosition);
			scrollOffsetAtDragStart = scrollOffset;
			dragDirectionDetected = false;
			dragLocked = false;
		}
	}

	public GameObject FindButton(object dragObject)
	{
		foreach (GameObject button in buttons)
		{
			DraggableButton component = button.GetComponent<DraggableButton>();
			if ((bool)component && component.DragObject == dragObject)
			{
				return component.gameObject;
			}
		}
		return null;
	}

	public void SetSelection(object targetObject)
	{
		foreach (GameObject button in buttons)
		{
			DraggableButton component = button.GetComponent<DraggableButton>();
			if ((bool)component && component.DragObject == targetObject)
			{
				Select(component, targetObject);
				component.Select();
			}
		}
	}

	public void Select(Widget widget, object targetObject)
	{
		foreach (GameObject button in buttons)
		{
			Widget component = button.GetComponent<Widget>();
			if ((bool)component && component != widget)
			{
				component.Deselect();
			}
		}
		if (m_listener != null)
		{
			m_listener.Select(widget, targetObject);
		}
	}

	public void StartDrag(Widget widget, object targetObject)
	{
		if (m_listener != null)
		{
			m_listener.StartDrag(widget, targetObject);
		}
	}

	public void CancelDrag(Widget widget, object targetObject)
	{
		if (m_listener != null)
		{
			m_listener.CancelDrag(widget, targetObject);
		}
	}

	public void Drop(Widget widget, Vector3 dropPosition, object targetObject)
	{
		if (m_listener != null)
		{
			m_listener.Drop(widget, dropPosition, targetObject);
		}
	}

	public void AddButton(Widget button)
	{
		button.SetListener(this);
		buttons.Add(button.gameObject);
		PlaceButtons(Action.Place);
	}

	private void Start()
	{
		Vector3 vector = leftButton.transform.Find("Button").GetComponent<Sprite>().Size;
		scrollButtonWidth = Mathf.Abs((leftButton.transform.Find("Button").transform.rotation * vector).x);
		scrollAreaWidth = 20f * (float)Screen.width / (float)Screen.height - 2f * scrollButtonWidth;
		maxButtonsInRow = (int)scrollAreaWidth / (int)offset.x;
	}

	private void Update()
	{
		if (scrollTargetSet)
		{
			float num = Mathf.Abs(scrollTargetStart - scrollTargetOffset) / (0.5f * scrollAreaWidth);
			if (num < 0.1f)
			{
				num = 0.1f;
			}
			float num2 = Mathf.Pow(4f * (Time.time - scrollTargetSetTime) / num, 1f);
			if (num2 > 1f)
			{
				num2 = 1f;
				scrollTargetSet = false;
			}
			scrollOffset = Mathf.Lerp(scrollTargetStart, scrollTargetOffset, num2);
			PlaceButtons(Action.Place);
		}
		if (dragging)
		{
			Vector3 b = GuiManager.Instance.FindCamera().ScreenToWorldPoint(Input.mousePosition);
			if (dragDirectionDetected && dragLocked)
			{
				float num3 = dragStart.x - b.x;
				if (num3 != 0f)
				{
					onLeftBorder = false;
					onRightBorder = false;
				}
				scrollOffset = scrollOffsetAtDragStart + num3;
				while (scrollHistory.Count > 0)
				{
					ScrollInfo scrollInfo = scrollHistory.Peek();
					if (scrollInfo.time < Time.time - 0.1f)
					{
						scrollHistory.Dequeue();
						continue;
					}
					float num4 = Time.time - scrollInfo.time;
					if (num4 > 0f)
					{
						scrollVelocity = (scrollOffset - scrollInfo.offset) / num4;
					}
					break;
				}
				scrollHistory.Enqueue(new ScrollInfo(Time.time, scrollOffset));
				PlaceButtons(Action.Place);
			}
			if (!dragDirectionDetected && Vector3.Distance(dragStart, b) > 0.5f)
			{
				dragDirectionDetected = true;
				if (Mathf.Abs(dragStart.x - b.x) < 2.5f * (b.y - dragStart.y))
				{
					dragging = false;
				}
				else
				{
					dragLocked = true;
				}
			}
			if (Input.GetMouseButtonUp(0) || (dragDirectionDetected && !dragLocked))
			{
				dragging = false;
			}
		}
		else if (Mathf.Abs(scrollVelocity) > 0.01f)
		{
			float num5 = Time.deltaTime * scrollVelocity;
			if (num5 != 0f)
			{
				onLeftBorder = false;
				onRightBorder = false;
			}
			scrollOffset += num5;
			scrollVelocity *= Mathf.Pow(0.9f, Time.deltaTime / (1f / 60f));
			PlaceButtons(Action.Place);
		}
		SetBorderStates();
		if (updatePlacement)
		{
			PlaceButtons(Action.Place);
			updatePlacement = false;
		}
	}

	public void Clear()
	{
		foreach (GameObject button in buttons)
		{
			Object.Destroy(button);
		}
		buttons.Clear();
	}

	private void OnDrawGizmos()
	{
		if ((bool)buttonPrefab)
		{
			PlaceButtons(Action.DrawGizmos);
		}
	}

	private void SetBorderStates()
	{
		float num = -0.5f * (float)horizontalCount * offset.x;
		float num2 = 10f * (float)Screen.width / (float)Screen.height;
		if (num > 0f - num2)
		{
			if (leftButton.active || rightButton.active)
			{
				if (scrollOffset != 0f || scrollVelocity != 0f)
				{
					scrollOffset = 0f;
					scrollVelocity = 0f;
					PlaceButtons(Action.Place);
				}
				leftButton.SetActiveRecursively(false);
				rightButton.SetActiveRecursively(false);
				canScroll = false;
			}
		}
		else if (!leftButton.active || !rightButton.active)
		{
			leftButton.SetActiveRecursively(true);
			EnableRendererRecursively(leftButton, true);
			rightButton.SetActiveRecursively(true);
			EnableRendererRecursively(rightButton, true);
			canScroll = true;
		}
		if (canScroll)
		{
			float num3 = num - scrollOffset;
			float num4 = 0f - num2 + scrollButtonWidth;
			if (num3 > num4)
			{
				scrollOffset = num - num4;
				scrollVelocity = 0f;
				EnableRendererRecursively(leftButton, false);
				onLeftBorder = true;
				PlaceButtons(Action.Place);
			}
			else if (!onLeftBorder)
			{
				EnableRendererRecursively(leftButton, true);
			}
			float num5 = 0f - num;
			float num6 = num5 - scrollOffset;
			float num7 = num2 - scrollButtonWidth;
			if (num6 < num7)
			{
				scrollOffset = num5 - num7;
				scrollVelocity = 0f;
				EnableRendererRecursively(rightButton, false);
				onRightBorder = true;
				PlaceButtons(Action.Place);
			}
			else if (!onRightBorder)
			{
				EnableRendererRecursively(rightButton, true);
			}
		}
		else
		{
			scrollOffset = 0f;
		}
	}

	private void EnableRendererRecursively(GameObject obj, bool enable)
	{
		if ((bool)obj.GetComponent<Renderer>())
		{
			obj.GetComponent<Renderer>().enabled = enable;
		}
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			EnableRendererRecursively(obj.transform.GetChild(i).gameObject, enable);
		}
	}

	private void PlaceScrollButtons()
	{
		if (usedRows > 1)
		{
			float y = 0.5f * (float)(usedRows - 1) * offset.y;
			scrollButtonOffset.transform.localPosition = new Vector3(0f, y, 0f);
		}
		else
		{
			scrollButtonOffset.transform.localPosition = Vector3.zero;
		}
	}

	private void PlaceButtons(Action action)
	{
		if (action == Action.Place)
		{
			horizontalCount = buttons.Count;
			scrollAreaWidth = 20f * (float)Screen.width / (float)Screen.height - 2f * scrollButtonWidth;
			maxButtonsInRow = (int)scrollAreaWidth / (int)offset.x;
			if (horizontalCount > maxButtonsInRow)
			{
				usedRows = maxRows;
				horizontalCount = horizontalCount / usedRows + ((horizontalCount % usedRows != 0) ? 1 : 0);
			}
			else
			{
				usedRows = 1;
			}
			PlaceScrollButtons();
		}
		int num = 0;
		int num2 = 0;
		Vector3 position = base.transform.position;
		position.x -= 0.5f * ((float)(horizontalCount - 1) * offset.x) + scrollOffset;
		position.y -= 0.5f * buttonPrefab.GetComponent<Sprite>().Size.y;
		position.y += (float)(usedRows - 1) * offset.y;
		Vector3 vector = position;
		int num3 = count;
		if (action == Action.Place)
		{
			num3 = buttons.Count;
		}
		for (int i = 0; i < num3; i++)
		{
			if (action == Action.Place)
			{
				buttons[i].transform.position = vector;
			}
			else
			{
				Gizmos.DrawWireCube(vector, buttonPrefab.GetComponent<Sprite>().Size);
			}
			vector.x += offset.x;
			num++;
			if (num >= horizontalCount)
			{
				num = 0;
				vector.x = position.x;
				vector.y -= offset.y;
				num2++;
				if (num2 >= usedRows)
				{
					vector = new Vector3(100000f, 0f, 0f);
				}
			}
		}
	}
}
