using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioVolumeFader : MonoBehaviour
{
	private enum ActiveFade
	{
		NoFade = 0,
		FadeOut = 1,
		FadeIn = 2
	}

	private const float FadeCoefficient = 2.5f;

	[SerializeField]
	private float fadeTime = 1f;

	private float originalVolume = 1f;

	private float fadeStep = 1f;

	private AudioSource fadeAudioSource;

	private ActiveFade activeFade;

	private void Start()
	{
		fadeAudioSource = base.gameObject.GetComponent<AudioSource>();
		Assert.IsValid(fadeAudioSource, "fadeAudioSource");
		Assert.Check(fadeAudioSource.loop, "AudioVolumeFader attached to AudioSource that is not loop");
		originalVolume = fadeAudioSource.volume;
		fadeStep = originalVolume / fadeTime;
		fadeAudioSource.volume = 0f;
	}

	public float FadeIn()
	{
		activeFade = ActiveFade.FadeIn;
		StartCoroutine(DoFadeIn());
		return fadeTime;
	}

	public float FadeOut()
	{
		activeFade = ActiveFade.FadeOut;
		StartCoroutine(DoFadeOut());
		return fadeTime;
	}

	private IEnumerator DoFadeOut()
	{
		while (activeFade == ActiveFade.FadeIn)
		{
			yield return new WaitForEndOfFrame();
		}
		activeFade = ActiveFade.FadeOut;
		float linearVolume2 = fadeAudioSource.volume;
		while (linearVolume2 > 0f && activeFade != ActiveFade.FadeIn)
		{
			linearVolume2 -= fadeStep * Time.deltaTime;
			linearVolume2 = Mathf.Clamp(linearVolume2, 0f, originalVolume);
			fadeAudioSource.volume = linearVolume2;
			yield return new WaitForEndOfFrame();
		}
		activeFade = ActiveFade.NoFade;
	}

	private IEnumerator DoFadeIn()
	{
		while (activeFade == ActiveFade.FadeOut)
		{
			yield return new WaitForEndOfFrame();
		}
		activeFade = ActiveFade.FadeIn;
		float linearVolume2 = fadeAudioSource.volume;
		while (linearVolume2 < originalVolume && activeFade != ActiveFade.FadeOut)
		{
			linearVolume2 += fadeStep * Time.deltaTime;
			linearVolume2 = Mathf.Clamp(linearVolume2, 0f, originalVolume);
			fadeAudioSource.volume = linearVolume2;
			yield return new WaitForEndOfFrame();
		}
		activeFade = ActiveFade.NoFade;
	}
}
