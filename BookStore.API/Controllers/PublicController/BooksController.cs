using BookStore.API.Data;
using BookStore.API.DTOs.Books;
using BookStore.API.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Controllers.PublicController
{
    [ApiController]
    [Route("public/books")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBookById(int id)
        {
            var now = DateTime.UtcNow;
            var book = await _context.Books
                .Where(b => b.BookId == id)
                .Select(b => new BookResponseDto
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    Description = b.Description,
                    Author = b.Author,
                    Genre = b.Genre,
                    Language = b.Language,
                    Format = b.Format,
                    Publisher = b.Publisher,
                    PublicationDate = b.PublicationDate,
                    Price = b.Price,
                    StockQuantity = b.StockQuantity,
                    IsExclusive = b.IsExclusive,
                    OnSale = b.OnSale,
                    //SalePrice = b.SalePrice,
                    SaleStart = b.SaleStart,
                    SaleEnd = b.SaleEnd,
                    ImageUrl = b.ImageUrl,

                    // ✅ Conditional SalePrice
                    SalePrice = b.OnSale == true
    && (!b.SaleStart.HasValue || b.SaleStart <= now)
    && (!b.SaleEnd.HasValue || b.SaleEnd >= now)
    ? b.SalePrice
    : null

                })
                .FirstOrDefaultAsync();

            if (book == null)
                return NotFound("Book not found.");

            return Ok(book);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetBooks(
    string? search,
    string? genre,
    string? format,
    bool? onSale,
    string? sortBy,
    string? order,
    int page = 1,
    int pageSize = 10)
        {
            var query = _context.Books.AsQueryable();

            // 🔍 Search by title, author, or description
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b =>
                    b.Title.ToLower().Contains(search.ToLower()) ||
                    b.Author.ToLower().Contains(search.ToLower()) ||
                    b.Description.ToLower().Contains(search.ToLower()));
            }

            // 🧪 Filter by genre (as string)
            if (!string.IsNullOrEmpty(genre) && Enum.TryParse(genre,true, out BookGenre parsedGenre))
            {
                query = query.Where(b => b.Genre == parsedGenre);
            }

            // 🧪 Filter by format (as string)
            if (!string.IsNullOrEmpty(format) && Enum.TryParse(format,true, out BookFormat parsedFormat))
            {
                query = query.Where(b => b.Format == parsedFormat);
            }
            var now = DateTime.UtcNow;

            // 🧪 Filter by active sale (flag + valid dates)
            if (onSale == true)
            {
                query = query.Where(b =>
                    b.OnSale &&
                    (!b.SaleStart.HasValue || b.SaleStart <= now) &&
                    (!b.SaleEnd.HasValue || b.SaleEnd >= now)
                );
            }


            // 🔽 Sorting
            query = (sortBy?.ToLower(), order?.ToLower()) switch
            {
                ("price", "asc") => query.OrderBy(b => b.Price),
                ("price", "desc") => query.OrderByDescending(b => b.Price),
                ("title", "asc") => query.OrderBy(b => b.Title),
                ("title", "desc") => query.OrderByDescending(b => b.Title),
                _ => query.OrderByDescending(b => b.PublicationDate)
            };

            // 📊 Pagination
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            //var now = DateTime.UtcNow;

            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(book => new BookResponseDto
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    ISBN = book.ISBN,
                    Description = book.Description,
                    Author = book.Author,
                    Genre = book.Genre,
                    Language = book.Language,
                    Format = book.Format,
                    Publisher = book.Publisher,
                    PublicationDate = book.PublicationDate,
                    Price = book.Price,
                    StockQuantity = book.StockQuantity,
                    IsExclusive = book.IsExclusive,
                    OnSale = book.OnSale,
                    //SalePrice = book.SalePrice,
                    SaleStart = book.SaleStart,
                    SaleEnd = book.SaleEnd,
                    ImageUrl = book.ImageUrl,

                    // ✅ Conditional SalePrice
                    SalePrice = book.OnSale == true
    && (!book.SaleStart.HasValue || book.SaleStart <= now)
    && (!book.SaleEnd.HasValue || book.SaleEnd >= now)
    ? book.SalePrice
    : null


                })
                .ToListAsync();

            return Ok(new
            {
                currentPage = page,
                pageSize,
                totalPages,
                totalCount,
                books
            });
        }


    }
}
