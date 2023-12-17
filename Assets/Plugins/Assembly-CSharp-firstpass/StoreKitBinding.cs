using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class StoreKitBinding
{
	[DllImport("__Internal")]
	private static extern bool _storeKitCanMakePayments();

	public static bool canMakePayments()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _storeKitCanMakePayments();
		}
		return false;
	}

	[DllImport("__Internal")]
	private static extern void _storeKitRequestProductData(string productIdentifier);

	public static void requestProductData(string[] productIdentifiers)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_storeKitRequestProductData(string.Join(",", productIdentifiers));
		}
	}

	[DllImport("__Internal")]
	private static extern void _storeKitPurchaseProduct(string productIdentifier, int quantity);

	public static void purchaseProduct(string productIdentifier, int quantity)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_storeKitPurchaseProduct(productIdentifier, quantity);
		}
	}

	[DllImport("__Internal")]
	private static extern void _storeKitRestoreCompletedTransactions();

	public static void restoreCompletedTransactions()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_storeKitRestoreCompletedTransactions();
		}
	}

	[DllImport("__Internal")]
	private static extern void _storeKitValidateReceipt(string base64EncodedTransactionReceipt, bool isTest);

	public static void validateReceipt(string base64EncodedTransactionReceipt, bool isTest)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_storeKitValidateReceipt(base64EncodedTransactionReceipt, isTest);
		}
	}

	[DllImport("__Internal")]
	private static extern void _storeKitValidateAutoRenewableReceipt(string base64EncodedTransactionReceipt, string secret, bool isTest);

	public static void validateAutoRenewableReceipt(string base64EncodedTransactionReceipt, string secret, bool isTest)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_storeKitValidateAutoRenewableReceipt(base64EncodedTransactionReceipt, secret, isTest);
		}
	}

	[DllImport("__Internal")]
	private static extern string _storeKitGetAllSavedTransactions();

	public static List<StoreKitTransaction> getAllSavedTransactions()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string json = _storeKitGetAllSavedTransactions();
			return StoreKitTransaction.transactionsFromJson(json);
		}
		return new List<StoreKitTransaction>();
	}
}
