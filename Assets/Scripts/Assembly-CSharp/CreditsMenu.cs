using UnityEngine;

public class CreditsMenu : MonoBehaviour
{
	public void CloseCredits()
	{
		GameObject.Find("MainMenuLogic").GetComponent<MainMenu>().CloseCredits();
	}

	public void Start()
	{
		if (DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios)
		{
			SocialGameManager.Instance.ReportAchievementProgress("grp.LITERATE", 100.0);
		}
	}
}
