using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable]
[XmlRoot("Configuration")]
public class Configuration
{
    [XmlElement("DatabaseList")]
    public List<string> DatabaseList { get; set; } = new List<string>();

    [XmlElement("Defeature")]
    public Defeature ConfigurableDefeature { get; set; }
}

public class Defeature
{
    [XmlElement("CoreReadToken")]
    public Token CoreReadToken { get; set; }

    [XmlElement("FinalToken")]
    public Token FinalToken { get; set; }

    [XmlElement("TrackingToken")]
    public Token TrackingToken { get; set; }
}

public class Token
{
    [XmlAttribute("PrefixOffset")]
    public string PrefixOffset { get; set; }

    [XmlElement("SQLSearchKey")]
    public string SQLSearchKey { 
      get => SQLSearchKeyList == null ? "" : string.Join(";", SQLSearchKeyList);
      set {
        SQLSearchKeyList = value.Split(';').Select(i => i.Trim()).ToList();
      }
    }

    [XmlIgnore]
    public List<string> SQLSearchKeyList { get; set; }
}

class Program
{
    static string xmlFilePath = "Configuration.xml";

    static void Main()
    {
        Configuration config;

        if (File.Exists(xmlFilePath))
        {
            // Deserialize existing XML file
            config = DeserializeXml(xmlFilePath);
            Console.WriteLine("XML file found. Deserialized content:");
        }
        else
        {
            // Create and serialize default XML
            config = new Configuration
            {
                DatabaseList = new List<string> { "Database1", "Database2" },
                ConfigurableDefeature = new Defeature
                {
                    CoreReadToken = new Token { SQLSearchKey = "DefaultCoreRead1; DefaultCoreRead2", PrefixOffset = "0" },
                    FinalToken = new Token { SQLSearchKey = "DefaultFinal", PrefixOffset = "1" },
                    TrackingToken = new Token { SQLSearchKey = "DefaultTracking1;DefaultTracking2;DefaultTracking3", PrefixOffset = "2" }
                }
            };

            SerializeXml(config, xmlFilePath);
            Console.WriteLine("XML file created with default values.");
        }

        PrintConfiguration(config);
    }

    static void SerializeXml(Configuration obj, string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            serializer.Serialize(writer, obj);
        }
    }

    static Configuration DeserializeXml(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
        using (StreamReader reader = new StreamReader(filePath))
        {
            return (Configuration)serializer.Deserialize(reader);
        }
    }

    static void PrintConfiguration(Configuration config)
    {
        Console.WriteLine("DatabaseList:");
        foreach (var db in config.DatabaseList)
        {
            Console.WriteLine($"  - {db}");
        }

        if (config.ConfigurableDefeature != null)
        {
            Console.WriteLine($"CoreReadToken: SQLSearchKey={config.ConfigurableDefeature.CoreReadToken?.SQLSearchKey}, PrefixOffset={config.ConfigurableDefeature.CoreReadToken?.PrefixOffset}");
            Console.WriteLine($"FinalToken: SQLSearchKey={config.ConfigurableDefeature.FinalToken?.SQLSearchKey}, PrefixOffset={config.ConfigurableDefeature.FinalToken?.PrefixOffset}");
            Console.WriteLine($"TrackingToken: SQLSearchKey={config.ConfigurableDefeature.TrackingToken?.SQLSearchKey}, PrefixOffset={config.ConfigurableDefeature.TrackingToken?.PrefixOffset}");
        }
    }
}
