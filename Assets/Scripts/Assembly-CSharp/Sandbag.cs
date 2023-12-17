using UnityEngine;

public class Sandbag : BasePart
{
	public bool m_inWorldCoordinates = true;

	public Vector3 m_direction = Vector3.up;

	public int m_numberOfBalloons = 1;

	public bool m_enabled = true;

	public Material m_stringMaterial;

	protected BasePart m_connectedPart;

	protected Vector3 m_connectedLocalPos;

	public override void Awake()
	{
		base.Awake();
	}

	public override bool IsIntegralPart()
	{
		return false;
	}

	public override bool WillDetach()
	{
		return true;
	}

	public bool IsAttached()
	{
		SpringJoint component = GetComponent<SpringJoint>();
		return (bool)component && (bool)component.connectedBody;
	}

	public override void Initialize()
	{
		base.contraption.ChangeOneShotPartAmount(BasePart.BaseType(m_partType), EffectDirection(), 1);
		m_connectedPart = base.contraption.FindPartAt(m_coordX, m_coordY + 1);
		m_partType = PartType.Sandbag;
		if (m_numberOfBalloons > 1)
		{
			GameObject gameObject = Object.Instantiate(base.gameObject) as GameObject;
			gameObject.transform.position = base.transform.position;
			Sandbag component = gameObject.GetComponent<Sandbag>();
			component.m_numberOfBalloons = m_numberOfBalloons - 1;
			base.contraption.AddRuntimePart(component);
			gameObject.transform.parent = base.contraption.transform;
		}
		if (!base.gameObject.GetComponent<SphereCollider>())
		{
			SphereCollider sphereCollider = base.gameObject.AddComponent<SphereCollider>();
			sphereCollider.radius = 0.13f;
			sphereCollider.center = new Vector3(0f, -0.1f, 0f);
		}
		if (!base.GetComponent<Rigidbody>())
		{
			base.gameObject.AddComponent<Rigidbody>();
		}
		base.GetComponent<Rigidbody>().mass = m_mass;
		base.GetComponent<Rigidbody>().drag = 1f;
		base.GetComponent<Rigidbody>().angularDrag = 10f;
		base.GetComponent<Rigidbody>().constraints = (RigidbodyConstraints)56;
		if ((bool)m_connectedPart)
		{
			Vector3 position = base.transform.position;
			base.transform.position = m_connectedPart.transform.position - Vector3.up * 0.5f;
			SpringJoint springJoint = base.gameObject.AddComponent<SpringJoint>();
			springJoint.connectedBody = m_connectedPart.GetComponent<Rigidbody>();
			m_connectedLocalPos = m_connectedPart.transform.InverseTransformPoint(base.transform.position);
			Vector3 vector;
			float maxDistance;
			switch (m_numberOfBalloons)
			{
			case 1:
				vector = new Vector3(-0.1f, -0.45f, -0.01f);
				maxDistance = 0.5f;
				break;
			case 2:
				vector = new Vector3(0.3f, -0.5f, -0.02f);
				maxDistance = 0.55f;
				break;
			default:
				vector = new Vector3(0f, -0.8f, -0.03f);
				maxDistance = 0.65f;
				break;
			}
			springJoint.minDistance = 0f;
			springJoint.maxDistance = maxDistance;
			springJoint.anchor = Vector3.up * 0.5f;
			springJoint.spring = 100f;
			springJoint.damper = 10f;
			base.transform.position = position + vector;
			LineRenderer lineRenderer = base.gameObject.AddComponent<LineRenderer>();
			lineRenderer.material = m_stringMaterial;
			lineRenderer.SetVertexCount(2);
			lineRenderer.SetWidth(0.05f, 0.05f);
			lineRenderer.SetColors(Color.black, Color.black);
		}
		else
		{
			Vector3 vector2;
			switch (m_numberOfBalloons)
			{
			case 1:
				vector2 = new Vector3(-0.1f, -0.45f, -0.01f);
				break;
			case 2:
				vector2 = new Vector3(0.3f, -0.5f, -0.02f);
				break;
			default:
				vector2 = new Vector3(0f, -0.8f, -0.03f);
				break;
			}
			base.transform.position += vector2;
		}
	}

	public void FixedUpdate()
	{
	}

	public override void ProcessTouch()
	{
		Pop();
	}

	public void OnCollisionEnter(Collision coll)
	{
		if (coll.collider.tag == "Ground")
		{
			AudioManager.Instance.SpawnOneShotEffect(AudioManager.Instance.CommonAudioCollection.sandbagCollision, coll.transform.position);
		}
	}

	public void Pop()
	{
		SpringJoint component = GetComponent<SpringJoint>();
		base.contraption.ChangeOneShotPartAmount(BasePart.BaseType(m_partType), EffectDirection(), -1);
		base.gameObject.layer = LayerMask.NameToLayer("DroppedSandbag");
		if ((bool)component && (bool)component.connectedBody)
		{
			component.connectedBody.AddForce(5f * Vector3.up, ForceMode.Impulse);
			base.GetComponent<Rigidbody>().AddForce(-4f * Vector3.up, ForceMode.Impulse);
		}
		if ((bool)component)
		{
			Object.Destroy(component);
		}
		LineRenderer component2 = GetComponent<LineRenderer>();
		if ((bool)component2)
		{
			Object.Destroy(component2);
		}
	}

	public void LateUpdate()
	{
		SpringJoint component = GetComponent<SpringJoint>();
		if ((bool)component)
		{
			LineRenderer component2 = GetComponent<LineRenderer>();
			Vector3 position = base.transform.position + base.transform.up * 0.4f;
			if ((bool)component.connectedBody)
			{
				Vector3 position2 = component.connectedBody.transform.TransformPoint(m_connectedLocalPos);
				component2.SetPosition(0, position);
				component2.SetPosition(1, position2);
			}
		}
	}
}
