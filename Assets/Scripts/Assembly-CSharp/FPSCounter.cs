using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
	private Text m_fps;

	private float updateInterval = 0.5f;

	private float accum;

	private float frames;

	private float timeleft;

	private void Awake()
	{
		m_fps = GetComponent(typeof(Text)) as Text;
	//	m_fps.pixelOffset = new Vector2(-Screen.width / 2, -Screen.height / 2);
	//	m_fps.alignment = TextAlignment.Left;
	//	m_fps.anchor = TextAnchor.LowerLeft;
		m_fps.text = "foo";
		Object.DontDestroyOnLoad(this);
	}

	private void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames += 1f;
		/*
		if ((double)timeleft <= 0.0)
		{
			base.Text.text = string.Empty + (accum / frames).ToString("f2");
			timeleft = updateInterval;
			accum = 0f;
			frames = 0f;
		//	m_fps.pixelOffset = new Vector2(-Screen.width / 2, -Screen.height / 2);
		}
		*/
	}
}
