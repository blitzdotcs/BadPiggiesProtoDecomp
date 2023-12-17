using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Contraption : WPFMonoBehaviour
{
	protected struct JointConnectionStruct
	{
		public BasePart partA;

		public BasePart partB;

		public Joint joint;
	}

	private class ComponentSeardhState
	{
		public Queue<BasePart> parts = new Queue<BasePart>();

		public Queue<BasePart> connectedParts = new Queue<BasePart>();

		public int componentIndex;

		public ConnectedComponent component;

		public List<ConnectedComponent> connectedComponents = new List<ConnectedComponent>();

		public int updatesUsed;

		public bool finished;
	}

	private struct ConnectedComponent
	{
		public bool hasEngine;

		public float enginePower;

		public float powerConsumption;

		public List<MotorWheel> motorWheels;

		public List<PoweredUmbrella> poweredUmbrellas;

		public int partCount;
	}

	public class PartPlacementInfo
	{
		public BasePart.PartType partType;

		public BasePart.Direction direction;

		public Vector3 averagePosition = Vector3.zero;

		public int count;

		public PartPlacementInfo(BasePart.PartType partType, BasePart.Direction direction, Vector3 averagePosition, int count)
		{
			this.partType = partType;
			this.direction = direction;
			this.averagePosition = averagePosition;
			this.count = count;
		}
	}

	private class PartOrder : IComparer<PartPlacementInfo>
	{
		public int Compare(PartPlacementInfo obj1, PartPlacementInfo obj2)
		{
			if (obj1.averagePosition.x < obj2.averagePosition.x)
			{
				return -1;
			}
			if (obj1.averagePosition.x > obj2.averagePosition.x)
			{
				return 1;
			}
			return 0;
		}
	}

	protected List<BasePart> m_parts = new List<BasePart>();

	protected Dictionary<int, BasePart> m_partMap = new Dictionary<int, BasePart>();

	protected List<BasePart> m_integralParts = new List<BasePart>();

	protected Dictionary<Rigidbody, Vector3> m_rigidbodyVelocityMap = new Dictionary<Rigidbody, Vector3>();

	protected Camera m_gameCamera;

	private ContraptionDataset m_contraptionDataSet;

	protected List<JointConnectionStruct> m_jointMap = new List<JointConnectionStruct>();

	protected bool m_running;

	public int m_enginesAmount;

	private float m_powerConsumption;

	private float m_stopTimer;

	private List<ConnectedComponent> m_connectedComponents = new List<ConnectedComponent>();

	private Dictionary<BasePart.PartType, int[]> m_oneShotPartAmount = new Dictionary<BasePart.PartType, int[]>();

	private Dictionary<BasePart.PartType, int[]> m_poweredPartAmount = new Dictionary<BasePart.PartType, int[]>();

	private Dictionary<BasePart.PartType, int[]> m_pushablePartAmount = new Dictionary<BasePart.PartType, int[]>();

	private int m_droppedSandbagLayer = LayerMask.NameToLayer("DroppedSandbag");

	private bool m_broken;

	private List<PartPlacementInfo> m_partPlacements = new List<PartPlacementInfo>();

	private int m_staticPartCount;

	private ComponentSeardhState m_componentSearchState;

	private string m_hashKey = string.Empty;

	public BasePart m_cameraTarget;

	public BasePart m_pig;

	protected float m_timeLastCollided;

	public List<BasePart> Parts
	{
		get
		{
			return m_parts;
		}
	}

	public bool isRunning
	{
		get
		{
			return m_running;
		}
	}

	public bool HasEngine
	{
		get
		{
			return m_enginesAmount > 0;
		}
	}

	public List<PartPlacementInfo> PartPlacements
	{
		get
		{
			return m_partPlacements;
		}
	}

	public float PowerConsumption
	{
		get
		{
			return m_powerConsumption;
		}
	}

	public void SetBroken()
	{
		m_broken = true;
	}

	public void ChangeOneShotPartAmount(BasePart.PartType type, BasePart.Direction direction, int change)
	{
		int[] value;
		if (m_oneShotPartAmount.TryGetValue(type, out value))
		{
			value[(int)direction] += change;
		}
		else if (change > 0)
		{
			value = new int[4];
			value[(int)direction] = change;
			m_oneShotPartAmount[type] = value;
		}
	}

	public string GetContraptionID()
	{
		string text = string.Empty;
		if (m_hashKey.CompareTo(string.Empty) == 0)
		{
			foreach (BasePart part in m_parts)
			{
				string text2 = text;
				text = string.Concat(text2, part.m_coordX, ",", part.m_coordY, ",", part.m_partType, ",", part.IsFlipped());
			}
			m_hashKey = BitConverter.ToString(CryptoUtility.ComputeHash(Encoding.ASCII.GetBytes(text)));
		}
		return m_hashKey;
	}

	public void ChangePoweredPartAmount(BasePart.PartType type, BasePart.Direction direction, int change)
	{
		int[] value;
		if (m_poweredPartAmount.TryGetValue(type, out value))
		{
			value[(int)direction] += change;
		}
		else if (change > 0)
		{
			value = new int[4];
			value[(int)direction] = change;
			m_poweredPartAmount[type] = value;
		}
	}

	public void ChangePushablePartAmount(BasePart.PartType type, BasePart.Direction direction, int change)
	{
		int[] value;
		if (m_pushablePartAmount.TryGetValue(type, out value))
		{
			value[(int)direction] += change;
		}
		else if (change > 0)
		{
			value = new int[4];
			value[(int)direction] = change;
			m_pushablePartAmount[type] = value;
		}
	}

	public bool HasActiveParts(BasePart.PartType type, BasePart.Direction direction)
	{
		return HasOneShotParts(type, direction) || HasPoweredParts(type, direction) || HasPushableParts(type, direction);
	}

	public bool HasOneShotParts(BasePart.PartType type, BasePart.Direction direction)
	{
		int[] value;
		if (m_oneShotPartAmount.TryGetValue(type, out value))
		{
			return value[(int)direction] > 0;
		}
		return false;
	}

	public bool HasPoweredParts(BasePart.PartType type, BasePart.Direction direction)
	{
		int[] value;
		if (m_poweredPartAmount.TryGetValue(type, out value))
		{
			return value[(int)direction] > 0;
		}
		return false;
	}

	public bool HasPushableParts(BasePart.PartType type, BasePart.Direction direction)
	{
		int[] value;
		if (m_pushablePartAmount.TryGetValue(type, out value))
		{
			return value[(int)direction] > 0;
		}
		return false;
	}

	public bool IsMovementStopped()
	{
		return m_stopTimer > 0.5f;
	}

	public bool HasPart(BasePart.PartType type)
	{
		foreach (BasePart part in m_parts)
		{
			if (part.m_partType == type)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsBroken()
	{
		return m_broken;
	}

	public float GetEnginePowerFactor(BasePart part)
	{
		int connectedComponent = part.ConnectedComponent;
		if (connectedComponent >= 0 && connectedComponent < m_connectedComponents.Count)
		{
			ConnectedComponent connectedComponent2 = m_connectedComponents[connectedComponent];
			float num = 0f;
			float num2 = connectedComponent2.powerConsumption;
			foreach (MotorWheel motorWheel in connectedComponent2.motorWheels)
			{
				if (!motorWheel.HasContact)
				{
					num2 -= 0.9f * motorWheel.m_powerConsumption;
				}
			}
			if (num2 > 1f)
			{
				num = Mathf.Min(connectedComponent2.enginePower / num2, 10f);
			}
			else if (connectedComponent2.enginePower > 0f)
			{
				num = 1f;
			}
			if (num > 1f)
			{
				return Mathf.Pow(num, 0.585f);
			}
			return Mathf.Pow(num, 0.75f);
		}
		return 0f;
	}

	public void AddRuntimePart(BasePart part)
	{
		m_parts.Add(part);
	}

	private void Awake()
	{
		m_contraptionDataSet = new ContraptionDataset();
		m_gameCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
	}

	public int NumOfIntegralParts()
	{
		return m_integralParts.Count;
	}

	public int DynamicPartCount()
	{
		return m_parts.Count - m_staticPartCount;
	}

	public void IncreaseStaticPartCount()
	{
		m_staticPartCount++;
	}

	public BasePart FindPig()
	{
		foreach (BasePart part in m_parts)
		{
			if (part.m_partType == BasePart.PartType.Pig)
			{
				return part;
			}
		}
		return null;
	}

	public bool CanConnectTo(BasePart part1, BasePart part2, BasePart.Direction direction)
	{
		if (part1 != null && part2 != null)
		{
			if (!part1.CanConnectTo(direction))
			{
				return false;
			}
			if (!part2.CanConnectTo(BasePart.InverseDirection(direction)))
			{
				return false;
			}
			if (part2.m_jointConnectionType != 0 && (part2.m_jointConnectionType == BasePart.JointConnectionType.Source || part1.m_jointConnectionType == BasePart.JointConnectionType.Source))
			{
				return true;
			}
			if ((direction == BasePart.Direction.Down || direction == BasePart.Direction.Up) && (part1.m_partType == BasePart.PartType.Wings || part1.m_partType == BasePart.PartType.MetalWing) && (part2.m_partType == BasePart.PartType.Wings || part2.m_partType == BasePart.PartType.MetalWing))
			{
				return true;
			}
		}
		return false;
	}

	public bool CanConnectTo(BasePart part, BasePart.JointConnectionDirection direction)
	{
		switch (direction)
		{
		case BasePart.JointConnectionDirection.Any:
			return true;
		case BasePart.JointConnectionDirection.LeftAndRight:
			return CanConnectTo(part, BasePart.Direction.Left) || CanConnectTo(part, BasePart.Direction.Right);
		case BasePart.JointConnectionDirection.UpAndDown:
			return CanConnectTo(part, BasePart.Direction.Up) || CanConnectTo(part, BasePart.Direction.Down);
		default:
			return CanConnectTo(part, BasePart.ConvertDirection(direction));
		}
	}

	public bool CanConnectTo(BasePart part, BasePart.Direction direction)
	{
		int coordX = part.m_coordX;
		int coordY = part.m_coordY;
		switch (direction)
		{
		case BasePart.Direction.Up:
		{
			BasePart part2 = FindPartAt(coordX, coordY + 1);
			return CanConnectTo(part, part2, direction);
		}
		case BasePart.Direction.Down:
		{
			BasePart part2 = FindPartAt(coordX, coordY - 1);
			return CanConnectTo(part, part2, direction);
		}
		case BasePart.Direction.Left:
		{
			BasePart part2 = FindPartAt(coordX - 1, coordY);
			return CanConnectTo(part, part2, direction);
		}
		case BasePart.Direction.Right:
		{
			BasePart part2 = FindPartAt(coordX + 1, coordY);
			return CanConnectTo(part, part2, direction);
		}
		default:
			return false;
		}
	}

	public BasePart GetPartAt(BasePart part, BasePart.Direction direction)
	{
		int coordX = part.m_coordX;
		int coordY = part.m_coordY;
		switch (direction)
		{
		case BasePart.Direction.Up:
			return FindPartAt(coordX, coordY + 1);
		case BasePart.Direction.Down:
			return FindPartAt(coordX, coordY - 1);
		case BasePart.Direction.Left:
			return FindPartAt(coordX - 1, coordY);
		case BasePart.Direction.Right:
			return FindPartAt(coordX + 1, coordY);
		default:
			return null;
		}
	}

	public void StartContraption()
	{
		m_broken = false;
		base.GetComponent<Rigidbody>().isKinematic = false;
		base.GetComponent<Rigidbody>().useGravity = false;
		base.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
		m_stopTimer = 0f;
		m_parts = new List<BasePart>(GetComponentsInChildren<BasePart>());
		m_powerConsumption = 0f;
		m_enginesAmount = 0;
		foreach (BasePart part in m_parts)
		{
			Vector3 localPosition = part.transform.localPosition;
			int x = Mathf.RoundToInt(localPosition.x);
			int y = Mathf.RoundToInt(localPosition.y);
			SetPartPos(x, y, part);
			if ((bool)part.enclosedInto)
			{
				SetPartPos(x, y, part.enclosedInto);
			}
			if (part.IsIntegralPart())
			{
				m_integralParts.Add(part);
			}
			part.gameObject.tag = "Contraption";
			foreach (Transform item in part.transform)
			{
				item.gameObject.tag = "Contraption";
			}
			if (part.m_partType == BasePart.PartType.Pig)
			{
				m_cameraTarget = part;
				m_pig = part;
			}
			m_powerConsumption += part.m_powerConsumption;
		}
		foreach (BasePart part2 in m_parts)
		{
			int coordX = part2.m_coordX;
			int coordY = part2.m_coordY;
			m_contraptionDataSet.AddPart(coordX, coordY, (int)part2.m_partType, part2.m_gridRotation, part2.m_flipped);
			part2.EnsureRigidbody();
			if (part2.m_jointConnectionType != 0)
			{
				BasePart basePart = FindPartAt(coordX + 1, coordY);
				BasePart basePart2 = FindPartAt(coordX, coordY - 1);
				if (CanConnectTo(part2, basePart, BasePart.Direction.Right))
				{
					if (part2.GetCustomJointConnectionDirection() == BasePart.JointConnectionDirection.Right)
					{
						AddCustomConnectionBetweenParts(part2, basePart);
					}
					else if (basePart.GetCustomJointConnectionDirection() == BasePart.JointConnectionDirection.Left)
					{
						AddCustomConnectionBetweenParts(basePart, part2);
					}
					else
					{
						AddFixedJointBetweenParts(part2, basePart);
					}
				}
				if (CanConnectTo(part2, basePart2, BasePart.Direction.Down))
				{
					if (part2.GetCustomJointConnectionDirection() == BasePart.JointConnectionDirection.Down)
					{
						AddCustomConnectionBetweenParts(part2, basePart2);
					}
					else if (basePart2.GetCustomJointConnectionDirection() == BasePart.JointConnectionDirection.Up)
					{
						AddCustomConnectionBetweenParts(basePart2, part2);
					}
					else
					{
						AddFixedJointBetweenParts(part2, basePart2);
					}
				}
				if (part2.m_partType == BasePart.PartType.Rope)
				{
					BasePart basePart3 = FindPartAt(coordX - 1, coordY);
					BasePart basePart4 = FindPartAt(coordX + 1, coordY);
					if ((bool)basePart3)
					{
						basePart3.EnsureRigidbody();
					}
					if ((bool)basePart4)
					{
						basePart4.EnsureRigidbody();
					}
					part2.GetComponent<Rope>().Create(basePart3, basePart4);
				}
				if (part2.m_partType == BasePart.PartType.Spring)
				{
					BasePart.Direction direction = BasePart.ConvertDirection(part2.GetCustomJointConnectionDirection());
					BasePart partAt = GetPartAt(part2, direction);
					if (!partAt || !CanConnectTo(part2, partAt, direction))
					{
						part2.GetComponent<Spring>().CreateSpringBody(direction);
					}
				}
			}
			if (part2.m_partType == BasePart.PartType.Pig && (bool)WPFMonoBehaviour.levelManager && WPFMonoBehaviour.levelManager.m_disablePigCollisions && part2.m_enclosedInto != null)
			{
				part2.gameObject.layer = LayerMask.NameToLayer("NonCollidingPart");
			}
			if (part2.m_partType != BasePart.PartType.KingPig)
			{
				continue;
			}
			for (int i = 0; i <= 1; i++)
			{
				for (int j = -2; j <= 2; j += 4)
				{
					BasePart basePart5 = FindPartAt(coordX + j, coordY + i);
					if ((bool)basePart5 && (basePart5.m_partType == BasePart.PartType.Wings || basePart5.m_partType == BasePart.PartType.MetalWing))
					{
						basePart5.EnsureRigidbody();
						if ((bool)basePart5.GetComponent<Collider>())
						{
							Physics.IgnoreCollision(part2.GetComponent<Collider>(), basePart5.GetComponent<Collider>());
						}
					}
				}
				for (int k = -1; k <= 1; k++)
				{
					if (k == 0 && i == 0)
					{
						continue;
					}
					BasePart basePart6 = FindPartAt(coordX + k, coordY + i);
					if (!basePart6)
					{
						continue;
					}
					if (basePart6.m_partType == BasePart.PartType.WoodenFrame || basePart6.m_partType == BasePart.PartType.MetalFrame)
					{
						basePart6.EnsureRigidbody();
						if (i == 0)
						{
							AddFixedJointBetweenParts(basePart6, part2);
						}
						else
						{
							Physics.IgnoreCollision(part2.GetComponent<Collider>(), basePart6.GetComponent<Collider>());
						}
					}
					else if (basePart6.m_partType == BasePart.PartType.Spring)
					{
						basePart6.EnsureRigidbody();
						if ((bool)basePart6.GetComponent<Collider>())
						{
							Physics.IgnoreCollision(part2.GetComponent<Collider>(), basePart6.GetComponent<Collider>());
						}
					}
				}
			}
		}
		FindConnectedComponents();
		CalculatePartPlacement();
		for (int l = 0; l < m_parts.Count; l++)
		{
			BasePart basePart7 = m_parts[l];
			basePart7.enabled = true;
			basePart7.contraption = this;
			basePart7.PreInitialize();
			basePart7.Initialize();
		}
		InitializeEngines();
		CountActiveParts();
		for (int m = 0; m < m_parts.Count; m++)
		{
			BasePart basePart8 = m_parts[m];
			basePart8.PostInitialize();
		}
		int[] value;
		if (m_oneShotPartAmount.TryGetValue(BasePart.PartType.Balloon, out value))
		{
			int num = value[0];
			if (num > 0)
			{
				for (int n = 0; n < m_parts.Count; n++)
				{
					BasePart basePart9 = m_parts[n];
					if (basePart9.m_partType == BasePart.PartType.Balloon)
					{
						basePart9.GetComponent<Balloon>().ConfigureExtraBalanceJoint(1f / (float)num);
					}
				}
			}
		}
		m_running = true;
	}

	public void SaveContraption()
	{
		GameProgress.SetInt(Application.loadedLevelName + "_contraption", 1);
		WPFPrefs.SaveContraptionDataset(Application.loadedLevelName, m_contraptionDataSet);
		GameProgress.Save();
	}

	private void AddCustomConnectionBetweenParts(BasePart part1, BasePart part2)
	{
		part2.EnsureRigidbody();
		Joint joint = part1.CustomConnectToPart(part2);
		AddJointToMap(part1, part2, joint);
	}

	public void CalculatePartPlacement()
	{
		m_partPlacements.Clear();
		for (int i = 0; i < m_parts.Count; i++)
		{
			BasePart part = m_parts[i];
			AddPartPlacement(part);
		}
		foreach (PartPlacementInfo partPlacement in m_partPlacements)
		{
			partPlacement.averagePosition /= (float)partPlacement.count;
		}
		m_partPlacements.Sort(new PartOrder());
	}

	private void AddPartPlacement(BasePart part)
	{
		BasePart.PartType partType = BasePart.BaseType(part.m_partType);
		foreach (PartPlacementInfo partPlacement in m_partPlacements)
		{
			if (partPlacement.partType == partType && partPlacement.direction == part.EffectDirection())
			{
				partPlacement.averagePosition += part.transform.position;
				partPlacement.count++;
				return;
			}
		}
		m_partPlacements.Add(new PartPlacementInfo(part.m_partType, part.EffectDirection(), part.transform.position, 1));
	}

	public int EnginePoweredPartTypeCount()
	{
		int num = 0;
		foreach (KeyValuePair<BasePart.PartType, int[]> item in m_poweredPartAmount)
		{
			if (item.Key != BasePart.PartType.Bellows)
			{
				if (item.Value[0] > 0)
				{
					num++;
				}
				if (item.Value[1] > 0)
				{
					num++;
				}
				if (item.Value[2] > 0)
				{
					num++;
				}
				if (item.Value[3] > 0)
				{
					num++;
				}
			}
		}
		return num;
	}

	public void CountActiveParts()
	{
		foreach (KeyValuePair<BasePart.PartType, int[]> item in m_poweredPartAmount)
		{
			item.Value[0] = 0;
			item.Value[1] = 0;
			item.Value[2] = 0;
			item.Value[3] = 0;
		}
		foreach (KeyValuePair<BasePart.PartType, int[]> item2 in m_pushablePartAmount)
		{
			item2.Value[0] = 0;
			item2.Value[1] = 0;
			item2.Value[2] = 0;
			item2.Value[3] = 0;
		}
		foreach (BasePart part in m_parts)
		{
			if (part.CanBeEnabled() && !part.IsEngine())
			{
				if (part.IsPowered())
				{
					ChangePoweredPartAmount(part.m_partType, part.EffectDirection(), 1);
				}
				else
				{
					ChangePushablePartAmount(part.m_partType, part.EffectDirection(), 1);
				}
			}
		}
	}

	public bool SomePoweredPartsEnabled()
	{
		foreach (BasePart part in m_parts)
		{
			if (part.m_powerConsumption > 0f && part.IsEnabled())
			{
				return true;
			}
		}
		return false;
	}

	public bool AllPoweredPartsEnabled()
	{
		bool result = false;
		foreach (BasePart part in m_parts)
		{
			if (part.m_powerConsumption > 0f)
			{
				if (!part.IsEnabled())
				{
					return false;
				}
				result = true;
			}
		}
		return result;
	}

	public bool AllPartsEnabled(BasePart.PartType type, BasePart.Direction direction)
	{
		bool result = false;
		foreach (BasePart part in m_parts)
		{
			if (part.m_partType == type && part.EffectDirection() == direction)
			{
				if (!part.IsEnabled())
				{
					return false;
				}
				result = true;
			}
		}
		return result;
	}

	public void ActivatePartType(BasePart.PartType type, BasePart.Direction direction)
	{
		switch (type)
		{
		case BasePart.PartType.Engine:
			ActivateAllPoweredParts();
			return;
		case BasePart.PartType.Balloon:
		case BasePart.PartType.Sandbag:
			ActivateOnePartOfType(type);
			return;
		}
		int num = 0;
		foreach (BasePart part in m_parts)
		{
			if (part.m_partType == type && part.EffectDirection() == direction && !part.IsEnabled() && part.HasOnOffToggle())
			{
				num++;
			}
		}
		bool flag = num > 0;
		foreach (BasePart part2 in m_parts)
		{
			if (part2.m_partType != type || part2.EffectDirection() != direction)
			{
				continue;
			}
			if (flag)
			{
				if (!part2.IsEnabled() || !part2.HasOnOffToggle())
				{
					part2.ProcessTouch();
				}
			}
			else if (part2.IsEnabled() || !part2.HasOnOffToggle())
			{
				part2.ProcessTouch();
			}
		}
	}

	private void ActivateOnePartOfType(BasePart.PartType type)
	{
		Vector3 zero = Vector3.zero;
		float num = 0f;
		foreach (BasePart part in m_parts)
		{
			if (part.ConnectedComponent == m_pig.ConnectedComponent)
			{
				zero += part.transform.position;
				num += 1f;
			}
		}
		if (num > 0f)
		{
			zero /= num;
		}
		float num2 = 0f;
		BasePart basePart = null;
		foreach (BasePart part2 in m_parts)
		{
			if ((bool)part2 && part2.m_partType == type && (part2.m_partType != BasePart.PartType.Sandbag || part2.GetComponent<Sandbag>().IsAttached()))
			{
				float num3 = Vector3.Distance(part2.transform.position, zero);
				if (num3 > num2)
				{
					num2 = num3;
					basePart = part2;
				}
			}
		}
		if ((bool)basePart)
		{
			basePart.ProcessTouch();
		}
	}

	public void ActivateAllPoweredParts()
	{
		int num = 0;
		foreach (BasePart part in m_parts)
		{
			if (part.CanBeEnabled() && !part.IsEnabled() && part.IsPowered())
			{
				num++;
			}
		}
		bool flag = num > 0;
		foreach (BasePart part2 in m_parts)
		{
			if (!part2.CanBeEnabled() || !part2.IsPowered())
			{
				continue;
			}
			if (flag)
			{
				if (!part2.IsEnabled())
				{
					part2.SetEnabled(true);
				}
			}
			else if (part2.IsEnabled())
			{
				part2.SetEnabled(false);
			}
		}
	}

	public int ActivateAllPoweredParts(int connectedComponent)
	{
		int num = 0;
		int num2 = 0;
		foreach (BasePart part in m_parts)
		{
			if (part.ConnectedComponent == connectedComponent && part.CanBeEnabled() && !part.IsEnabled() && part.IsPowered())
			{
				num2++;
			}
		}
		bool flag = num2 > 0;
		foreach (BasePart part2 in m_parts)
		{
			if (part2.ConnectedComponent != connectedComponent || !part2.CanBeEnabled() || !part2.IsPowered())
			{
				continue;
			}
			num++;
			if (flag)
			{
				if (!part2.IsEnabled())
				{
					part2.SetEnabled(true);
				}
			}
			else if (part2.IsEnabled())
			{
				part2.SetEnabled(false);
			}
		}
		return num;
	}

	public void UpdateEngineStates(int connectedComponent)
	{
		int num = 0;
		for (int i = 0; i < m_parts.Count; i++)
		{
			BasePart basePart = m_parts[i];
			if (basePart.ConnectedComponent == connectedComponent && basePart.CanBeEnabled() && basePart.IsEnabled() && basePart.IsPowered())
			{
				num++;
			}
		}
		EnableComponentEngines(connectedComponent, num > 0);
	}

	private void EnableComponentEngines(int connectedComponent, bool enable)
	{
		for (int i = 0; i < m_parts.Count; i++)
		{
			BasePart basePart = m_parts[i];
			if (basePart.IsEngine() && basePart.ConnectedComponent == connectedComponent)
			{
				basePart.SetEnabled(enable);
			}
		}
	}

	public void FindConnectedComponents()
	{
		StartConnectedComponentSearch();
		while (!m_componentSearchState.finished)
		{
			FindConnectedComponents(m_componentSearchState);
		}
		m_componentSearchState = null;
	}

	private void StartConnectedComponentSearch()
	{
		m_componentSearchState = new ComponentSeardhState();
		foreach (BasePart part in m_parts)
		{
			part.SearchConnectedComponent = -1;
			m_componentSearchState.parts.Enqueue(part);
		}
	}

	private void FindConnectedComponents(ComponentSeardhState state)
	{
		state.updatesUsed++;
		if (state.connectedParts.Count == 0)
		{
			while (state.parts.Count > 0)
			{
				BasePart basePart = state.parts.Dequeue();
				if ((bool)basePart && basePart.SearchConnectedComponent == -1)
				{
					state.component = default(ConnectedComponent);
					state.component.motorWheels = new List<MotorWheel>();
					state.component.poweredUmbrellas = new List<PoweredUmbrella>();
					state.connectedParts = new Queue<BasePart>();
					state.connectedParts.Enqueue(basePart);
					break;
				}
			}
		}
		if (state.connectedParts.Count > 0)
		{
			int num = 0;
			while (state.connectedParts.Count > 0)
			{
				BasePart basePart2 = state.connectedParts.Dequeue();
				if ((bool)basePart2 && basePart2.SearchConnectedComponent == -1)
				{
					basePart2.SearchConnectedComponent = state.componentIndex;
					state.component.partCount++;
					state.component.powerConsumption += basePart2.m_powerConsumption;
					state.component.enginePower += basePart2.m_enginePower;
					if (basePart2.m_enginePower > 0f && basePart2.m_partType != BasePart.PartType.Pig)
					{
						state.component.hasEngine = true;
					}
					if (basePart2.m_partType == BasePart.PartType.MotorWheel)
					{
						state.component.motorWheels.Add(basePart2.GetComponent<MotorWheel>());
					}
					if (basePart2.m_partType == BasePart.PartType.PoweredUmbrella)
					{
						state.component.poweredUmbrellas.Add(basePart2.GetComponent<PoweredUmbrella>());
					}
					FindConnectedParts(basePart2, ref state.connectedParts);
					num++;
					if (num > 5 && state.connectedParts.Count > 0)
					{
						return;
					}
				}
			}
			state.connectedComponents.Add(state.component);
			state.componentIndex = state.connectedComponents.Count;
		}
		if (state.parts.Count != 0 || state.connectedParts.Count != 0)
		{
			return;
		}
		foreach (BasePart part in m_parts)
		{
			if ((bool)part)
			{
				part.ConnectedComponent = part.SearchConnectedComponent;
				m_connectedComponents = state.connectedComponents;
			}
		}
		state.finished = true;
	}

	public void FindConnectedParts(BasePart part, ref Queue<BasePart> parts)
	{
		foreach (JointConnectionStruct item in m_jointMap)
		{
			if (item.partA == part && item.partB.SearchConnectedComponent == -1)
			{
				parts.Enqueue(item.partB);
			}
			else if (item.partB == part && item.partA.SearchConnectedComponent == -1)
			{
				parts.Enqueue(item.partA);
			}
		}
		if (part.enclosedPart != null && part.enclosedPart.SearchConnectedComponent == -1)
		{
			parts.Enqueue(part.enclosedPart);
		}
		if (part.enclosedInto != null && part.enclosedInto.SearchConnectedComponent == -1)
		{
			parts.Enqueue(part.enclosedInto);
		}
	}

	private void InitializeEngines()
	{
		foreach (BasePart part in m_parts)
		{
			part.InitializeEngine();
		}
	}

	public void StopContraption()
	{
		for (int i = 0; i < m_parts.Count; i++)
		{
			BasePart basePart = m_parts[i];
			if ((bool)basePart && (bool)basePart.gameObject)
			{
				UnityEngine.Object.Destroy(m_parts[i].gameObject);
			}
		}
		m_running = false;
	}

	public float GetJointConnectionStrength(BasePart.JointConnectionStrength strength)
	{
		switch (strength)
		{
		case BasePart.JointConnectionStrength.Weak:
			return WPFMonoBehaviour.gameData.m_jointConnectionStrengthWeak;
		case BasePart.JointConnectionStrength.Normal:
			return WPFMonoBehaviour.gameData.m_jointConnectionStrengthNormal;
		case BasePart.JointConnectionStrength.High:
			return WPFMonoBehaviour.gameData.m_jointConnectionStrengthHigh;
		case BasePart.JointConnectionStrength.Extreme:
			return WPFMonoBehaviour.gameData.m_jointConnectionStrengthExtreme;
		default:
			return 0f;
		}
	}

	private void AddFixedJointBetweenParts(BasePart part, BasePart rightNeighbour)
	{
		rightNeighbour.EnsureRigidbody();
		FixedJoint fixedJoint = part.gameObject.AddComponent<FixedJoint>();
		fixedJoint.connectedBody = rightNeighbour.gameObject.GetComponent<Rigidbody>();
		float jointConnectionStrength = GetJointConnectionStrength(part.GetJointConnectionStrength());
		float jointConnectionStrength2 = GetJointConnectionStrength(rightNeighbour.GetJointConnectionStrength());
		float breakForce = jointConnectionStrength + jointConnectionStrength2;
		fixedJoint.breakForce = breakForce;
		AddJointToMap(part, rightNeighbour, fixedJoint);
	}

	public void MoveOnGrid(int dx, int dy)
	{
		m_partMap.Clear();
		foreach (BasePart part in m_parts)
		{
			int num = part.m_coordX + dx;
			int num2 = part.m_coordY + dy;
			if (num >= WPFMonoBehaviour.levelManager.gridXmin && num <= WPFMonoBehaviour.levelManager.gridXmax && num2 >= 0 && num2 < WPFMonoBehaviour.levelManager.gridHeight)
			{
				SetPartPos(num, num2, part);
			}
		}
	}

	public bool CanMoveOnGrid(int dx, int dy)
	{
		if (m_parts.Count == 0)
		{
			return false;
		}
		foreach (BasePart part in m_parts)
		{
			int num = part.m_coordX + dx;
			int num2 = part.m_coordY + dy;
			if (num < WPFMonoBehaviour.levelManager.gridXmin || num > WPFMonoBehaviour.levelManager.gridXmax || num2 < 0 || num2 >= WPFMonoBehaviour.levelManager.gridHeight)
			{
				return false;
			}
		}
		return true;
	}

	private void SetPartPos(int x, int y, BasePart part)
	{
		if ((bool)part)
		{
			part.m_coordX = x;
			part.m_coordY = y;
			float num = (0f - (float)(x + 2 * y)) / 100000f;
			part.transform.localPosition = Vector3.right * x + Vector3.up * y + Vector3.forward * (-0.1f + part.m_ZOffset + num);
		}
		if (part == null || part.m_enclosedInto == null)
		{
			int key = x + y * 1000;
			m_partMap[key] = part;
		}
	}

	public BasePart FindPartAt(int x, int y)
	{
		int key = x + y * 1000;
		BasePart value = null;
		m_partMap.TryGetValue(key, out value);
		return value;
	}

	public BasePart FindPartOfType(BasePart.PartType type)
	{
		foreach (BasePart part in m_parts)
		{
			if (part.m_partType == type)
			{
				return part;
			}
		}
		return null;
	}

	public List<BasePart> FindNeighbours(int x, int y)
	{
		List<BasePart> list = new List<BasePart>();
		BasePart basePart = FindPartAt(x - 1, y);
		if ((bool)basePart)
		{
			list.Add(basePart);
		}
		basePart = FindPartAt(x + 1, y);
		if ((bool)basePart)
		{
			list.Add(basePart);
		}
		basePart = FindPartAt(x, y - 1);
		if ((bool)basePart)
		{
			list.Add(basePart);
		}
		basePart = FindPartAt(x, y + 1);
		if ((bool)basePart)
		{
			list.Add(basePart);
		}
		return list;
	}

	public void RefreshNeighbours(int x, int y)
	{
		BasePart basePart = FindPartAt(x - 1, y);
		if (basePart != null)
		{
			basePart.OnChangeConnections();
		}
		BasePart basePart2 = FindPartAt(x + 1, y);
		if (basePart2 != null)
		{
			basePart2.OnChangeConnections();
		}
		BasePart basePart3 = FindPartAt(x, y + 1);
		if (basePart3 != null)
		{
			basePart3.OnChangeConnections();
		}
		BasePart basePart4 = FindPartAt(x, y - 1);
		if (basePart4 != null)
		{
			basePart4.OnChangeConnections();
		}
	}

	public void RefreshNeighboursVisual(int x, int y)
	{
		BasePart basePart = FindPartAt(x - 1, y);
		if (basePart != null)
		{
			basePart.ChangeVisualConnections();
		}
		BasePart basePart2 = FindPartAt(x + 1, y);
		if (basePart2 != null)
		{
			basePart2.ChangeVisualConnections();
		}
		BasePart basePart3 = FindPartAt(x, y + 1);
		if (basePart3 != null)
		{
			basePart3.ChangeVisualConnections();
		}
		BasePart basePart4 = FindPartAt(x, y - 1);
		if (basePart4 != null)
		{
			basePart4.ChangeVisualConnections();
		}
	}

	public BasePart SetPartAt(int x, int y, BasePart newPart)
	{
		return SetPartAt(x, y, newPart, true);
	}

	public BasePart SetPartAt(int x, int y, BasePart newPart, bool refreshNeighbours)
	{
		BasePart basePart = RemovePartsAt(x, y);
		newPart.transform.parent = base.transform;
		newPart.contraption = this;
		SetPartPos(x, y, newPart);
		m_parts.Add(newPart);
		if ((bool)basePart && basePart.CanBeEnclosed() && newPart.CanEncloseParts())
		{
			newPart.enclosedPart = basePart;
			m_parts.Add(basePart);
		}
		else
		{
			if (newPart.CanBeEnclosed() && (bool)basePart && basePart.CanEncloseParts())
			{
				BasePart enclosedPart = basePart.enclosedPart;
				basePart.enclosedPart = newPart;
				m_parts.Add(basePart);
				SetPartPos(x, y, basePart);
				if (refreshNeighbours)
				{
					basePart.OnChangeConnections();
					RefreshNeighbours(x, y);
				}
				return enclosedPart;
			}
			if ((bool)basePart && (bool)basePart.enclosedPart && newPart.CanEncloseParts())
			{
				newPart.enclosedPart = basePart.enclosedPart;
				basePart.enclosedPart = null;
				m_parts.Add(newPart.enclosedPart);
				if (refreshNeighbours)
				{
					newPart.OnChangeConnections();
					RefreshNeighbours(x, y);
				}
				return basePart;
			}
			if ((bool)basePart)
			{
				if (refreshNeighbours)
				{
					newPart.OnChangeConnections();
					RefreshNeighbours(x, y);
				}
				return basePart;
			}
		}
		if (refreshNeighbours)
		{
			newPart.OnChangeConnections();
			RefreshNeighbours(x, y);
		}
		return null;
	}

	public BasePart RemovePartsAt(int x, int y)
	{
		BasePart basePart = FindPartAt(x, y);
		SetPartPos(x, y, null);
		if ((bool)basePart)
		{
			m_parts.Remove(basePart);
			if ((bool)basePart.enclosedPart)
			{
				m_parts.Remove(basePart.enclosedPart);
			}
		}
		return basePart;
	}

	public BasePart RemovePartAt(int x, int y)
	{
		BasePart basePart = FindPartAt(x, y);
		if ((bool)basePart)
		{
			BasePart enclosedPart = basePart.enclosedPart;
			if ((bool)enclosedPart)
			{
				basePart.enclosedPart = null;
				enclosedPart.enclosedInto = null;
				SetPartPos(x, y, basePart);
				basePart = enclosedPart;
			}
			else
			{
				SetPartPos(x, y, null);
			}
		}
		if ((bool)basePart)
		{
			m_parts.Remove(basePart);
		}
		RefreshNeighbours(x, y);
		return basePart;
	}

	public void RemovePart(BasePart part)
	{
		if ((bool)part)
		{
			RemovePartAt(part.m_coordX, part.m_coordY);
			if ((bool)part)
			{
				m_parts.Remove(part);
			}
		}
	}

	public void RemoveAllDynamicParts()
	{
		List<BasePart> list = new List<BasePart>();
		foreach (BasePart part in m_parts)
		{
			if (!part.m_static)
			{
				Debug.Log(string.Concat("removing ", part.m_partType, " at ", part.m_coordX, ", ", part.m_coordY));
				UnityEngine.Object.Destroy(part.gameObject);
				list.Add(part);
			}
		}
		foreach (BasePart item in list)
		{
			RemovePartAt(item.m_coordX, item.m_coordY);
		}
	}

	public void AutoAlign(BasePart part)
	{
		if (part.m_autoAlign == BasePart.AutoAlignType.None)
		{
			return;
		}
		if (part.m_autoAlign == BasePart.AutoAlignType.Rotate)
		{
			BasePart.JointConnectionDirection jointConnectionDirection = BasePart.JointConnectionDirection.Any;
			if (IsConnectionSourceTo(part.m_coordX - 1, part.m_coordY, BasePart.Direction.Right))
			{
				jointConnectionDirection = BasePart.JointConnectionDirection.Left;
			}
			else if (IsConnectionSourceTo(part.m_coordX + 1, part.m_coordY, BasePart.Direction.Left))
			{
				jointConnectionDirection = BasePart.JointConnectionDirection.Right;
			}
			else if (IsConnectionSourceTo(part.m_coordX, part.m_coordY + 1, BasePart.Direction.Down))
			{
				jointConnectionDirection = BasePart.JointConnectionDirection.Up;
			}
			else if (IsConnectionSourceTo(part.m_coordX, part.m_coordY - 1, BasePart.Direction.Up))
			{
				jointConnectionDirection = BasePart.JointConnectionDirection.Down;
			}
			if (jointConnectionDirection != 0)
			{
				BasePart.GridRotation rotation = part.AutoAlignRotation(jointConnectionDirection);
				part.SetRotation(rotation);
				part.OnChangeConnections();
				RefreshNeighboursVisual(part.m_coordX, part.m_coordY);
			}
		}
		else if (part.m_autoAlign == BasePart.AutoAlignType.FlipVertically)
		{
			if (IsChassisPart(part.m_coordX + 1, part.m_coordY))
			{
				part.SetFlipped(false);
			}
			else if (IsChassisPart(part.m_coordX - 1, part.m_coordY))
			{
				part.SetFlipped(true);
			}
		}
	}

	public bool Flip(BasePart part)
	{
		bool result = false;
		if (part.m_autoAlign == BasePart.AutoAlignType.FlipVertically)
		{
			for (int i = 0; i < 3; i++)
			{
				part.SetFlipped(!part.IsFlipped());
				result = i != 1;
				if (part.m_jointConnectionDirection == BasePart.JointConnectionDirection.Any || CanConnectTo(part, BasePart.ConvertDirection(part.GetJointConnectionDirection())))
				{
					break;
				}
			}
		}
		else if (part.m_autoAlign == BasePart.AutoAlignType.Rotate)
		{
			if (part.m_jointConnectionDirection == BasePart.JointConnectionDirection.Any)
			{
				part.RotateClockwise();
				part.OnChangeConnections();
				RefreshNeighboursVisual(part.m_coordX, part.m_coordY);
				result = true;
			}
			else
			{
				for (int j = 0; j < 5; j++)
				{
					part.RotateClockwise();
					result = j != 3;
					if (CanConnectTo(part, part.GetJointConnectionDirection()))
					{
						break;
					}
				}
				RefreshNeighboursVisual(part.m_coordX, part.m_coordY);
			}
		}
		return result;
	}

	private bool IsChassisPart(int coordX, int coordY)
	{
		BasePart basePart = FindPartAt(coordX, coordY);
		if (!basePart)
		{
			return false;
		}
		return basePart.IsPartOfChassis();
	}

	private bool IsConnectionSourceTo(int coordX, int coordY, BasePart.Direction direction)
	{
		BasePart basePart = FindPartAt(coordX, coordY);
		if (!basePart)
		{
			return false;
		}
		return basePart.m_jointConnectionType == BasePart.JointConnectionType.Source && basePart.CanConnectTo(direction);
	}

	public bool CanPlaceSpecificPartAt(int coordX, int coordY, BasePart newPart)
	{
		BasePart basePart = FindPartAt(coordX, coordY);
		if ((bool)basePart && (bool)basePart.enclosedPart)
		{
			basePart = basePart.enclosedPart;
		}
		if (!basePart)
		{
			return true;
		}
		if (basePart.m_static && (!basePart.CanEncloseParts() || !newPart.CanBeEnclosed()))
		{
			return false;
		}
		if (basePart.CanBeEnclosed() && newPart.CanEncloseParts())
		{
			return true;
		}
		return true;
	}

	public void Update()
	{
		if (!m_running)
		{
			return;
		}
		if (m_componentSearchState != null)
		{
			FindConnectedComponents(m_componentSearchState);
			if (m_componentSearchState.finished)
			{
				m_componentSearchState = null;
				InitializeEngines();
				CountActiveParts();
			}
		}
		if ((bool)m_pig)
		{
			if (m_pig.GetComponent<Rigidbody>().velocity.magnitude < 0.2f)
			{
				m_stopTimer += Time.deltaTime;
			}
			else
			{
				m_stopTimer = 0f;
			}
		}
		if (!WPFMonoBehaviour.gameData.m_useTouchControls)
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 mousePosition = Input.mousePosition;
				mousePosition.z = m_gameCamera.farClipPlane;
				Vector3 b = WPFMonoBehaviour.ScreenToZ0(mousePosition);
				foreach (BasePart part in m_parts)
				{
					if ((bool)part)
					{
						Vector3 position = part.transform.position;
						position.z = 0f;
						if (Vector3.Distance(position, b) <= part.m_interactiveRadius && !part.enclosedPart && part.gameObject.layer != m_droppedSandbagLayer)
						{
							part.ProcessTouch();
							break;
						}
					}
				}
			}
			if (Input.GetKey(KeyCode.R))
			{
				Rocket[] componentsInChildren = base.gameObject.GetComponentsInChildren<Rocket>();
				Rocket[] array = componentsInChildren;
				foreach (Rocket rocket in array)
				{
					rocket.ProcessTouch();
				}
			}
			return;
		}
		Touch[] touches = Input.touches;
		for (int j = 0; j < touches.Length; j++)
		{
			Touch touch = touches[j];
			if (touch.phase != 0)
			{
				continue;
			}
			Vector3 pos = touch.position;
			pos.z = m_gameCamera.farClipPlane;
			Vector3 b2 = WPFMonoBehaviour.ScreenToZ0(pos);
			foreach (BasePart part2 in m_parts)
			{
				if ((bool)part2 && Vector3.Distance(part2.transform.position, b2) <= part2.m_interactiveRadius)
				{
					part2.ProcessTouch();
					if ((bool)part2.enclosedPart)
					{
						part2.enclosedPart.ProcessTouch();
					}
					break;
				}
			}
		}
	}

	public void SetVisible(bool visible)
	{
		InternalSetVisible(base.transform, visible);
	}

	private void InternalSetVisible(Transform t, bool enable)
	{
		if (enable)
		{
			t.gameObject.active = enable;
		}
		foreach (Transform item in t)
		{
			InternalSetVisible(item, enable);
		}
		if (!enable)
		{
			t.gameObject.active = enable;
		}
	}

	public void RefreshConnections()
	{
		foreach (BasePart part in m_parts)
		{
			part.OnChangeConnections();
		}
	}

	private void CopyActiveStates(GameObject original, GameObject clone)
	{
		clone.active = original.active;
		for (int i = 0; i < original.transform.childCount; i++)
		{
			CopyActiveStates(original.transform.GetChild(i).gameObject, clone.transform.GetChild(i).gameObject);
		}
	}

	public Contraption Clone()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(base.gameObject) as GameObject;
		CopyActiveStates(base.gameObject, gameObject);
		return gameObject.GetComponent<Contraption>();
	}

	public bool ConsiderCollided()
	{
		return Time.time - m_timeLastCollided < 0.3f;
	}

	public void Freeze()
	{
		m_running = false;
		Rigidbody[] componentsInChildren = base.transform.GetComponentsInChildren<Rigidbody>();
		m_rigidbodyVelocityMap.Clear();
		Rigidbody[] array = componentsInChildren;
		foreach (Rigidbody rigidbody in array)
		{
			m_rigidbodyVelocityMap[rigidbody] = rigidbody.velocity;
			rigidbody.isKinematic = true;
		}
	}

	public void Unfreeze()
	{
		Rigidbody[] componentsInChildren = base.transform.GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array = componentsInChildren;
		foreach (Rigidbody rigidbody in array)
		{
			rigidbody.isKinematic = false;
			rigidbody.velocity = m_rigidbodyVelocityMap[rigidbody];
		}
		m_running = true;
	}

	public bool ValidateContraption()
	{
		bool flag = false;
		List<BasePart> list = new List<BasePart>();
		List<BasePart> list2 = new List<BasePart>();
		bool flag2 = true;
		foreach (BasePart part in m_parts)
		{
			if (part.m_partType == BasePart.PartType.Pig)
			{
				flag = true;
			}
			if (part.IsPartOfChassis())
			{
				if (list.Count == 0)
				{
					list.Add(part);
				}
				list2.Add(part);
			}
			part.valid = true;
			if (!part.ValidatePart())
			{
				part.valid = false;
				flag2 = false;
			}
		}
		bool flag3 = false;
		bool flag4 = true;
		do
		{
			flag3 = false;
			flag4 = true;
			foreach (BasePart item in list2)
			{
				if (list.Contains(item))
				{
					continue;
				}
				List<BasePart> list3 = FindNeighbours(item.m_coordX, item.m_coordY);
				foreach (BasePart item2 in list3)
				{
					if ((bool)item2 && list.Contains(item2))
					{
						flag3 = true;
						list.Add(item);
						break;
					}
				}
				flag4 = false;
			}
		}
		while (flag3);
		if (WPFMonoBehaviour.levelManager.RequireConnectedContraption && !flag4)
		{
			foreach (BasePart item3 in list2)
			{
				if (item3.valid)
				{
					item3.valid = false;
				}
			}
			return false;
		}
		if (!flag2)
		{
			return false;
		}
		if (!flag)
		{
			return false;
		}
		return true;
	}

	public void DestroyAllJoints()
	{
		foreach (JointConnectionStruct item in m_jointMap)
		{
			if ((bool)item.joint)
			{
				UnityEngine.Object.Destroy(item.joint);
			}
		}
		m_jointMap.Clear();
	}

	public IEnumerator OnJointDetached()
	{
		m_broken = true;
		yield return 0;
		List<JointConnectionStruct> j2r = new List<JointConnectionStruct>();
		foreach (JointConnectionStruct jcs2 in m_jointMap)
		{
			if (jcs2.joint == null || jcs2.partA == null || jcs2.partB == null)
			{
				if ((bool)jcs2.partA && (bool)jcs2.partA.enclosedInto)
				{
					jcs2.partA.enclosedInto.enclosedPart = null;
					jcs2.partA.m_enclosedInto = null;
					jcs2.partA.OnDetach();
				}
				if ((bool)jcs2.partB && (bool)jcs2.partB.enclosedInto)
				{
					jcs2.partB.enclosedInto.enclosedPart = null;
					jcs2.partB.m_enclosedInto = null;
					jcs2.partB.OnDetach();
				}
				j2r.Add(jcs2);
			}
		}
		foreach (JointConnectionStruct jcs in j2r)
		{
			m_jointMap.Remove(jcs);
		}
		StartConnectedComponentSearch();
	}

	public void AddJointToMap(BasePart endJointA, BasePart endJointB, Joint joint)
	{
		JointConnectionStruct item = default(JointConnectionStruct);
		item.partA = endJointA;
		item.partB = endJointB;
		item.joint = joint;
		m_jointMap.Add(item);
	}

	public bool HasComponentEngine(int componentIndex)
	{
		if (componentIndex >= 0 && componentIndex < m_connectedComponents.Count)
		{
			return m_connectedComponents[componentIndex].hasEngine;
		}
		return false;
	}

	public int ComponentPartCount(int componentIndex)
	{
		if (componentIndex >= 0 && componentIndex < m_connectedComponents.Count)
		{
			return m_connectedComponents[componentIndex].partCount;
		}
		return 0;
	}

	public bool HasPoweredPartsRunning(int componentIndex)
	{
		for (int i = 0; i < m_parts.Count; i++)
		{
			BasePart basePart = m_parts[i];
			if (basePart.ConnectedComponent == componentIndex && basePart.IsEnabled() && basePart.IsPowered())
			{
				return true;
			}
		}
		return false;
	}
}
