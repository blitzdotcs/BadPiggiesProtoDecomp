using System.Collections;
using UnityEngine;

public class AdLoader : MonoBehaviour
{
	private bool updateImage;

	private bool pollNewsServer = true;

	private LevelManager levelManager;

	private void Start()
	{
		base.GetComponent<Renderer>().enabled = false;
		levelManager = GameObject.Find("GameManager").GetComponent<LevelManager>();
		Assert.IsValid(levelManager, "levelManager");
		StartCoroutine(ImageLoader());
	}

	private void OnDestroy()
	{
		updateImage = false;
		pollNewsServer = false;
	}

	private string GenerateNewsUrl()
	{
		string text = "ipad";
		string text2 = "full";
		return "http://smoke.rovio.com/content/embed/popup/?d=" + text + "&p=abc&a=" + text2 + "&v=1.0&sw=1024&sh=768";
	}

	private string ExtractImageUrl(string rawNewsDataStr)
	{
		int num = rawNewsDataStr.IndexOf("image") + 9;
		int num2 = rawNewsDataStr.IndexOf('"', num);
		return rawNewsDataStr.Substring(num, num2 - num);
	}

	private void Update()
	{
		if (levelManager.gameState == LevelManager.GameState.PreviewWhileRunning || levelManager.gameState == LevelManager.GameState.PreviewWhileBuilding)
		{
			updateImage = true;
		}
		else
		{
			updateImage = false;
		}
	}

	private IEnumerator ImageLoader()
	{
		string newsUrl = GenerateNewsUrl();
		while (pollNewsServer)
		{
			if (updateImage)
			{
				WWW www = new WWW(newsUrl);
				yield return www;
				string imageUrl = ExtractImageUrl(www.text);
				WWW imageFromUrl = new WWW(imageUrl);
				yield return imageFromUrl;
				Debug.Log("Image loaded: " + imageUrl);
				base.GetComponent<Renderer>().material.mainTexture = imageFromUrl.texture;
				base.GetComponent<Renderer>().enabled = true;
				yield return new WaitForSeconds(5f);
			}
			else
			{
				yield return new WaitForSeconds(0.1f);
				base.GetComponent<Renderer>().enabled = false;
			}
		}
	}
}
