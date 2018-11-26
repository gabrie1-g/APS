using APS.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace APS.Models
{
	public class BookRepository : IBookRepository
	{
		private ApplicationDbContext _context;
        private readonly IHostingEnvironment _environment;

		public BookRepository(ApplicationDbContext context, IHostingEnvironment environment)
		{
			_context = context;
            _environment = environment;
		}

        public void RegisterBook(Book book)
        {
            book.InStock = book.InitialStock;
            _context.Books.Add(book);
            _context.UpdateRange();
            _context.SaveChanges();
        }

        public IEnumerable<Book> ListSoldBooks(string userId)
        {
            List<Book> soldBooks = new List<Book>();
            var books = _context.Books.Where(book => book.SellerId == userId);
            foreach (var book in books)
            {
                if (book.InitialStock > book.InStock)
                {
                    book.InitialStock -= book.InStock;
                    soldBooks.Add(book);
                }
            }
            return soldBooks.AsEnumerable();
        }

        public void UpdateQuantity(string bookId)
        {
            var book = _context.Books.Where(b => b.BookId == new Guid(bookId));
            book.First().InStock = book.First().InStock - 1;
            if (book.First().InStock == 0) book.First().BookStatus = false;
            _context.SaveChanges();
            
        }

        public void UpdateBook(Book book)
        {
            _context.Books.Update(book);
            _context.SaveChanges();
        }

        public string SaveImage(IFormFile image)
        {
            string path = _environment.WebRootPath + @"\images\livros\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (image != null && image.Length > 0)
            {
                if (image.Length > 0)
                {
                    using (var fileStream = new FileStream(Path.Combine(path, image.FileName), FileMode.Create, FileAccess.Write))
                    {
                        image.CopyTo(fileStream);
                    }
                    return @"\images\livros\" + image.FileName;
                }
            }
            return null;
        }

        public void DeleteBook(string bookId)
        {
            var book = _context.Books.Where(b => b.BookId == new Guid(bookId));
            book.First().BookStatus = false;
            _context.SaveChanges();
        }
        public IEnumerable<Book> Books => _context.Books;
	}
}
