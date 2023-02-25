using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot(ElementName = "osm")]
public class OSM
{
    [XmlElement(ElementName = "relation")]
    public RelationOSM relation;

	[XmlElement(ElementName = "node")]
	public NodeOSM node;

	[XmlElement(ElementName = "way")]
	public WayOSM way;
}

public class RelationOSM
{
	[XmlElement(ElementName = "tag")]
	public List<TagOSM> tags;
}

public class NodeOSM
{
	[XmlElement(ElementName = "tag")]
	public List<TagOSM> tags;
}

public class WayOSM
{
	[XmlElement(ElementName = "tag")]
	public List<TagOSM> tags;
}

public class TagOSM
{
	[XmlAttribute("k")]
	public string name;
	[XmlAttribute("v")]
	public string value;
}
