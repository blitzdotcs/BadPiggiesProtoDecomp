using UnityEngine;
using UnityEngine.UI;

public class HUD : WPFMonoBehaviour
{
	private void Start()
	{
		LevelStart levelStart = WPFMonoBehaviour.FindSceneObjectOfType<LevelStart>();
		Vector3 position = ((!levelStart) ? Vector3.zero : levelStart.transform.position);
		if ((bool)WPFMonoBehaviour.gameData.m_blueprintPrefab)
		{
			Transform transform = Object.Instantiate(WPFMonoBehaviour.gameData.m_blueprintPrefab, position, Quaternion.identity) as Transform;
			transform.name = "BlueprintUI";
			transform.parent = WPFMonoBehaviour.levelManager.transform;
			if ((bool)WPFMonoBehaviour.levelManager.m_blueprintTexture)
			{
			//	transform.Image.image = WPFMonoBehaviour.levelManager.m_blueprintTexture;
			}
		}
	}
}
