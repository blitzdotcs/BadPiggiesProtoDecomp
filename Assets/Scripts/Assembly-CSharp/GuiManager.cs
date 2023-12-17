using UnityEngine;

public class GuiManager : MonoBehaviour
{
	private int guiLayerMask = 1;

	private Widget target;

	private Widget mouseOver;

	private bool usingMouse = true;

	private static GuiManager instance;

	public static GuiManager Instance
	{
		get
		{
			return instance;
		}
	}

	public Camera FindCamera()
	{
		return GameObject.FindGameObjectWithTag("HUDCamera").GetComponent<Camera>();
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		instance = this;
		guiLayerMask = 1 << base.gameObject.layer;
	}

	private void Update()
	{
		Camera camera = FindCamera();
		guiLayerMask = 1 << camera.gameObject.layer;
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		Widget widget = null;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, guiLayerMask))
		{
			widget = hitInfo.collider.gameObject.GetComponent<Widget>();
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (Input.touchCount > 0)
			{
				usingMouse = false;
			}
			if ((bool)widget)
			{
				target = widget;
				target.SendInput(new InputEvent(InputEvent.EventType.Press));
			}
			else
			{
				target = null;
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			if ((bool)widget && widget == target)
			{
				widget.SendInput(new InputEvent(InputEvent.EventType.Release));
			}
			target = null;
		}
		if (usingMouse || (Input.touchCount > 0 && !Input.GetMouseButtonUp(0)))
		{
			if (widget != null && mouseOver != widget && (target == null || target == widget))
			{
				if (mouseOver != null)
				{
					mouseOver.SendInput(new InputEvent(InputEvent.EventType.MouseLeave));
				}
				mouseOver = widget;
				widget.SendInput(new InputEvent(InputEvent.EventType.MouseEnter));
				if (widget == target)
				{
					widget.SendInput(new InputEvent(InputEvent.EventType.MouseReturn));
				}
			}
			if (mouseOver != null && widget == null)
			{
				mouseOver.SendInput(new InputEvent(InputEvent.EventType.MouseLeave));
				mouseOver = null;
			}
		}
		if (Input.touchCount > 1 && mouseOver != null)
		{
			mouseOver.SendInput(new InputEvent(InputEvent.EventType.MouseLeave));
			mouseOver = null;
		}
	}
}
