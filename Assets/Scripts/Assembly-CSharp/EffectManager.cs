using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
	private class ParticleManager
	{
		private GameObject m_parent;

		private int maxSystems = 5;

		private ParticleSystem m_prefab;

		private Queue<ParticleSystem> m_playing = new Queue<ParticleSystem>();

		private Queue<ParticleSystem> m_stopped = new Queue<ParticleSystem>();

		public ParticleManager(ParticleSystem prefab, GameObject parent)
		{
			m_parent = parent;
			m_prefab = prefab;
		}

		public void Update()
		{
			if (m_playing.Count > 0)
			{
				ParticleSystem particleSystem = m_playing.Peek();
				if (!particleSystem.isPlaying)
				{
					m_playing.Dequeue();
					m_stopped.Enqueue(particleSystem);
				}
			}
		}

		public void CreateParticles(Vector3 position)
		{
			if (m_stopped.Count > 0)
			{
				ParticleSystem particleSystem = m_stopped.Dequeue();
				particleSystem.transform.position = position;
				particleSystem.Play();
				m_playing.Enqueue(particleSystem);
			}
			else if (m_playing.Count < maxSystems)
			{
				ParticleSystem component = ((GameObject)Object.Instantiate(m_prefab.gameObject, position, Quaternion.identity)).GetComponent<ParticleSystem>();
				component.transform.parent = m_parent.transform;
				m_playing.Enqueue(component);
			}
		}
	}

	private GameObject m_snapSprite;

	private GameObject m_krakSprite;

	private GameData gameData;

	private Dictionary<ParticleSystem, ParticleManager> m_particles = new Dictionary<ParticleSystem, ParticleManager>();

	private void Awake()
	{
		gameData = GameManager.Instance.gameData;
		m_snapSprite = (GameObject)Object.Instantiate(gameData.m_snapSprite);
		m_snapSprite.GetComponent<Renderer>().enabled = false;
		m_krakSprite = (GameObject)Object.Instantiate(gameData.m_krakSprite);
		m_krakSprite.GetComponent<Renderer>().enabled = false;
	}

	private void Update()
	{
		foreach (KeyValuePair<ParticleSystem, ParticleManager> particle in m_particles)
		{
			particle.Value.Update();
		}
	}

	public void CreateParticles(GameObject prefab, Vector3 position)
	{
		CreateParticles(prefab.GetComponent<ParticleSystem>(), position);
	}

	public void CreateParticles(ParticleSystem prefab, Vector3 position)
	{
		ParticleManager particleManager = GetParticleManager(prefab);
		particleManager.CreateParticles(position);
	}

	public void ShowBreakEffect(GameObject sprite, Vector3 position, Quaternion rotation)
	{
		if (sprite == gameData.m_snapSprite)
		{
			if (!m_snapSprite.GetComponent<Renderer>().enabled)
			{
				m_snapSprite.transform.position = position;
				m_snapSprite.transform.rotation = rotation;
				m_snapSprite.GetComponent<TimedHide>().Show();
			}
		}
		else if (sprite == gameData.m_krakSprite && !m_krakSprite.GetComponent<Renderer>().enabled)
		{
			m_krakSprite.transform.position = position;
			m_krakSprite.transform.rotation = rotation;
			m_krakSprite.GetComponent<TimedHide>().Show();
		}
	}

	private ParticleManager GetParticleManager(ParticleSystem prefab)
	{
		ParticleManager value;
		if (m_particles.TryGetValue(prefab, out value))
		{
			return value;
		}
		value = new ParticleManager(prefab, base.gameObject);
		m_particles[prefab] = value;
		return value;
	}
}
