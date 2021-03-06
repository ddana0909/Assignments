﻿using MusicStore.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MusicStore.Controllers
{
	public class HomeController : Controller
	{
		private readonly MvcMusicStoreEntities db;

		public HomeController()
		{
			db = new MvcMusicStoreEntities();
		}

		public ActionResult Index(DisplayOption displayOption)
		{
			displayOption.Init();

			var orders = GetOrders(displayOption);	

			displayOption.UpdateSortOrder();

			ViewBag.DisplayOption = displayOption;

			if(!orders.Any())
			{
				ViewBag.NotFound = "No orders fit your searching criteria";
			}
			
			return View(orders.ToPagedList(displayOption.GetPageNumber, displayOption.CurrentSize));
		
		}

		public ActionResult OrderAlbums(int orderId)
		{
			var order = db.Orders.Find(orderId);
			var albums = order.OrderDetails.Select(od => od.Album);
			if (albums.Count() != 0)
			{
				return View(order);
			}
			return RedirectToAction("NoAlbumFound");
		}

		public ActionResult NoAlbumFound()
		{
			return View();
		}



		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}

		private IEnumerable<Order> GetOrders(DisplayOption displayOption)
		{
			var orders = db.Orders
				.Where(or => displayOption.NameToSearch == null || or.FirstName.Contains(displayOption.NameToSearch))
				.OrderBy(o => o.FirstName);

			

			if (string.IsNullOrEmpty(displayOption.SortOrder) ||
				(String.Compare(displayOption.SortOrder, "desc", StringComparison.InvariantCultureIgnoreCase) != 0 &&
				 String.Compare(displayOption.SortOrder, "asc", StringComparison.InvariantCultureIgnoreCase) != 0))
				displayOption.SortOrder = "asc";

			switch (displayOption.SortField)
			{
				case "Total":
					if (String.Compare(displayOption.SortOrder, "asc", StringComparison.InvariantCultureIgnoreCase) == 0)
						orders = orders.OrderBy(o => o.Total);
					else
						orders = orders.OrderByDescending(o => o.Total);
					break;

				case "Date":
					if (String.Compare(displayOption.SortOrder, "asc", StringComparison.InvariantCultureIgnoreCase) == 0)
						orders = orders.OrderBy(o => o.OrderDate);
					else
						orders = orders.OrderByDescending(o => o.OrderDate);
					break;
			}
			


			return orders;
		}


	}
}