using UnityEngine;

public class DestroyWhenNoAudioPlaying : MonoBehaviour
{
	private void Update()
	{
		if ((bool)base.GetComponent<AudioSource>() && !base.GetComponent<AudioSource>().isPlaying)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
