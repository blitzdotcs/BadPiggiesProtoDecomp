using System.Collections.Generic;
using UnityEngine;

public class Wings : BasePart
{
	public float liftConstant = 0.8f;

	public float dragConstant = 0.8f;

	private ResponseCurve liftCoefficients = new ResponseCurve();

	public override bool ValidatePart()
	{
		if (!WPFMonoBehaviour.levelManager.RequireConnectedContraption)
		{
			return true;
		}
		List<BasePart> list = base.contraption.FindNeighbours(m_coordX, m_coordY);
		int num = 0;
		foreach (BasePart item in list)
		{
			if (item.IsPartOfChassis())
			{
				num++;
			}
		}
		if (num < 1)
		{
			return false;
		}
		return true;
	}

	public override void ChangeVisualConnections()
	{
		bool flag = base.contraption.CanConnectTo(this, Direction.Up) || base.contraption.CanConnectTo(this, Direction.Left) || base.contraption.CanConnectTo(this, Direction.Right);
		bool flag2 = base.contraption.CanConnectTo(this, Direction.Down) || base.contraption.CanConnectTo(this, Direction.Left) || base.contraption.CanConnectTo(this, Direction.Right);
		base.transform.Find("TopFrameSprite").gameObject.active = flag;
		base.transform.Find("BottomFrameSprite").gameObject.active = flag2 || !flag;
	}

	private void Start()
	{
		liftCoefficients.AddPoint(-180f, 0f);
		liftCoefficients.AddPoint(-135f, -0.2f);
		liftCoefficients.AddPoint(-90f, 0f);
		liftCoefficients.AddPoint(-45f, -0.2f);
		liftCoefficients.AddPoint(-10f, 0f);
		liftCoefficients.AddPoint(10f, 1.5f);
		liftCoefficients.AddPoint(15f, 1.75f);
		liftCoefficients.AddPoint(20f, 0.8f);
		liftCoefficients.AddPoint(25f, 0.1f);
		liftCoefficients.AddPoint(45f, 0.2f);
		liftCoefficients.AddPoint(90f, 0f);
		liftCoefficients.AddPoint(135f, -0.2f);
		liftCoefficients.AddPoint(180f, 0f);
	}

	public override void EnsureRigidbody()
	{
		Rigidbody rigidbody = base.gameObject.GetComponent<Rigidbody>();
		if (rigidbody == null)
		{
			rigidbody = base.gameObject.AddComponent<Rigidbody>();
		}
		rigidbody.constraints = (RigidbodyConstraints)56;
		rigidbody.mass = m_mass;
		rigidbody.drag = 1f;
		rigidbody.angularDrag = 0.2f;
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
	}

	public void FixedUpdate()
	{
		if ((bool)base.contraption && base.contraption.isRunning)
		{
			Vector3 vector = base.GetComponent<Rigidbody>().velocity - base.WindVelocity;
			base.WindVelocity = Vector3.zero;
			Vector3 right = base.transform.right;
			float num = ((!IsFlipped()) ? 1f : (-1f));
			float num2 = num * Mathf.Sign(Vector3.Cross(vector, right).z);
			float x = num2 * Vector3.Angle(vector, right);
			float num3 = liftCoefficients.Get(x);
			Vector3 vector2 = Vector3.Cross(base.transform.forward, vector.normalized);
			Vector3 vector3 = liftConstant * vector.sqrMagnitude * num3 * vector2;
			vector3 = Vector3.ClampMagnitude(vector3, 100f);
			base.GetComponent<Rigidbody>().AddForce(vector3, ForceMode.Force);
			Debug.DrawRay(base.transform.position, 5f * right, Color.yellow);
			Debug.DrawRay(base.transform.position, 0.25f * vector, Color.blue);
			Debug.DrawRay(base.transform.position, 0.1f * vector3);
		}
	}
}
