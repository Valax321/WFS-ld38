using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using UnityEngine;

public class NameGenerator : MonoBehaviour
{
    public static NameGenerator Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Name Generator", typeof(NameGenerator));
                instance = go.GetComponent<NameGenerator>();
            }

            return instance;
        }
    }

    private static NameGenerator instance;

    NameTable table;
    Dictionary<string, string[]> nameLookup;

    void Awake()
    {
        //Debug.Log("Making devxml");
        //table = new NameTable();
        //table.GroupTest = new List<StringCollection>();
        //table.GroupTest.Add(new StringCollection() { Key = "Test", Value = new string[] { "1", "2" } });
        //table.GroupTest.Add(new StringCollection() { Key = "Test2", Value = new string[] { "3", "4" } });
        //MakeXML();

        var ta = Resources.Load<TextAsset>("NameTable");
        if (ta != null)
        {
            table = XmlDeserialize<NameTable>(ta.text);
            nameLookup = table.GroupTestTyped;
        }
        else
        {
            Debug.LogError("NameTable.xml not found.");
        }
    }

    public string GetRandomName(string key)
    {
        if (table != null)
        {
            if (nameLookup.ContainsKey(key))
            {
                var collection = nameLookup[key];
                return collection[Random.Range(0, collection.Length)];
            }
        }

        return key;
    }

    T XmlDeserialize<T>(string xmls)
    {
        XmlSerializer xml = new XmlSerializer(typeof(T));
        object result;

        using (TextReader reader = new StringReader(xmls))
        {
            result = xml.Deserialize(reader);
            return (T)result;
        }
    }

    void MakeXML()
    {
        XmlSerializer xml = new XmlSerializer(typeof(NameTable));
        using (FileStream fs = new FileStream(Application.persistentDataPath + @"\devxml.xml", FileMode.Create, FileAccess.Write))
        {
            Debug.Log("Writing...");
            xml.Serialize(fs, table);
        }
    }
}

[System.Serializable]
public class StringCollection
{
    public string Key { get; set; }

    [XmlArray]
    [XmlArrayItem(ElementName = "Element")]
    public string[] Value { get; set; }
}

[XmlInclude(typeof(StringCollection))]
[System.Serializable]
public class NameTable
{
    public NameTable() { }

    [XmlArray]
    [XmlArrayItem(ElementName = "Element")]
    public List<StringCollection> GroupTest { get; set; }

    [XmlIgnore]
    public Dictionary<string, string[]> GroupTestTyped
    {
        get { return GroupTest.ToDictionary(x => x.Key, x => x.Value); }
        set { GroupTest = value.Select(x => new StringCollection() { Key = x.Key, Value = x.Value }).ToList(); }
    }
}