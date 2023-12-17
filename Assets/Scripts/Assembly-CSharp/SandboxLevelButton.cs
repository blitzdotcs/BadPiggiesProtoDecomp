using UnityEngine;

public class SandboxLevelButton : MonoBehaviour
{
	public int m_sandboxIndex;

	[SerializeField]
	private SandboxSelector m_sandboxSelector;

	[SerializeField]
	private TextMesh m_starsText;

	private void Start()
	{
		bool @bool = GameProgress.GetBool("UnlockAllLevels");
		m_starsText.text = GameProgress.SandboxStarCount(m_sandboxSelector.Levels[m_sandboxIndex]) + "/20";
		if (GameProgress.GetBool(m_sandboxSelector.Levels[m_sandboxIndex] + "_sandbox_unlocked") || @bool || BuildCustomizationLoader.Instance.IsDebugBuild)
		{
			Button component = GetComponent<Button>();
			component.MessageTargetObject = m_sandboxSelector.gameObject;
			component.MethodToInvoke = "LoadSandboxLevel";
			component.MessageParameter = m_sandboxIndex.ToString();
			base.transform.Find("Lock").gameObject.active = false;
		}
		else
		{
			base.transform.Find("StarSet").gameObject.SetActiveRecursively(false);
		}
	}

	private void Update()
	{
	}
}
