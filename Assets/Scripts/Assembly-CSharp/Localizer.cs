using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Localizer : MonoBehaviour
{
	public class LocaleParameters
	{
		public string translation = string.Empty;

		public float characterSizeFactor = 1f;

		public float lineSpacingFactor = 1f;
	}

	private const string defaultFontName = "default";

	private Dictionary<string, LocaleParameters> activeTranslations = new Dictionary<string, LocaleParameters>();

	private Font languageSpecificFont;

	private static Localizer instance;

	private bool localizationDataInitalized;

	private string currentLocale = string.Empty;

	public static Localizer Instance
	{
		get
		{
			return instance;
		}
	}

	public Font LanguageFont
	{
		get
		{
			return languageSpecificFont;
		}
	}

	public static bool IsInstantiated()
	{
		return instance;
	}

	public static XmlDocument LoadLocalizationFile()
	{
		XmlDocument xmlDocument = new XmlDocument();
		TextAsset textAsset = (TextAsset)Resources.Load("Localization/localization_data", typeof(TextAsset));
		if ((bool)textAsset)
		{
			Debug.Log("Localization asset loaded: " + textAsset.name);
		}
		else
		{
			Assert.ErrorBreak("localization file failed to load");
		}
		xmlDocument.LoadXml(textAsset.text);
		return xmlDocument;
	}

	private void Awake()
	{
		Assert.Check(instance == null, "Singleton " + base.name + " spawned twice");
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
		currentLocale = DetectLocale();
		PopulateTranslations(currentLocale);
		localizationDataInitalized = true;
	}

	private string DetectLocale()
	{
		string text = "en-EN";
		switch (Application.systemLanguage)
		{
		case SystemLanguage.English:
			text = "en-EN";
			break;
		case SystemLanguage.French:
			text = "fr-FR";
			break;
		case SystemLanguage.Italian:
			text = "it-IT";
			break;
		case SystemLanguage.German:
			text = "de-DE";
			break;
		case SystemLanguage.Spanish:
			text = "es-ES";
			break;
		case SystemLanguage.Chinese:
			Debug.LogWarning("Chinese language dialect cannot be detected, using locale: 'zh-TW'");
			text = "zh-TW";
			break;
		case SystemLanguage.Japanese:
			text = "ja-JA";
			break;
		case SystemLanguage.Russian:
			text = "ru-RU";
			break;
		case SystemLanguage.Arabic:
			text = "ar-AR";
			break;
		case SystemLanguage.Portuguese:
			Debug.LogWarning("Portugese language dialect cannot be detected, using locale: 'pt-PT'");
			text = "pt-PT";
			break;
		case SystemLanguage.Polish:
			text = "pl-PL";
			break;
		default:
			text = "en-EN";
			break;
		}
		Debug.Log(string.Concat("Unity system language: ", Application.systemLanguage, ", using game locale: ", text));
		return text;
	}

	public LocaleParameters Resolve(string textId)
	{
		Assert.Check(localizationDataInitalized, "Tried to fetch translation:" + textId + " But localization data is not initialized");
		Assert.Check(activeTranslations.Count > 0, "Could not resolve locale, localization data is empty");
		if (!activeTranslations.ContainsKey(textId) || activeTranslations[textId].translation == string.Empty)
		{
			LocaleParameters localeParameters = new LocaleParameters();
			Debug.LogWarning("LOCALIZATION KEY MISSING OR EMPTY (" + textId + ")");
			localeParameters.translation = textId;
			return localeParameters;
		}
		return activeTranslations[textId];
	}

	private void LoadFont(string fontName)
	{
		if (fontName != "default")
		{
			languageSpecificFont = (Font)Resources.Load("Localization/" + fontName, typeof(Font));
			if ((bool)languageSpecificFont)
			{
				Debug.Log("language specific font loaded: " + fontName);
			}
			else
			{
				Debug.LogWarning("Failed to load language specific font: " + fontName);
			}
		}
	}

	private void PopulateTranslations(string localeId)
	{
		XmlDocument xmlDocument = LoadLocalizationFile();
		XmlNode xmlNode = xmlDocument.SelectSingleNode("/texts/languages/" + localeId);
		if (xmlNode != null)
		{
			LoadFont(xmlNode["font"].InnerText);
		}
		else
		{
			Debug.LogWarning("Could not find localization language settings for locale: " + localeId);
		}
		XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/texts/text");
		foreach (XmlNode item in xmlNodeList)
		{
			string innerText = item["text_id"].InnerText;
			XmlElement xmlElement = item[localeId];
			if (xmlElement == null)
			{
				xmlElement = item["en-EN"];
			}
			if (activeTranslations.ContainsKey(innerText))
			{
				if (Application.isEditor)
				{
					Assert.ErrorBreak("Localization data contains duplicate key: " + innerText);
				}
			}
			else
			{
				activeTranslations.Add(innerText, FormulateTranslation(xmlElement));
			}
		}
	}

	private LocaleParameters FormulateTranslation(XmlElement xn)
	{
		LocaleParameters localeParameters = new LocaleParameters();
		localeParameters.translation = xn.InnerText;
		string attribute = xn.GetAttribute("character_size");
		string attribute2 = xn.GetAttribute("line_spacing");
		if (attribute != string.Empty)
		{
			localeParameters.characterSizeFactor = (float)Convert.ToDouble(attribute);
		}
		if (attribute2 != string.Empty)
		{
			localeParameters.lineSpacingFactor = (float)Convert.ToDouble(attribute2);
		}
		return localeParameters;
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			string text = DetectLocale();
			if (text != currentLocale)
			{
				RefreshLocalization();
			}
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			string text = DetectLocale();
			if (text != currentLocale)
			{
				RefreshLocalization();
			}
		}
	}

	private void RefreshLocalization()
	{
		Debug.Log("Relocalize all texts");
		activeTranslations.Clear();
		languageSpecificFont = null;
		currentLocale = DetectLocale();
		PopulateTranslations(currentLocale);
		EventManager.Send(new LocalizationReloaded(currentLocale));
	}
}
