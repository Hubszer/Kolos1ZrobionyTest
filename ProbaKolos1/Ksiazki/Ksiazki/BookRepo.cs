using Microsoft.Data.SqlClient;

namespace Ksiazki;

public class BookRepo : IBookRepo
{
    private readonly IConfiguration _configuration;

    public BookRepo(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesBookExists(int id)
    {
        var query = "SELECT 1 FROM Books WHERE PK = @ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<bool> DoesAuthorExists(int id)
    {
        var query = "SELECT 1 FROM Authors WHERE PK = @ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }
    

    public async Task<BookDTO> GetAuthors(int id)
    {
        var query = @"SELECT Books.PK as BookPK,
                    title,
                    Authors.first_name as firstName,
                    Authors.last_name as lastName
                    From Books
                    join books_authors on books.PK = books_authors.FK_book
                    join authors on authors.PK = books_authors.FK_author
                    where books.PK = @ID

        ";
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
	    
        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();

        var bookIdOrdinal = reader.GetOrdinal("BookPK");
        var titleOrdinal = reader.GetOrdinal("Title");
        var firstNameOrdianl = reader.GetOrdinal("firstName");
        var lastNameOrdinal = reader.GetOrdinal("lastName");
        BookDTO books = null;

        while (await reader.ReadAsync())
        {
            if (books is not null)
            {
                books = new BookDTO()

                {
                    Id = reader.GetInt32(bookIdOrdinal),
                    Title = reader.GetString(titleOrdinal),
                    Authors = new List<AuthorsDTO>()
                    {
                        new AuthorsDTO()
                        {
                            firstName = reader.GetString(firstNameOrdianl),
                            lastName = reader.GetString(lastNameOrdinal)
                        }
                    }
                };
            }
            else
            {
                books.Authors.Add(new AuthorsDTO()
                    {
                        firstName = reader.GetString(firstNameOrdianl),
                        lastName = reader.GetString(lastNameOrdinal)
                    }
                );
            }
        }

        if (books is null) throw new Exception();
        
        return books;
    }

    public async Task<int> addAuthor(AuthorsDTO authorsDto)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        await connection.OpenAsync();
        command.CommandText = "INSERT INTO Authors VALUES (@firstName,@lastName);" +
                              "SELECT @@IDENTITY AS ID";

        command.Parameters.AddWithValue("@firstName", authorsDto.firstName);
        command.Parameters.AddWithValue("@lastName", authorsDto.lastName);

        var id = Convert.ToInt32(await command.ExecuteScalarAsync());
        return id;
    }

    public async Task<int> addBook(string title)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        await connection.OpenAsync();
        command.CommandText = "INSERT INTO Books VALUES (@title);" +
                              "SELECT @@IDENTITY AS ID";

        command.Parameters.AddWithValue(@title, title);
        await connection.OpenAsync();

        var id = Convert.ToInt32(await command.ExecuteScalarAsync());
        return id;
    }

    public async Task addBookAndAuthor(int idBook, int idAuthor)
    {
       
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        await connection.OpenAsync();

        command.CommandText = "INSERT INTO Books_authors VALUES(@IdBook,@IdAuthor)";
        command.Parameters.AddWithValue("@IdBook", idBook);
        command.Parameters.AddWithValue("@IdAuthor", idAuthor);

        await command.ExecuteScalarAsync();
    }
}