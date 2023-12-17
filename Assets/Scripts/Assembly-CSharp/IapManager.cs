using System.Collections.Generic;
using UnityEngine;

public class IapManager : MonoBehaviour
{
	public enum InAppPurchaseStatus
	{
		Idle = 0,
		FetchingItems = 1,
		PurchasingItem = 2
	}

	public enum InAppPurchaseItemType
	{
		BlueprintSmall = 0,
		BlueprintMedium = 1,
		BlueprintLarge = 2,
		UnlockAllLevels = 3,
		UnlockFullVersion = 4
	}

	public delegate void PurchaseSucceeded(InAppPurchaseItemType type);

	public delegate void PurchaseFailed(InAppPurchaseItemType type);

	[SerializeField]
	private GameObject m_iapBlueprintsPage;

	[SerializeField]
	private GameObject m_iapUnlockAllLevelsPage;

	[SerializeField]
	private GameObject m_iapUnlockFullVersionPage;

	private Dictionary<InAppPurchaseItemType, string> m_itemDictionary = new Dictionary<InAppPurchaseItemType, string>();

	private InAppPurchaseStatus m_state;

	private InAppPurchaseItemType m_activePurchase;

	private GameObject m_inAppPurchaseKit;

	private bool m_readyForTransaction;

	private static IapManager instance;

	private bool m_restoringPurchases;

	public InAppPurchaseStatus Status
	{
		get
		{
			return m_state;
		}
	}

	public bool ReadyForTransaction
	{
		get
		{
			return m_readyForTransaction;
		}
	}

	public static IapManager Instance
	{
		get
		{
			return instance;
		}
	}

	public static event PurchaseSucceeded onPurchaseSucceeded;

	public static event PurchaseFailed onPurchaseFailed;

	public static bool IsInstantiated()
	{
		return instance;
	}

	private void Awake()
	{
		Assert.Check(instance == null, "Singleton " + base.name + " spawned twice");
		instance = this;
		Object.DontDestroyOnLoad(this);
		InitInAppPurchaseItemDictionary();
		GameObject original = Resources.Load("inAppPurchase/inAppPurchase_iOS", typeof(GameObject)) as GameObject;
		m_inAppPurchaseKit = Object.Instantiate(original) as GameObject;
		m_inAppPurchaseKit.transform.parent = base.transform;
		InitIOSStoreKit();
		InitInAppMenuPages();
		m_state = InAppPurchaseStatus.FetchingItems;
	}

	private void InitIOSStoreKit()
	{
		StoreKitManager.purchaseSuccessful += HandleStoreKitManagerpurchaseSuccessful;
		StoreKitManager.purchaseFailed += HandleStoreKitManagerpurchaseFailed;
		StoreKitManager.purchaseCancelled += HandleStoreKitManagerpurchaseCancelled;
		StoreKitManager.productListReceived += HandleStoreKitManagerproductListReceived;
		StoreKitManager.productListRequestFailed += HandleStoreKitManagerproductListRequestFailed;
		StoreKitManager.restoreTransactionsFinished += HandleStoreKitManagerrestoreTransactionsFinished;
		StoreKitManager.restoreTransactionsFailed += HandleStoreKitManagerrestoreTransactionsFailed;
		FetchPurchasableItemList();
	}

	private void HandleStoreKitManagerproductListRequestFailed(string obj)
	{
		Debug.Log("IapManager::HandleStoreKitManagerproductListRequestFailed()");
		m_state = InAppPurchaseStatus.Idle;
	}

	private void HandleStoreKitManagerproductListReceived(List<StoreKitProduct> obj)
	{
		Debug.Log("IapManager::HandleStoreKitManagerproductListReceived()");
		m_state = InAppPurchaseStatus.Idle;
		m_readyForTransaction = true;
	}

	private void HandleStoreKitManagerpurchaseCancelled(string obj)
	{
		Debug.Log("IapManager::HandleStoreKitManagerpurchaseCancelled()");
		m_state = InAppPurchaseStatus.Idle;
		if (IapManager.onPurchaseFailed != null)
		{
			IapManager.onPurchaseFailed(m_activePurchase);
		}
	}

	private void HandleStoreKitManagerpurchaseFailed(string obj)
	{
		Debug.Log("IapManager::HandleStoreKitManagerpurchaseFailed()");
		m_state = InAppPurchaseStatus.Idle;
		if (IapManager.onPurchaseFailed != null)
		{
			IapManager.onPurchaseFailed(m_activePurchase);
		}
	}

	private void HandleStoreKitManagerpurchaseSuccessful(StoreKitTransaction obj)
	{
		Debug.Log("IapManager::HandleStoreKitManagerpurchaseSuccessful()");
		if (m_restoringPurchases && obj.productIdentifier == m_itemDictionary[InAppPurchaseItemType.UnlockAllLevels])
		{
			m_activePurchase = InAppPurchaseItemType.UnlockAllLevels;
		}
		FinalizeSuccessfulTransaction();
		m_state = InAppPurchaseStatus.Idle;
	}

	private void HandleStoreKitManagerrestoreTransactionsFinished()
	{
		Debug.Log("IapManager::HandleStoreKitManagerrestoreTransactionsFinished()");
		m_state = InAppPurchaseStatus.Idle;
	}

	private void HandleStoreKitManagerrestoreTransactionsFailed(string obj)
	{
		Debug.Log("IapManager::HandleStoreKitManagerrestoreTransactionsFailed(" + obj + ")");
		m_state = InAppPurchaseStatus.Idle;
	}

	public void PurchaseItem(InAppPurchaseItemType type)
	{
		Debug.Log(string.Concat("IapManager::PurchaseItem(", type, ")"));
		if (StoreKitBinding.canMakePayments() && m_readyForTransaction)
		{
			m_state = InAppPurchaseStatus.PurchasingItem;
			m_activePurchase = type;
			StoreKitBinding.purchaseProduct(m_itemDictionary[type], 1);
		}
	}

	public void FetchPurchasableItemList()
	{
		StoreKitBinding.requestProductData(GetPurchasableItemIdentifiers());
	}

	public void RestorePurchasedItems()
	{
		Debug.Log("IapManager::RestorePurchasedItems()");
		if (StoreKitBinding.canMakePayments() && m_readyForTransaction)
		{
			m_restoringPurchases = true;
			StoreKitBinding.restoreCompletedTransactions();
		}
	}

	private void InitInAppPurchaseItemDictionary()
	{
		string text = "badpiggies";
		if (BuildCustomizationLoader.Instance.IsHDVersion)
		{
			text += "hd";
		}
		m_itemDictionary.Add(InAppPurchaseItemType.BlueprintSmall, "com.rovio." + text + ".blueprints_small");
		m_itemDictionary.Add(InAppPurchaseItemType.BlueprintMedium, "com.rovio." + text + ".blueprints_medium");
		m_itemDictionary.Add(InAppPurchaseItemType.BlueprintLarge, "com.rovio." + text + ".blueprints_large");
		m_itemDictionary.Add(InAppPurchaseItemType.UnlockAllLevels, "com.rovio." + text + ".unlockalllevels");
		if (BuildCustomizationLoader.Instance.IsFreeVersion)
		{
			m_itemDictionary.Add(InAppPurchaseItemType.UnlockFullVersion, "com.rovio." + text + ".unlockfullcontent");
		}
	}

	private void InitInAppMenuPages()
	{
		m_iapBlueprintsPage = Object.Instantiate(m_iapBlueprintsPage) as GameObject;
		m_iapBlueprintsPage.transform.parent = base.transform;
		m_iapBlueprintsPage.SendMessage("SetVisible", false);
		m_iapUnlockAllLevelsPage = Object.Instantiate(m_iapUnlockAllLevelsPage) as GameObject;
		m_iapUnlockAllLevelsPage.transform.parent = base.transform;
		m_iapUnlockAllLevelsPage.SendMessage("SetVisible", false);
		m_iapUnlockFullVersionPage = Object.Instantiate(m_iapUnlockFullVersionPage) as GameObject;
		m_iapUnlockFullVersionPage.transform.parent = base.transform;
		m_iapUnlockFullVersionPage.SendMessage("SetVisible", false);
	}

	private string[] GetPurchasableItemIdentifiers()
	{
		string[] array = new string[m_itemDictionary.Count];
		int num = 0;
		foreach (KeyValuePair<InAppPurchaseItemType, string> item in m_itemDictionary)
		{
			array[num] = item.Value;
			num++;
		}
		return array;
	}

	public void EnableBlueprintsPurchasePage(bool enabled)
	{
		m_iapBlueprintsPage.transform.position = GameObject.FindGameObjectWithTag("HUDCamera").transform.position + Vector3.forward * 5f;
		m_iapBlueprintsPage.SendMessage("SetVisible", enabled);
	}

	public void EnableUnlockAllLevelsPurchasePage(bool enabled)
	{
		m_iapUnlockAllLevelsPage.transform.position = GameObject.FindGameObjectWithTag("HUDCamera").transform.position + Vector3.forward * 5f;
		m_iapUnlockAllLevelsPage.SendMessage("SetVisible", enabled);
	}

	public void EnableUnlockFullVersionPurchasePage()
	{
		m_iapUnlockFullVersionPage.transform.position = GameObject.FindGameObjectWithTag("HUDCamera").transform.position + Vector3.forward * 5f;
		m_iapUnlockFullVersionPage.SendMessage("SetVisible", base.enabled);
	}

	private void FinalizeSuccessfulTransaction()
	{
		int @int = GameProgress.GetInt("Blueprints_Available");
		switch (m_activePurchase)
		{
		case InAppPurchaseItemType.BlueprintSmall:
			@int += 3;
			GameProgress.SetInt("Blueprints_Available", @int);
			break;
		case InAppPurchaseItemType.BlueprintMedium:
			@int += 6;
			GameProgress.SetInt("Blueprints_Available", @int);
			break;
		case InAppPurchaseItemType.BlueprintLarge:
			@int += 9;
			GameProgress.SetInt("Blueprints_Available", @int);
			break;
		case InAppPurchaseItemType.UnlockAllLevels:
			GameProgress.SetBool("UnlockAllLevels", true);
			break;
		case InAppPurchaseItemType.UnlockFullVersion:
			GameProgress.SetBool("FullVersionUnlocked", true);
			if (BuildCustomizationLoader.Instance.AdsEnabled)
			{
				BurstlyManager.Instance.DisableAds();
			}
			break;
		}
		if (IapManager.onPurchaseSucceeded != null)
		{
			IapManager.onPurchaseSucceeded(m_activePurchase);
		}
	}
}
