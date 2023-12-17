using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(Sprite))]
[RequireComponent(typeof(BoxCollider))]
public class DraggableButton : Widget
{
	private const float HoverSoundDelay = 0.15f;

	private WidgetListener m_listener;

	[SerializeField]
	private GameObject icon;

	[SerializeField]
	private object dragObject;

	[SerializeField]
	private GameObject dragIconPrefab;

	[SerializeField]
	private float dragIconScale = 1f;

	[SerializeField]
	private GameObject messageTargetObject;

	[SerializeField]
	private string targetComponent;

	[SerializeField]
	private string methodToInvoke;

	[SerializeField]
	private string messageParameter;

	[SerializeField]
	private UIEvent.Type eventToSend;

	[SerializeField]
	private bool animate = true;

	private Component component;

	private MethodInfo methodInfo;

	private bool hasStringParameter;

	private object[] parameterArray;

	private bool down;

	private GameObject dragIcon;

	private bool dragging;

	private GameObject selectedVisual;

	public GameObject Icon
	{
		get
		{
			return icon;
		}
		set
		{
			icon = value;
		}
	}

	public object DragObject
	{
		get
		{
			return dragObject;
		}
		set
		{
			dragObject = value;
		}
	}

	public GameObject DragIconPrefab
	{
		get
		{
			return dragIconPrefab;
		}
		set
		{
			dragIconPrefab = value;
		}
	}

	public float DragIconScale
	{
		get
		{
			return dragIconScale;
		}
		set
		{
			dragIconScale = value;
		}
	}

	public GameObject MessageTargetObject
	{
		get
		{
			return messageTargetObject;
		}
		set
		{
			messageTargetObject = value;
			BindTarget();
		}
	}

	public string TargetComponent
	{
		get
		{
			return targetComponent;
		}
		set
		{
			targetComponent = value;
			BindTarget();
		}
	}

	public string MethodToInvoke
	{
		get
		{
			return methodToInvoke;
		}
		set
		{
			methodToInvoke = value;
			BindTarget();
		}
	}

	public string MessageParameter
	{
		get
		{
			return messageParameter;
		}
		set
		{
			messageParameter = value;
			BindTarget();
		}
	}

	public override void SetListener(WidgetListener listener)
	{
		m_listener = listener;
	}

	public void CancelDrag()
	{
		if (dragging)
		{
			dragging = false;
			dragIcon.SetActiveRecursively(false);
			dragIcon.transform.localPosition = Vector3.zero;
			if (m_listener != null)
			{
				m_listener.CancelDrag(this, dragObject);
			}
		}
	}

	public override void Select()
	{
		selectedVisual.GetComponent<Renderer>().enabled = true;
		base.GetComponent<Renderer>().enabled = false;
	}

	public override void Deselect()
	{
		CancelDrag();
		selectedVisual.GetComponent<Renderer>().enabled = false;
		base.GetComponent<Renderer>().enabled = true;
	}

	private void Awake()
	{
		BindTarget();
		ButtonAwake();
		selectedVisual = base.transform.Find("Selected").gameObject;
		selectedVisual.GetComponent<Renderer>().enabled = false;
	}

	private void Start()
	{
		if ((bool)dragIconPrefab)
		{
			dragIcon = (GameObject)Object.Instantiate(dragIconPrefab);
			dragIcon.transform.parent = base.transform;
			dragIcon.transform.localScale = new Vector3(dragIconScale, dragIconScale, 1f);
			dragIcon.transform.localPosition = Vector3.zero;
			dragIcon.SetActiveRecursively(false);
		}
	}

	protected virtual void ButtonAwake()
	{
	}

	private void BindTarget()
	{
		methodInfo = null;
		if (!messageTargetObject || !(targetComponent != string.Empty) || !(methodToInvoke != string.Empty))
		{
			return;
		}
		component = messageTargetObject.GetComponent(targetComponent);
		if (!component)
		{
			return;
		}
		methodInfo = component.GetType().GetMethod(methodToInvoke);
		if (methodInfo != null)
		{
			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length > 0)
			{
				hasStringParameter = parameters[0].ParameterType == typeof(string);
				parameterArray = new object[1] { messageParameter };
			}
		}
	}

	protected override void OnActivate()
	{
		if ((bool)messageTargetObject && methodInfo != null)
		{
			if (hasStringParameter)
			{
				methodInfo.Invoke(component, parameterArray);
			}
			else
			{
				methodInfo.Invoke(component, null);
			}
		}
		if (eventToSend != 0)
		{
			EventManager.Send(new UIEvent(eventToSend));
		}
	}

	protected override void OnInput(InputEvent input)
	{
		AudioManager instance = AudioManager.Instance;
		if (input.type == InputEvent.EventType.Press)
		{
			down = true;
			dragging = true;
			if ((bool)dragIcon)
			{
				dragIcon.SetActiveRecursively(true);
			}
			if (m_listener != null)
			{
				m_listener.StartDrag(this, dragObject);
			}
			selectedVisual.GetComponent<Renderer>().enabled = true;
			base.GetComponent<Renderer>().enabled = false;
			if (m_listener != null)
			{
				m_listener.Select(this, dragObject);
			}
		}
		else if (input.type == InputEvent.EventType.Release)
		{
			down = false;
			Activate();
			instance.Play2dEffect(instance.CommonAudioCollection.menuClick);
		}
		else if (input.type == InputEvent.EventType.MouseEnter)
		{
			down = true;
			PlayHoverSound(instance);
		}
		else if (input.type == InputEvent.EventType.MouseLeave)
		{
			down = false;
		}
	}

	private void PlayHoverSound(AudioManager audioManager)
	{
		if (!DeviceInfo.Instance.UsesTouchInput)
		{
			audioManager.Play2dEffect(audioManager.CommonAudioCollection.menuHover);
		}
	}

	private void Update()
	{
		if (dragging)
		{
			Vector3 vector = GuiManager.Instance.FindCamera().ScreenToWorldPoint(Input.mousePosition);
			float z = base.transform.position.z - 1f;
			Vector3 position = new Vector3(vector.x, vector.y, z);
			if ((bool)dragIcon)
			{
				dragIcon.transform.position = position;
			}
			if (Input.GetMouseButtonUp(0))
			{
				if ((bool)dragIcon)
				{
					dragIcon.SetActiveRecursively(false);
					dragIcon.transform.localPosition = Vector3.zero;
				}
				dragging = false;
				if (m_listener != null)
				{
					m_listener.Drop(this, position, dragObject);
				}
			}
		}
		if (animate)
		{
			float num = base.transform.localScale.x;
			if (down && num < 1.2f)
			{
				num = Mathf.Min(num + Time.deltaTime * 7f, 1.2f);
			}
			else if (!down && num > 1f)
			{
				num = Mathf.Max(num - Time.deltaTime * 7f, 1f);
			}
			base.transform.localScale = new Vector3(num, num, 1f);
		}
		ButtonUpdate();
	}

	protected virtual void ButtonUpdate()
	{
	}
}
