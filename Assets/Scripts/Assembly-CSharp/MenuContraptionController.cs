using System.Collections.Generic;
using UnityEngine;

public class MenuContraptionController : MonoBehaviour
{
	public GameData m_gameData;

	private TextAsset m_contraptionData;

	private Contraption m_contraption;

	private float m_timer;

	private bool m_removeTimerStarted;

	private float m_removeTime;

	private bool m_removePhase;

	private Queue<BasePart> m_partsToRemove = new Queue<BasePart>();

	public void StartRemoveTimer(float time)
	{
		m_removeTimerStarted = true;
		m_removeTime = time;
	}

	public Contraption CreateContraption(TextAsset contraptionData)
	{
		m_contraptionData = contraptionData;
		Vector2 vector = base.transform.position;
		vector.x = -5f + -15f * (float)Screen.width / (float)Screen.height;
		base.transform.position = vector;
		Transform transform = Object.Instantiate(m_gameData.m_contraptionPrefab, base.transform.position, Quaternion.identity) as Transform;
		m_contraption = transform.GetComponent<Contraption>();
		ContraptionDataset cds = WPFPrefs.LoadContraptionDataset(m_contraptionData);
		BuildContraption(cds);
		m_contraption.StartContraption();
		m_contraption.ActivateAllPoweredParts();
		return m_contraption;
	}

	private void Update()
	{
		if (m_removeTimerStarted)
		{
			m_timer += Time.deltaTime;
			if (!(m_timer > m_removeTime))
			{
				return;
			}
			m_removeTimerStarted = false;
			m_removePhase = true;
			m_timer = 0f;
			m_partsToRemove = new Queue<BasePart>(m_contraption.Parts);
			m_contraption.DestroyAllJoints();
			{
				foreach (BasePart part in m_contraption.Parts)
				{
					if ((bool)part && (BasePart.BaseType(part.m_partType) == BasePart.PartType.Balloon || BasePart.BaseType(part.m_partType) == BasePart.PartType.Sandbag))
					{
						part.ProcessTouch();
					}
				}
				return;
			}
		}
		if (!m_removePhase)
		{
			return;
		}
		if (m_partsToRemove.Count > 0)
		{
			m_timer += Time.deltaTime;
			if (m_timer > 0.05f)
			{
				m_timer -= 0.05f;
				BasePart basePart = m_partsToRemove.Dequeue();
				if ((bool)basePart)
				{
					WPFMonoBehaviour.effectManager.CreateParticles(m_gameData.m_dustParticles.GetComponent<ParticleSystem>(), basePart.transform.position);
					basePart.transform.position += 1000f * Vector3.right;
				}
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
			Object.Destroy(m_contraption.gameObject);
		}
	}

	private void BuildContraption(ContraptionDataset cds)
	{
		foreach (ContraptionDataset.ContraptionDatasetUnit contraptionDataset in cds.ContraptionDatasetList)
		{
			BasePart component = m_gameData.GetPart((BasePart.PartType)contraptionDataset.partType).GetComponent<BasePart>();
			BuildPart(contraptionDataset, component.GetComponent<BasePart>());
		}
	}

	private BasePart BuildPart(ContraptionDataset.ContraptionDatasetUnit cdu, BasePart partPrefab)
	{
		BasePart basePart = InstantiatePart(cdu.x, cdu.y, partPrefab);
		if (cdu.flipped)
		{
			basePart.SetFlipped(true);
		}
		else
		{
			basePart.SetRotation((BasePart.GridRotation)cdu.rot);
		}
		return basePart;
	}

	private BasePart InstantiatePart(int coordX, int coordY, BasePart partPrefab)
	{
		GameObject gameObject = Object.Instantiate(partPrefab.gameObject) as GameObject;
		BasePart component = gameObject.GetComponent<BasePart>();
		component.PrePlaced();
		m_contraption.SetPartAt(coordX, coordY, component);
		return component;
	}
}
