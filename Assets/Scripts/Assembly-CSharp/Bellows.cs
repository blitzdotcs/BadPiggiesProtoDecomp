using UnityEngine;

public class Bellows : BasePropulsion
{
	public const float BoostDuration = 0.5f;

	public const float WaitDuration = 0.3f;

	public const float InflateDuration = 0.3f;

	public const float CompressedScale = 0.3f;

	public Vector3 m_direction = Vector3.up;

	public bool m_enabled;

	public float m_boostForce = 10f;

	public ParticleSystem smokeEmitter;

	protected float m_timeBoostStarted;

	protected float m_currentScale;

	private bool m_isConnected;

	public override bool CanBeEnabled()
	{
		return m_isConnected;
	}

	public override bool HasOnOffToggle()
	{
		return false;
	}

	public override bool IsEnabled()
	{
		return m_enabled;
	}

	public override Direction EffectDirection()
	{
		return BasePart.Rotate(Direction.Right, m_gridRotation);
	}

	public override void InitializeEngine()
	{
		m_isConnected = base.contraption.ComponentPartCount(base.ConnectedComponent) > 1;
	}

	public void Start()
	{
		m_timeBoostStarted = -1000f;
	}

	public void FixedUpdate()
	{
		float num = Time.time - m_timeBoostStarted;
		if (num > 1.1f)
		{
			m_enabled = false;
			return;
		}
		if (num < 0.5f)
		{
			m_enabled = true;
			float num2 = 1f - num / 0.5f;
			num2 = 1f - num2 * num2;
			float num3 = num2 * m_boostForce;
			Vector3 vector = base.transform.TransformDirection(m_direction);
			Vector3 vector2 = base.transform.position + vector * 0.5f;
			Vector3 vector3 = vector;
			base.GetComponent<Rigidbody>().AddForceAtPosition(num3 * vector3, vector2, ForceMode.Force);
			Debug.DrawRay(vector2, vector3 * num3, Color.green);
		}
		Vector3 one = Vector3.one;
		one.y *= CompressionScale(num);
		base.transform.localScale = one;
	}

	public override void ProcessTouch()
	{
		if (m_isConnected)
		{
			float num = Time.time - m_timeBoostStarted;
			if (!(num < 1.1f))
			{
				m_timeBoostStarted = Time.time;
				AudioManager.Instance.SpawnOneShotEffect(AudioManager.Instance.CommonAudioCollection.bellowsPuff, base.transform.position);
				smokeEmitter.Emit(Random.Range(1, 2));
			}
		}
	}

	public static float CompressionScale(float time)
	{
		float t = 0f;
		if (time < 0.5f)
		{
			t = time / 0.5f;
		}
		else if (time < 0.8f)
		{
			t = 1f;
		}
		else if (time < 1.1f)
		{
			t = 1f - (time - 0.5f - 0.3f) / 0.3f;
		}
		return Mathf.Lerp(1f, 0.3f, t);
	}
}
