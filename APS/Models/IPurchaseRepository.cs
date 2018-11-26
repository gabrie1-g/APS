using System;
using System.Collections.Generic;

namespace APS.Models
{
    public interface IPurchaseRepository
	{
        void SavePurchase(string userId, string bookId, Purchase purchase);
        IEnumerable<Purchase> ListBoughtBooks(string userId);
        void RatePurchase(string purchaseId, string rating);
        IEnumerable<Purchase> ListSoldBooks(string sellerId);
        void UpdateStatus(string status, string purchaseId);
        int NewSold(string id);
        Decimal getAverageRate(Guid sellerId);
    }
}
