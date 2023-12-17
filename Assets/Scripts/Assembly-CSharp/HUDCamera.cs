using UnityEngine;

public class HUDCamera : WPFMonoBehaviour
{
	protected int m_layerHUD;

	protected Transform m_activeItem;

	private void Start()
	{
		m_layerHUD = 1024;
	}

	private void Update()
	{
		if (!WPFMonoBehaviour.gameData.m_useTouchControls)
		{
			if (Input.GetMouseButton(0))
			{
				Ray ray = base.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 5f, m_layerHUD))
				{
					hitInfo.transform.SendMessage("OnTouchDown", SendMessageOptions.DontRequireReceiver);
					if (m_activeItem != hitInfo.transform)
					{
						if ((bool)m_activeItem)
						{
							m_activeItem.SendMessage("OnTouchExit", SendMessageOptions.DontRequireReceiver);
						}
						m_activeItem = hitInfo.transform;
					}
				}
				else if ((bool)m_activeItem)
				{
					m_activeItem.SendMessage("OnTouchExit", SendMessageOptions.DontRequireReceiver);
				}
			}
			if (Input.GetMouseButtonUp(0))
			{
				Ray ray2 = base.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
				RaycastHit hitInfo2;
				if (Physics.Raycast(ray2, out hitInfo2, 5f, m_layerHUD))
				{
					hitInfo2.transform.SendMessage("OnTouchRelease", SendMessageOptions.DontRequireReceiver);
					m_activeItem = hitInfo2.transform;
				}
				else if ((bool)m_activeItem)
				{
					m_activeItem.SendMessage("OnTouchExit", SendMessageOptions.DontRequireReceiver);
				}
			}
			return;
		}
		Touch[] touches = Input.touches;
		for (int i = 0; i < touches.Length; i++)
		{
			Touch touch = touches[i];
			if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
			{
				Ray ray3 = base.GetComponent<Camera>().ScreenPointToRay(touch.position);
				RaycastHit hitInfo3;
				if (Physics.Raycast(ray3, out hitInfo3, 5f, m_layerHUD))
				{
					hitInfo3.transform.SendMessage("OnTouchDown", SendMessageOptions.DontRequireReceiver);
					if (m_activeItem != hitInfo3.transform)
					{
						if ((bool)m_activeItem)
						{
							m_activeItem.SendMessage("OnTouchExit", SendMessageOptions.DontRequireReceiver);
						}
						m_activeItem = hitInfo3.transform;
					}
				}
				else if ((bool)m_activeItem)
				{
					m_activeItem.SendMessage("OnTouchExit", SendMessageOptions.DontRequireReceiver);
				}
			}
			if (touch.phase == TouchPhase.Ended)
			{
				Ray ray4 = base.GetComponent<Camera>().ScreenPointToRay(touch.position);
				RaycastHit hitInfo4;
				if (Physics.Raycast(ray4, out hitInfo4, 5f, m_layerHUD))
				{
					hitInfo4.transform.SendMessage("OnTouchRelease", SendMessageOptions.DontRequireReceiver);
					m_activeItem = hitInfo4.transform;
				}
				else if ((bool)m_activeItem)
				{
					m_activeItem.SendMessage("OnTouchExit", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
}
