using UnityEngine;

public class Assert : MonoBehaviour
{
	public static bool IsValid(Object objectToCheck, string objectName)
	{
		if ((bool)objectToCheck)
		{
			return true;
		}
		Debug.LogError("Object is not valid: " + objectName, objectToCheck);
		Debug.Break();
		return false;
	}

	public static void ErrorBreak(string message)
	{
		Debug.LogError("Error: " + message);
		Debug.Break();
	}

	public static void Check(bool condition, string errorMessage)
	{
		if (!condition)
		{
			Debug.LogError(errorMessage);
			Debug.Break();
		}
	}
}
