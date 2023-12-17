using UnityEngine;

public class TextMeshLocale : MonoBehaviour
{
	private TextMesh targetTextMesh;

	private string originalTextContents = string.Empty;

	private float originalCharacterSize;

	private float originalLineSpacing;

	private string postfix = string.Empty;

	private Localizer.LocaleParameters localeParameters;

	public string Postfix
	{
		set
		{
			postfix = value;
			targetTextMesh.text = localeParameters.translation + postfix;
		}
	}

	private void Start()
	{
		ApplyLocale();
	}

	private void ApplyLocale()
	{
		localeParameters = Localizer.Instance.Resolve(originalTextContents);
		if ((bool)Localizer.Instance.LanguageFont)
		{
			Color color = targetTextMesh.GetComponent<Renderer>().material.color;
			targetTextMesh.font = Localizer.Instance.LanguageFont;
			targetTextMesh.GetComponent<Renderer>().material = Localizer.Instance.LanguageFont.material;
			targetTextMesh.GetComponent<Renderer>().material.color = color;
		}
		targetTextMesh.text = localeParameters.translation + postfix;
		targetTextMesh.characterSize = originalCharacterSize * localeParameters.characterSizeFactor;
		targetTextMesh.lineSpacing = originalLineSpacing * localeParameters.lineSpacingFactor;
	}

	public void RefreshTranslation()
	{
		originalTextContents = targetTextMesh.text;
		ApplyLocale();
	}

	private void Awake()
	{
		targetTextMesh = GetComponent<TextMesh>();
		Assert.IsValid(targetTextMesh, "targetTextMesh");
		originalCharacterSize = targetTextMesh.characterSize;
		originalLineSpacing = targetTextMesh.lineSpacing;
		originalTextContents = targetTextMesh.text;
		EventManager.Connect<LocalizationReloaded>(ReceiveLocalizationReloaded);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<LocalizationReloaded>(ReceiveLocalizationReloaded);
	}

	private void ReceiveLocalizationReloaded(LocalizationReloaded localizationReloaded)
	{
		ApplyLocale();
	}
}
