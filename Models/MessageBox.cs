namespace FirstProject.Models;

public class MessageBox
{
    public int MessageBoxID { get; set; }
    
    public IEnumerable<ApplicationUser> Participants { get; set; }
    
    public IEnumerable<Message> Messages { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }
        
        MessageBox other = obj as MessageBox;
        if (other is null)
        {
            return false;
        }

        if (other.Participants.Count() != this.Participants.Count())
        {
            return false;
        }

        foreach (var user in other.Participants)
        {
            if (!this.Participants.Contains(user))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Participants);
    }
}