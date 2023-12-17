using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public enum AudioMaterial
	{
		None = 0,
		Wood = 1,
		Metal = 2
	}

	private const string AudioMuteKey = "AudioMuted";

	private const float AudioClipRepeatLimit = 0.15f;

	[SerializeField]
	private CommonAudio commonAudio;

	private bool audioMuted;

	private Dictionary<int, float> previousPlayTimes = new Dictionary<int, float>();

	private List<AudioSource> activeLoopingSounds = new List<AudioSource>();

	private List<string> activeOneShotSounds = new List<string>();

	private static AudioManager instance;

	public bool AudioMuted
	{
		get
		{
			return audioMuted;
		}
	}

	public CommonAudio CommonAudioCollection
	{
		get
		{
			return commonAudio;
		}
	}

	public static AudioManager Instance
	{
		get
		{
			return instance;
		}
	}

	public static bool IsInstantiated()
	{
		return instance;
	}

	public void PlayBreakAudio(BasePart breakingPart)
	{
		AudioSource[] array = null;
		switch (breakingPart.AudioMaterial)
		{
		case AudioMaterial.Metal:
			array = CommonAudioCollection.collisionMetalBreak;
			break;
		case AudioMaterial.Wood:
			array = CommonAudioCollection.collisionWoodDestroy;
			break;
		default:
			array = null;
			break;
		}
		if (array != null)
		{
			SpawnOneShotEffect(array, breakingPart.transform.position);
		}
	}

	public void PlayCollisionAudio(BasePart collisionPart, Collision collisionData)
	{
		if (collisionData.collider.tag != "Ground")
		{
			return;
		}
		Rigidbody rigidbody = collisionPart.GetComponent<Rigidbody>();
		AudioSource[] array = null;
		AudioSource[] array2 = null;
		switch (collisionPart.AudioMaterial)
		{
		case AudioMaterial.Metal:
			array = CommonAudioCollection.collisionMetalHit;
			array2 = CommonAudioCollection.collisionMetalDamage;
			break;
		case AudioMaterial.Wood:
			array = CommonAudioCollection.collisionWoodHit;
			array2 = CommonAudioCollection.collisionWoodDamage;
			break;
		default:
			array = null;
			array2 = null;
			break;
		}
		if (array == null || array2 == null)
		{
			return;
		}
		foreach (ContactPoint collisionDatum in collisionData)
		{
			float num = Mathf.Abs(Vector3.Dot(rigidbody.GetPointVelocity(collisionDatum.point), collisionDatum.normal));
			if (num > 2f)
			{
				SpawnOneShotEffect(array2, collisionDatum.point);
			}
			else if (num > 0.1f)
			{
				SpawnOneShotEffect(array, collisionDatum.point);
			}
		}
	}

	public void Play2dEffect(AudioClip effectClip)
	{
		Assert.Check(effectClip != null, "Tried to play AudioClip that is null");
		if (!AudioMuted && CheckRepeatLimit(ref effectClip))
		{
			AudioSource.PlayClipAtPoint(effectClip, Vector3.zero);
		}
	}

	public void SpawnOneShotEffect(AudioSource[] effectSources, Vector3 soundPosition)
	{
		if (!AudioMuted && CheckRepeatLimit(ref effectSources[0]))
		{
			int num = Random.Range(0, effectSources.Length);
			SpawnOneShotEffect(effectSources[num], soundPosition);
		}
	}

	public void SpawnOneShotEffect(AudioSource[] effectSources, Transform sourceParent)
	{
		if (!AudioMuted && CheckRepeatLimit(ref effectSources[0]))
		{
			int num = Random.Range(0, effectSources.Length);
			SpawnOneShotEffect(effectSources[num], sourceParent);
		}
	}

	public void PlayOneShotEffect(ref AudioSource effectSource)
	{
		if (!AudioMuted)
		{
			Assert.Check(effectSource != null, "Tried to play AudioClip that is null");
			effectSource.Play();
		}
	}

	public void PlayLoopingEffect(ref AudioSource effectSource)
	{
		Assert.IsValid(effectSource, "effectSource");
		StartLoopingEffect(ref effectSource, null);
	}

	public void SpawnOneShotEffect(AudioSource effectSource, Vector3 soundPosition)
	{
		if (!AudioMuted && activeOneShotSounds.Count < 20)
		{
			Assert.Check(effectSource != null, "Tried to play AudioClip that is null");
			AudioSource audioSource = (AudioSource)Object.Instantiate(effectSource);
			audioSource.transform.position = soundPosition;
			audioSource.gameObject.name = "AudioOneShot -" + effectSource.name;
			audioSource.Play();
			activeOneShotSounds.Add(audioSource.clip.name);
			StartCoroutine(DestroyOneShotEffect(audioSource));
		}
	}

	private IEnumerator DestroyOneShotEffect(AudioSource audioSource)
	{
		string audioname = audioSource.clip.name;
		yield return new WaitForSeconds(audioSource.clip.length);
		activeOneShotSounds.Remove(audioname);
		if (audioSource != null)
		{
			Object.Destroy(audioSource.gameObject);
		}
	}

	public void SpawnOneShotEffect(AudioSource effectSource, Transform sourceParent)
	{
		if (!AudioMuted && activeOneShotSounds.Count < 20)
		{
			Assert.Check(effectSource != null, "Tried to play AudioClip that is null");
			AudioSource audioSource = (AudioSource)Object.Instantiate(effectSource);
			audioSource.transform.parent = sourceParent;
			audioSource.transform.localPosition = Vector3.zero;
			audioSource.gameObject.name = "AudioOneShot -" + effectSource.name;
			audioSource.Play();
			activeOneShotSounds.Add(audioSource.clip.name);
			StartCoroutine(DestroyOneShotEffect(audioSource));
		}
	}

	public GameObject SpawnLoopingEffect(AudioSource effectSource, Transform soundHost)
	{
		Assert.IsValid(effectSource, "effectSource");
		Transform transform = soundHost.Find("LoopingSound-" + effectSource.GetComponent<AudioSource>().clip.name);
		AudioSource loopingSource = ((!(transform == null)) ? transform.GetComponent<AudioSource>() : ((AudioSource)Object.Instantiate(effectSource)));
		loopingSource.gameObject.name = "LoopingSound-" + loopingSource.GetComponent<AudioSource>().clip.name;
		StartLoopingEffect(ref loopingSource, soundHost);
		return loopingSource.gameObject;
	}

	public void StopLoopingEffect(ref AudioSource loopingEffect)
	{
		if ((bool)loopingEffect && loopingEffect.isPlaying)
		{
			activeLoopingSounds.Remove(loopingEffect);
			AudioVolumeFader component = loopingEffect.gameObject.GetComponent<AudioVolumeFader>();
			if ((bool)component)
			{
				float delay = component.FadeOut();
				StartCoroutine(DelayedStop(loopingEffect, delay));
			}
			else
			{
				loopingEffect.Stop();
			}
		}
	}

	private IEnumerator DelayedStop(AudioSource stoppingSource, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (stoppingSource != null)
		{
			stoppingSource.Stop();
		}
	}

	public void RemoveLoopingEffect(ref GameObject loopingEffect)
	{
		if ((bool)loopingEffect && (bool)loopingEffect.GetComponent<AudioSource>())
		{
			loopingEffect.GetComponent<AudioSource>().Stop();
			activeLoopingSounds.Remove(loopingEffect.GetComponent<AudioSource>());
			loopingEffect = null;
		}
	}

	public void MuteLoopingSounds(bool muteOn)
	{
		foreach (AudioSource activeLoopingSound in activeLoopingSounds)
		{
			if ((bool)activeLoopingSound)
			{
				activeLoopingSound.mute = muteOn;
			}
		}
	}

	private void Awake()
	{
		LoadAudioParams();
		Assert.Check(instance == null, "Singleton " + base.name + " spawned twice");
		instance = this;
		Object.DontDestroyOnLoad(this);
		EventManager.Connect<GameTimePaused>(ReceiveGameTimePaused);
		if (DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Osx || DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Pc)
		{
			KeyListener.keyPressed += HandleKeyListenerkeyPressed;
		}
	}

	private void HandleKeyListenerkeyPressed(KeyCode obj)
	{
		if (obj == KeyCode.S)
		{
			ToggleMute();
		}
	}

	private void StartLoopingEffect(ref AudioSource loopingSource, Transform optionalSoundHost)
	{
		loopingSource.loop = true;
		if ((bool)optionalSoundHost)
		{
			loopingSource.transform.parent = optionalSoundHost;
			loopingSource.transform.localPosition = Vector3.zero;
		}
		AudioVolumeFader component = loopingSource.gameObject.GetComponent<AudioVolumeFader>();
		if ((bool)component)
		{
			component.FadeIn();
		}
		loopingSource.Play();
		loopingSource.mute = audioMuted;
		activeLoopingSounds.Add(loopingSource);
	}

	private bool CheckRepeatLimit(ref AudioClip audioClip)
	{
		int instanceID = audioClip.GetInstanceID();
		if (previousPlayTimes.ContainsKey(instanceID))
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (previousPlayTimes[instanceID] < realtimeSinceStartup - 0.15f)
			{
				previousPlayTimes[instanceID] = realtimeSinceStartup;
				return true;
			}
			return false;
		}
		previousPlayTimes[instanceID] = Time.realtimeSinceStartup;
		return true;
	}

	private bool CheckRepeatLimit(ref AudioSource audioSource)
	{
		int instanceID = audioSource.clip.GetInstanceID();
		if (previousPlayTimes.ContainsKey(instanceID))
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (previousPlayTimes[instanceID] < realtimeSinceStartup - 0.15f)
			{
				previousPlayTimes[instanceID] = realtimeSinceStartup;
				return true;
			}
			return false;
		}
		previousPlayTimes[instanceID] = Time.realtimeSinceStartup;
		return true;
	}

	private void LoadAudioParams()
	{
		audioMuted = UserSettings.GetBool("AudioMuted");
	}

	private void SaveAudioParams()
	{
		UserSettings.SetBool("AudioMuted", audioMuted);
		UserSettings.Save();
	}

	public void ToggleMute()
	{
		audioMuted = !audioMuted;
		if (audioMuted)
		{
			AudioListener.volume = 0f;
		}
		else
		{
			AudioListener.volume = 1f;
		}
		MuteLoopingSounds(AudioMuted);
		SaveAudioParams();
	}

	private void ReceiveGameTimePaused(GameTimePaused data)
	{
		MuteLoopingSounds(data.paused);
	}
}
