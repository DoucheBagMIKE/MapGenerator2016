using UnityEngine;
using System.IO;
using System.Xml.Serialization;

public static class Serialization<T> where T : class
{

    public static T DeserializeFromXmlFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            Debug.Log(string.Format("No file in this directory named {0}", fileName));
            return null;
        }

        XmlSerializer Serializer = new XmlSerializer(typeof(T));

        using (Stream stream = File.OpenRead(fileName))
        {
            return (T)Serializer.Deserialize(stream);
        }
    }
}