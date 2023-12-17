using UnityEngine;

public class Balloon : BasePart
{
	public float m_force = 10f;

	public float m_maximumSpeed = 10f;

	public bool m_inWorldCoordinates = true;

	public Vector3 m_direction = Vector3.up;

	public int m_numberOfBalloons = 1;

	public bool m_enabled = true;

	public Material m_stringMaterial;

	protected BasePart m_connectedPart;

	protected Vector3 m_connectedLocalPos;

	protected BalloonBalancer m_balancer;

	protected SpringJoint m_springJoint;

	protected RopeVisualization m_rope;

	public override void Awake()
	{
		base.Awake();
		base.enabled = false;
	}

	public override bool WillDetach()
	{
		return true;
	}

	public override bool IsIntegralPart()
	{
		return false;
	}

	public override void PrePlaced()
	{
	}

	public override void Initialize()
	{
		for (int i = 1; i < 5; i++)
		{
			if ((bool)m_connectedPart)
			{
				break;
			}
			m_connectedPart = base.contraption.FindPartAt(m_coordX, m_coordY - i);
			if ((bool)m_connectedPart && !m_connectedPart.IsPartOfChassis() && m_connectedPart.m_partType != PartType.Pig)
			{
				m_connectedPart = null;
			}
		}
		m_partType = PartType.Balloon;
		base.contraption.ChangeOneShotPartAmount(BasePart.BaseType(m_partType), EffectDirection(), 1);
		if (m_numberOfBalloons > 1)
		{
			GameObject gameObject = Object.Instantiate(base.gameObject) as GameObject;
			gameObject.transform.position = base.transform.position;
			Balloon component = gameObject.GetComponent<Balloon>();
			component.m_numberOfBalloons = m_numberOfBalloons - 1;
			base.contraption.AddRuntimePart(component);
			gameObject.transform.parent = base.contraption.transform;
		}
		if (!base.gameObject.GetComponent<SphereCollider>())
		{
			SphereCollider sphereCollider = base.gameObject.AddComponent<SphereCollider>();
			sphereCollider.radius = 0.5f;
		}
		if (!base.GetComponent<Rigidbody>())
		{
			base.gameObject.AddComponent<Rigidbody>();
		}
		base.GetComponent<Rigidbody>().mass = 0.1f;
		base.GetComponent<Rigidbody>().drag = 2f;
		base.GetComponent<Rigidbody>().angularDrag = 0.5f;
		base.GetComponent<Rigidbody>().constraints = (RigidbodyConstraints)48;
		if ((bool)m_connectedPart)
		{
			m_connectedPart.EnsureRigidbody();
			Vector3 position = base.transform.position;
			float num = Vector3.Distance(m_connectedPart.transform.position, position);
			Vector3 vector = ((m_connectedPart.m_partType != PartType.Pig) ? (Vector3.up * 0.5f) : Vector3.zero);
			base.transform.position = m_connectedPart.transform.position + vector;
			m_springJoint = base.gameObject.AddComponent<SpringJoint>();
			m_springJoint.connectedBody = m_connectedPart.GetComponent<Rigidbody>();
			float maxDistance = Random.Range(0.8f, 1.2f) * num;
			m_springJoint.minDistance = 0f;
			m_springJoint.maxDistance = maxDistance;
			m_springJoint.anchor = Vector3.up * -0.5f;
			m_springJoint.spring = 100f;
			m_springJoint.damper = 10f;
			m_balancer = m_connectedPart.gameObject.GetComponent<BalloonBalancer>();
			if (!m_balancer)
			{
				m_balancer = m_connectedPart.gameObject.AddComponent<BalloonBalancer>();
			}
			m_balancer.AddBalloon();
			Transform transform = base.transform;
			if ((bool)m_actualVisualizationNode)
			{
				transform = m_actualVisualizationNode.transform;
			}
			m_rope = transform.gameObject.AddComponent<RopeVisualization>();
			m_connectedLocalPos = m_connectedPart.transform.InverseTransformPoint(base.transform.position);
			m_rope.m_stringMaterial = m_stringMaterial;
			m_rope.m_pos1Anchor = Vector3.up * -0.5f + 0.1f * Vector3.forward;
			m_rope.m_pos2Transform = m_connectedPart.transform;
			m_rope.m_pos2Anchor = m_connectedLocalPos + 0.1f * Vector3.forward;
			base.transform.position = position + Vector3.up * 0.75f + Random.Range(-1f, 1f) * Vector3.forward + Random.Range(-1f, 1f) * Vector3.right * 0.5f;
		}
	}

	public void ConfigureExtraBalanceJoint(float powerFactor)
	{
		if ((bool)m_balancer)
		{
			m_balancer.Configure(powerFactor);
		}
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

	public void FixedUpdate()
	{
		if (m_enabled)
		{
			float num = LimitForceForSpeed(m_force, m_direction);
			base.GetComponent<Rigidbody>().AddForce(num * m_direction, ForceMode.Force);
			if ((bool)m_springJoint && !m_springJoint.connectedBody)
			{
				Object.Destroy(m_rope.GetComponent<LineRenderer>());
				Object.Destroy(m_rope);
				Object.Destroy(m_springJoint);
				base.contraption.ChangeOneShotPartAmount(BasePart.BaseType(m_partType), EffectDirection(), -1);
			}
		}
	}

	public override void ProcessTouch()
	{
		Pop();
	}

	public void OnCollisionEnter(Collision coll)
	{
		int num = LayerMask.NameToLayer("Ground");
		ContactPoint[] contacts = coll.contacts;
		foreach (ContactPoint contactPoint in contacts)
		{
			if (contactPoint.otherCollider.gameObject.layer == num)
			{
				Pop();
			}
		}
	}

	public void Pop()
	{
		AudioManager.Instance.SpawnOneShotEffect(AudioManager.Instance.CommonAudioCollection.balloonPop, base.transform.position);
		GameObject obj = Object.Instantiate(WPFMonoBehaviour.gameData.m_ballonParticles, base.transform.position, Quaternion.identity) as GameObject;
		Object.Destroy(obj, 0.12f);
		base.contraption.ChangeOneShotPartAmount(BasePart.BaseType(m_partType), EffectDirection(), -1);
		if ((bool)m_balancer)
		{
			m_balancer.RemoveBalloon();
		}
		Object.Destroy(base.gameObject);
	}

	public override void EnsureRigidbody()
	{
		Rigidbody rigidbody = base.gameObject.GetComponent<Rigidbody>();
		if (rigidbody == null)
		{
			rigidbody = base.gameObject.AddComponent<Rigidbody>();
		}
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
	}

	public void CheckBalloonPopperAchievement()
	{
		if (DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios)
		{
			int num = GameProgress.GetInt("Popped_Ballons") + 1;
			GameProgress.SetInt("Popped_Ballons", num);
			if (num > AchievementData.Instance.GetAchievementLimit("grp.POPPERS_THEORY_3"))
			{
				SocialGameManager.Instance.ReportAchievementProgress("grp.POPPERS_THEORY_3", 100.0);
			}
			else if (num > AchievementData.Instance.GetAchievementLimit("grp.POPPERS_THEORY_2"))
			{
				SocialGameManager.Instance.ReportAchievementProgress("grp.POPPERS_THEORY_3", num / AchievementData.Instance.GetAchievementLimit("grp.POPPERS_THEORY_3"));
				SocialGameManager.Instance.ReportAchievementProgress("grp.POPPERS_THEORY_2", 100.0);
			}
			else if (num > AchievementData.Instance.GetAchievementLimit("grp.POPPERS_THEORY_1"))
			{
				SocialGameManager.Instance.ReportAchievementProgress("grp.POPPERS_THEORY_3", num / AchievementData.Instance.GetAchievementLimit("grp.POPPERS_THEORY_3"));
				SocialGameManager.Instance.ReportAchievementProgress("grp.POPPERS_THEORY_2", num / AchievementData.Instance.GetAchievementLimit("grp.POPPERS_THEORY_2"));
				SocialGameManager.Instance.ReportAchievementProgress("grp.POPPERS_THEORY_1", 100.0);
			}
		}
	}
}
