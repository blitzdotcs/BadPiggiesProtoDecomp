using UnityEngine;

public class BasePart : WPFMonoBehaviour
{
	public enum PartType
	{
		Unknown = 0,
		Balloon = 1,
		Balloons2 = 2,
		Balloons3 = 3,
		Fan = 4,
		WoodenFrame = 5,
		Bellows = 6,
		CartWheel = 7,
		Basket = 8,
		Sandbag = 9,
		Pig = 10,
		Sandbag2 = 11,
		Sandbag3 = 12,
		Propeller = 13,
		Wings = 14,
		Tailplane = 15,
		Engine = 16,
		Rocket = 17,
		MetalFrame = 18,
		SmallWheel = 19,
		MetalWing = 20,
		MetalTail = 21,
		Rotor = 22,
		MotorWheel = 23,
		TNT = 24,
		EngineSmall = 25,
		EngineBig = 26,
		NormalWheel = 27,
		Spring = 28,
		Umbrella = 29,
		Rope = 30,
		CokeBottle = 31,
		KingPig = 32,
		RedRocket = 33,
		SodaBottle = 34,
		PoweredUmbrella = 35,
		MAX = 36
	}

	public enum AutoAlignType
	{
		None = 0,
		Rotate = 1,
		FlipVertically = 2
	}

	public enum Direction
	{
		Right = 0,
		Up = 1,
		Left = 2,
		Down = 3
	}

	public enum GridRotation
	{
		Deg_0 = 0,
		Deg_90 = 1,
		Deg_180 = 2,
		Deg_270 = 3
	}

	public enum JointConnectionType
	{
		None = 0,
		Source = 1,
		Target = 2
	}

	public enum JointConnectionDirection
	{
		Any = 0,
		Right = 1,
		Up = 2,
		Left = 3,
		Down = 4,
		LeftAndRight = 5,
		UpAndDown = 6,
		None = 7
	}

	public enum JointConnectionStrength
	{
		Weak = 0,
		Normal = 1,
		High = 2,
		Extreme = 3
	}

	private static float m_lastTimeUsedCollisionParticles;

	public int m_coordX;

	public int m_coordY;

	public float m_mass = 1f;

	public float m_interactiveRadius = 0.5f;

	public float m_breakVelocity;

	public float m_powerConsumption;

	public float m_enginePower;

	public float m_ZOffset;

	[SerializeField]
	private AudioManager.AudioMaterial audioMaterial;

	public PartType m_partType;

	public AutoAlignType m_autoAlign;

	public bool m_flipped;

	public GridRotation m_gridRotation;

	public int m_gridXmin;

	public int m_gridXmax;

	public int m_gridYmin;

	public int m_gridYmax;

	public bool m_static;

	public JointConnectionStrength m_jointConnectionStrength;

	public JointConnectionType m_jointConnectionType = JointConnectionType.Target;

	public JointConnectionDirection m_jointConnectionDirection;

	public JointConnectionDirection m_customJointConnectionDirection = JointConnectionDirection.None;

	private Contraption m_contraption;

	private bool m_broken;

	private int m_connectedComponent = -1;

	private int m_searchConnectedComponent = -1;

	public BasePart m_enclosedPart;

	public BasePart m_enclosedInto;

	public Transform m_actualVisualizationNode;

	public Sprite m_constructionIconSprite;

	protected bool m_valid;

	protected SpriteManager m_spriteManager;

	private Vector3 m_windVelocity;

	public AudioManager.AudioMaterial AudioMaterial
	{
		get
		{
			return audioMaterial;
		}
	}

	public Contraption contraption
	{
		get
		{
			return m_contraption;
		}
		set
		{
			m_contraption = value;
		}
	}

	public int ConnectedComponent
	{
		get
		{
			return m_connectedComponent;
		}
		set
		{
			m_connectedComponent = value;
		}
	}

	public int SearchConnectedComponent
	{
		get
		{
			return m_searchConnectedComponent;
		}
		set
		{
			m_searchConnectedComponent = value;
		}
	}

	public BasePart enclosedPart
	{
		get
		{
			return m_enclosedPart;
		}
		set
		{
			m_enclosedPart = value;
			if ((bool)value)
			{
				value.enclosedInto = this;
			}
		}
	}

	public BasePart enclosedInto
	{
		get
		{
			return m_enclosedInto;
		}
		set
		{
			m_enclosedInto = value;
		}
	}

	public Vector3 WindVelocity
	{
		get
		{
			return m_windVelocity;
		}
		set
		{
			m_windVelocity = value;
		}
	}

	public bool valid
	{
		get
		{
			return m_valid;
		}
		set
		{
			m_valid = value;
		}
	}

	public virtual JointConnectionStrength GetJointConnectionStrength()
	{
		return m_jointConnectionStrength;
	}

	public static PartType BaseType(PartType type)
	{
		switch (type)
		{
		case PartType.Balloons2:
			return PartType.Balloon;
		case PartType.Balloons3:
			return PartType.Balloon;
		case PartType.Sandbag2:
			return PartType.Sandbag;
		case PartType.Sandbag3:
			return PartType.Sandbag;
		default:
			return type;
		}
	}

	public virtual void Awake()
	{
		m_valid = true;
		m_spriteManager = GetComponent<SpriteManager>();
	}

	public virtual void Initialize()
	{
	}

	public virtual void InitializeEngine()
	{
	}

	public virtual bool CanBeEnabled()
	{
		return false;
	}

	public virtual bool HasOnOffToggle()
	{
		return CanBeEnabled();
	}

	public virtual bool IsEnabled()
	{
		return false;
	}

	public virtual void SetEnabled(bool enabled)
	{
	}

	public virtual bool IsPowered()
	{
		return m_powerConsumption > 0f;
	}

	public bool IsEngine()
	{
		return m_enginePower > 0f;
	}

	public virtual Direction EffectDirection()
	{
		return Direction.Right;
	}

	public virtual GridRotation AutoAlignRotation(JointConnectionDirection target)
	{
		return RotationTo(m_jointConnectionDirection, target);
	}

	public bool IsFlipped()
	{
		return m_flipped;
	}

	public void SetFlipped(bool flipped)
	{
		m_flipped = flipped;
		if (m_flipped)
		{
			base.transform.localRotation = Quaternion.AngleAxis(180f, Vector3.up);
		}
		else
		{
			base.transform.localRotation = Quaternion.identity;
		}
	}

	public static Direction ConvertDirection(JointConnectionDirection direction)
	{
		Assert.Check(direction != 0 && direction != JointConnectionDirection.None && direction != JointConnectionDirection.LeftAndRight && direction != JointConnectionDirection.UpAndDown, string.Concat("Cannot convert ", direction, " to Direction"));
		return (Direction)(direction - 1);
	}

	public JointConnectionDirection GetJointConnectionDirection()
	{
		return GlobalJointConnectionDirection(m_jointConnectionDirection);
	}

	public JointConnectionDirection GetCustomJointConnectionDirection()
	{
		return GlobalJointConnectionDirection(m_customJointConnectionDirection);
	}

	public JointConnectionDirection GlobalJointConnectionDirection(JointConnectionDirection localDirection)
	{
		if (localDirection == JointConnectionDirection.Any || localDirection == JointConnectionDirection.None)
		{
			return localDirection;
		}
		JointConnectionDirection jointConnectionDirection = localDirection;
		if (localDirection == JointConnectionDirection.LeftAndRight && (m_gridRotation == GridRotation.Deg_90 || m_gridRotation == GridRotation.Deg_270))
		{
			jointConnectionDirection = JointConnectionDirection.UpAndDown;
		}
		if (localDirection == JointConnectionDirection.UpAndDown)
		{
			if (m_gridRotation == GridRotation.Deg_90 || m_gridRotation == GridRotation.Deg_270)
			{
				jointConnectionDirection = JointConnectionDirection.LeftAndRight;
			}
		}
		else
		{
			jointConnectionDirection = (JointConnectionDirection)(((int)(localDirection - 1) + (int)m_gridRotation) % 4 + 1);
		}
		if (m_flipped)
		{
			switch (jointConnectionDirection)
			{
			case JointConnectionDirection.Left:
				return JointConnectionDirection.Right;
			case JointConnectionDirection.Right:
				return JointConnectionDirection.Left;
			}
		}
		return jointConnectionDirection;
	}

	public static Direction InverseDirection(Direction direction)
	{
		switch (direction)
		{
		case Direction.Right:
			return Direction.Left;
		case Direction.Up:
			return Direction.Down;
		case Direction.Left:
			return Direction.Right;
		case Direction.Down:
			return Direction.Up;
		default:
			return Direction.Right;
		}
	}

	public bool CanConnectTo(Direction direction)
	{
		switch (GetJointConnectionDirection())
		{
		case JointConnectionDirection.Any:
			return true;
		case JointConnectionDirection.None:
			return false;
		case JointConnectionDirection.Right:
			return direction == Direction.Right;
		case JointConnectionDirection.Up:
			return direction == Direction.Up;
		case JointConnectionDirection.Left:
			return direction == Direction.Left;
		case JointConnectionDirection.Down:
			return direction == Direction.Down;
		case JointConnectionDirection.LeftAndRight:
			return direction == Direction.Left || direction == Direction.Right;
		case JointConnectionDirection.UpAndDown:
			return direction == Direction.Up || direction == Direction.Down;
		default:
			return false;
		}
	}

	public static Direction Rotate(Direction direction, GridRotation rotation)
	{
		return (Direction)(((int)direction + (int)rotation) % 4);
	}

	public static GridRotation RotationTo(JointConnectionDirection source, JointConnectionDirection target)
	{
		if (source == JointConnectionDirection.Any || target == JointConnectionDirection.Any || source == JointConnectionDirection.None || target == JointConnectionDirection.None)
		{
			return GridRotation.Deg_0;
		}
		switch (source)
		{
		case JointConnectionDirection.LeftAndRight:
			if (target == JointConnectionDirection.UpAndDown || target == JointConnectionDirection.Up || target == JointConnectionDirection.Down)
			{
				return GridRotation.Deg_90;
			}
			return GridRotation.Deg_0;
		case JointConnectionDirection.UpAndDown:
			if (target == JointConnectionDirection.LeftAndRight || target == JointConnectionDirection.Left || target == JointConnectionDirection.Right)
			{
				return GridRotation.Deg_90;
			}
			return GridRotation.Deg_0;
		default:
		{
			int num = target - source;
			return (GridRotation)((num + 4) % 4);
		}
		}
	}

	public float GetRotationAngle(GridRotation rotation)
	{
		switch (rotation)
		{
		case GridRotation.Deg_0:
			return 0f;
		case GridRotation.Deg_90:
			return 90f;
		case GridRotation.Deg_180:
			return 180f;
		case GridRotation.Deg_270:
			return 270f;
		default:
			return 0f;
		}
	}

	public void SetRotation(GridRotation rotation)
	{
		m_gridRotation = rotation;
		base.transform.localRotation = Quaternion.AngleAxis(GetRotationAngle(rotation), Vector3.forward);
	}

	public void RotateClockwise()
	{
		switch (m_gridRotation)
		{
		case GridRotation.Deg_0:
			SetRotation(GridRotation.Deg_270);
			break;
		case GridRotation.Deg_90:
			SetRotation(GridRotation.Deg_0);
			break;
		case GridRotation.Deg_180:
			SetRotation(GridRotation.Deg_90);
			break;
		case GridRotation.Deg_270:
			SetRotation(GridRotation.Deg_180);
			break;
		}
	}

	public virtual void ProcessTouch()
	{
	}

	private void OnCollisionEnter(Collision c)
	{
		AudioManager.Instance.PlayCollisionAudio(this, c);
		foreach (ContactPoint item in c)
		{
			if (Time.time - m_lastTimeUsedCollisionParticles > 0.25f && item.otherCollider.tag == "Untagged")
			{
				m_lastTimeUsedCollisionParticles = Time.time;
				WPFMonoBehaviour.effectManager.CreateParticles(WPFMonoBehaviour.gameData.m_dustParticles, base.transform.position);
				float num = Vector3.Dot(base.GetComponent<Rigidbody>().GetPointVelocity(item.point), item.normal);
				if (m_breakVelocity > 0f && num > m_breakVelocity && !m_broken)
				{
					OnBreak();
					m_broken = true;
				}
			}
		}
	}

	public void PreInitialize()
	{
		Renderer renderer = base.GetComponent<Renderer>();
		if ((bool)m_actualVisualizationNode)
		{
			renderer = m_actualVisualizationNode.GetComponent<Renderer>();
			renderer.enabled = true;
			if ((bool)base.GetComponent<Renderer>())
			{
				base.GetComponent<Renderer>().enabled = false;
			}
		}
	}

	public virtual void PostInitialize()
	{
	}

	public virtual void PrePlaced()
	{
	}

	public static Vector3 GetDirectionVector(Direction direction)
	{
		switch (direction)
		{
		case Direction.Down:
			return -Vector3.up;
		case Direction.Up:
			return Vector3.up;
		case Direction.Left:
			return -Vector3.right;
		case Direction.Right:
			return Vector3.right;
		default:
			return Vector3.up;
		}
	}

	public virtual bool WillDetach()
	{
		return false;
	}

	public virtual bool IsIntegralPart()
	{
		return true;
	}

	public virtual bool CanEncloseParts()
	{
		return false;
	}

	public virtual bool CanBeEnclosed()
	{
		return false;
	}

	public virtual bool IsPartOfChassis()
	{
		return false;
	}

	public virtual JointConnectionType GetJointConnectionType()
	{
		return m_jointConnectionType;
	}

	public virtual bool ValidatePart()
	{
		return true;
	}

	public virtual void EnsureRigidbody()
	{
		Rigidbody rigidbody = base.gameObject.GetComponent<Rigidbody>();
		if (rigidbody == null)
		{
			rigidbody = base.gameObject.AddComponent<Rigidbody>();
		}
		rigidbody.constraints = (RigidbodyConstraints)56;
		rigidbody.mass = m_mass;
		rigidbody.drag = 0.2f;
		rigidbody.angularDrag = 0.05f;
		rigidbody.useGravity = true;
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		base.gameObject.layer = LayerMask.NameToLayer("Contraption");
		for (int i = 0; i < base.transform.childCount; i++)
		{
			base.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Contraption");
		}
	}

	public virtual Joint CustomConnectToPart(BasePart part)
	{
		return null;
	}

	public virtual void OnChangeConnections()
	{
		if (m_jointConnectionDirection != 0 && m_jointConnectionDirection != JointConnectionDirection.None && !contraption.CanConnectTo(this, GetJointConnectionDirection()))
		{
			contraption.AutoAlign(this);
		}
		ChangeVisualConnections();
	}

	public virtual void ChangeVisualConnections()
	{
	}

	public virtual void OnDetach()
	{
	}

	private void OnJointBreak(float breakForce)
	{
		CheckForBrokenPartsAchievement();
		StartCoroutine(contraption.OnJointDetached());
		AudioManager.Instance.PlayBreakAudio(this);
		EventManager.Send(new ScoreChanged(base.gameObject.transform.position));
		Vector3 position = base.transform.position;
		Vector3 normalized = base.GetComponent<Rigidbody>().velocity.normalized;
		GameObject sprite = ((m_partType != PartType.WoodenFrame && m_partType != PartType.Pig) ? WPFMonoBehaviour.gameData.m_snapSprite : WPFMonoBehaviour.gameData.m_krakSprite);
		WPFMonoBehaviour.effectManager.ShowBreakEffect(sprite, position - 2f * normalized + new Vector3(0f, 0f, -10f), Quaternion.AngleAxis(Random.Range(-30f, 30f), Vector3.forward));
	}

	public virtual void OnBreak()
	{
	}

	public void CheckForBrokenPartsAchievement()
	{
		int num = GameProgress.GetInt("Broken_Parts") + 1;
		GameProgress.SetInt("Broken_Parts", num);
		if (DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios)
		{
			if (num > AchievementData.Instance.GetAchievementLimit("grp.VETERAN_WRECKER"))
			{
				SocialGameManager.Instance.ReportAchievementProgress("grp.VETERAN_WRECKER", 100.0);
			}
			else if (num > AchievementData.Instance.GetAchievementLimit("grp.QUALIFIED_WRECKER"))
			{
				SocialGameManager.Instance.ReportAchievementProgress("grp.QUALIFIED_WRECKER", 100.0);
			}
			else if (num > AchievementData.Instance.GetAchievementLimit("grp.JUNIOR_WRECKER"))
			{
				SocialGameManager.Instance.ReportAchievementProgress("grp.JUNIOR_WRECKER", 100.0);
			}
		}
	}
}
