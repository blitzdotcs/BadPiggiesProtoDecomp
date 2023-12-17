using System.Collections.Generic;
using UnityEngine;

public class StoreKitEventListener : MonoBehaviour
{
	private void OnEnable()
	{
		StoreKitManager.purchaseSuccessful += purchaseSuccessful;
		StoreKitManager.purchaseCancelled += purchaseCancelled;
		StoreKitManager.purchaseFailed += purchaseFailed;
		StoreKitManager.receiptValidationFailed += receiptValidationFailed;
		StoreKitManager.receiptValidationRawResponseReceived += receiptValidationRawResponseReceived;
		StoreKitManager.receiptValidationSuccessful += receiptValidationSuccessful;
		StoreKitManager.productListReceived += productListReceived;
		StoreKitManager.productListRequestFailed += productListRequestFailed;
		StoreKitManager.restoreTransactionsFailed += restoreTransactionsFailed;
		StoreKitManager.restoreTransactionsFinished += restoreTransactionsFinished;
	}

	private void OnDisable()
	{
		StoreKitManager.purchaseSuccessful -= purchaseSuccessful;
		StoreKitManager.purchaseCancelled -= purchaseCancelled;
		StoreKitManager.purchaseFailed -= purchaseFailed;
		StoreKitManager.receiptValidationFailed -= receiptValidationFailed;
		StoreKitManager.receiptValidationRawResponseReceived -= receiptValidationRawResponseReceived;
		StoreKitManager.receiptValidationSuccessful -= receiptValidationSuccessful;
		StoreKitManager.productListReceived -= productListReceived;
		StoreKitManager.productListRequestFailed -= productListRequestFailed;
		StoreKitManager.restoreTransactionsFailed -= restoreTransactionsFailed;
		StoreKitManager.restoreTransactionsFinished -= restoreTransactionsFinished;
	}

	private void productListReceived(List<StoreKitProduct> productList)
	{
		Debug.Log("total productsReceived: " + productList.Count);
		foreach (StoreKitProduct product in productList)
		{
			Debug.Log(product.ToString() + "\n");
		}
	}

	private void productListRequestFailed(string error)
	{
		Debug.Log("productListRequestFailed: " + error);
	}

	private void receiptValidationSuccessful()
	{
		Debug.Log("receipt validation successful");
	}

	private void receiptValidationFailed(string error)
	{
		Debug.Log("receipt validation failed with error: " + error);
	}

	private void receiptValidationRawResponseReceived(string response)
	{
		Debug.Log("receipt validation raw response: " + response);
	}

	private void purchaseFailed(string error)
	{
		Debug.Log("purchase failed with error: " + error);
	}

	private void purchaseCancelled(string error)
	{
		Debug.Log("purchase cancelled with error: " + error);
	}

	private void purchaseSuccessful(StoreKitTransaction transaction)
	{
		Debug.Log("purchased product: " + transaction);
	}

	private void restoreTransactionsFailed(string error)
	{
		Debug.Log("restoreTransactionsFailed: " + error);
	}

	private void restoreTransactionsFinished()
	{
		Debug.Log("restoreTransactionsFinished");
	}
}
