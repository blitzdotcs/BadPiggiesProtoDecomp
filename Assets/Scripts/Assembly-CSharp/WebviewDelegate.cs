using UnityEngine;

public class WebviewDelegate : MonoBehaviour
{
	public virtual void webViewDidFinishLoad(string pageTitle)
	{
		Debug.Log("webViewDidFinishLoad: " + pageTitle);
	}

	public virtual void webViewDidFail(string errorCode)
	{
		Debug.Log("webViewDidFail: " + errorCode);
	}
}
