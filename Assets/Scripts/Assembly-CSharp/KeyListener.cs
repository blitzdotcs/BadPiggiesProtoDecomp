using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyListener : MonoBehaviour
{
	public List<KeyCode> m_hotkeys;

	private static KeyListener instance;

	public static KeyListener Instance
	{
		get
		{
			return instance;
		}
	}

	public static event Action<KeyCode> keyPressed;

	public static event Action<KeyCode> keyReleased;

	public static event Action<KeyCode> keyHold;

	private void Update()
	{
		if (KeyListener.keyPressed == null && KeyListener.keyReleased == null && KeyListener.keyHold == null)
		{
			return;
		}
		foreach (KeyCode hotkey in m_hotkeys)
		{
			if (Input.GetKeyUp(hotkey) && KeyListener.keyReleased != null)
			{
				KeyListener.keyReleased(hotkey);
			}
			if (Input.GetKeyDown(hotkey) && KeyListener.keyPressed != null)
			{
				KeyListener.keyPressed(hotkey);
			}
			if (Input.GetKey(hotkey) && KeyListener.keyHold != null)
			{
				KeyListener.keyHold(hotkey);
			}
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
	}
}
