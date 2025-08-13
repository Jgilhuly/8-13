using Microsoft.AspNetCore.Mvc;
using RestaurantOps.Legacy.Data;
using RestaurantOps.Legacy.Models;

namespace RestaurantOps.Legacy.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IngredientRepository _ingRepo = new();
        private readonly InventoryRepository _txRepo = new();

        public IActionResult Index()
        {
            var ingredients = _ingRepo.GetAll();
            return View(ingredients);
        }

        public IActionResult Create()
        {
            return View(new Ingredient());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ingredient ing)
        {
            if (!ModelState.IsValid)
            {
                return View(ing);
            }
            _ingRepo.Add(ing);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var ing = _ingRepo.GetById(id);
            if (ing == null) return NotFound();
            return View(ing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Ingredient ing)
        {
            if (!ModelState.IsValid)
            {
                return View(ing);
            }
            _ingRepo.Update(ing);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Adjust(int ingredientId, decimal quantityChange, string? notes)
        {
            if (ingredientId <= 0)
            {
                TempData["Error"] = "Invalid ingredient.";
                return RedirectToAction(nameof(Index));
            }
            if (quantityChange == 0)
            {
                TempData["Error"] = "Quantity change cannot be zero.";
                return RedirectToAction(nameof(Index));
            }
            if (Math.Abs(quantityChange) > 100000)
            {
                TempData["Error"] = "Quantity change is too large.";
                return RedirectToAction(nameof(Index));
            }
            _txRepo.AdjustStock(ingredientId, quantityChange, notes);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ReorderReport(string? format)
        {
            var lowStock = _ingRepo.GetAll().Where(i => i.NeedsReorder).ToList();
            if (format == "csv")
            {
                var lines = new List<string>{"Name,Unit,OnHand,ReorderLevel"};
                lines.AddRange(lowStock.Select(i => $"{i.Name},{i.Unit},{i.QuantityOnHand},{i.ReorderThreshold}"));
                var bytes = System.Text.Encoding.UTF8.GetBytes(string.Join("\n", lines));
                return File(bytes, "text/csv", "reorder-report.csv");
            }
            return View(lowStock);
        }
    }
} 