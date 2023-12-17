using System.Collections;
using System.Collections.Generic;

public class StoreKitTransaction
{
	public string productIdentifier;

	public string base64EncodedTransactionReceipt;

	public int quantity;

	public static List<StoreKitTransaction> transactionsFromJson(string json)
	{
		List<StoreKitTransaction> list = new List<StoreKitTransaction>();
		ArrayList arrayList = json.arrayListFromJson();
		foreach (Hashtable item in arrayList)
		{
			list.Add(transactionFromHashtable(item));
		}
		return list;
	}

	public static StoreKitTransaction transactionFromJson(string json)
	{
		return transactionFromHashtable(json.hashtableFromJson());
	}

	public static StoreKitTransaction transactionFromHashtable(Hashtable ht)
	{
		StoreKitTransaction storeKitTransaction = new StoreKitTransaction();
		if (ht.ContainsKey("productIdentifier"))
		{
			storeKitTransaction.productIdentifier = ht["productIdentifier"].ToString();
		}
		if (ht.ContainsKey("base64EncodedReceipt"))
		{
			storeKitTransaction.base64EncodedTransactionReceipt = ht["base64EncodedReceipt"].ToString();
		}
		if (ht.ContainsKey("quantity"))
		{
			storeKitTransaction.quantity = int.Parse(ht["quantity"].ToString());
		}
		return storeKitTransaction;
	}

	public override string ToString()
	{
		return string.Format("<StoreKitTransaction>\nID: {0}\nReceipt: {1}\nQuantity: {2}", productIdentifier, base64EncodedTransactionReceipt, quantity);
	}
}
