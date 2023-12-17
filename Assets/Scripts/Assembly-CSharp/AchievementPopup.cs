using System.Collections;
using UnityEngine;

public class AchievementPopup : MonoBehaviour
{
	private GameObject m_popup;

	private Material m_icon;

	private TextMesh m_text;

	private TextMeshLocale m_localeText;

	private void Start()
	{
		m_popup = base.transform.Find("Popup").gameObject;
		m_text = base.transform.Find("Popup/Text").GetComponent<TextMesh>();
		m_icon = base.transform.Find("Popup/Icon").GetComponent<Renderer>().material;
		m_localeText = base.transform.Find("Popup/Text").GetComponent<TextMeshLocale>();
		m_popup.transform.position = Vector3.up * 13f;
		Object.DontDestroyOnLoad(this);
	}

	public void Show(string achievementId)
	{
		m_text.text = achievementId;
		m_icon.mainTexture = AchievementData.Instance.AchievementsLimits[achievementId].icon;
		m_localeText.RefreshTranslation();
		m_popup.GetComponent<Animation>().Play("AchievementPopupEnter");
	}

	private IEnumerator Test()
	{
		while (true)
		{
			Show("test");
			yield return new WaitForSeconds(3f);
		}
	}
}
