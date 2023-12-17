using UnityEngine;

public class EpisodeButton : WPFMonoBehaviour
{
	public Color m_bgcolor;

	[SerializeField]
	private bool m_isSandbox;

	[SerializeField]
	private int m_episodeLevelsGameDataIndex;

	private void Awake()
	{
		base.transform.Find("Background").GetComponent<Renderer>().material.color = m_bgcolor;
		int num = 0;
		int num2 = 0;
		if (m_isSandbox)
		{
			num = CalculateSandboxStars();
			num2 = 20 * WPFMonoBehaviour.gameData.m_sandboxTitles.Count;
		}
		else
		{
			num = CalculateEpisodeStars();
			num2 = 3 * WPFMonoBehaviour.gameData.m_episodeLevels[m_episodeLevelsGameDataIndex].Levels.Count;
		}
		Transform transform = base.transform.Find("StarText");
		if (transform != null)
		{
			transform.GetComponent<TextMesh>().text = string.Empty + num + "/" + num2;
		}
	}

	public void GoToLevelSelection(string levelSelectionPage)
	{
		Loader.Instance.LoadLevel(levelSelectionPage, false);
	}

	private int CalculateEpisodeStars()
	{
		int num = 0;
		foreach (string level in WPFMonoBehaviour.gameData.m_episodeLevels[m_episodeLevelsGameDataIndex].Levels)
		{
			num += GameProgress.GetInt(level + "_stars");
		}
		return num;
	}

	private int CalculateSandboxStars()
	{
		int num = 0;
		foreach (GameData.SandBoxInfo sandboxTitle in WPFMonoBehaviour.gameData.m_sandboxTitles)
		{
			num += GameProgress.GetInt(sandboxTitle.name + "_stars");
		}
		return num;
	}
}
