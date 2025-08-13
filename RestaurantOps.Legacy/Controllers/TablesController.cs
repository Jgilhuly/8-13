using Microsoft.AspNetCore.Mvc;
using RestaurantOps.Legacy.Data;
using RestaurantOps.Legacy.Models;

namespace RestaurantOps.Legacy.Controllers
{
    public class TablesController : Controller
    {
        private readonly TableRepository _tableRepo = new();
        private readonly OrderRepository _orderRepo = new();

        public IActionResult Index()
        {
            var tables = _tableRepo.GetAll();
            return View(tables);
        }

        public IActionResult Seat(int id)
        {
            var order = _orderRepo.GetCurrentByTable(id);
            if (order == null)
            {
                order = _orderRepo.Create(id);
                _tableRepo.UpdateOccupied(id, true);
            }
            return RedirectToAction("Details", "Order", new { id = order.OrderId });
        }
    }
} 