using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using RestaurantOps.Legacy.Models;

namespace RestaurantOps.Legacy.Data
{
    public class EmployeeRepository
    {
        public IEnumerable<Employee> GetAll(bool includeInactive = false)
        {
            var sql = "SELECT EmployeeId, FirstName, LastName, Role, HireDate, IsActive FROM Employees" +
                      (includeInactive ? string.Empty : " WHERE IsActive = 1") + " ORDER BY LastName, FirstName";
            var dt = SqlHelper.ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                yield return Map(row);
            }
        }

        public Employee? GetById(int id)
        {
            const string sql = "SELECT EmployeeId, FirstName, LastName, Role, HireDate, IsActive FROM Employees WHERE EmployeeId = @id";
            var dt = SqlHelper.ExecuteDataTable(sql, new SqlParameter("@id", id));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public void Add(Employee emp)
        {
            const string sql = @"INSERT INTO Employees (FirstName, LastName, Role, HireDate, IsActive)
                                 VALUES (@fn, @ln, @role, @hd, @act)";
            SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@fn", emp.FirstName),
                new SqlParameter("@ln", emp.LastName),
                new SqlParameter("@role", emp.Role),
                new SqlParameter("@hd", emp.HireDate.Date),
                new SqlParameter("@act", emp.IsActive));
        }

        public void Update(Employee emp)
        {
            const string sql = @"UPDATE Employees SET FirstName=@fn, LastName=@ln, Role=@role, IsActive=@act WHERE EmployeeId=@id";
            SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@fn", emp.FirstName),
                new SqlParameter("@ln", emp.LastName),
                new SqlParameter("@role", emp.Role),
                new SqlParameter("@act", emp.IsActive),
                new SqlParameter("@id", emp.EmployeeId));
        }

        private static Employee Map(DataRow row) => new()
        {
            EmployeeId = (int)row["EmployeeId"],
            FirstName = row["FirstName"].ToString()!,
            LastName = row["LastName"].ToString()!,
            Role = row["Role"].ToString()!,
            HireDate = (DateTime)row["HireDate"],
            IsActive = (bool)row["IsActive"]
        };
    }
} 