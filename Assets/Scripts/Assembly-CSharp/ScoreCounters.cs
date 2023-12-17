using System;
using UnityEngine;

public class ScoreCounters : MonoBehaviour
{
	private const bool showScoreFloaters = false;

	private GameObject scoreCounter;

	private GameObject timeCounter;

	private TextMesh timeTextShadow;

	private TextMesh timeText;

	private TextMesh scoreText;

	private TextMesh scoreShadowText;

	private Pig pig;

	private float score;

	private bool firstRun = true;

	private bool running;

	private int pigCount;

	private LevelManager levelManager;

	private void Awake()
	{
		EventManager.Connect<GameStateChanged>(ReceiveGameStateChangeEvent);
		EventManager.Connect<ScoreChanged>(ReceiveScoreChanged);
		EventManager.Connect<UIEvent>(ReceiveUIEvent);
		scoreCounter = GameObject.Find("ScoreCounters/ScoreCounter");
		timeCounter = GameObject.Find("ScoreCounters/TimeCounter");
		timeText = GameObject.Find("ScoreCounters/TimeCounter/TimeCounter").GetComponent<TextMesh>();
		Assert.IsValid(timeText, "distanceText");
		timeTextShadow = GameObject.Find("ScoreCounters/TimeCounter/TimeCounter/TimeCounterShadow").GetComponent<TextMesh>();
		Assert.IsValid(timeTextShadow, "distanceTextShadow");
		scoreText = GameObject.Find("ScoreCounters/ScoreCounter/ScoreCounter").GetComponent<TextMesh>();
		Assert.IsValid(scoreText, "scoreText");
		scoreShadowText = GameObject.Find("ScoreCounters/ScoreCounter/ScoreCounter/ScoreCounterShadow").GetComponent<TextMesh>();
		Assert.IsValid(scoreShadowText, "scoreShadowText");
		GameObject gameObject = GameObject.Find("LevelManager");
		if ((bool)gameObject)
		{
			levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
			Assert.IsValid(levelManager, "levelManager");
		}
		UpdateScore(0);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(ReceiveGameStateChangeEvent);
		EventManager.Disconnect<ScoreChanged>(ReceiveScoreChanged);
		EventManager.Disconnect<UIEvent>(ReceiveUIEvent);
	}

	private void ReceiveUIEvent(UIEvent data)
	{
		UIEvent.Type type = data.type;
		if (type == UIEvent.Type.Building)
		{
			score = 0f;
			UpdateScore(0);
		}
	}

	private void ReceiveScoreChanged(ScoreChanged newScore)
	{
		score += 100f;
		UpdateScore((int)score);
	}

	private void ReceiveGameStateChangeEvent(GameStateChanged newState)
	{
		if (newState.state == LevelManager.GameState.Running)
		{
			running = true;
			pig = UnityEngine.Object.FindObjectOfType(typeof(Pig)) as Pig;
			pigCount = UnityEngine.Object.FindObjectsOfType(typeof(Pig)).Length;
			if (firstRun)
			{
				firstRun = false;
			}
		}
		else
		{
			running = false;
		}
	}

	private void OnEnable()
	{
		scoreCounter.SetActiveRecursively(false);
		if ((bool)levelManager && levelManager.TimeLimit == 0f)
		{
			timeCounter.SetActiveRecursively(false);
		}
	}

	private void UpdateScore(int newScore)
	{
		scoreText.text = newScore.ToString();
		scoreShadowText.text = newScore.ToString();
	}

	private void UpdateTime()
	{
		float num = Mathf.Max(levelManager.TimeLimit - levelManager.TimeElapsed, 0f);
		TimeSpan timeSpan = TimeSpan.FromSeconds(num);
		string text = string.Format("{0:D2}:{1:D2}", timeSpan.Seconds, timeSpan.Milliseconds / 10);
		timeText.text = text;
		timeTextShadow.text = text;
	}

	private void Update()
	{
		if (running)
		{
			UpdateTime();
			score += pig.GetComponent<Rigidbody>().velocity.magnitude * ((float)pigCount / 2f);
			UpdateScore((int)score);
		}
	}
}
