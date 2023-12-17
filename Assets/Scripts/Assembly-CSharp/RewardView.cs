using UnityEngine;

public class RewardView : WPFMonoBehaviour
{
	public GameData m_gameData;

	private GameObject m_locked;

	private GameObject m_open;

	private void Awake()
	{
		if ((bool)base.transform.Find("Locked"))
		{
			m_locked = base.transform.Find("Locked").gameObject;
			EnableRendererRecursively(m_locked, false);
		}
		m_open = base.transform.Find("Open").gameObject;
		EnableRendererRecursively(m_open, false);
	}

	public void SetPart(BasePart.PartType type)
	{
		GameObject part = m_gameData.GetPart(type);
		Sprite constructionIconSprite = part.GetComponent<BasePart>().m_constructionIconSprite;
		GameObject gameObject = (GameObject)Object.Instantiate(constructionIconSprite.gameObject);
		gameObject.transform.parent = base.transform.Find("Open").transform.Find("PartOffset");
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
	}

	public bool HasLocked()
	{
		return m_locked != null;
	}

	public void ShowLocked()
	{
		EnableRendererRecursively(m_open, false);
		EnableRendererRecursively(m_locked, true);
	}

	public void ShowOpen()
	{
		if ((bool)m_locked)
		{
			EnableRendererRecursively(m_locked, false);
		}
		EnableRendererRecursively(m_open, true);
	}

	public void Hide()
	{
		EnableRendererRecursively(m_open, false);
		if ((bool)m_locked)
		{
			EnableRendererRecursively(m_locked, false);
		}
	}

	private void EnableRendererRecursively(GameObject obj, bool enable)
	{
		if ((bool)obj.GetComponent<Renderer>())
		{
			obj.GetComponent<Renderer>().enabled = enable;
		}
		if ((bool)obj.GetComponent<Collider>())
		{
			obj.GetComponent<Collider>().enabled = enable;
		}
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			EnableRendererRecursively(obj.transform.GetChild(i).gameObject, enable);
		}
	}
}
