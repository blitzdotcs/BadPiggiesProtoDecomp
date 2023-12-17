using System;
using System.Collections;
using UnityEngine;

public class Pig : BasePart
{
	public enum Expressions
	{
		Normal = 0,
		Happy = 1,
		Happy2 = 2,
		Fear = 3,
		Fear2 = 4,
		Hit = 5,
		Blink = 6,
		MAX = 7
	}

	[Serializable]
	public class Expression
	{
		public Expressions type;

		public Texture texture;

		public AudioSource[] sound;
	}

	public Expression[] m_expressions;

	public float fallFearThreshold;

	public float speedFearThreshold;

	[HideInInspector]
	public Expressions m_currentExpression;

	private float m_blinkTimer;

	private bool m_isPlayingAnimation;

	private Transform m_visualizationPart;

	public ParticleSystem collisionStars;

	public ParticleSystem collisionSweat;

	public ParticleSystem sweatLoop;

	public ParticleSystem starsLoop;

	private float m_starsTimer;

	private float m_rolledDistance;

	private float m_traveledDistance;

	private float m_currentMagnitude;

	private float m_previousMagnitude;

	public float rolledDistance
	{
		get
		{
			return m_rolledDistance;
		}
	}

	public float traveledDistance
	{
		get
		{
			return m_traveledDistance;
		}
	}

	public override bool IsIntegralPart()
	{
		if ((bool)base.enclosedInto)
		{
			return true;
		}
		return false;
	}

	public override bool WillDetach()
	{
		if ((bool)base.enclosedInto)
		{
			return false;
		}
		return base.contraption.NumOfIntegralParts() > 0;
	}

	public override bool CanBeEnclosed()
	{
		return true;
	}

	public override void Initialize()
	{
		m_visualizationPart = base.transform.GetChild(0);
		starsLoop.transform.parent = null;
		sweatLoop.transform.parent = null;
	}

	public override void PostInitialize()
	{
	}

	public override void EnsureRigidbody()
	{
		Rigidbody rigidbody = base.gameObject.GetComponent<Rigidbody>();
		if (rigidbody == null)
		{
			rigidbody = base.gameObject.AddComponent<Rigidbody>();
		}
		rigidbody.constraints = (RigidbodyConstraints)56;
		base.GetComponent<Rigidbody>().mass = m_mass;
		base.GetComponent<Rigidbody>().drag = 0.2f;
		base.GetComponent<Rigidbody>().angularDrag = 0.05f;
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(starsLoop);
		UnityEngine.Object.Destroy(sweatLoop);
	}

	private void FixedUpdate()
	{
		if ((bool)base.contraption && base.contraption.isRunning)
		{
			float magnitude = base.GetComponent<Rigidbody>().velocity.magnitude;
			if (magnitude < 1f)
			{
				base.GetComponent<Rigidbody>().drag = 0.2f + 2.5f * (1f - magnitude);
				base.GetComponent<Rigidbody>().angularDrag = 0.2f + 2.5f * (1f - magnitude);
			}
			else
			{
				base.GetComponent<Rigidbody>().drag = 0.2f;
				base.GetComponent<Rigidbody>().angularDrag = 0.05f;
			}
		}
	}

	private void Start()
	{
		m_traveledDistance = GameProgress.GetFloat("traveledDistance");
		m_rolledDistance = GameProgress.GetFloat("rolledDistance");
	}

	private void Update()
	{
		if (starsLoop != null)
		{
			starsLoop.transform.position = base.transform.position + new Vector3(0f, 0.7f, -0.1f);
		}
		if (sweatLoop != null)
		{
			sweatLoop.transform.position = base.transform.position + new Vector3(0f, 0.06f, -0.6f);
		}
		if (!base.contraption || !base.contraption.isRunning)
		{
			return;
		}
		if (base.enclosedInto == null)
		{
			m_rolledDistance += base.GetComponent<Rigidbody>().velocity.magnitude * Time.deltaTime;
		}
		else
		{
			m_traveledDistance += base.GetComponent<Rigidbody>().velocity.magnitude * Time.deltaTime;
		}
		if (!m_isPlayingAnimation)
		{
			Vector3 velocity = base.GetComponent<Rigidbody>().velocity;
			m_currentMagnitude = velocity.magnitude;
			Expressions expression = Expressions.Normal;
			if (Mathf.Abs(m_currentMagnitude - m_previousMagnitude) > 5f)
			{
				StartCoroutine(PlayAnimation(Expressions.Hit, 1f));
				starsLoop.Play();
				m_starsTimer = 4f;
				m_previousMagnitude = m_currentMagnitude;
				return;
			}
			m_previousMagnitude = m_currentMagnitude;
			if (velocity.magnitude > speedFearThreshold)
			{
				expression = Expressions.Fear;
			}
			else if (velocity.y < 0f - fallFearThreshold)
			{
				expression = Expressions.Fear2;
			}
			m_blinkTimer -= Time.deltaTime;
			if (m_blinkTimer <= 0f && m_currentExpression == Expressions.Normal)
			{
				StartCoroutine(PlayAnimation(Expressions.Blink, 0.1f));
				m_blinkTimer = UnityEngine.Random.Range(1.5f, 3f);
				return;
			}
			SetExpression(expression);
		}
		if (!base.contraption.HasComponentEngine(base.ConnectedComponent) && base.contraption.HasPoweredPartsRunning(base.ConnectedComponent))
		{
			PlayWorkingAnimation();
			if (!sweatLoop.isPlaying)
			{
				sweatLoop.Play();
			}
		}
		else if (sweatLoop.isPlaying)
		{
			sweatLoop.Stop();
		}
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

	public void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > 2f)
		{
			collisionStars.Play();
		}
		else if (!(collision.relativeVelocity.magnitude > 1f))
		{
		}
	}

	public void SetExpression(Expressions exp)
	{
		if (m_currentExpression != exp)
		{
			AudioSource[] sound = m_expressions[(int)exp].sound;
			if (sound.Length > 0)
			{
				AudioManager.Instance.SpawnOneShotEffect(sound, base.transform);
			}
		}
		m_currentExpression = exp;
		m_visualizationPart.GetComponent<Renderer>().material.mainTexture = m_expressions[(int)exp].texture;
	}

	public IEnumerator PlayAnimation(Expressions exp, float time)
	{
		m_isPlayingAnimation = true;
		SetExpression(exp);
		yield return new WaitForSeconds(time);
		m_isPlayingAnimation = false;
	}

	public IEnumerator PlayLaughterAnimation()
	{
		AudioSource[] expressionSound = m_expressions[1].sound;
		if (expressionSound.Length > 0)
		{
			AudioManager.Instance.SpawnOneShotEffect(expressionSound, base.transform);
		}
		m_isPlayingAnimation = true;
		while (m_isPlayingAnimation)
		{
			m_visualizationPart.GetComponent<Renderer>().material.mainTexture = m_expressions[1].texture;
			yield return new WaitForSeconds(0.1f);
			m_visualizationPart.GetComponent<Renderer>().material.mainTexture = m_expressions[2].texture;
			yield return new WaitForSeconds(0.1f);
		}
	}

	private void PlayWorkingAnimation()
	{
		m_visualizationPart.localScale = new Vector3(Mathf.PingPong(Time.time * 0.2f + 0.1f, 0.1f) + 1f, Mathf.PingPong(Time.time * 0.2f, 0.1f) + 0.9f, 1f);
		m_visualizationPart.localPosition = new Vector3(0f, 0f - Mathf.PingPong(Time.time * 0.2f + 0.1f, 0.1f), 0f);
	}

	public override void ProcessTouch()
	{
		base.contraption.ActivateAllPoweredParts(base.ConnectedComponent);
	}
}
