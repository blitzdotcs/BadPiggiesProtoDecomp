using UnityEngine;

public class Cork : MonoBehaviour
{
	private bool flying;

	private Vector3 velocity;

	private float lifetime;

	private float rotation;

	public void Fly(Vector3 velocity, float rotation, float lifetime)
	{
		this.velocity = velocity;
		this.rotation = rotation;
		this.lifetime = lifetime;
		flying = true;
		AudioManager.Instance.SpawnOneShotEffect(AudioManager.Instance.CommonAudioCollection.bottleCork, base.transform.position);
	}

	private void Update()
	{
		if (flying)
		{
			base.transform.position += Time.deltaTime * (velocity + Time.deltaTime * 9.81f * Vector3.down);
			base.transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * rotation, Vector3.forward);
			lifetime -= Time.deltaTime;
			if (lifetime <= 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
