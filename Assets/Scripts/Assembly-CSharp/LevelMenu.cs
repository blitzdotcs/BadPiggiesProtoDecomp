using UnityEngine;

public class LevelMenu : MonoBehaviour
{
	private void Awake()
	{
	}

	private void Update()
	{
	}

	public void BackButtonPressed()
	{
		Loader.Instance.LoadLevel("MainMenu", true);
	}

	public void OpenEpisode(string episode)
	{
		Application.LoadLevel(episode);
	}
}
