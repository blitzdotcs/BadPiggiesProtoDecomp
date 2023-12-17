using System.Collections.Generic;
using UnityEngine;

public class InGameFlightMenu : WPFMonoBehaviour
{
	public class PartButtonOrder : IComparer<GameObject>
	{
		private float middle;

		public PartButtonOrder(float middle)
		{
			this.middle = middle;
		}

		public int Compare(GameObject obj1, GameObject obj2)
		{
			GadgetButton component = obj1.GetComponent<GadgetButton>();
			GadgetButton component2 = obj2.GetComponent<GadgetButton>();
			float placementOrder = middle;
			float placementOrder2 = middle;
			if ((bool)component)
			{
				placementOrder = component.PlacementOrder;
			}
			if ((bool)component2)
			{
				placementOrder2 = component2.PlacementOrder;
			}
			if (placementOrder < placementOrder2)
			{
				return -1;
			}
			if (placementOrder > placementOrder2)
			{
				return 1;
			}
			return 0;
		}
	}

	private void OnEnable()
	{
		if ((bool)WPFMonoBehaviour.levelManager && (bool)WPFMonoBehaviour.levelManager.contraptionRunning)
		{
			SetGadgetButtonOrder(WPFMonoBehaviour.levelManager.contraptionRunning.PartPlacements);
		}
		if (!BuildCustomizationLoader.Instance.IsDebugBuild || WPFMonoBehaviour.levelManager.m_sandbox)
		{
			base.transform.Find("CheatButton3Stars").gameObject.active = false;
			base.transform.Find("CheatButton1Star").gameObject.active = false;
		}
	}

	private void SetGadgetButtonOrder(List<Contraption.PartPlacementInfo> parts)
	{
		ButtonList component = base.transform.Find("PartButtons").GetComponent<ButtonList>();
		int num = 0;
		foreach (GameObject button in component.Buttons)
		{
			GadgetButton component2 = button.GetComponent<GadgetButton>();
			if (!component2)
			{
				continue;
			}
			for (int i = 0; i < parts.Count; i++)
			{
				if (parts[i].partType == component2.m_partType && parts[i].direction == component2.m_direction)
				{
					component2.PlacementOrder = i;
					if (parts[i].count > 0)
					{
						num++;
					}
					break;
				}
			}
		}
		component.Sort(new PartButtonOrder((float)num / 2f + ((num % 2 != 0) ? 0.5f : 0f)));
	}

	public void CompleteLevelWithThreeStars()
	{
		if (BuildCustomizationLoader.Instance.IsDebugBuild)
		{
			Collectable[] array = Object.FindObjectsOfType(typeof(Collectable)) as Collectable[];
			Collectable[] array2 = array;
			foreach (Collectable collectable in array2)
			{
				collectable.OnGoalEnter();
			}
			WPFMonoBehaviour.levelManager.contraptionRunning.transform.position = GameObject.FindWithTag("Goal").transform.position;
		}
	}

	public void CompleteLevelWithOneStar()
	{
		if (BuildCustomizationLoader.Instance.IsDebugBuild)
		{
			WPFMonoBehaviour.levelManager.contraptionRunning.SetBroken();
			WPFMonoBehaviour.levelManager.TimeElapsed = 9999f;
			WPFMonoBehaviour.levelManager.contraptionRunning.transform.position = GameObject.FindWithTag("Goal").transform.position;
		}
	}
}
