using UnityEngine;

public class GizmoUtils : MonoBehaviour
{
	public static void DrawArrow(Vector3 pos, Vector3 direction)
	{
		if (direction.magnitude != 0f)
		{
			float num = 0.35f;
			float num2 = 30f;
			Vector3 vector = Quaternion.AngleAxis(num2 + 180f, Vector3.forward) * direction;
			Vector3 vector2 = Quaternion.AngleAxis(0f - num2 - 180f, Vector3.forward) * direction;
			Gizmos.DrawRay(pos, direction);
			Gizmos.DrawRay(pos + direction, vector * num);
			Gizmos.DrawRay(pos + direction, vector2 * num);
		}
	}
}
