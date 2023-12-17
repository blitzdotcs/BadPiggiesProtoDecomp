using System.Collections;
using UnityEngine;

public class InfoPanelTutorial : WPFMonoBehaviour
{
	private enum State
	{
		Initialized = 0,
		Starting = 1,
		BringingIn = 2,
		Playing = 3,
		Stopped = 4
	}

	private State m_state;

	private GameObject m_background;

	private float m_bringInTimer;

	private void Start()
	{
		EventManager.Connect<GameStateChanged>(ReceiveGameStateChanged);
		m_background = base.transform.Find("Background").gameObject;
		m_background.SetActiveRecursively(false);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(ReceiveGameStateChanged);
	}

	private void Update()
	{
		if (m_state == State.Starting)
		{
			m_state = State.BringingIn;
			m_bringInTimer = 0f;
			m_background.SetActiveRecursively(true);
			m_background.transform.localScale = Vector3.zero;
		}
		else if (m_state == State.BringingIn)
		{
			m_bringInTimer += GameTime.RealTimeDelta;
			if (m_bringInTimer < 0.75f)
			{
				float num = MathsUtil.EaseInOutQuad(m_bringInTimer, 0f, 1f, 0.75f);
				m_background.transform.localScale = num * Vector3.one;
			}
			else
			{
				m_background.transform.localScale = Vector3.one;
				m_state = State.Playing;
			}
		}
	}

	private void SetupTutorial()
	{
		m_state = State.Starting;
	}

	private void ReceiveGameStateChanged(GameStateChanged data)
	{
		if (data.state == LevelManager.GameState.Building)
		{
			if (m_state != State.Playing && m_state != State.BringingIn)
			{
				StartCoroutine(StartTutorial());
			}
		}
		else if (data.state == LevelManager.GameState.PreviewWhileBuilding)
		{
			if (m_state != State.Stopped)
			{
				m_state = State.Initialized;
			}
			m_background.SetActiveRecursively(false);
		}
		else if (data.state == LevelManager.GameState.Running)
		{
			m_state = State.Stopped;
			m_background.SetActiveRecursively(false);
		}
	}

	private IEnumerator StartTutorial()
	{
		Vector3 cameraPosition;
		do
		{
			cameraPosition = WPFMonoBehaviour.ingameCamera.transform.position;
			yield return new WaitForSeconds(0.2f);
		}
		while (!(Vector3.Distance(WPFMonoBehaviour.ingameCamera.transform.position, cameraPosition) < 0.05f));
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Building)
		{
			SetupTutorial();
		}
	}
}
