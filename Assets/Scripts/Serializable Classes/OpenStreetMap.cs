using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot(ElementName = "osm")]
public class OpenStreetMap
{
    [XmlElement(ElementName = "relation")]
    public RelationOSM relation;

	[XmlElement(ElementName = "node")]
	public NodeOSM node;

	[XmlElement(ElementName = "way")]
	public WayOSM way;

	public OpenStreetMap()
    {
		this.relation = null;
		this.node = null;
		this.way = null;
	}

	public OpenStreetMap(string xmlText)
    {
		XmlSerializer serializer = new XmlSerializer(typeof(OpenStreetMap));
		using (StringReader reader = new StringReader(xmlText))
        {
			OpenStreetMap obj = (OpenStreetMap)serializer.Deserialize(reader);
			this.relation = obj.relation;
			this.node = obj.node;
			this.way = obj.way;
		}
	}

	public List<TagOSM> GetTags()
    {
		/*
			An OSM object only has either a Relation element, a Node one, or a Way one.

			Therefore, check if an element is null, and if not, check if its tag list is null.
			If it is, then check the next element, otherwise return the tag list.
			This tag list may be empty, but at least it's not null.

			If for some reason no tag list has been found, then return an empty list.
		*/
		return this.relation?.tags ?? this.node?.tags ?? this.way?.tags ?? new List<TagOSM>();
	}
}

public class RelationOSM
{
	[XmlElement(ElementName = "tag")]
	public List<TagOSM> tags;

	public RelationOSM()
    {
		this.tags = new List<TagOSM>();
    }
}

public class NodeOSM
{
	[XmlElement(ElementName = "tag")]
	public List<TagOSM> tags;

	public NodeOSM()
	{
		this.tags = new List<TagOSM>();
	}
}

public class WayOSM
{
	[XmlElement(ElementName = "tag")]
	public List<TagOSM> tags;

	public WayOSM()
	{
		this.tags = new List<TagOSM>();
	}
}

public class TagOSM
{
	[XmlAttribute("k")]
	public string name;
	[XmlAttribute("v")]
	public string value;

	public TagOSM()
    {
		this.name = null;
		this.value = null;
    }

	public TagOSM(string name, string value)
    {
		this.name = name;
		this.value = value;
	}
}
