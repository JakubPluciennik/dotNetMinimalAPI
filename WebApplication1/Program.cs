using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<BookRepository>();
var app = builder.Build();

app.MapGet("/books", ([FromServices] BookRepository repo) =>
{
    return repo.GetBooks();
});

app.MapGet("/books/{id}", ([FromServices] BookRepository repo, Guid id) =>
{
    var book = repo.GetById(id);
    if (book != null)
    {
        return Results.Ok(book);
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapPost("/books", ([FromServices] BookRepository repo, Book book) =>
{
    repo.Create(book);
    return Results.Created($"/books/{book.id}", book);
});

app.MapPut("/books/{id}", ([FromServices] BookRepository repo, Guid id, Book uBook) =>
{
    Book book = repo.GetById(id);
    if (book != null)
    {
        return Results.NotFound();
    }

    repo.Update(uBook);
    return Results.Ok(uBook);
});

app.MapDelete("/books/{id}", ([FromServices] BookRepository repo, Guid id) =>
{
    repo.Delete(id);
    return Results.Ok();
});

app.Run();

class Book
{
    public Guid id { get; set; }
    public string author { get; set; }
    public string title { get; set; }
    public int pages { get; set; }
}


class BookRepository
{
    private readonly Dictionary<Guid, Book> _books = new();

    public void Create(Book book)
    {
        if (book == null)
        {
            return;
        }
        if (book.id.Equals(Guid.Empty)) { book.id = Guid.NewGuid(); }
        _books[book.id] = book;
    }

    public Book GetById(Guid id)
    {
        return _books[id];
    }

    public List<Book> GetBooks()
    {
        return _books.Values.ToList();
    }

    public void Update(Book book)
    {
        Book existingBook = GetById(book.id);
        if (existingBook == null) { return; }
        _books[book.id] = book;
    }

    public void Delete(Guid id)
    { _books.Remove(id); }
}