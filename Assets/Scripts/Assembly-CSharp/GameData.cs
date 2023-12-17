using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData : ScriptableObject
{
	[Serializable]
	public class SandBoxInfo
	{
		public string name;

		public Sprite titleSprite;
	}

	public bool m_ghostFeatureEnabled;

	public GUIStyle m_buttonStyle;

	public GameObject m_particles;

	public GameObject m_ballonParticles;

	public GameObject m_dustParticles;

	public Transform m_constructionUIPrefab;

	public Transform m_contraptionPrefab;

	public Transform m_blueprintPrefab;

	public Transform m_hudPrefab;

	public GameObject m_krakSprite;

	public GameObject m_snapSprite;

	public Font m_font;

	public AudioClip m_audioAmbience;

	public float m_jointConnectionStrengthWeak;

	public float m_jointConnectionStrengthNormal;

	public float m_jointConnectionStrengthHigh;

	public float m_jointConnectionStrengthExtreme;

	public bool m_useTouchControls;

	public GameObject singletonSpawner;

	public GameObject effectManager;

	public TextAsset m_partOrderList;

	public List<SandBoxInfo> m_sandboxTitles;

	public List<GameObject> m_parts;

	public List<EpisodeLevels> m_episodeLevels;

	public GameObject GetPart(BasePart.PartType type)
	{
		foreach (GameObject part in m_parts)
		{
			if (part.GetComponent<BasePart>().m_partType == type)
			{
				return part;
			}
		}
		return null;
	}

	public Sprite GetSandboxTitle(string levelName)
	{
		foreach (SandBoxInfo sandboxTitle in m_sandboxTitles)
		{
			if (sandboxTitle.name == levelName)
			{
				return sandboxTitle.titleSprite;
			}
		}
		return null;
	}
}
