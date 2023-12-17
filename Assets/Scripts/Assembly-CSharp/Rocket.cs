using UnityEngine;

public class Rocket : BasePropulsion
{
	public Vector3 m_direction = Vector3.up;

	public bool m_enabled;

	public float m_boostForce = 10f;

	public float m_ignitionTime = 1f;

	public float m_boostDuration = 2f;

	public float m_boostEndDuration = 0.5f;

	public float m_maximumSpeed;

	public Transform m_particlesIgnition;

	public Transform m_particlesFiring;

	public Transform m_particlesSmoke;

	public AudioSource launchAudio;

	protected bool m_boostUsed;

	protected Vector3 m_origScale;

	protected float m_timeBoostStarted;

	protected float m_currentScale;

	protected bool m_firedRocket;

	public ParticleSystem m_particlesIgnitionInstance;

	public ParticleSystem m_particlesFiringInstance;

	private GameObject m_leftAttachment;

	private GameObject m_rightAttachment;

	private GameObject m_topAttachment;

	private GameObject m_bottomAttachment;

	private GameObject m_visualization;

	public Renderer m_content;

	public Renderer m_content2;

	private float m_currentAlpha;

	private float m_currentAlpha2;

	public override bool IsEnabled()
	{
		float num = Time.time - m_timeBoostStarted;
		return num < m_ignitionTime + m_boostDuration + m_boostEndDuration;
	}

	public override Direction EffectDirection()
	{
		return BasePart.Rotate(Direction.Right, m_gridRotation);
	}

	public override void Awake()
	{
		base.Awake();
		m_leftAttachment = base.transform.Find("LeftAttachment").gameObject;
		m_rightAttachment = base.transform.Find("RightAttachment").gameObject;
		m_topAttachment = base.transform.Find("TopAttachment").gameObject;
		m_bottomAttachment = base.transform.Find("BottomAttachment").gameObject;
		m_leftAttachment.active = false;
		m_rightAttachment.active = false;
		m_topAttachment.active = false;
		m_bottomAttachment.active = false;
		Transform transform = base.transform.Find("BottleVisualization");
		if ((bool)transform)
		{
			m_visualization = transform.gameObject;
		}
		m_origScale = base.transform.localScale;
		m_timeBoostStarted = -1000f;
		m_boostUsed = false;
		m_currentAlpha = 1f;
		m_currentAlpha2 = 1f;
		m_enabled = false;
		if ((bool)m_content2)
		{
			m_content2.material.color = new Color(1f, 1f, 1f, 0f);
		}
	}

	public override void ChangeVisualConnections()
	{
		bool flag = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.Up, m_gridRotation));
		bool flag2 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.Down, m_gridRotation));
		bool flag3 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.Left, m_gridRotation));
		bool flag4 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.Right, m_gridRotation));
		m_leftAttachment.active = flag3;
		m_rightAttachment.active = flag4;
		m_topAttachment.active = flag;
		m_bottomAttachment.active = flag2 || (!flag && !flag3 && !flag4);
		m_leftAttachment.GetComponent<BoxCollider>().isTrigger = !m_leftAttachment.active;
		m_rightAttachment.GetComponent<BoxCollider>().isTrigger = !m_rightAttachment.active;
		m_topAttachment.GetComponent<BoxCollider>().isTrigger = !m_topAttachment.active;
		m_bottomAttachment.GetComponent<BoxCollider>().isTrigger = !m_bottomAttachment.active;
	}

	public override GridRotation AutoAlignRotation(JointConnectionDirection target)
	{
		switch (target)
		{
		case JointConnectionDirection.Right:
			return GridRotation.Deg_90;
		case JointConnectionDirection.Up:
			return GridRotation.Deg_0;
		case JointConnectionDirection.Left:
			return GridRotation.Deg_90;
		case JointConnectionDirection.Down:
			return GridRotation.Deg_0;
		default:
			return GridRotation.Deg_0;
		}
	}

	public override void Initialize()
	{
		base.contraption.ChangeOneShotPartAmount(m_partType, EffectDirection(), 1);
	}

	public void Update()
	{
		float num = Time.time - m_timeBoostStarted;
		if (m_boostUsed && (bool)m_content && m_currentAlpha > 0f)
		{
			m_currentAlpha -= Time.deltaTime;
			if (m_currentAlpha < 0f)
			{
				m_currentAlpha = 0f;
			}
			m_content.material.color = new Color(1f, 1f, 1f, m_currentAlpha);
			if ((bool)m_content2)
			{
				m_content2.material.color = new Color(1f, 1f, 1f, 1f - m_currentAlpha);
			}
		}
		if (!(num < m_ignitionTime) && m_boostUsed && (bool)m_content2 && m_currentAlpha2 > 0f)
		{
			m_currentAlpha2 -= Time.deltaTime;
			if (m_currentAlpha2 < 0f)
			{
				m_currentAlpha2 = 0f;
			}
			Color color = new Color(1f, 1f, 1f, m_currentAlpha2);
			m_content2.material.color = color;
		}
	}

	public void FixedUpdate()
	{
		if (!m_enabled)
		{
			return;
		}
		float num = Time.time - m_timeBoostStarted;
		if (num < m_ignitionTime)
		{
			if ((bool)m_visualization)
			{
				m_visualization.transform.localPosition = (Vector3)Random.insideUnitCircle * 0.1f;
			}
			return;
		}
		float num2 = 1f;
		if (num > m_ignitionTime)
		{
			if (m_boostUsed)
			{
				if ((bool)m_particlesIgnitionInstance && m_particlesIgnitionInstance.isPlaying)
				{
					m_particlesIgnitionInstance.Stop();
				}
				if ((bool)m_visualization)
				{
					Transform transform = m_visualization.transform.Find("Cork");
					if ((bool)transform)
					{
						transform.parent = base.transform.parent;
						transform.GetComponent<Cork>().Fly(-20f * base.transform.right, 200f, 0.75f);
					}
				}
			}
			if (num < m_ignitionTime + m_boostDuration + m_boostEndDuration)
			{
				if (!m_particlesFiringInstance.isPlaying)
				{
					m_particlesFiringInstance.Play();
				}
			}
			else if ((bool)m_particlesFiringInstance)
			{
				m_particlesFiringInstance.Stop();
			}
		}
		if (num > m_ignitionTime + m_boostDuration + m_boostEndDuration)
		{
			Object.Destroy(m_particlesIgnitionInstance.gameObject);
			Object.Destroy(m_particlesFiringInstance.gameObject);
			m_enabled = false;
		}
		if (num > m_ignitionTime + m_boostDuration)
		{
			num2 = 1f - (num - m_boostDuration - m_ignitionTime) / m_boostEndDuration;
		}
		float forceMagnitude = num2 * m_boostForce;
		Vector3 zero = Vector3.zero;
		Vector3 vector = base.transform.position + zero * 0.5f;
		Vector3 vector2 = base.transform.TransformDirection(m_direction);
		forceMagnitude = LimitForceForSpeed(forceMagnitude, vector2);
		base.GetComponent<Rigidbody>().AddForceAtPosition(forceMagnitude * vector2, vector, ForceMode.Force);
		Debug.DrawRay(vector, 0.1f * vector2 * forceMagnitude, Color.white);
	}

	private float LimitForceForSpeed(float forceMagnitude, Vector3 forceDir)
	{
		Vector3 velocity = base.GetComponent<Rigidbody>().velocity;
		float num = Vector3.Dot(velocity.normalized, forceDir);
		if (num > 0f)
		{
			Vector3 vector = velocity * num;
			if (vector.magnitude > m_maximumSpeed)
			{
				return forceMagnitude / (1f + vector.magnitude - m_maximumSpeed);
			}
		}
		return forceMagnitude;
	}

	public override void ProcessTouch()
	{
		if (!m_boostUsed)
		{
			m_enabled = !m_enabled;
			m_particlesIgnitionInstance.Play();
			m_timeBoostStarted = Time.time;
			m_boostUsed = true;
			base.contraption.ChangeOneShotPartAmount(m_partType, EffectDirection(), -1);
			AudioManager.Instance.SpawnOneShotEffect(launchAudio, base.transform);
		}
	}
}
