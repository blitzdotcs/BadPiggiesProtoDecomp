using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("GhostPlayer")]
public class GhostPlayer
{
	public class Coord
	{
		[XmlAttribute("x")]
		public float x;

		[XmlAttribute("y")]
		public float y;

		[XmlAttribute("z")]
		public float z;
	}

	[XmlArrayItem("Coord")]
	[XmlArray("Coords")]
	protected List<Coord> Coords = new List<Coord>();

	public List<Coord> PositionData
	{
		get
		{
			return Coords;
		}
	}

	public void AddPosition(float x, float y, float z)
	{
		Coord coord = new Coord();
		coord.x = x;
		coord.y = y;
		coord.z = z;
		Coords.Add(coord);
	}
}
