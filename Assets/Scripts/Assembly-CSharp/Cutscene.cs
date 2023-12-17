using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
	public enum Type
	{
		EpisodeStart = 0,
		EpisodeEnd = 1
	}

	private struct FrameData
	{
		public float moveTime;

		public float showTime;

		public GameObject frame;

		public FrameData(float moveTime, float showTime, GameObject frame)
		{
			this.moveTime = moveTime;
			this.showTime = showTime;
			this.frame = frame;
		}
	}

	[SerializeField]
	private Type m_cutsceneType;

	public GameObject continueButton;

	public float continueButtonDelay;

	public float moveTime = 1f;

	public float showTime;

	private List<FrameData> frames = new List<FrameData>();

	private int currentFrame;

	private float timer;

	private Vector3 startPosition;

	private Quaternion startRotation;

	private bool skip;

	public void Awake()
	{
		continueButton.SetActiveRecursively(false);
		int num = 0;
		while (true)
		{
			GameObject gameObject = GameObject.Find("Frame" + (num + 1));
			if ((bool)gameObject)
			{
				CartoonFrameTimer component = gameObject.GetComponent<CartoonFrameTimer>();
				float num2 = component.moveTime;
				if (num2 == 0f)
				{
					num2 = moveTime;
				}
				float num3 = component.showTime;
				if (num3 == 0f)
				{
					num3 = showTime;
				}
				frames.Add(new FrameData(num2, num3, gameObject));
				num++;
				continue;
			}
			break;
		}
		foreach (FrameData frame2 in frames)
		{
			GameObject frame = frame2.frame;
			CartoonFrameSprite component2 = frame.GetComponent<CartoonFrameSprite>();
			int num4 = component2.m_UVx + component2.m_width / 2 - component2.m_subdivisionsX / 2;
			int num5 = component2.m_UVy + component2.m_height / 2 - component2.m_subdivisionsY / 2;
			frame.transform.position = new Vector3(num4, num5, -10f);
			frame.transform.rotation = Quaternion.AngleAxis(180f, new Vector3(0f, 0f, 1f));
		}
		if (frames.Count > 0)
		{
			timer = 0f;
			startPosition = frames[0].frame.transform.position;
			startRotation = frames[0].frame.transform.rotation;
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			skip = true;
		}
		if (currentFrame < frames.Count)
		{
			timer += Time.deltaTime;
			FrameData frameData = frames[currentFrame];
			GameObject frame = frameData.frame;
			if (frame.transform.position != Vector3.zero)
			{
				float f = timer / frameData.moveTime;
				f = Mathf.Clamp(Mathf.Pow(f, 0.2f), 0f, 1f);
				frame.transform.position = Vector3.Slerp(startPosition, Vector3.zero, f);
				frame.transform.rotation = Quaternion.Slerp(startRotation, Quaternion.identity, f);
			}
			else if (timer >= frameData.moveTime + frameData.showTime)
			{
				currentFrame++;
				timer = 0f;
				if (skip)
				{
					timer = 1000f;
				}
				if (currentFrame < frames.Count)
				{
					startPosition = frames[currentFrame].frame.transform.position;
					startRotation = frames[currentFrame].frame.transform.rotation;
				}
			}
		}
		else if (currentFrame == frames.Count)
		{
			timer += Time.deltaTime;
			if (timer >= continueButtonDelay)
			{
				continueButton.SetActiveRecursively(true);
				currentFrame++;
				timer = 0f;
			}
		}
	}

	public void Continue()
	{
		switch (m_cutsceneType)
		{
		case Type.EpisodeStart:
			if (GameProgress.GetInt(Application.loadedLevelName + "_played") == 1)
			{
				Loader.Instance.LoadLevel(GameManager.Instance.CurrentEpisode, false);
			}
			else
			{
				GameManager.Instance.LoadLevel(0);
			}
			break;
		case Type.EpisodeEnd:
			Loader.Instance.LoadLevel(GameManager.Instance.CurrentEpisode, false);
			break;
		}
		GameProgress.SetInt(Application.loadedLevelName + "_played", 1);
	}
}
