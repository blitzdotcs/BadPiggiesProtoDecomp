using UnityEngine;

public class ProgressResetButton : MonoBehaviour
{
	private void OnGUI()
	{
		if (GUI.Button(new Rect(0f, 20f, 120f, 100f), "Open Cheats"))
		{
			Loader.Instance.LoadLevel("CheatsPanel", true);
		}
	}
}
