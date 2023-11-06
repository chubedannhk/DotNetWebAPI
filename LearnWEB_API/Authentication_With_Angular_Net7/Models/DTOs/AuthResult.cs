namespace Authentication_With_Angular_Net7.Models.DTOs;

public class AuthResult
{
    public string Token { get; set; } = string.Empty;
    public bool Result { get; set; }  
    public List<string> Errors { get; set; } 
}
