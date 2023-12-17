using UnityEngine;

public class MainMenu : MonoBehaviour
{
	[SerializeField]
	private GameObject mainMenuNode;

	[SerializeField]
	private GameObject creditsMenuPrefab;

	private GameObject audioOffButton;

	private GameObject audioOnButton;

	private GameObject creditsMenu;

	private GameObject gcButton;

	private GameObject iapButton;

	private GameObject fullVersionButton;

	private void Awake()
	{
		audioOffButton = GameObject.Find("AudioOffButton");
		audioOnButton = GameObject.Find("AudioOnButton");
		Assert.IsValid(audioOffButton, "audioOffButton");
		Assert.IsValid(audioOnButton, "audioOnButton");
		Assert.IsValid(mainMenuNode, "mainMenuNode");
		Assert.IsValid(creditsMenuPrefab, "creditsMenuPrefab");
	}

	private void OnEnable()
	{
		GameCenterManager.onAuthenticationSucceeded += ShowGameCenterButton;
	}

	private void OnDisable()
	{
		GameCenterManager.onAuthenticationSucceeded -= ShowGameCenterButton;
		if (BuildCustomizationLoader.Instance.IAPEnabled)
		{
			IapManager.onPurchaseSucceeded -= HandleIapManageronPurchaseSucceeded;
		}
	}

	private void HandleIapManageronPurchaseSucceeded(IapManager.InAppPurchaseItemType type)
	{
		if (type == IapManager.InAppPurchaseItemType.UnlockFullVersion)
		{
			Application.LoadLevel(Application.loadedLevelName);
		}
	}

	private void ShowGameCenterButton(bool show)
	{
		gcButton.active = show;
	}

	private void InitButtons(DeviceInfo.DeviceFamily platform)
	{
		gcButton = GameObject.Find("GameCenterButton");
		iapButton = GameObject.Find("IapButton");
		fullVersionButton = GameObject.Find("UnlockFullVersionButton");
		GameObject gameObject = GameObject.Find("HDBadge");
		gameObject.active = BuildCustomizationLoader.Instance.IsHDVersion;
		gcButton.active = GameCenterManager.Authenticated;
		iapButton.active = false;
		fullVersionButton.SetActiveRecursively(false);
		switch (platform)
		{
		case DeviceInfo.DeviceFamily.Ios:
			if (BuildCustomizationLoader.Instance.IAPEnabled)
			{
				iapButton.active = true;
			}
			else
			{
				gcButton.transform.position -= Vector3.right * gcButton.transform.position.x;
			}
			break;
		case DeviceInfo.DeviceFamily.Android:
			gcButton.active = false;
			if (BuildCustomizationLoader.Instance.IAPEnabled)
			{
				if (BuildCustomizationLoader.Instance.IsFreeVersion)
				{
					fullVersionButton.SetActiveRecursively(true);
					iapButton.active = false;
					fullVersionButton.transform.position -= Vector3.right * fullVersionButton.transform.position.x;
				}
				else
				{
					iapButton.active = true;
					iapButton.transform.position -= Vector3.right * iapButton.transform.position.x;
				}
			}
			break;
		default:
			gcButton.active = false;
			break;
		}
	}

	private void Start()
	{
		creditsMenu = (GameObject)Object.Instantiate(creditsMenuPrefab);
		creditsMenu.SetActiveRecursively(false);
		audioOnButton.active = false;
		audioOffButton.active = false;
		InitButtons(DeviceInfo.Instance.ActiveDeviceFamily);
		RefreshAudioButtonState();
		if (BuildCustomizationLoader.Instance.IAPEnabled)
		{
			IapManager.onPurchaseSucceeded += HandleIapManageronPurchaseSucceeded;
		}
	}

	public void OpenLevelMenu()
	{
		SendStartFlurryEvent();
		Loader.Instance.LoadLevel("EpisodeSelection", true);
	}

	public void CloseCredits()
	{
		mainMenuNode.SetActiveRecursively(true);
		creditsMenu.SetActiveRecursively(false);
		RefreshAudioButtonState();
		InitButtons(DeviceInfo.Instance.ActiveDeviceFamily);
	}

	public void OpenCredits()
	{
		mainMenuNode.SetActiveRecursively(false);
		creditsMenu.SetActiveRecursively(true);
	}

	private void RefreshAudioButtonState()
	{
		if (AudioManager.IsInstantiated() && AudioManager.Instance.AudioMuted)
		{
			audioOnButton.active = false;
			audioOffButton.active = true;
		}
		else
		{
			audioOffButton.active = false;
			audioOnButton.active = true;
		}
	}

	public void ToggleAudio()
	{
		AudioManager.Instance.ToggleMute();
		RefreshAudioButtonState();
	}

	public void OpenGameCenter()
	{
		if (DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios)
		{
			SocialGameManager.Instance.ShowAchievementsView();
		}
	}

	public void OpenIapPurchasePage()
	{
		if (BuildCustomizationLoader.Instance.IAPEnabled)
		{
			IapManager.Instance.EnableBlueprintsPurchasePage(true);
		}
	}

	public void OpenUnlockFullVersionPurchasePage()
	{
		if (BuildCustomizationLoader.Instance.IAPEnabled)
		{
			IapManager.Instance.EnableUnlockFullVersionPurchasePage();
		}
	}

	public void QuitGame()
	{
		Debug.Log("Quitting application");
		Application.Quit();
	}

	public void SendStartFlurryEvent()
	{
		if (BuildCustomizationLoader.Instance.Flurry)
		{
			FlurryManager.Instance.LogEvent("Start");
		}
	}
}
