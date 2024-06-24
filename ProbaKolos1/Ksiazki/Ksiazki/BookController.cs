using Microsoft.AspNetCore.Mvc;

namespace Ksiazki;

[Route("api/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly IBookRepo _bookRepo;

    public BookController(IBookRepo bookRepo)
    {
        _bookRepo = bookRepo;
    }

    [HttpGet]
    [Route("{id}/authors")]
    public async Task<IActionResult> GetAuthors(int id)
    {
        if (!await _bookRepo.DoesBookExists(id))
        {
            return NotFound();
        }

        var book = await _bookRepo.GetAuthors(id);

        return Ok(book);
    }

    [HttpPost]
    [Route("books")]
    public async Task<IActionResult> AddBooksWithAuthors(AddBookDTO addBookDto)
    {
        var idBook = await _bookRepo.addBook(addBookDto.Title);

        BookDTO bookDto = new BookDTO();
        bookDto.Id = idBook;
        bookDto.Title = addBookDto.Title;
        bookDto.Authors = new List<AuthorsDTO>(addBookDto.Authors);

        foreach (var author in addBookDto.Authors)
        {
            var idAuthor = await _bookRepo.addAuthor(author);

            await _bookRepo.addBookAndAuthor(idBook, idAuthor);
        }

        return Created(Request.Path.Value ?? $"api/{idBook}/authors", bookDto);
    }

}
