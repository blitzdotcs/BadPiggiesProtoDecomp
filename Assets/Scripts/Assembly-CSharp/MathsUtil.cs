using UnityEngine;

public class MathsUtil : MonoBehaviour
{
	public static float AngleDir(Vector3 targetDir)
	{
		Vector3 lhs = Vector3.Cross(Vector3.forward, targetDir.normalized);
		float num = Vector3.Dot(lhs, Vector3.up);
		if ((double)num > 0.0)
		{
			return 1f;
		}
		if (num < 0f)
		{
			return -1f;
		}
		return 0f;
	}

	public static float CatmullRomInterpolate(float a, float b, float c, float d, float i)
	{
		return a * ((0f - i + 2f) * i - 1f) * i * 0.5f + b * ((3f * i - 5f) * i * i + 2f) * 0.5f + c * ((-3f * i + 4f) * i + 1f) * i * 0.5f + d * ((i - 1f) * i * i) * 0.5f;
	}

	public static Vector3 CatmullRomInterpolate(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float i)
	{
		return new Vector3(CatmullRomInterpolate(a.x, b.x, c.x, d.x, i), CatmullRomInterpolate(a.y, b.y, c.y, d.y, i), CatmullRomInterpolate(a.z, b.z, c.z, d.z, i));
	}

	public static float EasingInQuad(float t, float b, float c, float d)
	{
		t /= d;
		return c * t * t + b;
	}

	public static float EasingOutQuad(float t, float b, float c, float d)
	{
		t /= d;
		return (0f - c) * t * (t - 2f) + b;
	}

	public static float EaseInOutQuad(float t, float b, float c, float d)
	{
		t /= d / 2f;
		if (t < 1f)
		{
			return c / 2f * t * t + b;
		}
		t -= 1f;
		return (0f - c) / 2f * (t * (t - 2f) - 1f) + b;
	}
}
