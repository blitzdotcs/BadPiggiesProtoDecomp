using UnityEngine;

public class GameTime : MonoBehaviour
{
	private static GameTime m_instance;

	private static float m_realTimeDelta;

	private static float m_previousRealTime;

	private static float m_previousFixedUpdateTime;

	private static float m_fixedUpdateRealTimeDelta;

	private static bool m_isFixedUpdate;

	private static bool m_paused;

	public static float RealTimeDelta
	{
		get
		{
			if (m_isFixedUpdate)
			{
				return m_fixedUpdateRealTimeDelta;
			}
			return m_realTimeDelta;
		}
	}

	public static bool IsPaused()
	{
		return m_paused;
	}

	public static void Pause(bool pause)
	{
		m_paused = pause;
		if (pause)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}
		EventManager.Send(new GameTimePaused(m_paused));
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		Assert.Check(m_instance == null, "Attempted to create two GameTime instances");
		m_instance = this;
	}

	private void Update()
	{
		m_isFixedUpdate = false;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		m_realTimeDelta = realtimeSinceStartup - m_previousRealTime;
		m_previousRealTime = realtimeSinceStartup;
	}

	private void FixedUpdate()
	{
		m_isFixedUpdate = true;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		m_fixedUpdateRealTimeDelta = realtimeSinceStartup - m_previousFixedUpdateTime;
		m_previousFixedUpdateTime = realtimeSinceStartup;
	}
}
