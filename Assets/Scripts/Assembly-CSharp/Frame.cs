using UnityEngine;

public class Frame : BasePart
{
	public enum FrameMaterial
	{
		Wood = 0,
		Metal = 1
	}

	public FrameMaterial m_material;

	public Texture2D[] m_brokenTextures;

	private bool isInWater;

	public override bool CanEncloseParts()
	{
		return true;
	}

	public override bool IsPartOfChassis()
	{
		return true;
	}

	public override void Initialize()
	{
		if ((bool)m_enclosedPart)
		{
			FixedJoint fixedJoint = m_enclosedPart.gameObject.AddComponent<FixedJoint>();
			fixedJoint.connectedBody = base.gameObject.GetComponent<Rigidbody>();
			float breakForce = base.contraption.GetJointConnectionStrength(GetJointConnectionStrength()) + base.contraption.GetJointConnectionStrength(m_enclosedPart.GetJointConnectionStrength());
			fixedJoint.breakForce = breakForce;
			base.contraption.AddJointToMap(this, m_enclosedPart, fixedJoint);
			Physics.IgnoreCollision(base.GetComponent<Collider>(), m_enclosedPart.GetComponent<Collider>());
		}
	}

	public override void OnBreak()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Water")
		{
			isInWater = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Water")
		{
			isInWater = false;
		}
	}

	private void FixedUpdate()
	{
		if (isInWater && (bool)base.GetComponent<Rigidbody>())
		{
			Vector3 vector = new Vector3(0f, 460f, 0f) * Time.fixedDeltaTime;
			base.GetComponent<Rigidbody>().AddForce(vector, ForceMode.Force);
			Debug.DrawLine(base.GetComponent<Rigidbody>().transform.position + base.GetComponent<Rigidbody>().centerOfMass, base.GetComponent<Rigidbody>().transform.position + base.GetComponent<Rigidbody>().centerOfMass + vector, Color.yellow, 1f);
		}
	}
}
