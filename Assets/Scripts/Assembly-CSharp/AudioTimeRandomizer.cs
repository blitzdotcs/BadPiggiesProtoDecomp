using System.Collections;
using UnityEngine;

public class AudioTimeRandomizer : MonoBehaviour
{
	[SerializeField]
	private float minDelay = 1f;

	[SerializeField]
	private float maxDelay = 2f;

	private AudioSource baseAudioSource;

	private AudioManager audioManager;

	private bool doRandomize = true;

	private void Start()
	{
		baseAudioSource = GetComponent<AudioSource>();
		audioManager = AudioManager.Instance;
		Assert.IsValid(baseAudioSource, "baseAudioSource");
		Assert.Check(!baseAudioSource.loop, "RandomTimeAudio attached to AudioSource that is a loop");
		StartCoroutine(SpawnAudios());
	}

	private void OnDestroy()
	{
		doRandomize = false;
	}

	private IEnumerator SpawnAudios()
	{
		while (doRandomize)
		{
			yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
			audioManager.PlayOneShotEffect(ref baseAudioSource);
		}
	}
}
