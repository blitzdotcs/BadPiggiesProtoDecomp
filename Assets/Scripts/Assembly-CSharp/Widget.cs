using UnityEngine;

public class Widget : MonoBehaviour
{
	public virtual void SetListener(WidgetListener listener)
	{
	}

	public void SendInput(InputEvent input)
	{
		if ((bool)base.transform.parent && (bool)base.transform.parent.GetComponent<Widget>())
		{
			base.transform.parent.GetComponent<Widget>().SendInput(input);
		}
		OnInput(input);
	}

	public void Activate()
	{
		if ((bool)base.transform.parent && (bool)base.transform.parent.GetComponent<Widget>())
		{
			base.transform.parent.GetComponent<Widget>().Activate();
		}
		OnActivate();
	}

	public virtual void Select()
	{
	}

	public virtual void Deselect()
	{
	}

	protected virtual void OnInput(InputEvent input)
	{
	}

	protected virtual void OnActivate()
	{
	}
}
