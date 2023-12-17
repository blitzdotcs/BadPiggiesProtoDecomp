using System;
using UnityEngine;

public class MotorWheel : BasePart
{
	public float m_force;

	public bool m_enabled;

	public float m_maximumSpeed;

	private float m_maximumForce;

	private Vector3 m_lastPosition = Vector3.zero;

	private Vector3 m_lastContactDirection = Vector3.zero;

	private float m_radius;

	private float m_circumference;

	private Transform m_wheelPivot;

	private Transform m_fakeWheelPivot;

	private float m_spinSpeed;

	private Vector3 m_lastForceDirection;

	private float m_angle;

	private Collider m_supportCollider;

	private bool m_hasContact = true;

	private float m_thrust;

	private float m_thrustTimer;

	private Rigidbody colliderRigidbody;

	private AudioSource loopingWheelSound;

	private bool m_grounded;

	public bool HasContact
	{
		get
		{
			return m_hasContact;
		}
	}

	public override bool CanBeEnabled()
	{
		return m_maximumSpeed > 0f;
	}

	public override bool IsEnabled()
	{
		return m_enabled;
	}

	private void Start()
	{
		m_wheelPivot = base.transform.Find("WheelPivot");
		m_fakeWheelPivot = base.transform.Find("FakeWheelPivot");
		m_radius = GetComponent<SphereCollider>().radius;
		m_circumference = (float)Math.PI * 2f * m_radius;
		if ((bool)base.transform.Find("SupportCollider"))
		{
			m_supportCollider = base.transform.Find("SupportCollider").GetComponent<Collider>();
		}
		loopingWheelSound = AudioManager.Instance.SpawnLoopingEffect(AudioManager.Instance.CommonAudioCollection.motorWheelLoop, base.transform).GetComponent<AudioSource>();
		Assert.IsValid(loopingWheelSound, "loopingWheelSound");
		loopingWheelSound.volume = 0f;
	}

	public override void InitializeEngine()
	{
		float enginePowerFactor = base.contraption.GetEnginePowerFactor(this);
		m_maximumForce = m_force * enginePowerFactor;
		m_maximumSpeed = 15f * enginePowerFactor;
	}

	private void ReinitializeEngine()
	{
		float enginePowerFactor = base.contraption.GetEnginePowerFactor(this);
		m_maximumForce = m_force * enginePowerFactor;
	}

	private void OnCollisionStay(Collision collisionInfo)
	{
		for (int i = 0; i < collisionInfo.contacts.Length; i++)
		{
			if (collisionInfo.contacts[i].otherCollider != m_supportCollider)
			{
				m_lastContactDirection = (collisionInfo.contacts[i].point - m_lastPosition).normalized;
				break;
			}
		}
	}

	private void Update()
	{
		if ((bool)base.contraption && base.contraption.isRunning)
		{
			if (m_spinSpeed == 0f)
			{
				m_spinSpeed = SpeedInDirection(m_lastForceDirection);
			}
			float z = base.transform.rotation.eulerAngles.z;
			m_angle += -360f * m_spinSpeed / m_circumference * Time.deltaTime;
			if ((bool)m_wheelPivot)
			{
				m_wheelPivot.transform.localRotation = Quaternion.AngleAxis(0f - z + m_angle, Vector3.forward);
			}
			if ((bool)m_fakeWheelPivot)
			{
				float angle = 0f - z + Mathf.Sin(2f * m_angle * ((float)Math.PI / 180f)) * 8f;
				m_fakeWheelPivot.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
			}
			UpdateSoundEffect();
		}
	}

	private void UpdateSoundEffect()
	{
		float num = 0.9f;
		float num2 = 1f;
		float num3 = 5f;
		float num4 = 1f;
		if (Mathf.Abs(m_spinSpeed) > num4 && m_grounded)
		{
			float value = (num2 - num) / num3 * Mathf.Abs(m_spinSpeed);
			value = Mathf.Clamp(value, num, num2);
			loopingWheelSound.pitch = value;
			loopingWheelSound.volume = Mathf.Abs(m_spinSpeed) / num4 - 1f;
		}
		else
		{
			loopingWheelSound.volume = 0f;
		}
	}

	private void FixedUpdate()
	{
		if (!base.contraption || !base.contraption.isRunning)
		{
			return;
		}
		ReinitializeEngine();
		if (m_enabled)
		{
			m_thrustTimer += Time.deltaTime * 1f;
			m_thrustTimer = Mathf.Min(m_thrustTimer, 1f);
			m_thrust = Mathf.Pow(m_thrustTimer, 0.4f);
		}
		else
		{
			m_thrustTimer = 0f;
			m_thrust = 0f;
		}
		m_lastPosition = m_wheelPivot.transform.position;
		Debug.DrawRay(m_wheelPivot.transform.position, (m_radius + 0.1f) * m_lastContactDirection);
		RaycastHit hitInfo;
		if (Physics.Raycast(m_wheelPivot.transform.position, m_lastContactDirection, out hitInfo, m_radius + 0.1f) && hitInfo.collider != m_supportCollider)
		{
			float num = SpeedInDirection(base.transform.right);
			Vector3 vector = (m_lastForceDirection = Vector3.Cross(hitInfo.normal, Vector3.forward));
			m_spinSpeed = 0f;
			m_hasContact = true;
			colliderRigidbody = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
			if (m_enabled && m_maximumSpeed > 0f && num < m_maximumSpeed)
			{
				float f = 1f - num / m_maximumSpeed;
				f = Mathf.Pow(f, 0.5f);
				float num2 = m_thrust * m_maximumForce * f;
				base.GetComponent<Rigidbody>().AddForceAtPosition(num2 * vector, hitInfo.point, ForceMode.Force);
				if (!colliderRigidbody && hitInfo.collider.transform.parent != null)
				{
					colliderRigidbody = hitInfo.collider.transform.parent.GetComponent<Rigidbody>();
				}
				if (colliderRigidbody != null && !colliderRigidbody.isKinematic)
				{
					colliderRigidbody.AddForceAtPosition((0f - num2) * vector, hitInfo.point, ForceMode.Force);
				}
				Debug.DrawRay(hitInfo.point, 0.1f * num2 * vector);
			}
			m_grounded = true;
		}
		else
		{
			m_hasContact = false;
			colliderRigidbody = null;
			if (m_enabled)
			{
				m_spinSpeed = m_maximumSpeed;
			}
			else
			{
				m_spinSpeed *= 0.98f;
			}
		}
	}

	private float SpeedInDirection(Vector3 direction)
	{
		Vector3 velocity = base.GetComponent<Rigidbody>().velocity;
		if ((bool)colliderRigidbody)
		{
			velocity -= colliderRigidbody.velocity;
		}
		return Vector3.Dot(velocity, direction);
	}

	public override void ProcessTouch()
	{
		SetEnabled(!m_enabled);
	}

	public override void SetEnabled(bool enabled)
	{
		m_enabled = enabled;
		base.contraption.UpdateEngineStates(base.ConnectedComponent);
	}
}
