using System.Collections;
using UnityEngine;

public class LevelClickDetector : MonoBehaviour
{
	private const float PigDistanceLimit = 0.1f;

	private const float PigMovementSpeed = 5f;

	[SerializeField]
	private LevelButtonConnection[] connections;

	[SerializeField]
	private LineRenderer connectionLinePrefab;

	private Camera guiCamera;

	private GameObject pigIcon;

	private LevelButton targetButton;

	private bool pigIsMoving;

	private void Start()
	{
		guiCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
		Assert.IsValid(guiCamera, "guiCamera");
		pigIcon = GameObject.Find("CharacterPigOnMap");
		Assert.IsValid(pigIcon, "pigIcon");
		DrawConnections();
	}

	private void DrawConnections()
	{
		Vector3 vector = new Vector3(0f, 0f, 1f);
		LevelButtonConnection[] array = connections;
		foreach (LevelButtonConnection levelButtonConnection in array)
		{
			LineRenderer lineRenderer = (LineRenderer)Object.Instantiate(connectionLinePrefab);
			lineRenderer.SetPosition(0, levelButtonConnection.levelButton1.transform.position + vector);
			lineRenderer.SetPosition(1, levelButtonConnection.levelButton2.transform.position + vector);
		}
	}

	private void Update()
	{
		if (pigIsMoving || !Input.GetMouseButtonDown(0))
		{
			return;
		}
		Ray ray = guiCamera.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin, ray.direction * 100f, Color.yellow, 2f);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo))
		{
			return;
		}
		targetButton = hitInfo.collider.gameObject.GetComponent<LevelButton>();
		if ((bool)targetButton)
		{
			string text = targetButton.OnLevelButtonClicked();
			Debug.Log("Level button clicked " + text + " distance: " + Vector3.Distance(targetButton.transform.position, pigIcon.transform.position));
			if (Vector3.Distance(targetButton.transform.position, pigIcon.transform.position) <= 0.1f)
			{
				Debug.Log("Pig is in target, start level: " + text);
			}
			else
			{
				StartCoroutine(MovePig(text));
			}
		}
	}

	private IEnumerator MovePig(string levelToStart)
	{
		pigIsMoving = true;
		while (Vector3.Distance(targetButton.transform.position, pigIcon.transform.position) > 0.1f)
		{
			yield return new WaitForEndOfFrame();
			Vector3 direction = targetButton.transform.position - pigIcon.transform.position;
			direction.Normalize();
			pigIcon.transform.position += direction * Time.deltaTime * 5f;
			pigIcon.transform.position = new Vector3(pigIcon.transform.position.x, pigIcon.transform.position.y, -0.1f);
		}
		yield return new WaitForSeconds(0.1f);
		pigIsMoving = false;
		if (levelToStart != string.Empty)
		{
			Loader.Instance.LoadLevel(levelToStart, true);
		}
	}
}
