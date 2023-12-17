using UnityEngine;

public class InGameBuildMenu : MonoBehaviour
{
	public struct AutoBuildEvent : EventManager.Event
	{
		public int availableBlueprints;

		public AutoBuildEvent(int availableBlueprints)
		{
			this.availableBlueprints = availableBlueprints;
		}
	}

	private void Awake()
	{
		int @int = GameProgress.GetInt("Blueprints_Available");
		base.transform.Find("AutoBuildButton").Find("AmountText").GetComponent<TextMesh>()
			.text = @int.ToString();
		SetAutoBuildAvailable(GameProgress.GetBool(Application.loadedLevelName + "_autobuild_available"));
		EventManager.Connect<AutoBuildEvent>(RefreshAutoBuildButtonAmount);
	}

	private void OnEnable()
	{
		if (DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios)
		{
			IapManager.onPurchaseSucceeded += HandleIapManageronPurchaseSucceeded;
		}
	}

	private void OnDisable()
	{
		if (DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios)
		{
			IapManager.onPurchaseSucceeded -= HandleIapManageronPurchaseSucceeded;
		}
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<AutoBuildEvent>(RefreshAutoBuildButtonAmount);
	}

	public void SetAutoBuildAvailable(bool available)
	{
		Sprite component = base.transform.Find("AutoBuildButton").GetComponent<Sprite>();
		if (available)
		{
			base.transform.Find("AutoBuildButton").Find("AmountText").GetComponent<TextMesh>()
				.text = string.Empty;
			component.m_UVx = 4;
		}
		else
		{
			component.m_UVx = 2;
		}
		component.RebuildMesh();
	}

	public void RefreshAutoBuildButtonAmount(AutoBuildEvent autoBuildData)
	{
		SetAutoBuildAvailable(true);
	}

	private void HandleIapManageronPurchaseSucceeded(IapManager.InAppPurchaseItemType type)
	{
		base.transform.Find("AutoBuildButton").Find("AmountText").GetComponent<TextMesh>()
			.text = GameProgress.GetInt("Blueprints_Available").ToString();
	}
}
