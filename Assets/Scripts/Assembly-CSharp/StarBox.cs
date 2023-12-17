using UnityEngine;

public class StarBox : OneTimeCollectable
{
	private void Awake()
	{
		if (!base.transform.parent || base.transform.parent.name != "StarBoxes")
		{
			Assert.Check(false, "StarBox objects must be placed under parent object called StarBoxes");
			DisableGoal();
		}
		if (GameProgress.HasSandboxStar(Application.loadedLevelName, base.name))
		{
			DisableGoal();
		}
	}

	public override void OnCollected()
	{
		Object.Instantiate(collectedEffect, base.transform.position, Quaternion.identity);
		GameProgress.AddSandboxStar(Application.loadedLevelName, base.name);
	}
}
