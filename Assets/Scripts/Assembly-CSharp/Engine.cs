using UnityEngine;

public class Engine : BasePart
{
	public bool m_running;

	private Transform m_visualizationPart;

	private Vector3 m_visualizationPartPosition;

	private bool m_engineBroken;

	private GameObject loopingSound;

	private AudioSource m_engineSound;

	private AudioManager audioManager;

	public ParticleSystem smokeEmitter;

	public ParticleSystem flameEmitter;

	public override bool CanBeEnabled()
	{
		return true;
	}

	public override bool IsEnabled()
	{
		return m_running;
	}

	public override bool IsIntegralPart()
	{
		return true;
	}

	public override bool WillDetach()
	{
		return false;
	}

	public override bool CanBeEnclosed()
	{
		return true;
	}

	public override bool ValidatePart()
	{
		if (m_enclosedInto != null)
		{
			return true;
		}
		return false;
	}

	public override void Initialize()
	{
		base.contraption.m_enginesAmount++;
		m_visualizationPart = base.transform.GetChild(0);
		m_visualizationPartPosition = m_visualizationPart.localPosition;
	}

	public override void OnDetach()
	{
		if (m_running)
		{
			SetEnabled(false);
		}
		m_engineBroken = true;
		base.contraption.m_enginesAmount--;
		audioManager.RemoveLoopingEffect(ref loopingSound);
		if (smokeEmitter != null)
		{
			smokeEmitter.Stop();
		}
		if (flameEmitter != null)
		{
			flameEmitter.Stop();
		}
		base.OnDetach();
	}

	private void Start()
	{
		audioManager = AudioManager.Instance;
		switch (m_partType)
		{
		case PartType.EngineSmall:
			m_engineSound = audioManager.CommonAudioCollection.electricEngine;
			break;
		case PartType.Engine:
			m_engineSound = audioManager.CommonAudioCollection.engine;
			break;
		case PartType.EngineBig:
			m_engineSound = audioManager.CommonAudioCollection.V8Engine;
			break;
		}
	}

	private void Update()
	{
		if (m_running && !loopingSound)
		{
			loopingSound = audioManager.SpawnLoopingEffect(m_engineSound, base.gameObject.transform);
			loopingSound.GetComponent<AudioSource>().pitch = Mathf.Clamp(0.8f + 0.1f * (float)base.contraption.m_enginesAmount, 0f, 1f);
		}
		else if (!m_running && (bool)loopingSound)
		{
			audioManager.RemoveLoopingEffect(ref loopingSound);
		}
		if (m_running)
		{
			PlayEngineAnimation();
		}
	}

	private void PlayEngineAnimation()
	{
		if (Time.deltaTime > 0f)
		{
			m_visualizationPart.localPosition = m_visualizationPartPosition + (Vector3)Random.insideUnitCircle * 0.1f;
		}
	}

	public override void ProcessTouch()
	{
		if (base.contraption.ActivateAllPoweredParts(base.ConnectedComponent) == 0)
		{
			SetEnabled(!m_running);
		}
	}

	public override void SetEnabled(bool enabled)
	{
		m_running = enabled && !m_engineBroken;
		if (smokeEmitter != null)
		{
			if (enabled)
			{
				smokeEmitter.Play();
			}
			else
			{
				smokeEmitter.Stop();
			}
		}
		if (flameEmitter != null)
		{
			if (enabled)
			{
				flameEmitter.Play();
			}
			else
			{
				flameEmitter.Stop();
			}
		}
	}
}
