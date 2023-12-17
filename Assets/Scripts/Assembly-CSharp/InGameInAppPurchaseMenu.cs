using UnityEngine;

public class InGameInAppPurchaseMenu : MonoBehaviour
{
	public delegate void ViewDismissed(string name);

	private GameObject m_loader;

	public static event ViewDismissed onViewDismissed;

	private void Start()
	{
		if (!IapManager.Instance.ReadyForTransaction && IapManager.Instance.Status == IapManager.InAppPurchaseStatus.Idle)
		{
			IapManager.Instance.FetchPurchasableItemList();
		}
	}

	private void OnEnable()
	{
		IapManager.onPurchaseSucceeded += HandleIapManageronPurchaseSucceeded;
		IapManager.onPurchaseFailed += HandleIapManageronPurchaseFailed;
		if (m_loader == null)
		{
			m_loader = base.transform.Find("PurchaseLoader").gameObject;
		}
		m_loader.SetActiveRecursively(false);
	}

	private void OnDisable()
	{
		IapManager.onPurchaseSucceeded -= HandleIapManageronPurchaseSucceeded;
		IapManager.onPurchaseFailed -= HandleIapManageronPurchaseFailed;
		if ((bool)m_loader)
		{
			m_loader.SetActiveRecursively(false);
		}
	}

	public void PurchaseBlueprintPackSmall()
	{
		if (IapManager.Instance.ReadyForTransaction && IapManager.Instance.Status == IapManager.InAppPurchaseStatus.Idle)
		{
			IapManager.Instance.PurchaseItem(IapManager.InAppPurchaseItemType.BlueprintSmall);
			m_loader.SetActiveRecursively(true);
		}
	}

	public void PurchaseBlueprintPackMedium()
	{
		if (IapManager.Instance.ReadyForTransaction && IapManager.Instance.Status == IapManager.InAppPurchaseStatus.Idle)
		{
			IapManager.Instance.PurchaseItem(IapManager.InAppPurchaseItemType.BlueprintMedium);
			m_loader.SetActiveRecursively(true);
		}
	}

	public void PurchaseBlueprintPackLarge()
	{
		if (IapManager.Instance.ReadyForTransaction && IapManager.Instance.Status == IapManager.InAppPurchaseStatus.Idle)
		{
			IapManager.Instance.PurchaseItem(IapManager.InAppPurchaseItemType.BlueprintLarge);
			m_loader.SetActiveRecursively(true);
		}
	}

	public void PurchaseUnlockAllLevels()
	{
		if (IapManager.Instance.ReadyForTransaction && IapManager.Instance.Status == IapManager.InAppPurchaseStatus.Idle)
		{
			IapManager.Instance.PurchaseItem(IapManager.InAppPurchaseItemType.UnlockAllLevels);
			m_loader.SetActiveRecursively(true);
		}
	}

	public void PurchaseUnlockFullVersion()
	{
		if (IapManager.Instance.ReadyForTransaction && IapManager.Instance.Status == IapManager.InAppPurchaseStatus.Idle)
		{
			IapManager.Instance.PurchaseItem(IapManager.InAppPurchaseItemType.UnlockFullVersion);
			m_loader.SetActiveRecursively(true);
		}
	}

	public void DismissDialog()
	{
		SetVisible(false);
	}

	private void HandleIapManageronPurchaseSucceeded(IapManager.InAppPurchaseItemType type)
	{
		SetVisible(false);
	}

	private void HandleIapManageronPurchaseFailed(IapManager.InAppPurchaseItemType type)
	{
		m_loader.SetActiveRecursively(false);
	}

	public void SetVisible(bool visible)
	{
		base.gameObject.SetActiveRecursively(visible);
		if (visible)
		{
			m_loader.SetActiveRecursively(!visible);
		}
		base.gameObject.active = true;
		if (!visible && InGameInAppPurchaseMenu.onViewDismissed != null)
		{
			InGameInAppPurchaseMenu.onViewDismissed(base.name);
		}
	}
}
