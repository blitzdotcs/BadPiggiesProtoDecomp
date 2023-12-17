using UnityEngine;

public class WPFMonoBehaviour : MonoBehaviour
{
	protected static IngameCamera s_ingameCamera;

	protected static HUDCamera s_hudCamera;

	protected static LevelManager s_levelManager;

	protected static GameData s_gameData;

	protected static EffectManager s_effectManager;

	public static IngameCamera ingameCamera
	{
		get
		{
			if ((bool)s_ingameCamera)
			{
				return s_ingameCamera;
			}
			IngameCamera[] array = Object.FindSceneObjectsOfType(typeof(IngameCamera)) as IngameCamera[];
			if (array.Length > 0)
			{
				s_ingameCamera = array[0];
			}
			return s_ingameCamera;
		}
	}

	public static HUDCamera hudCamera
	{
		get
		{
			if ((bool)s_hudCamera)
			{
				return s_hudCamera;
			}
			HUDCamera[] array = Object.FindSceneObjectsOfType(typeof(HUDCamera)) as HUDCamera[];
			if (array.Length > 0)
			{
				s_hudCamera = array[0];
			}
			return s_hudCamera;
		}
	}

	public static LevelManager levelManager
	{
		get
		{
			if ((bool)s_levelManager)
			{
				return s_levelManager;
			}
			LevelManager[] array = Object.FindSceneObjectsOfType(typeof(LevelManager)) as LevelManager[];
			if (array.Length > 0)
			{
				s_levelManager = array[0];
			}
			return s_levelManager;
		}
	}

	public static EffectManager effectManager
	{
		get
		{
			if ((bool)s_effectManager)
			{
				return s_effectManager;
			}
			EffectManager[] array = Object.FindSceneObjectsOfType(typeof(EffectManager)) as EffectManager[];
			if (array.Length > 0)
			{
				s_effectManager = array[0];
			}
			return s_effectManager;
		}
	}

	public static GameData gameData
	{
		get
		{
			if ((bool)s_gameData)
			{
				return s_gameData;
			}
			s_gameData = GameManager.Instance.gameData;
			return s_gameData;
		}
	}

	public static T FindObjectComponent<T>(string name) where T : Component
	{
		GameObject gameObject = GameObject.Find(name);
		if ((bool)gameObject)
		{
			return gameObject.GetComponent<T>();
		}
		return (T)null;
	}

	public static Vector3 ScreenToZ0(Vector3 pos)
	{
		if ((bool)ingameCamera && ingameCamera.GetComponent<Camera>().orthographic)
		{
			Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
			pos.z = camera.farClipPlane;
			Vector3 result = camera.ScreenToWorldPoint(pos);
			result.z = 0f;
			return result;
		}
		Camera camera2 = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		pos.z = camera2.farClipPlane;
		Vector3 result2 = camera2.ScreenToWorldPoint(pos);
		result2.z = 0f;
		return result2;
	}

	public static Vector3 GUIToZ0(Vector3 pos)
	{
		pos.y = (float)Screen.height - pos.y;
		return ScreenToZ0(pos);
	}

	public static T FindSceneObjectOfType<T>() where T : Object
	{
		T[] array = Object.FindSceneObjectsOfType(typeof(T)) as T[];
		if (array.Length > 0)
		{
			return array[0];
		}
		return (T)null;
	}

	public static int GetNumberOfHighestBit(int val)
	{
		for (int num = 30; num >= 0; num--)
		{
			if ((val & (1 << num)) != 0)
			{
				return num;
			}
		}
		return -1;
	}

	public static Vector3 ViewportToZ0(Vector3 pos)
	{
		Camera mainCamera = Camera.main;
		pos.z = mainCamera.farClipPlane;
		Vector3 vector = mainCamera.ViewportToWorldPoint(pos);
		Vector3 position = mainCamera.transform.position;
		Vector3 vector2 = vector - position;
		Vector3 vector3 = vector2 / vector2.z;
		return position + vector3 * (0f - position.z);
	}

	public static Vector3 ClipAgainstViewport(Vector3 pos1, Vector3 pos2)
	{
		Camera mainCamera = Camera.main;
		Vector3 vector = mainCamera.WorldToViewportPoint(pos1);
		Vector3 vector2 = mainCamera.WorldToViewportPoint(pos2);
		Vector3 vector3 = vector2 - vector;
		float num = 1f;
		if (vector3.x < 0f)
		{
			float num2 = vector.x / (0f - vector3.x);
			if (num2 < num)
			{
				num = num2;
			}
		}
		if (vector3.y < 0f)
		{
			float num3 = vector.y / (0f - vector3.y);
			if (num3 < num)
			{
				num = num3;
			}
		}
		if (vector3.x > 0f)
		{
			float num4 = (1f - vector.x) / vector3.x;
			if (num4 < num)
			{
				num = num4;
			}
		}
		if (vector3.y > 0f)
		{
			float num5 = (1f - vector.y) / vector3.y;
			if (num5 < num)
			{
				num = num5;
			}
		}
		return mainCamera.ViewportToWorldPoint(vector + vector3 * num);
	}
}
