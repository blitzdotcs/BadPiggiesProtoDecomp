using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
	public List<GameObject> m_cameraPrefabs;

	private void Awake()
	{
		Vector3 position = base.transform.position;
		position.z += -100f;
		foreach (GameObject cameraPrefab in m_cameraPrefabs)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(cameraPrefab);
			gameObject.name = cameraPrefab.name;
			gameObject.transform.parent = base.transform;
			gameObject.transform.position = position;
		}
	}
}
