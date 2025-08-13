using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using RestaurantOps.Legacy.Models;

namespace RestaurantOps.Legacy.Data
{
    public class ShiftRepository
    {
        public IEnumerable<Shift> GetByDateRange(DateTime start, DateTime end)
        {
            const string sql = @"SELECT s.ShiftId, s.EmployeeId, e.FirstName + ' ' + e.LastName AS EmployeeName,
                                         s.ShiftDate, s.StartTime, s.EndTime, s.Role
                                    FROM Shifts s JOIN Employees e ON e.EmployeeId = s.EmployeeId
                                   WHERE s.ShiftDate BETWEEN @start AND @end
                                   ORDER BY s.ShiftDate, s.StartTime";
            var dt = SqlHelper.ExecuteDataTable(sql,
                new SqlParameter("@start", start.Date),
                new SqlParameter("@end", end.Date));
            foreach (DataRow row in dt.Rows)
            {
                yield return Map(row);
            }
        }

        public void Add(Shift shift)
        {
            const string sql = @"INSERT INTO Shifts (EmployeeId, ShiftDate, StartTime, EndTime, Role)
                                 VALUES (@emp, @date, @start, @end, @role)";
            SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@emp", shift.EmployeeId),
                new SqlParameter("@date", shift.ShiftDate.Date),
                new SqlParameter("@start", shift.StartTime),
                new SqlParameter("@end", shift.EndTime),
                new SqlParameter("@role", shift.Role));
        }

        public void Delete(int shiftId)
        {
            const string sql = "DELETE FROM Shifts WHERE ShiftId = @id";
            SqlHelper.ExecuteNonQuery(sql, new SqlParameter("@id", shiftId));
        }

        // Time-off logic
        public IEnumerable<TimeOff> GetPendingTimeOff()
        {
            const string sql = @"SELECT t.TimeOffId, t.EmployeeId, e.FirstName + ' ' + e.LastName AS EmployeeName,
                                         t.StartDate, t.EndDate, t.Status
                                    FROM TimeOff t JOIN Employees e ON e.EmployeeId = t.EmployeeId
                                   WHERE t.Status = 'Pending'";
            var dt = SqlHelper.ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                yield return MapTimeOff(row);
            }
        }

        public void SetTimeOffStatus(int timeOffId, string status)
        {
            const string sql = "UPDATE TimeOff SET Status=@st WHERE TimeOffId=@id";
            SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@st", status),
                new SqlParameter("@id", timeOffId));
        }

        public bool HasOverlap(int employeeId, DateTime date, TimeSpan start, TimeSpan end)
        {
            const string sql = @"SELECT COUNT(*) FROM Shifts
                                  WHERE EmployeeId = @emp AND ShiftDate = @date
                                    AND @start < EndTime AND @end > StartTime";
            var count = (int)SqlHelper.ExecuteScalar(sql,
                new SqlParameter("@emp", employeeId),
                new SqlParameter("@date", date.Date),
                new SqlParameter("@start", start),
                new SqlParameter("@end", end));
            return count > 0;
        }

        public bool IsDuringApprovedTimeOff(int employeeId, DateTime date)
        {
            const string sql = @"SELECT COUNT(*) FROM TimeOff
                                  WHERE EmployeeId=@emp AND Status='Approved'
                                    AND @date BETWEEN StartDate AND EndDate";
            var count = (int)SqlHelper.ExecuteScalar(sql,
                new SqlParameter("@emp", employeeId),
                new SqlParameter("@date", date.Date));
            return count > 0;
        }

        private static Shift Map(DataRow row) => new()
        {
            ShiftId = (int)row["ShiftId"],
            EmployeeId = (int)row["EmployeeId"],
            EmployeeName = row["EmployeeName"].ToString(),
            ShiftDate = (DateTime)row["ShiftDate"],
            StartTime = (TimeSpan)row["StartTime"],
            EndTime = (TimeSpan)row["EndTime"],
            Role = row["Role"].ToString()!
        };

        private static TimeOff MapTimeOff(DataRow row) => new()
        {
            TimeOffId = (int)row["TimeOffId"],
            EmployeeId = (int)row["EmployeeId"],
            EmployeeName = row["EmployeeName"].ToString(),
            StartDate = (DateTime)row["StartDate"],
            EndDate = (DateTime)row["EndDate"],
            Status = row["Status"].ToString()!
        };
    }
} 