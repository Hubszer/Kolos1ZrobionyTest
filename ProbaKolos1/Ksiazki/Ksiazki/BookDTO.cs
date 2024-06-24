namespace Ksiazki;

public class BookDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<AuthorsDTO> Authors { get; set; } = new List<AuthorsDTO>();
    /*public AuthorsDTO AuthorsDto { get; set; } = null;*/
}

public class AuthorsDTO
{
    public string firstName { get; set; } = String.Empty;
    public string lastName { get; set; } = String.Empty;
    
}