using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : WPFMonoBehaviour
{
	public class Pointer
	{
		public delegate void OnPress();

		private GameObject m_pointer;

		private GameObject m_clickIndicator;

		private float m_pressTimer;

		private bool m_clickIndicatorOn;

		private OnPress onPress;

		public Pointer(GameObject pointer, GameObject clickIndicator)
		{
			m_pointer = pointer;
			m_pointer.transform.localScale = 1.05f * Vector3.zero;
			m_clickIndicator = clickIndicator;
		}

		public void SetPressHandler(OnPress onPress)
		{
			this.onPress = onPress;
		}

		public void SetPosition(Vector3 position)
		{
			m_pointer.transform.position = position;
		}

		public void Show(bool show)
		{
			m_pointer.SetActiveRecursively(show);
			if (!show && (bool)m_clickIndicator)
			{
				m_clickIndicator.SetActiveRecursively(false);
			}
		}

		public void Press()
		{
			m_pressTimer = 0f;
			m_clickIndicatorOn = true;
			if ((bool)m_clickIndicator)
			{
				m_clickIndicator.SetActiveRecursively(true);
				m_clickIndicator.transform.position = m_pointer.transform.position;
			}
			m_pointer.transform.localScale = 0.85f * Vector3.one;
			if (onPress != null)
			{
				onPress();
			}
		}

		public void Release()
		{
			m_pointer.transform.localScale = 1.05f * Vector3.one;
		}

		public void Update()
		{
			m_pressTimer += Time.deltaTime;
			if (m_clickIndicatorOn && m_pressTimer >= 0.5f)
			{
				m_clickIndicatorOn = false;
				if ((bool)m_clickIndicator)
				{
					m_clickIndicator.SetActiveRecursively(false);
				}
			}
		}
	}

	public class PointerTimeLine
	{
		public class PointerEvent
		{
			protected Pointer m_pointer;

			protected bool m_finished;

			public void SetPointer(Pointer pointer)
			{
				m_pointer = pointer;
			}

			public virtual void Start()
			{
			}

			public virtual void Update()
			{
			}

			public virtual bool Finished()
			{
				return m_finished;
			}
		}

		public class Move : PointerEvent
		{
			private float m_time;

			private float m_timer;

			private List<Vector3> m_positions;

			public Move(List<Vector3> positions, float time)
			{
				m_time = time;
				m_positions = positions;
			}

			public override void Start()
			{
				m_timer = 0f;
				m_finished = false;
				if (m_positions.Count > 0)
				{
					m_pointer.SetPosition(m_positions[0]);
				}
				m_pointer.Show(true);
			}

			public override void Update()
			{
				m_timer += Time.deltaTime;
				if (m_timer >= m_time)
				{
					m_finished = true;
				}
				float num = MathsUtil.EaseInOutQuad(m_timer, 0f, 1f, m_time);
				num = 0.5f * num + 0.5f * (m_timer / m_time);
				m_pointer.SetPosition(PositionOnSpline(m_positions, num));
			}
		}

		public class Wait : PointerEvent
		{
			private float m_time;

			private float m_timer;

			public Wait(float time)
			{
				m_time = time;
			}

			public override void Start()
			{
				m_finished = false;
				m_timer = 0f;
			}

			public override void Update()
			{
				m_timer += Time.deltaTime;
				if (m_timer > m_time)
				{
					m_finished = true;
				}
			}
		}

		public class Press : PointerEvent
		{
			public override void Start()
			{
				m_pointer.Press();
				m_finished = true;
			}
		}

		public class Release : PointerEvent
		{
			public override void Start()
			{
				m_pointer.Release();
				m_finished = true;
			}
		}

		public class Hide : PointerEvent
		{
			public override void Start()
			{
				m_pointer.Show(false);
				m_finished = true;
			}
		}

		private Pointer m_pointer;

		private List<PointerEvent> m_events = new List<PointerEvent>();

		private Vector3 m_position;

		private int m_eventIndex;

		private bool m_finished;

		public PointerTimeLine(Pointer pointer)
		{
			m_pointer = pointer;
		}

		public bool IsFinished()
		{
			return m_finished;
		}

		public void Start()
		{
			m_pointer.Release();
			m_eventIndex = 0;
			if (m_eventIndex < m_events.Count)
			{
				m_events[m_eventIndex].Start();
			}
			m_finished = false;
		}

		public void Update()
		{
			if (m_eventIndex < m_events.Count)
			{
				PointerEvent pointerEvent = m_events[m_eventIndex];
				pointerEvent.Update();
				if (pointerEvent.Finished())
				{
					m_eventIndex++;
					if (m_eventIndex < m_events.Count)
					{
						m_events[m_eventIndex].Start();
					}
					else
					{
						m_finished = true;
					}
				}
			}
			m_pointer.Update();
		}

		public void AddEvent(PointerEvent e)
		{
			e.SetPointer(m_pointer);
			m_events.Add(e);
		}
	}

	public GameObject m_pointerPrefab;

	public GameObject m_clickIndicatorPrefab;

	public int m_dragTargetX;

	public int m_dragTargetY = 1;

	private bool m_playing;

	private bool m_finished;

	private PartSelector m_partselector;

	private ConstructionUI m_constructionUI;

	private GameObject m_pointerVisual;

	private GameObject m_clickIndicator;

	private Pointer m_pointer;

	private PointerTimeLine m_dragPigTimeline;

	private PointerTimeLine m_startContraptionTimeline;

	private PointerTimeLine m_startEnginesTimeLine;

	private PointerTimeLine m_timeline;

	private int m_state;

	private void Start()
	{
		EventManager.Connect<GameStateChanged>(ReceiveGameStateChanged);
		m_pointerVisual = (GameObject)Object.Instantiate(m_pointerPrefab);
		m_clickIndicator = (GameObject)Object.Instantiate(m_clickIndicatorPrefab);
		m_clickIndicator.SetActiveRecursively(false);
		SetRenderQueue(m_pointerVisual, 3002);
		SetRenderQueue(m_clickIndicator, 3002);
		m_pointer = new Pointer(m_pointerVisual, m_clickIndicator);
	}

	private void SetRenderQueue(GameObject parent, int queue)
	{
		if ((bool)parent.GetComponent<Renderer>() && (bool)parent.GetComponent<Renderer>().sharedMaterial)
		{
			parent.GetComponent<Renderer>().sharedMaterial.renderQueue = queue;
		}
		for (int i = 0; i < parent.transform.childCount; i++)
		{
			SetRenderQueue(parent.transform.GetChild(i).gameObject, queue);
		}
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(ReceiveGameStateChanged);
	}

	public static Vector3 PositionOnSpline(List<Vector3> controlPoints, float t)
	{
		int count = controlPoints.Count;
		int num = Mathf.FloorToInt(t * (float)(count - 1));
		Vector3 a = controlPoints[Mathf.Clamp(num - 1, 0, count - 1)];
		Vector3 b = controlPoints[Mathf.Clamp(num, 0, count - 1)];
		Vector3 c = controlPoints[Mathf.Clamp(num + 1, 0, count - 1)];
		Vector3 d = controlPoints[Mathf.Clamp(num + 2, 0, count - 1)];
		float i = t * (float)(count - 1) - (float)num;
		return MathsUtil.CatmullRomInterpolate(a, b, c, d, i);
	}

	private void Update()
	{
		if (m_state == 1 && WPFMonoBehaviour.levelManager.contraptionProto.FindPig() != null)
		{
			m_state = 2;
			m_playing = true;
			m_timeline = m_startContraptionTimeline;
			m_timeline.Start();
		}
		if (!m_playing && m_state == 1 && (bool)m_constructionUI && !m_constructionUI.IsDragging())
		{
			m_playing = true;
			m_timeline = m_dragPigTimeline;
			m_timeline.Start();
		}
		if (m_playing)
		{
			m_timeline.Update();
			if (m_timeline.IsFinished())
			{
				m_timeline.Start();
			}
			if (m_state == 1 && m_constructionUI.IsDragging())
			{
				m_playing = false;
				m_pointer.Show(false);
			}
			if (m_state == 2 && WPFMonoBehaviour.levelManager.contraptionProto.FindPig() == null)
			{
				m_state = 1;
				m_playing = false;
				m_pointer.Show(false);
			}
			if (m_state == 3 && (bool)WPFMonoBehaviour.levelManager.contraptionRunning && WPFMonoBehaviour.levelManager.contraptionRunning.SomePoweredPartsEnabled())
			{
				m_playing = false;
				m_pointer.Show(false);
			}
		}
	}

	private void SetupTutorial()
	{
		m_state = 1;
		m_playing = true;
		GameObject gameObject = GameObject.Find("InGameGUI");
		m_partselector = gameObject.transform.Find("InGameBuildMenu").Find("PartSelector").GetComponent<PartSelector>();
		m_constructionUI = GameObject.Find("ConstructionUI").GetComponent<ConstructionUI>();
		GameObject gameObject2 = gameObject.transform.Find("InGameBuildMenu").Find("PlayButton").gameObject;
		ConstructionUI.PartDesc partDesc = m_constructionUI.FindPartDesc(BasePart.PartType.Pig);
		GameObject gameObject3 = m_partselector.FindPartButton(partDesc);
		Vector3 position = gameObject3.transform.position;
		Vector3 vector = m_constructionUI.GridPositionToGuiPosition(m_dragTargetX, m_dragTargetY);
		m_dragPigTimeline = new PointerTimeLine(m_pointer);
		List<Vector3> list = new List<Vector3>();
		list.Add(position + 3f * Vector3.down + 1f * Vector3.left);
		list.Add(position);
		m_dragPigTimeline.AddEvent(new PointerTimeLine.Move(list, 1.5f));
		List<Vector3> list2 = new List<Vector3>();
		list2.Add(position);
		list2.Add(0.5f * (position + vector) + 0.5f * Vector3.left);
		list2.Add(vector);
		m_dragPigTimeline.AddEvent(new PointerTimeLine.Press());
		m_dragPigTimeline.AddEvent(new PointerTimeLine.Wait(0.5f));
		m_dragPigTimeline.AddEvent(new PointerTimeLine.Move(list2, 1.75f));
		m_dragPigTimeline.AddEvent(new PointerTimeLine.Wait(0.2f));
		m_dragPigTimeline.AddEvent(new PointerTimeLine.Release());
		m_dragPigTimeline.AddEvent(new PointerTimeLine.Wait(0.5f));
		m_dragPigTimeline.AddEvent(new PointerTimeLine.Hide());
		m_dragPigTimeline.AddEvent(new PointerTimeLine.Wait(2f));
		m_startContraptionTimeline = new PointerTimeLine(m_pointer);
		List<Vector3> list3 = new List<Vector3>();
		list3.Add(gameObject2.transform.position + 11f * Vector3.down);
		list3.Add(gameObject2.transform.position + 5.5f * Vector3.down + 0.5f * Vector3.left);
		list3.Add(gameObject2.transform.position);
		m_startContraptionTimeline.AddEvent(new PointerTimeLine.Wait(1f));
		m_startContraptionTimeline.AddEvent(new PointerTimeLine.Move(list3, 2f));
		m_startContraptionTimeline.AddEvent(new PointerTimeLine.Press());
		m_startContraptionTimeline.AddEvent(new PointerTimeLine.Wait(0.5f));
		m_startContraptionTimeline.AddEvent(new PointerTimeLine.Release());
		m_startContraptionTimeline.AddEvent(new PointerTimeLine.Wait(0.75f));
		m_startContraptionTimeline.AddEvent(new PointerTimeLine.Hide());
		if (WPFMonoBehaviour.levelManager.contraptionProto.FindPig() == null)
		{
			m_timeline = m_dragPigTimeline;
			m_timeline.Start();
		}
		else
		{
			m_timeline = m_startContraptionTimeline;
			m_timeline.Start();
			m_state = 2;
		}
	}

	private void ReceiveGameStateChanged(GameStateChanged data)
	{
		if (data.state == LevelManager.GameState.Building)
		{
			if (!m_finished)
			{
				StartCoroutine(StartTutorial());
			}
		}
		else if (data.state == LevelManager.GameState.Running)
		{
			m_playing = false;
			m_pointer.Show(false);
			StartCoroutine(StartEngineTutorial());
		}
		else
		{
			m_playing = false;
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

	private IEnumerator StartEngineTutorial()
	{
		yield return new WaitForSeconds(0.5f);
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Running)
		{
			m_startEnginesTimeLine = new PointerTimeLine(m_pointer);
			GameObject enginesButton = GameObject.Find("300_EnginesButton");
			if ((bool)enginesButton)
			{
				List<Vector3> points4 = new List<Vector3>
				{
					enginesButton.transform.position + 11f * Vector3.down,
					enginesButton.transform.position + 5.5f * Vector3.down + 0.5f * Vector3.left,
					enginesButton.transform.position
				};
				m_startEnginesTimeLine.AddEvent(new PointerTimeLine.Wait(1f));
				m_startEnginesTimeLine.AddEvent(new PointerTimeLine.Move(points4, 2f));
				m_startEnginesTimeLine.AddEvent(new PointerTimeLine.Press());
				m_startEnginesTimeLine.AddEvent(new PointerTimeLine.Wait(0.5f));
				m_startEnginesTimeLine.AddEvent(new PointerTimeLine.Release());
				m_startEnginesTimeLine.AddEvent(new PointerTimeLine.Wait(0.75f));
				m_startEnginesTimeLine.AddEvent(new PointerTimeLine.Hide());
				m_timeline = m_startEnginesTimeLine;
				m_timeline.Start();
				m_state = 3;
				m_playing = true;
			}
		}
	}
}
