using UnityEngine;

public class PartCounter : MonoBehaviour
{
	public BasePart.PartType m_partType;

	private TextMesh m_textMesh;

	private void Awake()
	{
		m_textMesh = GetComponent<TextMesh>();
		EventManager.Connect<PartCountChanged>(ReceivePartCountChangedEvent);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<PartCountChanged>(ReceivePartCountChangedEvent);
	}

	private void ReceivePartCountChangedEvent(PartCountChanged data)
	{
		if (data.partType != m_partType)
		{
			return;
		}
		m_textMesh.text = data.count.ToString();
		if (data.count == 0)
		{
			GameObject icon = base.transform.parent.GetComponent<DraggableButton>().Icon;
			if ((bool)icon)
			{
				icon.GetComponent<Renderer>().material.color = new Color(0.4f, 0.4f, 0.4f, 0.4f);
			}
		}
		else
		{
			GameObject icon2 = base.transform.parent.GetComponent<DraggableButton>().Icon;
			if ((bool)icon2)
			{
				icon2.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);
			}
		}
	}
}
