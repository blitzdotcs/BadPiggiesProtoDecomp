using UnityEngine;

public class FanPropeller : BasePropulsion
{
	private const float LowPowerSoundLimit = 1f;

	public float m_force;

	public Direction m_forceDirection;

	public bool m_isRotor;

	public bool m_enabled;

	public float m_defaultSpeed;

	public Transform m_fanVisualization;

	protected Quaternion m_origRot;

	protected float m_angle;

	private Vector3 m_originalDirection;

	private float m_maximumRotationSpeed;

	private float m_rotationSpeed;

	private float m_maximumForce;

	private float m_maximumSpeed;

	private GameObject loopingSound;

	private float powerFactor;

	public ParticleSystem smokeCloud;

	public override JointConnectionStrength GetJointConnectionStrength()
	{
		if (m_partType == PartType.Fan && m_gridRotation == GridRotation.Deg_270)
		{
			return JointConnectionStrength.High;
		}
		return m_jointConnectionStrength;
	}

	public override bool CanBeEnabled()
	{
		return m_maximumRotationSpeed > 0f;
	}

	public override bool IsEnabled()
	{
		return m_enabled;
	}

	public override Direction EffectDirection()
	{
		return BasePart.Rotate(m_forceDirection, m_gridRotation);
	}

	public override void Awake()
	{
		base.Awake();
		m_enabled = false;
	}

	public override void Initialize()
	{
		m_origRot = m_fanVisualization.transform.localRotation;
		m_enabled = false;
		Vector3 directionVector = BasePart.GetDirectionVector(m_forceDirection);
		m_originalDirection = base.transform.TransformDirection(directionVector);
	}

	public override void InitializeEngine()
	{
		powerFactor = base.contraption.GetEnginePowerFactor(this);
		if (powerFactor > 1f)
		{
			powerFactor = Mathf.Pow(powerFactor, 0.75f);
		}
		m_maximumSpeed = powerFactor * m_defaultSpeed;
		m_maximumForce = m_force * powerFactor;
		m_maximumRotationSpeed = 1000f * powerFactor;
		if (m_maximumRotationSpeed > 0f)
		{
			m_maximumRotationSpeed += 700f;
		}
		else
		{
			m_angle = 0f;
			if (m_enabled)
			{
				SetEnabled(false);
			}
		}
		if ((bool)loopingSound)
		{
			AudioManager.Instance.RemoveLoopingEffect(ref loopingSound);
		}
	}

	public void FixedUpdate()
	{
		if (!base.contraption || !base.contraption.isRunning)
		{
			return;
		}
		if (m_enabled)
		{
			m_rotationSpeed = m_maximumRotationSpeed;
		}
		else if (m_rotationSpeed < 450f)
		{
			m_rotationSpeed *= 0.9f;
		}
		else
		{
			m_rotationSpeed *= 0.98f;
		}
		m_angle += m_rotationSpeed * Time.deltaTime;
		if (m_angle > 180f)
		{
			m_angle -= 360f;
		}
		if (!m_enabled)
		{
			if (m_isRotor)
			{
				base.GetComponent<Rigidbody>().angularDrag = 1f;
			}
			return;
		}
		if (m_isRotor)
		{
			base.GetComponent<Rigidbody>().angularDrag = 1000f;
		}
		Vector3 directionVector = BasePart.GetDirectionVector(m_forceDirection);
		Vector3 vector = base.transform.TransformDirection(directionVector);
		Vector3 vector2 = base.transform.position + vector * 0.5f;
		Vector3 vector3 = vector;
		if (m_isRotor)
		{
			vector3 = 0.5f * (vector3 + m_originalDirection);
		}
		float num = LimitForceForSpeed(m_maximumForce, vector3);
		float num2 = 1f;
		if (m_forceDirection == Direction.Left)
		{
			float value = Vector3.Dot(base.GetComponent<Rigidbody>().velocity, -vector3);
			float num3 = Mathf.Clamp(value, -2f, 2f);
			Vector3 position = base.transform.position;
			position.z = 0f;
			RaycastHit hitInfo;
			if (Physics.Raycast(position, -vector3, out hitInfo, 1f) && m_enabled)
			{
				num2 = 1f + (2f + num3) * Mathf.Pow(1f - hitInfo.distance, 1.5f);
			}
		}
		if (m_partType == PartType.Rotor && m_enabled)
		{
			Vector3 velocity = base.GetComponent<Rigidbody>().velocity;
			if (velocity.magnitude > m_maximumSpeed && Vector3.Dot(vector3, velocity) > 0f)
			{
				float num4 = velocity.magnitude - m_maximumSpeed;
				base.GetComponent<Rigidbody>().AddForceAtPosition(-4f * num4 * num4 * velocity.normalized, vector2, ForceMode.Force);
			}
		}
		base.GetComponent<Rigidbody>().AddForceAtPosition(num * num2 * vector3, vector2, ForceMode.Force);
		Debug.DrawRay(vector2, 0.1f * num * num2 * vector3, Color.green);
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
		SetEnabled(!m_enabled);
	}

	public override void SetEnabled(bool toggle)
	{
		if (toggle && m_maximumRotationSpeed == 0f)
		{
			return;
		}
		m_enabled = toggle;
		if (!m_enabled)
		{
			m_rotationSpeed = 800f;
			m_angle = 292.3f;
			if ((bool)smokeCloud)
			{
				smokeCloud.Stop();
			}
		}
		else if ((bool)smokeCloud)
		{
			smokeCloud.Play();
		}
		base.contraption.UpdateEngineStates(base.ConnectedComponent);
		if (m_enabled)
		{
			PlayPropellerSound();
		}
		else if ((bool)loopingSound)
		{
			AudioManager.Instance.RemoveLoopingEffect(ref loopingSound);
		}
		Billboard component = GetComponent<Billboard>();
		if ((bool)component)
		{
			component.enabled = !m_enabled;
		}
	}

	public void PlayPropellerSound()
	{
		AudioManager instance = AudioManager.Instance;
		switch (m_partType)
		{
		case PartType.Fan:
			loopingSound = instance.SpawnLoopingEffect(instance.CommonAudioCollection.fan, base.gameObject.transform);
			break;
		case PartType.Rotor:
			loopingSound = instance.SpawnLoopingEffect(instance.CommonAudioCollection.rotorLoop, base.gameObject.transform);
			break;
		default:
			loopingSound = instance.SpawnLoopingEffect(instance.CommonAudioCollection.propeller, base.gameObject.transform);
			break;
		}
		loopingSound.GetComponent<AudioSource>().time = loopingSound.GetComponent<AudioSource>().clip.length * Random.value;
	}

	public void LateUpdate()
	{
		Vector3 axis = ((!m_isRotor) ? Vector3.right : Vector3.up);
		m_fanVisualization.transform.localRotation = m_origRot * Quaternion.AngleAxis(m_angle, axis);
	}

	public override void OnDetach()
	{
		Debug.Log("on detach audio");
		if ((bool)loopingSound)
		{
			AudioManager.Instance.RemoveLoopingEffect(ref loopingSound);
		}
	}
}
