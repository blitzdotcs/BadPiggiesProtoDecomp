using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public class AudioArea : MonoBehaviour
{
	private enum TriggeringType
	{
		Pig = 0,
		AudioListener = 1
	}

	[SerializeField]
	private TriggeringType triggeringType;

	private BoxCollider boxCollider;

	private AudioSource areaAudioSource;

	private AudioManager audioManager;

	private void Start()
	{
		boxCollider = GetComponent<BoxCollider>();
		areaAudioSource = GetComponent<AudioSource>();
		audioManager = AudioManager.Instance;
		Assert.IsValid(boxCollider, "boxCollider");
		Assert.Check(boxCollider.isTrigger, "AudioArea collider is not a trigger.");
		Assert.IsValid(areaAudioSource, "areaAudioSource");
	}

	private bool CheckTriggerType(ref Collider other)
	{
		if (triggeringType == TriggeringType.AudioListener && (bool)other.GetComponent<AudioListener>())
		{
			return true;
		}
		if (triggeringType == TriggeringType.Pig && (bool)other.GetComponent<Pig>())
		{
			return true;
		}
		return false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!CheckTriggerType(ref other))
		{
			return;
		}
		Debug.Log("Object: " + other.name + " entered audio area '" + base.name + "'");
		if (!areaAudioSource.isPlaying)
		{
			if (areaAudioSource.loop)
			{
				audioManager.PlayLoopingEffect(ref areaAudioSource);
			}
			else
			{
				audioManager.PlayOneShotEffect(ref areaAudioSource);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (CheckTriggerType(ref other))
		{
			Debug.Log("Object: " + other.name + " exited audio area '" + base.name + "'");
			if (areaAudioSource.isPlaying && areaAudioSource.loop)
			{
				audioManager.StopLoopingEffect(ref areaAudioSource);
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (!boxCollider)
		{
			boxCollider = GetComponent<BoxCollider>();
		}
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(base.transform.position, boxCollider.bounds.size);
	}
}
