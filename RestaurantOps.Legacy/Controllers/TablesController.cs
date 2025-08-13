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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Seat(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Invalid table ID.";
                return RedirectToAction(nameof(Index));
            }

            var table = _tableRepo.GetAll().FirstOrDefault(t => t.TableId == id);
            if (table == null)
            {
                TempData["Error"] = "Table not found.";
                return RedirectToAction(nameof(Index));
            }

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