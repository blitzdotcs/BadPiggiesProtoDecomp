using UnityEngine;

public class SplashPlayer : MonoBehaviour
{
	private void Start()
	{
		SplashScreenSequence original = ((!BuildCustomizationLoader.Instance.IsHDVersion) ? ((SplashScreenSequence)Resources.Load("Splashes/Sequences/SplashSequence_iPhone", typeof(SplashScreenSequence))) : ((SplashScreenSequence)Resources.Load("Splashes/Sequences/SplashSequence_iPad", typeof(SplashScreenSequence))));
		Object.Instantiate(original);
	}
}
