namespace Homeverse.Domain.Enums;


#region DescriptionAttribute
public class DescriptionAttribute : Attribute
{
    public DescriptionAttribute(string stringValue)
    {
        this.stringValue = stringValue;
    }
    private string stringValue;
    public string StringValue
    {
        get { return stringValue; }
        set { stringValue = value; }
    }
}
#endregion

public static class EnumExtension
{
    static T GetAttribute<T>(this Enum value) where T : Attribute
    {
        var type = value.GetType();
        var memberInfo = type.GetMember(value.ToString());
        var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
        return (T)attributes[0];
    }

    public static string GetDescription(this Enum value)
    {
        var attribute = value.GetAttribute<DescriptionAttribute>();
        return attribute == null ? value.ToString() : attribute.StringValue;
    }

    public static IEnumerable<string> GetAllDescriptions<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<Enum>().Select(x => x.GetDescription());
    }

    public static Dictionary<int, string> GetDictionaryDescriptions<T>()
    {
        return Enum.GetValues(typeof(T))
                    .Cast<Enum>()
                    .ToDictionary(t => (int)(object)t, t => t.GetDescription());
    }

    public static List<KeyValuePair<int, string>> GetListDescriptions<T>()
    {
        return Enum.GetValues(typeof(T))
                   .Cast<Enum>()
                   .Select(e => new KeyValuePair<int, string>((int)(object)e, e.GetDescription()))
                   .ToList();
    }
}
