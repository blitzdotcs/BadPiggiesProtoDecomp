using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstlyManager : MonoBehaviour
{
	private struct BannerState
	{
		public bool isReady;

		public bool isShowing;
	}

	public enum AdType
	{
		Banner = 0,
		Expandable = 1,
		Interstitial = 2
	}

	private static BurstlyManager instance;

	private Dictionary<AdType, BannerState> m_bannerStates = new Dictionary<AdType, BannerState>();

	public bool BannerAdReady
	{
		get
		{
			return m_bannerStates[AdType.Banner].isReady;
		}
	}

	public bool BannerAdShown
	{
		get
		{
			return m_bannerStates[AdType.Banner].isShowing;
		}
	}

	public bool ExpandableAdReady
	{
		get
		{
			return m_bannerStates[AdType.Expandable].isReady;
		}
	}

	public bool ExpandableAdShown
	{
		get
		{
			return m_bannerStates[AdType.Expandable].isShowing;
		}
	}

	public bool InterstitialAdReady
	{
		get
		{
			return m_bannerStates[AdType.Interstitial].isReady;
		}
	}

	public bool InterstitialAdShown
	{
		get
		{
			return m_bannerStates[AdType.Interstitial].isShowing;
		}
	}

	public static BurstlyManager Instance
	{
		get
		{
			return instance;
		}
	}

	private static void _RequestBanner(AdType type)
	{
		Debug.Log("BurstlyInterface: _RequestBanner not implemented for this platform");
	}

	private static void _ShowBanner(AdType type)
	{
		Debug.Log("BurstlyInterface: _ShowBanner not implemented for this platform");
	}

	private static void _HideBanner(AdType type)
	{
		Debug.Log("BurstlyInterface: _HideBanner not implemented for this platform");
	}

	private static void _RegisterListener(string listener)
	{
		Debug.Log("BurstlyInterface: _RegisterListener not implemented for this platform");
	}

	public void RequestBanner(AdType type)
	{
		_RequestBanner(type);
	}

	public void ShowBanner(AdType type)
	{
		if (!m_bannerStates[type].isShowing && m_bannerStates[type].isReady)
		{
			_ShowBanner(type);
			BannerState value = m_bannerStates[type];
			value.isShowing = true;
			value.isReady = false;
			m_bannerStates[type] = value;
		}
	}

	public void HideBanner(AdType type)
	{
		if (m_bannerStates[type].isShowing)
		{
			_HideBanner(type);
			BannerState value = m_bannerStates[type];
			value.isShowing = false;
			m_bannerStates[type] = value;
		}
	}

	public static bool IsInstantiated()
	{
		return instance;
	}

	private void Awake()
	{
		Assert.Check(instance == null, "Singleton " + base.name + " spawned twice");
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
		initBannerStateDictionary();
		if (!BuildCustomizationLoader.Instance.IsFreeVersion)
		{
			DisableAds();
			return;
		}
		string listener = base.name.Replace("(Clone)", string.Empty);
		_RegisterListener(listener);
		StartCoroutine(FetchBannerAds());
		StartCoroutine(FetchInterstitialAds());
	}

	private IEnumerator FetchBannerAds()
	{
		while (true)
		{
			Debug.Log("BurstlyManager::FetchBannerAds()");
			RequestBanner(AdType.Banner);
			yield return new WaitForSeconds(20f);
		}
	}

	private IEnumerator FetchExpandableAds()
	{
		while (true)
		{
			Debug.Log("BurstlyManager::FetchExpandableAds()");
			RequestBanner(AdType.Expandable);
			yield return new WaitForSeconds(20f);
		}
	}

	private IEnumerator FetchInterstitialAds()
	{
		while (true)
		{
			Debug.Log("BurstlyManager::FetchInterstitialAds()");
			RequestBanner(AdType.Interstitial);
			yield return new WaitForSeconds(60f);
		}
	}

	private void initBannerStateDictionary()
	{
		foreach (int value2 in Enum.GetValues(typeof(AdType)))
		{
			BannerState value = default(BannerState);
			value.isReady = false;
			value.isShowing = false;
			m_bannerStates.Add((AdType)value2, value);
		}
	}

	public void DisableAds()
	{
		StopAllCoroutines();
		HideBanner(AdType.Banner);
		BuildCustomizationLoader.Instance.AdsEnabled = false;
	}

	protected void onBannerAdRequestCompleted(string success)
	{
		BannerState value = m_bannerStates[AdType.Banner];
		value.isReady = ((success == "true") ? true : false);
		m_bannerStates[AdType.Banner] = value;
		Debug.Log("BurstlyManager::onBannerAdRequestCompleted(" + success + ")");
	}

	protected void onBannerAdWasHidden(string msg)
	{
		Debug.Log("BurstlyManager::onBannerAdWasHidden()");
	}

	protected void onExpandableAdReady(string msg)
	{
		BannerState value = m_bannerStates[AdType.Expandable];
		value.isReady = ((msg == "true") ? true : false);
		m_bannerStates[AdType.Expandable] = value;
		Debug.Log("BurstlyManager::onExpandableAdReady()");
	}

	protected void onExpandableAdRequestFailed(string msg)
	{
		BannerState value = m_bannerStates[AdType.Expandable];
		value.isReady = false;
		m_bannerStates[AdType.Expandable] = value;
		Debug.Log("BurstlyManager::onExpandableAdRequestFailed()");
	}

	protected void onExpandableAdWillExpand(string msg)
	{
		Debug.Log("BurstlyManager::onExpandableAdWillExpand()");
	}

	protected void onExpandableAdWasHidden(string msg)
	{
		Debug.Log("BurstlyManager::onExpandableAdWasHidden()");
	}

	protected void onInterstitialAdRequestCompleted(string msg)
	{
		BannerState value = m_bannerStates[AdType.Interstitial];
		value.isReady = ((msg == "true") ? true : false);
		m_bannerStates[AdType.Interstitial] = value;
		Debug.Log("BurstlyManager::onInterstitialAdRequestCompleted(" + msg + ")");
	}
}
