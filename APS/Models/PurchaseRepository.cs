using APS.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APS.Models
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private ApplicationDbContext _context;
        private IBookRepository _bookRepository;
        public PurchaseRepository(ApplicationDbContext context, IBookRepository bookRepository)
        {
            _context = context;
            _bookRepository = bookRepository;
        }

        public IEnumerable<Purchase> ListBoughtBooks(string userId)
        {
            return _context.Purchases.Where(p => p.BuyerId == userId).Include(p => p.Buyer).Include(p => p.Book);
        }

        public void RatePurchase(string purchaseId, string rating)
        {
            var purchase = _context.Purchases.Find(new Guid(purchaseId));
            purchase.Rating = Convert.ToInt32(rating);
            _context.Purchases.Update(purchase);
            _context.SaveChanges();
        }

        public void SavePurchase(string userId, string bookId, Purchase purchase)
        {
            purchase.Status = "Compra efetuada";
            purchase.BuyerId = userId;
            purchase.BookId = new Guid(purchase.ItemId);
            var book = _context.Books.Find(purchase.BookId);
            purchase.SellerId = book.SellerId;
            _context.Add(purchase);
            _bookRepository.UpdateQuantity(bookId);
            _context.SaveChanges();
        }

        public IEnumerable<Purchase> ListSoldBooks(string sellerId)
        {
            return _context.Purchases.Where(i => i.SellerId == sellerId).Include(x => x.Book);
        }

        public void UpdateStatus(string status, string purchaseId)
        {
            var purchase = _context.Purchases.Where(x => x.PurchaseId == new Guid(purchaseId)).First();
            purchase.Status = status;
            _context.Update(purchase);
            _context.SaveChanges();
        }

        public int NewSold(string id)
        {
            return _context.Purchases.Where(a => a.SellerId == id && a.Status == "Compra efetuada").Count();
        }
    }
}
