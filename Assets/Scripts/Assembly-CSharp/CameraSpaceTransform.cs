using UnityEngine;

public class CameraSpaceTransform : MonoBehaviour
{
	private const float SizeCorrectionFactor = 0.1f;

	private Camera sourceCamera;

	private Camera targetCamera;

	private Transform proxyTransform;

	private Transform targetTransform;

	public void Start()
	{
		if ((bool)GameObject.Find("3dPropsCamera"))
		{
			sourceCamera = GameObject.Find("GameCamera").GetComponent<Camera>();
			targetCamera = GameObject.Find("3dPropsCamera").GetComponent<Camera>();
			Assert.IsValid(sourceCamera, "sourceCamera");
			Assert.IsValid(targetCamera, "targetCamera");
			proxyTransform = base.transform;
			targetTransform = base.transform.Find("RotatingMap").transform;
			if ((bool)targetTransform && (bool)targetTransform.gameObject && (bool)targetTransform.gameObject.GetComponentInChildren<MeshRenderer>())
			{
				proxyTransform.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
				targetTransform.gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
			}
		}
		else
		{
			Debug.LogWarning("3dPropsCamera is not enabled, camera space transfrom is not used!");
			Object.Destroy(base.gameObject);
		}
	}

	private void LateUpdate()
	{
		Start();
		targetTransform.position = targetCamera.ScreenToWorldPoint(sourceCamera.WorldToScreenPoint(proxyTransform.position));
		Vector3 b = targetCamera.ScreenToWorldPoint(sourceCamera.WorldToScreenPoint(proxyTransform.position + Vector3.right));
		float num = Vector3.Distance(targetTransform.position, b) * 0.1f;
		targetTransform.localScale = new Vector3(num, num, num);
	}
}
