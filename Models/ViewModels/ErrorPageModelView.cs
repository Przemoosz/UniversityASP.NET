namespace FirstProject.Models.ViewModels;

public class ErrorPageModelView
{
    public string? ErrorName { get; set; }
    public int? ErrorId { get; set; }
    
    public string? ErrorDescription { get; set; }
    
    public string? ErrorSolution { get; set; }
    public string? ErrorPlace { get; set; }
}

// Taken
// 1 - Selected person does not exists
// 2 - Can not receive username
// 3 - Cant attach person, person is already taken
// 4 - Person not found in DB
// 5 - User not found in DB
// 6 - Typed current User username
// 7 - Selected Message Box does not exists
// 8 - User already have conversation with this person
// 9
// 10
// 11
// 12
// 13
// 14
// 15