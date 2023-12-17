using UnityEngine;

public class GadgetButton : Button
{
	public BasePart.PartType m_partType;

	public BasePart.Direction m_direction;

	private float m_placementOrder;

	private LevelManager levelManager;

	private GameObject m_gadgetSprite;

	private float m_stateUpdateTimer;

	private bool m_partsEnabled;

	private float m_enabledTimer;

	public float PlacementOrder
	{
		get
		{
			return m_placementOrder;
		}
		set
		{
			m_placementOrder = value;
			m_stateUpdateTimer = 0.01f * m_placementOrder;
		}
	}

	protected override void ButtonAwake()
	{
		LevelManager[] array = Object.FindSceneObjectsOfType(typeof(LevelManager)) as LevelManager[];
		if (array.Length > 0)
		{
			levelManager = array[0];
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		EventManager.Send(new GadgetControlEvent(m_partType, m_direction));
	}

	private void OnEnable()
	{
		m_gadgetSprite = base.transform.Find("Gadget").gameObject;
		UpdateState();
	}

	protected override void ButtonUpdate()
	{
		m_stateUpdateTimer += Time.deltaTime;
		if (m_stateUpdateTimer >= 0.2f)
		{
			if (m_partType != BasePart.PartType.Engine)
			{
				UpdateState();
			}
			else if ((bool)levelManager)
			{
				m_partsEnabled = levelManager.contraptionRunning.AllPoweredPartsEnabled();
			}
			m_stateUpdateTimer = 0f;
		}
		if (!m_partsEnabled)
		{
			return;
		}
		m_enabledTimer += Time.deltaTime;
		if (m_partType == BasePart.PartType.Bellows)
		{
			Vector3 localScale = m_gadgetSprite.transform.localScale;
			localScale.y = Bellows.CompressionScale(m_enabledTimer);
			m_gadgetSprite.transform.localScale = localScale;
			if (m_enabledTimer > 1.1f)
			{
				m_enabledTimer = 0f;
			}
		}
		else
		{
			m_gadgetSprite.transform.localPosition = (Vector3)Random.insideUnitCircle * 0.075f + new Vector3(0f, 0f, -0.1f);
		}
	}

	private void UpdateState()
	{
		if (!levelManager || !levelManager.contraptionProto)
		{
			return;
		}
		bool flag = levelManager.contraptionRunning.AllPartsEnabled(m_partType, m_direction);
		if (!flag && m_partsEnabled)
		{
			m_gadgetSprite.transform.localPosition = new Vector3(0f, 0f, -0.1f);
			m_gadgetSprite.transform.localScale = Vector3.one;
			m_enabledTimer = 0f;
		}
		m_partsEnabled = flag;
		if (m_partType != BasePart.PartType.Engine)
		{
			if (!levelManager.m_showOnlyEngineButton)
			{
				SetEnabled(levelManager.contraptionRunning.HasActiveParts(m_partType, m_direction));
			}
			else
			{
				SetEnabled(false);
			}
		}
	}

	private void SetEnabled(bool enabled)
	{
		if (base.gameObject.GetComponent<Renderer>().enabled == enabled)
		{
			return;
		}
		base.gameObject.GetComponent<Renderer>().enabled = enabled;
		base.gameObject.GetComponent<Collider>().enabled = enabled;
		for (int i = 0; i < base.gameObject.transform.childCount; i++)
		{
			Transform child = base.gameObject.transform.GetChild(i);
			if ((bool)child.GetComponent<Renderer>())
			{
				child.GetComponent<Renderer>().enabled = enabled;
			}
		}
	}
}
