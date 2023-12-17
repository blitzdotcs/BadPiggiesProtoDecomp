using UnityEngine;

public class VisibilityCondition : WPFMonoBehaviour
{
	public enum Condition
	{
		None = 0,
		HasValidContraption = 1,
		ShowEngineButton = 2,
		HasRockets = 3,
		IsPausedWhileRunning = 4,
		HasContraption = 5,
		QuestModeCanBuild = 6,
		IsPuzzleMode = 7,
		ShowPauseMenuReplayButton = 8,
		HasMotorWheels = 9,
		HasFans = 10,
		HasPropellers = 11,
		HasRotors = 12,
		ShowBuyBluePrintButton = 13,
		ShowAutoBuildButton = 14
	}

	public Condition condition;

	private void OnEnable()
	{
		UpdateState();
	}

	private void Update()
	{
		UpdateState();
	}

	private void UpdateState()
	{
		if ((bool)WPFMonoBehaviour.levelManager && (bool)WPFMonoBehaviour.levelManager.contraptionProto)
		{
			switch (condition)
			{
			case Condition.ShowEngineButton:
				SetEnabled(WPFMonoBehaviour.levelManager.contraptionRunning.HasEngine && WPFMonoBehaviour.levelManager.contraptionRunning.EnginePoweredPartTypeCount() > 1);
				break;
			case Condition.HasValidContraption:
				SetEnabled(WPFMonoBehaviour.levelManager.contraptionProto.ValidateContraption());
				break;
			case Condition.IsPausedWhileRunning:
				SetEnabled(WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.PausedWhileRunning);
				break;
			case Condition.HasContraption:
				SetEnabled(WPFMonoBehaviour.levelManager.contraptionProto.DynamicPartCount() > 0);
				break;
			case Condition.QuestModeCanBuild:
				SetEnabled(false);
				break;
			case Condition.IsPuzzleMode:
				SetEnabled(true);
				break;
			case Condition.ShowPauseMenuReplayButton:
				SetEnabled(WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.PausedWhileRunning);
				break;
			case Condition.ShowAutoBuildButton:
				SetEnabled(WPFMonoBehaviour.levelManager.m_autoBuildUnlocked && BuildCustomizationLoader.Instance.IAPEnabled);
				break;
			case Condition.HasRockets:
			case Condition.HasMotorWheels:
			case Condition.HasFans:
			case Condition.HasPropellers:
			case Condition.HasRotors:
			case Condition.ShowBuyBluePrintButton:
				break;
			}
		}
	}

	private void SetEnabled(bool enabled)
	{
		base.gameObject.GetComponent<Renderer>().enabled = enabled;
		base.gameObject.GetComponent<Collider>().enabled = enabled;
		for (int i = 0; i < base.gameObject.transform.childCount; i++)
		{
			Transform child = base.gameObject.transform.GetChild(i);
			if ((bool)child.GetComponent<Renderer>())
			{
				child.GetComponent<Renderer>().enabled = enabled;
			}
		}
	}
}
