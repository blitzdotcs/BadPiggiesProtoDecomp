using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreKitManager : MonoBehaviour
{
	public static event Action<StoreKitTransaction> purchaseSuccessful;

	public static event Action<List<StoreKitProduct>> productListReceived;

	public static event Action<string> productListRequestFailed;

	public static event Action<string> purchaseFailed;

	public static event Action<string> purchaseCancelled;

	public static event Action<string> receiptValidationFailed;

	public static event Action<string> receiptValidationRawResponseReceived;

	public static event Action receiptValidationSuccessful;

	public static event Action<string> restoreTransactionsFailed;

	public static event Action restoreTransactionsFinished;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void productPurchased(string json)
	{
		if (StoreKitManager.purchaseSuccessful != null)
		{
			StoreKitManager.purchaseSuccessful(StoreKitTransaction.transactionFromJson(json));
		}
	}

	public void productPurchaseFailed(string error)
	{
		if (StoreKitManager.purchaseFailed != null)
		{
			StoreKitManager.purchaseFailed(error);
		}
	}

	public void productPurchaseCancelled(string error)
	{
		if (StoreKitManager.purchaseCancelled != null)
		{
			StoreKitManager.purchaseCancelled(error);
		}
	}

	public void productsReceived(string json)
	{
		if (StoreKitManager.productListReceived != null)
		{
			StoreKitManager.productListReceived(StoreKitProduct.productsFromJson(json));
		}
	}

	public void productsRequestDidFail(string error)
	{
		if (StoreKitManager.productListRequestFailed != null)
		{
			StoreKitManager.productListRequestFailed(error);
		}
	}

	public void validateReceiptFailed(string error)
	{
		if (StoreKitManager.receiptValidationFailed != null)
		{
			StoreKitManager.receiptValidationFailed(error);
		}
	}

	public void validateReceiptRawResponse(string response)
	{
		if (StoreKitManager.receiptValidationRawResponseReceived != null)
		{
			StoreKitManager.receiptValidationRawResponseReceived(response);
		}
	}

	public void validateReceiptFinished(string statusCode)
	{
		if (statusCode == "0")
		{
			if (StoreKitManager.receiptValidationSuccessful != null)
			{
				StoreKitManager.receiptValidationSuccessful();
			}
		}
		else if (StoreKitManager.receiptValidationFailed != null)
		{
			StoreKitManager.receiptValidationFailed("Receipt validation failed with statusCode: " + statusCode);
		}
	}

	public void restoreCompletedTransactionsFailed(string error)
	{
		if (StoreKitManager.restoreTransactionsFailed != null)
		{
			StoreKitManager.restoreTransactionsFailed(error);
		}
	}

	public void restoreCompletedTransactionsFinished(string empty)
	{
		if (StoreKitManager.restoreTransactionsFinished != null)
		{
			StoreKitManager.restoreTransactionsFinished();
		}
	}
}
