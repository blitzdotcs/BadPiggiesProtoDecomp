using UnityEngine;

public class PitchRandomizer : AudioFX
{
	public float m_pitchMin;

	public float m_pitchMax;

	protected override void ProcessAudio()
	{
		base.ProcessAudio();
		Assert.Check(m_pitchMin <= m_pitchMax, "PitchRandomizer: Min pitch is larger than max, GameObject:" + base.name);
		Mathf.Clamp(m_pitchMin, -3f, m_pitchMax);
		Mathf.Clamp(m_pitchMax, m_pitchMin, 3f);
		base.GetComponent<AudioSource>().pitch = Random.Range(m_pitchMin, m_pitchMax);
	}
}
