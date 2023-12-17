using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
	private void Update()
	{
		if (!Input.GetMouseButtonDown(0))
		{
			return;
		}
		Camera component = GameObject.Find("MainCamera").GetComponent<Camera>();
		Ray ray = component.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo))
		{
			MainMenuButton component2 = hitInfo.collider.gameObject.GetComponent<MainMenuButton>();
			if ((bool)component2)
			{
				Loader.Instance.LoadLevel("MainMenu", true);
			}
		}
	}
}
