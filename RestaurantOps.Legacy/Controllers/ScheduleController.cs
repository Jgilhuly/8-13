using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RestaurantOps.Legacy.Data;

namespace RestaurantOps.Legacy.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly EmployeeRepository _empRepo = new();
        private readonly ShiftRepository _shiftRepo = new();

        public IActionResult Index(string? week)
        {
            // Determine week start (Monday)
            DateTime start = week != null && DateTime.TryParse(week, out var parsed)
                ? parsed.Date
                : DateTime.Today.StartOfWeek(DayOfWeek.Monday);
            if (start.DayOfWeek != DayOfWeek.Monday)
                start = start.StartOfWeek(DayOfWeek.Monday);
            DateTime end = start.AddDays(6);

            var shifts = _shiftRepo.GetByDateRange(start, end).ToList();
            ViewBag.Start = start;
            ViewBag.End = end;
            ViewBag.Employees = _empRepo.GetAll();
            return View(shifts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddShift(int employeeId, DateTime shiftDate, TimeSpan startTime, TimeSpan endTime, string role)
        {
            // Input validation
            if (employeeId <= 0)
            {
                TempData["Error"] = "Invalid employee ID.";
                return RedirectToAction(nameof(Index));
            }
            if (string.IsNullOrWhiteSpace(role))
            {
                TempData["Error"] = "Role is required.";
                return RedirectToAction(nameof(Index));
            }
            if (shiftDate.Date < DateTime.Today.AddDays(-30))
            {
                TempData["Error"] = "Cannot schedule shifts more than 30 days in the past.";
                return RedirectToAction(nameof(Index));
            }
            if (endTime <= startTime)
            {
                TempData["Error"] = "End time must be after start time.";
                return RedirectToAction(nameof(Index));
            }
            if (_shiftRepo.HasOverlap(employeeId, shiftDate, startTime, endTime))
            {
                TempData["Error"] = "Shift overlaps with existing shift for this employee.";
                return RedirectToAction(nameof(Index), new { week = shiftDate.StartOfWeek(DayOfWeek.Monday).ToString("yyyy-MM-dd") });
            }
            if (_shiftRepo.IsDuringApprovedTimeOff(employeeId, shiftDate))
            {
                TempData["Error"] = "Employee has approved time-off on this date.";
                return RedirectToAction(nameof(Index), new { week = shiftDate.StartOfWeek(DayOfWeek.Monday).ToString("yyyy-MM-dd") });
            }
            _shiftRepo.Add(new Models.Shift
            {
                EmployeeId = employeeId,
                ShiftDate = shiftDate.Date,
                StartTime = startTime,
                EndTime = endTime,
                Role = role
            });
            var monday = shiftDate.StartOfWeek(DayOfWeek.Monday);
            return RedirectToAction(nameof(Index), new { week = monday.ToString("yyyy-MM-dd") });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteShift(int id, DateTime week)
        {
            _shiftRepo.Delete(id);
            return RedirectToAction(nameof(Index), new { week = week.ToString("yyyy-MM-dd") });
        }

        public IActionResult TimeOff()
        {
            var pending = _shiftRepo.GetPendingTimeOff();
            return View(pending);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetTimeOffStatus(int id, string status)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Invalid time-off request ID.";
                return RedirectToAction(nameof(TimeOff));
            }
            if (string.IsNullOrWhiteSpace(status) || !new[] { "Approved", "Denied" }.Contains(status))
            {
                TempData["Error"] = "Invalid status. Must be 'Approved' or 'Denied'.";
                return RedirectToAction(nameof(TimeOff));
            }
            _shiftRepo.SetTimeOffStatus(id, status);
            return RedirectToAction(nameof(TimeOff));
        }
    }

    public static class DateExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
} 