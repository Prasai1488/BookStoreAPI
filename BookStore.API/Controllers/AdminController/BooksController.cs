using BookStore.API.Data;
using BookStore.API.DTOs.Books;
using BookStore.API.Enums;
using BookStore.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Controllers.AdminController
{
    [ApiController]
    [Route("admin/books")]
    [Authorize(Roles = "Admin")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 GET All Books (Admin)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _context.Books
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
                    SalePrice = book.SalePrice,
                    SaleStart = book.SaleStart,
                    SaleEnd = book.SaleEnd,
                    ImageUrl = book.ImageUrl
                })
                .ToListAsync();

            return Ok(books);
        }

        // 🔹 GET Book by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            var dto = new BookResponseDto
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
                SalePrice = book.SalePrice,
                SaleStart = book.SaleStart,
                SaleEnd = book.SaleEnd,
                ImageUrl = book.ImageUrl
            };

            return Ok(dto);
        }

        // 🔹 POST: Create Book
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookDto dto)
        {
            var book = new Book
            {
                Title = dto.Title,
                ISBN = dto.ISBN,
                Description = dto.Description,
                Author = dto.Author,
                Genre = dto.Genre,
                Language = dto.Language,
                Format = dto.Format,
                Publisher = dto.Publisher,
                PublicationDate = dto.PublicationDate?.ToUniversalTime(),
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                IsExclusive = dto.IsExclusive,
                OnSale = dto.OnSale,
                SalePrice = dto.SalePrice,
                SaleStart = dto.SaleStart?.ToUniversalTime(),
                SaleEnd = dto.SaleEnd?.ToUniversalTime(),
                ImageUrl = dto.ImageUrl
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Book created successfully", book.BookId });
        }

        // 🔹 PUT: Update Book
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDto dto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            // Update only if values provided
            book.Title = dto.Title ?? book.Title;
            book.ISBN = dto.ISBN ?? book.ISBN;
            book.Description = dto.Description ?? book.Description;
            book.Author = dto.Author ?? book.Author;
            book.Genre = dto.Genre ?? book.Genre;
            book.Language = dto.Language ?? book.Language;
            book.Format = dto.Format ?? book.Format;
            book.Publisher = dto.Publisher ?? book.Publisher;
            book.PublicationDate = dto.PublicationDate?.ToUniversalTime() ?? book.PublicationDate;
            book.Price = dto.Price ?? book.Price;
            book.StockQuantity = dto.StockQuantity ?? book.StockQuantity;
            book.IsExclusive = dto.IsExclusive ?? book.IsExclusive;
            book.OnSale = dto.OnSale ?? book.OnSale;
            book.SalePrice = dto.SalePrice ?? book.SalePrice;
            book.SaleStart = dto.SaleStart?.ToUniversalTime() ?? book.SaleStart;
            book.SaleEnd = dto.SaleEnd?.ToUniversalTime() ?? book.SaleEnd;
            book.ImageUrl = dto.ImageUrl ?? book.ImageUrl;

            await _context.SaveChangesAsync();
            return Ok("Book updated successfully.");
        }

        // 🔹 DELETE Book
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return Ok("Book deleted successfully.");
        }
    }
}
