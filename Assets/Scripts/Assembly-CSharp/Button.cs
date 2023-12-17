using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(Sprite))]
[RequireComponent(typeof(BoxCollider))]
public class Button : Widget
{
	private const float HoverSoundDelay = 0.15f;

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

	[SerializeField]
	private bool activateOnHold;

	private Component component;

	private MethodInfo methodInfo;

	private bool hasStringParameter;

	private object[] parameterArray;

	private bool mouseOver;

	private bool down;

	private Vector3 originalScale;

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

	public UIEvent.Type EventToSend
	{
		get
		{
			return eventToSend;
		}
		set
		{
			eventToSend = value;
			BindTarget();
		}
	}

	private void Awake()
	{
		originalScale = base.transform.localScale;
		BindTarget();
		ButtonAwake();
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
			else
			{
				hasStringParameter = false;
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
			mouseOver = true;
			down = true;
		}
		else if (input.type == InputEvent.EventType.Release)
		{
			down = false;
			if (!activateOnHold)
			{
				Activate();
			}
			instance.Play2dEffect(instance.CommonAudioCollection.menuClick);
		}
		else if (input.type == InputEvent.EventType.MouseEnter)
		{
			mouseOver = true;
			PlayHoverSound(instance);
		}
		else if (input.type == InputEvent.EventType.MouseReturn)
		{
			down = true;
		}
		else if (input.type == InputEvent.EventType.MouseLeave)
		{
			mouseOver = false;
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
		if (activateOnHold && down)
		{
			Activate();
		}
		if (animate)
		{
			bool flag = !down && mouseOver;
			float num = base.transform.localScale.x / originalScale.x;
			if (flag && num < 1.2f)
			{
				num = Mathf.Min(num + GameTime.RealTimeDelta * 7f, 1.2f);
				base.transform.localScale = num * originalScale;
			}
			else if (!flag && num > 1f)
			{
				num = Mathf.Max(num - GameTime.RealTimeDelta * 7f, 1f);
				base.transform.localScale = num * originalScale;
			}
		}
		ButtonUpdate();
	}

	protected virtual void ButtonUpdate()
	{
	}
}
