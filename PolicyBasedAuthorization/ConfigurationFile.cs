using System.Text.Json;
using System.Text.Json.Serialization;

namespace FirstProject.PolicyBasedAuthorization;

internal static class ConfigurationFile
{
    private const string FileName = "AuthorizationConfig.json";
    private static string _path = Directory.GetCurrentDirectory() + "\\" + FileName; 
    
    public static void Save(Dictionary<string, List<string>> Policy)
    {
        var option = new JsonSerializerOptions() {WriteIndented = true};
        var jsonData = JsonSerializer.Serialize(Policy,option);
        File.WriteAllText(_path, jsonData);
    }
    
    public static Dictionary<string,List<string>>? Load()
    {
        var loadedJson = File.ReadAllText(_path);
        Dictionary<string, List<string>>? result = new Dictionary<string, List<string>>();
        result = JsonSerializer.Deserialize<Dictionary<string,List<string>>?>(loadedJson);
        return result;
    }
    
    public static async Task SaveAsync(Dictionary<string, List<string>> Policy)
    {
        var option = new JsonSerializerOptions() {WriteIndented = true};
        var jsonData = JsonSerializer.Serialize(Policy,option);
        await File.WriteAllTextAsync(_path, jsonData);
    }

    public static async Task<Dictionary<string,List<string>>?> LoadAsync()
    {
        var loadedJson = await File.ReadAllTextAsync(_path);
        Dictionary<string, List<string>>? result = new Dictionary<string, List<string>>();
        result = JsonSerializer.Deserialize<Dictionary<string,List<string>>?>(loadedJson);
        return result;
    }
}