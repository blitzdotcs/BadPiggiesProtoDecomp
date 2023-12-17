using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTutorial : WPFMonoBehaviour
{
	private enum State
	{
		Initialized = 0,
		Waiting = 1,
		BringingIn = 2,
		Playing = 3,
		Stopped = 4
	}

	public GameObject m_pointerPrefab;

	public GameObject m_clickIndicatorPrefab;

	private State m_state;

	private GameObject m_pointerVisual;

	private GameObject m_clickIndicator;

	private Tutorial.Pointer m_pointer;

	private Tutorial.PointerTimeLine m_timeline;

	private GameObject m_background;

	private GameObject m_part;

	private float m_bringInTimer;

	private void Start()
	{
		EventManager.Connect<GameStateChanged>(ReceiveGameStateChanged);
		m_pointerVisual = (GameObject)Object.Instantiate(m_pointerPrefab);
		if ((bool)m_clickIndicatorPrefab)
		{
			m_clickIndicator = (GameObject)Object.Instantiate(m_clickIndicatorPrefab);
			m_clickIndicator.SetActiveRecursively(false);
		}
		m_pointer = new Tutorial.Pointer(m_pointerVisual, m_clickIndicator);
		m_pointer.SetPressHandler(OnPress);
		m_background = base.transform.Find("Background").gameObject;
		m_background.SetActiveRecursively(false);
		m_part = m_background.transform.Find("Bottle").gameObject;
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(ReceiveGameStateChanged);
	}

	private void OnPress()
	{
		m_part.transform.Rotate(Vector3.forward, -90f);
	}

	private void Update()
	{
		if (m_state == State.Waiting)
		{
			if (WPFMonoBehaviour.levelManager.contraptionProto.HasPart(BasePart.PartType.CokeBottle))
			{
				m_state = State.BringingIn;
				m_bringInTimer = 0f;
				m_background.SetActiveRecursively(true);
				m_background.transform.localScale = Vector3.zero;
			}
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
				m_timeline.Start();
				m_state = State.Playing;
			}
		}
		else if (m_state == State.Playing)
		{
			m_timeline.Update();
			if (m_timeline.IsFinished())
			{
				m_timeline.Start();
			}
			if (WPFMonoBehaviour.levelManager.constructionUI.RotationCount > 0)
			{
				m_state = State.Stopped;
				m_background.SetActiveRecursively(false);
				m_pointer.Show(false);
			}
		}
	}

	private void SetupTutorial()
	{
		m_state = State.Waiting;
		m_timeline = new Tutorial.PointerTimeLine(m_pointer);
		List<Vector3> list = new List<Vector3>();
		list.Add(m_background.transform.position + new Vector3(0.8f, -0.8f, 0f));
		m_timeline.AddEvent(new Tutorial.PointerTimeLine.Move(list, 0.5f));
		m_timeline.AddEvent(new Tutorial.PointerTimeLine.Press());
		m_timeline.AddEvent(new Tutorial.PointerTimeLine.Wait(0.5f));
		m_timeline.AddEvent(new Tutorial.PointerTimeLine.Release());
		m_timeline.AddEvent(new Tutorial.PointerTimeLine.Wait(0.5f));
	}

	private void ReceiveGameStateChanged(GameStateChanged data)
	{
		if (data.state == LevelManager.GameState.Building)
		{
			if (m_state != State.Stopped)
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
			m_pointer.Show(false);
		}
		else if (data.state == LevelManager.GameState.Running)
		{
			m_state = State.Stopped;
			m_background.SetActiveRecursively(false);
			m_pointer.Show(false);
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
