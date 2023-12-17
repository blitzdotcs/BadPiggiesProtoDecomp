using UnityEngine;

public class PoweredUmbrella : BasePart
{
	private const float MinScale = 0.4f;

	public float m_force;

	public bool m_enabled;

	private float m_maximumForce;

	private bool m_isMotored;

	private GameObject m_visualization;

	private GameObject m_openSprite;

	private GameObject m_closedSprite;

	private bool m_open = true;

	private float m_timer;

	private float m_moveTime = 0.25f;

	private Vector3 m_closedPosition;

	private Vector3 m_openPosition;

	private bool m_isConnected;

	private float MaxScale = 1f;

	public override bool CanBeEnabled()
	{
		return m_isConnected;
	}

	public override bool HasOnOffToggle()
	{
		return m_isMotored;
	}

	public override bool IsEnabled()
	{
		return m_enabled;
	}

	public override bool IsPowered()
	{
		return m_isMotored;
	}

	public override Direction EffectDirection()
	{
		return BasePart.Rotate(Direction.Up, m_gridRotation);
	}

	public override void Awake()
	{
		base.Awake();
		m_visualization = base.transform.Find("UmbrellaVisualization").gameObject;
		m_openSprite = m_visualization.transform.Find("OpenSprite").gameObject;
		m_closedSprite = m_visualization.transform.Find("ClosedSprite").gameObject;
		m_closedPosition = -0.05f * Vector3.up;
		m_closedPosition.z = m_visualization.transform.localPosition.z;
		m_openPosition = 0.25f * Vector3.up;
		m_openPosition.z = m_visualization.transform.localPosition.z;
		m_closedSprite.GetComponent<Collider>().enabled = false;
		m_closedSprite.GetComponent<Collider>().isTrigger = true;
		m_closedSprite.GetComponent<Renderer>().enabled = false;
	}

	public override void InitializeEngine()
	{
		m_isConnected = base.contraption.ComponentPartCount(base.ConnectedComponent) > 1;
		float num = base.contraption.GetEnginePowerFactor(this);
		if (num > 1f)
		{
			num = Mathf.Pow(num, 0.75f);
		}
		if (base.contraption.HasComponentEngine(base.ConnectedComponent))
		{
			m_isMotored = true;
			m_openPosition = 0.25f * Vector3.up;
			float num2 = 7f * num;
			m_moveTime = 1f / num2;
			MaxScale = 1.1f;
		}
		else
		{
			m_isMotored = true;
			m_openPosition = 0.25f * Vector3.up;
			float num3 = 7f * num;
			m_moveTime = 1f / num3;
			MaxScale = 1.1f;
			num *= 0.5f;
		}
		if (m_open)
		{
			m_visualization.transform.localPosition = m_openPosition;
			Vector3 localScale = m_visualization.transform.localScale;
			localScale.x = MaxScale;
			localScale.y = 1f;
			m_visualization.transform.localScale = localScale;
			if (m_isMotored)
			{
				m_timer = 0.45f * m_moveTime;
				float a = m_timer / m_moveTime;
				a = Mathf.Min(a, 1f);
				a = Mathf.Min(a, 1f);
				Vector3 localPosition = Vector3.Slerp(m_openPosition, m_closedPosition, a);
				localScale = m_visualization.transform.localScale;
				a = Mathf.Pow(a, 3f);
				localScale.x = a * 0.4f + (1f - a) * MaxScale;
				localScale.y = a * 2f + (1f - a) * 1f;
				localPosition.y -= 0.45f * (localScale.y - 1f);
				m_visualization.transform.localScale = localScale;
				m_visualization.transform.localPosition = localPosition;
			}
		}
		else
		{
			m_visualization.transform.localPosition = m_closedPosition;
		}
		m_maximumForce = m_force * num;
	}

	private void FixedUpdate()
	{
		if (!base.contraption || !base.contraption.isRunning)
		{
			return;
		}
		if (m_open)
		{
			Vector3 vector = base.transform.position + 0.2f * base.transform.up;
			Vector3 vector2 = base.GetComponent<Rigidbody>().velocity - base.WindVelocity;
			base.WindVelocity = Vector3.zero;
			float num = Vector3.Dot(base.transform.up, vector2.normalized);
			float magnitude = (num * vector2).magnitude;
			float num2 = Mathf.Sign(Vector3.Cross(vector2, base.transform.right).z);
			float num3 = num2 * 1f * magnitude * magnitude;
			Vector3 up = base.transform.up;
			base.GetComponent<Rigidbody>().AddForceAtPosition(num3 * up, vector, ForceMode.Force);
			Debug.DrawRay(vector, 0.1f * num3 * up);
			num = Vector3.Dot(base.transform.right, vector2.normalized);
			magnitude = (num * vector2).magnitude;
			num2 = Mathf.Sign(Vector3.Cross(base.transform.up, vector2).z);
			num3 = num2 * 0.5f * magnitude * magnitude;
			up = base.transform.right;
			base.GetComponent<Rigidbody>().AddForceAtPosition(num3 * up, vector, ForceMode.Force);
			Debug.DrawRay(vector, 0.1f * num3 * up);
		}
		if (!m_enabled)
		{
			return;
		}
		m_timer += Time.deltaTime;
		if (m_open)
		{
			float a = m_timer / m_moveTime;
			a = Mathf.Min(a, 1f);
			Vector3 localPosition = Vector3.Slerp(m_openPosition, m_closedPosition, a);
			Vector3 localScale = m_visualization.transform.localScale;
			a = Mathf.Pow(a, 3f);
			localScale.x = a * 0.4f + (1f - a) * MaxScale;
			localScale.y = a * 2f + (1f - a) * 1f;
			localPosition.y -= 0.45f * (localScale.y - 1f);
			m_visualization.transform.localScale = localScale;
			m_visualization.transform.localPosition = localPosition;
			float maximumForce = m_maximumForce;
			base.GetComponent<Rigidbody>().AddForce(maximumForce * base.transform.up, ForceMode.Force);
			Debug.DrawRay(base.transform.position, maximumForce * base.transform.up);
		}
		else
		{
			float a2 = m_timer / m_moveTime;
			a2 = Mathf.Min(a2, 1f);
			Vector3 localPosition2 = Vector3.Slerp(m_closedPosition, m_openPosition, a2);
			Vector3 localScale2 = m_visualization.transform.localScale;
			a2 = Mathf.Pow(a2, 6f);
			localScale2.x = a2 * MaxScale + (1f - a2) * 0.4f;
			localScale2.y = a2 * 1f + (1f - a2) * 2f;
			localPosition2.y -= 0.45f * (localScale2.y - 1f);
			m_visualization.transform.localScale = localScale2;
			m_visualization.transform.localPosition = localPosition2;
			float num4 = -0.2f * m_maximumForce;
			base.GetComponent<Rigidbody>().AddForce(num4 * base.transform.up, ForceMode.Force);
			Debug.DrawRay(base.transform.position, 0.1f * num4 * base.transform.up);
		}
		if (m_timer > m_moveTime)
		{
			m_timer = 0f;
			m_open = !m_open;
			if (!m_isMotored)
			{
				m_enabled = false;
			}
		}
	}

	public override void ProcessTouch()
	{
		if (m_isConnected || m_enabled)
		{
			SetEnabled(!m_enabled);
		}
	}

	public override void SetEnabled(bool enabled)
	{
		m_enabled = enabled;
		if (m_isMotored)
		{
			base.contraption.UpdateEngineStates(base.ConnectedComponent);
		}
	}

	private void Open(bool open)
	{
		if (m_open != open)
		{
			m_open = open;
			if (open)
			{
				m_openSprite.GetComponent<Renderer>().enabled = true;
				m_openSprite.GetComponent<Collider>().enabled = true;
				m_openSprite.GetComponent<Collider>().isTrigger = false;
				m_closedSprite.GetComponent<Renderer>().enabled = false;
				m_closedSprite.GetComponent<Collider>().enabled = false;
				m_closedSprite.GetComponent<Collider>().isTrigger = true;
				m_visualization.transform.localPosition = m_openPosition;
			}
			else
			{
				m_openSprite.GetComponent<Renderer>().enabled = false;
				m_openSprite.GetComponent<Collider>().enabled = false;
				m_openSprite.GetComponent<Collider>().isTrigger = true;
				m_closedSprite.GetComponent<Renderer>().enabled = true;
				m_closedSprite.GetComponent<Collider>().enabled = true;
				m_closedSprite.GetComponent<Collider>().isTrigger = false;
				m_visualization.transform.localPosition = m_closedPosition;
			}
		}
	}
}
