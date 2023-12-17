using UnityEngine;

public class KingPig : BasePart
{
	public ParticleSystem collisionStars;

	public ParticleSystem collisionSweat;

	public ParticleSystem sweatLoop;

	public ParticleSystem starsLoop;

	private float m_starsTimer;

	private float m_sweatTimer;

	public override bool CanBeEnclosed()
	{
		return true;
	}

	public override void Initialize()
	{
		base.GetComponent<Rigidbody>().drag = 0.5f;
		base.GetComponent<Rigidbody>().angularDrag = 1f;
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude >= 4f)
		{
			starsLoop.Play();
			m_starsTimer = 4f;
		}
		if (collision.relativeVelocity.magnitude > 2f)
		{
			collisionStars.Play();
			collisionSweat.Play();
		}
		else if (collision.relativeVelocity.magnitude > 1f)
		{
			collisionSweat.Play();
		}
	}

	private void Update()
	{
		if (!starsLoop.isPlaying)
		{
			return;
		}
		if (m_starsTimer > 0f)
		{
			if (m_starsTimer > 2f)
			{
				starsLoop.emissionRate = m_starsTimer * 2f;
			}
			else
			{
				starsLoop.emissionRate = m_starsTimer;
			}
			m_starsTimer -= Time.deltaTime;
		}
		else
		{
			starsLoop.Stop();
		}
	}
}
