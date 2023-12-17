using System.Collections;
using System.Collections.Generic;

public class StoreKitProduct
{
	public string productIdentifier;

	public string title;

	public string description;

	public string price;

	public string currencySymbol;

	public string currencyCode;

	public string formattedPrice;

	public static List<StoreKitProduct> productsFromJson(string json)
	{
		List<StoreKitProduct> list = new List<StoreKitProduct>();
		ArrayList arrayList = json.arrayListFromJson();
		foreach (Hashtable item in arrayList)
		{
			list.Add(productFromHashtable(item));
		}
		return list;
	}

	public static StoreKitProduct productFromHashtable(Hashtable ht)
	{
		StoreKitProduct storeKitProduct = new StoreKitProduct();
		if (ht.ContainsKey("productIdentifier"))
		{
			storeKitProduct.productIdentifier = ht["productIdentifier"].ToString();
		}
		if (ht.ContainsKey("localizedTitle"))
		{
			storeKitProduct.title = ht["localizedTitle"].ToString();
		}
		if (ht.ContainsKey("localizedDescription"))
		{
			storeKitProduct.description = ht["localizedDescription"].ToString();
		}
		if (ht.ContainsKey("price"))
		{
			storeKitProduct.price = ht["price"].ToString();
		}
		if (ht.ContainsKey("currencySymbol"))
		{
			storeKitProduct.currencySymbol = ht["currencySymbol"].ToString();
		}
		if (ht.ContainsKey("currencyCode"))
		{
			storeKitProduct.currencyCode = ht["currencyCode"].ToString();
		}
		if (ht.ContainsKey("formattedPrice"))
		{
			storeKitProduct.formattedPrice = ht["formattedPrice"].ToString();
		}
		return storeKitProduct;
	}

	public override string ToString()
	{
		return string.Format("<StoreKitProduct>\nID: {0}\nTitle: {1}\nDescription: {2}\nPrice: {3}\nCurrency Symbol: {4}\nFormatted Price: {5}\nCurrency Code: {6}", productIdentifier, title, description, price, currencySymbol, formattedPrice, currencyCode);
	}
}
