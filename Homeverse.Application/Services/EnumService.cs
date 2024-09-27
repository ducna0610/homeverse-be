using Homeverse.Domain.Enums;

namespace Homeverse.Application.Services;

public interface IEnumService
{
    IEnumerable<KeyValuePair<int, string>> GetCaegoryEnum();
    IEnumerable<KeyValuePair<int, string>> GetFurnishEnum();
}

public class EnumService : IEnumService
{
    public IEnumerable<KeyValuePair<int, string>> GetCaegoryEnum()
    {
        return EnumExtension.GetListDescriptions<CategoryEnum>();
    }

    public IEnumerable<KeyValuePair<int, string>> GetFurnishEnum()
    {
        return EnumExtension.GetListDescriptions<FurnishEnum>();
    }
}
