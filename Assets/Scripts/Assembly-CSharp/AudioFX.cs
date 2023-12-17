using UnityEngine;

public class AudioFX : MonoBehaviour
{
	public enum FXType
	{
		Preprocess = 0,
		Continuous = 1
	}

	[SerializeField]
	protected FXType type;

	public virtual void Awake()
	{
		if (type == FXType.Preprocess)
		{
			ProcessAudio();
		}
	}

	public virtual void Update()
	{
		if (type == FXType.Continuous)
		{
			ProcessAudio();
		}
	}

	protected virtual void ProcessAudio()
	{
		if (!(base.GetComponent<AudioSource>() == null))
		{
		}
	}
}
