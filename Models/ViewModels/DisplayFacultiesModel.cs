using System.Security.Policy;

namespace FirstProject.Models.ViewModels;

public class DisplayFacultiesModel
{
    // public DisplayFacultiesModel(ICollection<Faculty> faculties)
    // {
    //     Faculties = InitDictionary(faculties);
    // }
    public static Dictionary<int,Faculty> InitDictionary(ICollection<Faculty> faculties)
    {
        Dictionary<int, Faculty> dictionary = new Dictionary<int, Faculty>(faculties.Count);
        int index = 0;
        foreach (Faculty faculty in faculties)
        {
            dictionary.Add(index,faculty);
            index++;
        }

        return dictionary;
    }
}