
namespace Dal;
using DO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

static class XMLTools
{
    static readonly string s_xmlDir = GetXmlDirectory();
    // added by me to find or create the "xml" directory dynamically
    private static string GetXmlDirectory()
    {
        // from current directory and up find the "xml" folder
        string currentDir = Directory.GetCurrentDirectory();

        while (currentDir != null)
        {
            string xmlPath = Path.Combine(currentDir, "xml");
            if (Directory.Exists(xmlPath))
            {
                return xmlPath + Path.DirectorySeparatorChar;
            }

            // Go up one directory level
            DirectoryInfo parent = Directory.GetParent(currentDir);
            currentDir = parent?.FullName;
        }

        // if not found, create in the current directory
        string defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "xml");
        Directory.CreateDirectory(defaultPath);
        return defaultPath + Path.DirectorySeparatorChar;
    }


    #region SaveLoadWithXMLSerializer

    public static void SaveListToXMLSerializer<T>(List<T> list, string filePath)
    {
        try
        {
            FileStream file = new FileStream(s_xmlDir + filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            XmlSerializer x = new XmlSerializer(list.GetType());
            x.Serialize(file, list);
            file.Close();
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"fail to create xml file: , {ex.Message}");
        }
    }
    public static List<T> LoadListFromXMLSerializer<T>(string filePath)
    {
        try
        {
            if (File.Exists(s_xmlDir + filePath))
            {
                List<T> list;
                XmlSerializer x = new XmlSerializer(typeof(List<T>));
                using FileStream file = new FileStream(s_xmlDir + filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                list = (List<T>)x.Deserialize(file)!;
                file.Close();
                return list;
            }
            else
                return new List<T>();
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"fail to load xml file: {filePath}, {ex.Message}");
        }
    }
    #endregion

    #region SaveLoadWithXElement
    public static void SaveListToXMLElement(XElement rootElem, string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            rootElem.Save(xmlFilePath);
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"fail to create xml file: {s_xmlDir + xmlFilePath}, {ex.Message}");
        }
    }
    public static XElement LoadListFromXMLElement(string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            if (File.Exists(xmlFilePath))
                return XElement.Load(xmlFilePath);
            XElement rootElem = new(xmlFileName);
            rootElem.Save(xmlFilePath);
            return rootElem;
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"fail to load xml file: {s_xmlDir + xmlFilePath}, {ex.Message}");
        }
    }
    #endregion

    #region XmlConfig
    public static int GetAndIncreaseConfigIntVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        int nextId = root.ToIntNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
        root.Element(elemName)?.SetValue((nextId + 1).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
        return nextId;
    }
    public static int GetConfigIntVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        int num = root.ToIntNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
        return num;
    }
    public static DateTime GetConfigDateVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        DateTime dt = root.ToDateTimeNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
        return dt;
    }
    public static void SetConfigIntVal(string xmlFileName, string elemName, int elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue((elemVal).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }
    public static void SetConfigDateVal(string xmlFileName, string elemName, DateTime elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue((elemVal).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }
    //////////////////////////////////////////
    public static TimeSpan GetConfigRiskRange(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        TimeSpan dt = root.ToTimeSpanNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
        return dt;
    }
    public static void SetConfigIntTimeSpan(string xmlFileName, string elemName, int elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue((elemVal).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }
    public static void SetConfigTimeSpan(string xmlFileName, string elemName, TimeSpan elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue((elemVal).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }
    ///////////////////////////////////////////////////
    #endregion


    #region ExtensionFuctions
    public static T? ToEnumNullable<T>(this XElement element, string name) where T : struct, Enum =>
        Enum.TryParse<T>((string?)element.Element(name), out var result) ? (T?)result : null;
    public static DateTime? ToDateTimeNullable(this XElement element, string name) =>
        DateTime.TryParse((string?)element.Element(name), out var result) ? (DateTime?)result : null;
    public static double? ToDoubleNullable(this XElement element, string name) =>
        double.TryParse((string?)element.Element(name), out var result) ? (double?)result : null;
    public static int? ToIntNullable(this XElement element, string name) =>
        int.TryParse((string?)element.Element(name), out var result) ? (int?)result : null;
    #endregion


    //////////////////////
    public static TimeSpan? ToTimeSpanNullable(this XElement element, string name) =>
        TimeSpan.TryParse((string?)element.Element(name), out var result) ? (TimeSpan?)result : null;
    //////////////////////
}
