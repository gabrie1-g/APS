﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APS.Models;
using APS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using APS.Models.HomeViewModels;
using System.Security.Claims;

namespace APS.Controllers
{
	public class HomeController : Controller 
	{
		private readonly IBookRepository _bookRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly UserManager<ApplicationUser> _userManager;

		public HomeController(IPurchaseRepository purchaseRepository, IBookRepository bookRepository, UserManager<ApplicationUser> userManager)
		{
            _purchaseRepository = purchaseRepository;
   			_bookRepository = bookRepository;
            _userManager = userManager;
		}
		public IActionResult Index()
		{
            var user = _userManager.GetUserId(User);
            var model = _bookRepository.Books.Where(b => b.BookStatus == true && b.SellerId != user);
			return View(model.GroupBy(b => b.Category));
		}

		public IActionResult DisplayCategory(string SelectedCategory)
		{
            var user = _userManager.GetUserId(User);
            decimal itemsInCategory = _bookRepository.Books.GroupBy(b => b.Category == SelectedCategory && b.BookStatus == true && b.SellerId != user).Count();
            ViewBag.ItemsPerLine = (int) Math.Ceiling(itemsInCategory / 4);
			return View("DisplayCategory", _bookRepository.Books.Where(b => b.Category == SelectedCategory && b.BookStatus == true));
		}

        public IActionResult DisplayBook(string SelectedBook)
        {
            var book = _bookRepository.Books.Where(b => b.BookId == new Guid(SelectedBook));
            ViewBag.whatever = _purchaseRepository.getAverageRate(new Guid(book.First().SellerId));
            return View("DisplayBook", _bookRepository.Books.Where(b => b.BookId == new Guid(SelectedBook)));
        }

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

        [HttpGet]
        public IActionResult BuyBook(string bookId)
        {
            
            ViewBag.BookId = bookId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyBook(Purchase purchase)
        {
            var user = await _userManager.GetUserAsync(User);
            _purchaseRepository.SavePurchase(user.Id, purchase.ItemId.ToString(), purchase);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public int CountNewPurchase()
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int newPurchases = _purchaseRepository.NewSold(userId.ToString());
            return newPurchases;
        }

    }
}