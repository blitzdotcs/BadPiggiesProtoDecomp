using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("ContraptionDataset")]
public class ContraptionDataset
{
	public class ContraptionDatasetUnit
	{
		[XmlAttribute("x")]
		public int x;

		[XmlAttribute("y")]
		public int y;

		[XmlAttribute("partType")]
		public int partType;

		[XmlAttribute("rot")]
		public int rot;

		[XmlAttribute("flipped")]
		public bool flipped;
	}

	[XmlArrayItem("ContraptionDatasetUnit")]
	[XmlArray("ContraptionDatasetList")]
	protected List<ContraptionDatasetUnit> m_contraptionDataSet = new List<ContraptionDatasetUnit>();

	public List<ContraptionDatasetUnit> ContraptionDatasetList
	{
		get
		{
			return m_contraptionDataSet;
		}
	}

	public void AddPart(int x, int y, int partType, BasePart.GridRotation rotation, bool flipped)
	{
		ContraptionDatasetUnit contraptionDatasetUnit = new ContraptionDatasetUnit();
		contraptionDatasetUnit.x = x;
		contraptionDatasetUnit.y = y;
		contraptionDatasetUnit.partType = partType;
		contraptionDatasetUnit.rot = (int)rotation;
		contraptionDatasetUnit.flipped = flipped;
		m_contraptionDataSet.Add(contraptionDatasetUnit);
	}
}
