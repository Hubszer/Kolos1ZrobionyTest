namespace Ksiazki;

public interface IBookRepo
{
    Task<bool> DoesBookExists(int id);
    Task<bool> DoesAuthorExists(int id);
    Task<BookDTO> GetAuthors(int id);

    Task<int> addAuthor(AuthorsDTO authorsDto);
    Task<int> addBook(String title);

    Task addBookAndAuthor(int idBook, int idAuthor);
    
}