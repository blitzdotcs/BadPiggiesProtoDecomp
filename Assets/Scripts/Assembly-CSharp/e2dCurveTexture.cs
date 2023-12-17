using System;
using UnityEngine;

[Serializable]
public class e2dCurveTexture
{
	public Texture texture;

	public Vector2 size;

	public bool fixedAngle;

	public float fadeThreshold;

	public e2dCurveTexture(Texture _texture)
	{
		texture = _texture;
		size = new Vector2(1f, 1f);
		fixedAngle = false;
		fadeThreshold = 0.3f;
	}

	public e2dCurveTexture(e2dCurveTexture other)
	{
		texture = other.texture;
		size = other.size;
		fixedAngle = other.fixedAngle;
		fadeThreshold = other.fadeThreshold;
	}
}
