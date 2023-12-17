using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
	private enum AwarnessState
	{
		Sleeping = 0,
		Watching = 1,
		Attacking = 2
	}

	[SerializeField]
	private Material birdAwake;

	[SerializeField]
	private AudioClip wakeUpSound;

	[SerializeField]
	private AudioClip attackSound;

	[SerializeField]
	private float attackDistance = 8f;

	[SerializeField]
	private float wakeUpDistance = 10f;

	[SerializeField]
	private float attackPower = 1000f;

	private LevelManager levelManager;

	private Pig targetPig;

	private Transform cachedTransform;

	private float attackForce;

	private ParticleSystem sleepingParticles;

	private BasePart[] noisyContraptionParts = new BasePart[0];

	private AwarnessState awarnessState;

	private Vector3 originalPosition;

	private Quaternion originalRotation;

	private Material originalMaterial;

	private LevelManager.GameState oldGameState;

	private void ResetToInitialState()
	{
		cachedTransform.position = originalPosition;
		cachedTransform.rotation = originalRotation;
		sleepingParticles.GetComponent<Renderer>().enabled = true;
		sleepingParticles.Play();
		base.GetComponent<Renderer>().material = originalMaterial;
		targetPig = null;
		base.GetComponent<Rigidbody>().useGravity = false;
		base.GetComponent<Rigidbody>().velocity = Vector3.zero;
		base.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		awarnessState = AwarnessState.Sleeping;
	}

	private void Start()
	{
		cachedTransform = base.transform;
		levelManager = GameObject.Find("GameManager").GetComponent<LevelManager>();
		Assert.IsValid(levelManager, "levelManager");
		sleepingParticles = GetComponentInChildren<ParticleSystem>();
		Assert.IsValid(sleepingParticles, "sleepingParticles");
		base.GetComponent<Rigidbody>().useGravity = false;
		attackForce = attackPower * base.GetComponent<Rigidbody>().mass;
		originalPosition = base.transform.position;
		originalRotation = base.transform.rotation;
		originalMaterial = base.GetComponent<Renderer>().material;
	}

	private bool IsNoisy(BasePart part)
	{
		if (part.m_partType == BasePart.PartType.Engine || part.m_partType == BasePart.PartType.Propeller || part.m_partType == BasePart.PartType.Rocket || part.m_partType == BasePart.PartType.Rotor)
		{
			return true;
		}
		return false;
	}

	private void CheckForWakeUp()
	{
		if (!targetPig)
		{
			targetPig = (Pig)Object.FindObjectOfType(typeof(Pig));
			Contraption contraption = (Contraption)Object.FindObjectOfType(typeof(Contraption));
			BasePart[] componentsInChildren = contraption.GetComponentsInChildren<BasePart>();
			List<BasePart> list = new List<BasePart>();
			BasePart[] array = componentsInChildren;
			foreach (BasePart basePart in array)
			{
				if (IsNoisy(basePart))
				{
					list.Add(basePart);
				}
			}
			noisyContraptionParts = list.ToArray();
			return;
		}
		BasePart[] array2 = noisyContraptionParts;
		foreach (BasePart basePart2 in array2)
		{
			if (Vector3.Distance(basePart2.transform.position, cachedTransform.position) < wakeUpDistance)
			{
				AudioSource component = basePart2.gameObject.GetComponent<AudioSource>();
				if ((bool)component && component.isPlaying)
				{
					DoWakeUp();
					break;
				}
			}
		}
	}

	private void CheckForAttack()
	{
		if (Vector3.Distance(targetPig.transform.position, cachedTransform.position) < attackDistance)
		{
			awarnessState = AwarnessState.Attacking;
			StartCoroutine(Attack());
		}
	}

	private void DoWakeUp()
	{
		sleepingParticles.GetComponent<Renderer>().enabled = false;
		base.GetComponent<Renderer>().material = birdAwake;
		AudioManager.Instance.Play2dEffect(wakeUpSound);
		awarnessState = AwarnessState.Watching;
	}

	private IEnumerator Attack()
	{
		awarnessState = AwarnessState.Attacking;
		yield return new WaitForSeconds(0.8f);
		base.GetComponent<Rigidbody>().useGravity = true;
		yield return new WaitForFixedUpdate();
		base.GetComponent<Rigidbody>().AddForce(Vector3.up * attackForce * 0.5f);
		base.GetComponent<Rigidbody>().AddTorque(new Vector3(0f, 0f, 100f));
		yield return new WaitForSeconds(1f);
		base.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		AudioManager.Instance.Play2dEffect(attackSound);
		Vector3 targetDir = (targetPig.transform.position - cachedTransform.position).normalized;
		base.GetComponent<Rigidbody>().AddForce(targetDir * attackForce);
	}

	private void Update()
	{
		if (levelManager.gameState == LevelManager.GameState.Running)
		{
			if (awarnessState == AwarnessState.Sleeping)
			{
				CheckForWakeUp();
			}
			else if (awarnessState == AwarnessState.Watching)
			{
				CheckForAttack();
			}
		}
		if (levelManager.gameState == LevelManager.GameState.Building && oldGameState != LevelManager.GameState.Building)
		{
			Debug.Log("Gamestate changed to building");
			oldGameState = LevelManager.GameState.Building;
			ResetToInitialState();
		}
		oldGameState = levelManager.gameState;
	}
}
