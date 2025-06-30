using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QMS.Datas;
using System; // <-- Add this
using System.Collections; // Already present
using System.Collections.Generic; // <-- Add this
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace QMS.Controllers
{
    public class DBMSController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DBMSController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var tableNames = _context.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType &&
                            p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Name
                })
                .ToList();

            ViewBag.TableNames = tableNames;
            return View();
        }

        [HttpGet]
        public IActionResult TableData(string tableName, int page = 1, int pageSize = 10000)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return BadRequest(new { success = false, message = "Table name is required." });
            }

            var tableProperty = _context.GetType()
                .GetProperties()
                .FirstOrDefault(p => p.Name == tableName &&
                                     p.PropertyType.IsGenericType &&
                                     p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

            if (tableProperty == null)
            {
                return NotFound(new { success = false, message = "Table not found." });
            }

            var tableData = (IEnumerable)tableProperty.GetValue(_context);
            var data = tableData.Cast<object>().Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Json(new
            {
                success = true,
                data,
                currentPage = page,
                pageSize,
                hasNextPage = tableData.Cast<object>().Skip(page * pageSize).Any()
            });
        }


        [HttpPost]
        public IActionResult Update([FromBody] UpdateRequest request)
        {
            //Cek Input
            if (string.IsNullOrEmpty(request.TableName) || request.UpdatedRow == null)
            {
                return BadRequest(new { success = false, message = "Invalid input." });
            }

            //Cek Tabel Property
            var tableProperty = _context.GetType()
                .GetProperties()
                .FirstOrDefault(p => p.Name == request.TableName &&
                                     p.PropertyType.IsGenericType &&
                                     p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

            //Tabel property null maka kirim error table notfound
            if (tableProperty == null)
            {
                return NotFound(new { success = false, message = "Table not found." });
            }

            var entityType = tableProperty.PropertyType.GetGenericArguments()[0];
            var primaryKey = entityType.GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());
            var primaryKeyName = primaryKey?.Name.ToLower();

            //if (!request.UpdatedRow.Keys.Any(key => string.Equals(key, primaryKey.Name, StringComparison.OrdinalIgnoreCase)))
            //{
            //    bool hasPrimaryKey = false;
            //}

            if (primaryKey == null || !request.UpdatedRow.Keys.Any(key => string.Equals(key, primaryKeyName, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest(new { success = false, message = "Primary key is missing." });
            }

            var primaryKeyType = Nullable.GetUnderlyingType(primaryKey.PropertyType) ?? primaryKey.PropertyType;
            var primaryKeyValue = request.UpdatedRow[primaryKeyName];
            // Check if the value is a JsonElement
            if (primaryKeyValue is System.Text.Json.JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.String)
                {
                    // Extract the string value
                    primaryKeyValue = jsonElement.GetString();
                }
                else if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.Number)
                {
                    // Extract the numeric value as a string
                    primaryKeyValue = jsonElement.GetRawText();
                }
                else
                {
                    return BadRequest(new { success = false, message = "Invalid primary key value type." });
                }
            }

            var primaryKeyValueConverted = Convert.ChangeType(primaryKeyValue, primaryKeyType);

            var dbSet = tableProperty.GetValue(_context);
            var findMethod = dbSet.GetType().GetMethod("Find");
            var entity = findMethod.Invoke(dbSet, new object[] { new object[] { primaryKeyValueConverted } });

            if (entity == null)
            {
                return NotFound(new { success = false, message = "Row not found." });
            }

            foreach (var key in request.UpdatedRow.Keys)
            {
                var property = entityType.GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    var value = request.UpdatedRow[key];
                    // Check if the value is a JsonElement
                    if (value is System.Text.Json.JsonElement jsonElement2)
                    {
                        if (jsonElement2.ValueKind == System.Text.Json.JsonValueKind.String)
                        {
                            // Extract the string value
                            value = jsonElement2.GetString();
                        }
                        else if (jsonElement2.ValueKind == System.Text.Json.JsonValueKind.Number)
                        {
                            // Extract the numeric value as a string
                            value = jsonElement2.GetRawText();
                        }
                        else
                        {
                            return BadRequest(new { success = false, message = "Invalid value type." });
                        }
                    }
                    if (value != null && value.ToString() == string.Empty && property.PropertyType != typeof(string))
                    {
                        value = null;
                    }

                    if (value != null)
                    {
                        var convertedValue = Convert.ChangeType(value, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                        property.SetValue(entity, convertedValue);
                    }
                    else
                    {
                        property.SetValue(entity, null);
                    }
                }
            }

            _context.SaveChanges();

            return Json(new { success = true, message = "Row updated successfully." });
        }

        [HttpPost]
        public IActionResult Delete([FromBody] DeleteRequest request)
        {
            if (string.IsNullOrEmpty(request.TableName) || request.RowToDelete == null)
            {
                return BadRequest(new { success = false, message = "Invalid input." });
            }

            var tableProperty = _context.GetType()
                .GetProperties()
                .FirstOrDefault(p => p.Name == request.TableName &&
                                     p.PropertyType.IsGenericType &&
                                     p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

            if (tableProperty == null)
            {
                return NotFound(new { success = false, message = "Table not found." });
            }

            var entityType = tableProperty.PropertyType.GetGenericArguments()[0];
            var primaryKey = entityType.GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());
            var primaryKeyName = primaryKey?.Name.ToLower();

            if (primaryKey == null || !request.RowToDelete.Keys.Any(key => string.Equals(key, primaryKeyName, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest(new { success = false, message = "Primary key is missing." });
            }

            var primaryKeyType = Nullable.GetUnderlyingType(primaryKey.PropertyType) ?? primaryKey.PropertyType;
            var primaryKeyValue = request.RowToDelete[primaryKeyName];

            // Check if the value is a JsonElement
            if (primaryKeyValue is System.Text.Json.JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.String)
                {
                    primaryKeyValue = jsonElement.GetString();
                }
                else if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.Number)
                {
                    primaryKeyValue = jsonElement.GetRawText();
                }
                else
                {
                    return BadRequest(new { success = false, message = "Invalid primary key value type." });
                }
            }

            var primaryKeyValueConverted = Convert.ChangeType(primaryKeyValue, primaryKeyType);

            var dbSet = tableProperty.GetValue(_context);
            var findMethod = dbSet.GetType().GetMethod("Find");
            var entity = findMethod.Invoke(dbSet, new object[] { new object[] { primaryKeyValueConverted } });

            if (entity == null)
            {
                return NotFound(new { success = false, message = "Row not found." });
            }

            dbSet.GetType().GetMethod("Remove").Invoke(dbSet, new[] { entity });
            _context.SaveChanges();

            return Json(new { success = true, message = "Row deleted successfully." });
        }

        public class UpdateRequest
        {
            public string TableName { get; set; }
            public Dictionary<string, object> UpdatedRow { get; set; }
        }

        [HttpPost]
        [HttpPost]
        public IActionResult Create([FromBody] CreateRequest request)
        {
            // Cek input
            if (string.IsNullOrEmpty(request.TableName) || request.NewRow == null)
            {
                return BadRequest(new { success = false, message = "Invalid input." });
            }

            // Cek tabel property
            var tableProperty = _context.GetType()
                .GetProperties()
                .FirstOrDefault(p => p.Name == request.TableName &&
                                     p.PropertyType.IsGenericType &&
                                     p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

            if (tableProperty == null)
            {
                return NotFound(new { success = false, message = "Table not found." });
            }

            var entityType = tableProperty.PropertyType.GetGenericArguments()[0];
            var newEntity = Activator.CreateInstance(entityType);

            // Cari primary key
            var primaryKey = entityType.GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            foreach (var key in request.NewRow.Keys)
            {
                var property = entityType.GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    // Jika properti adalah primary key dan auto-increment, lewati pengaturan nilainya
                    if (primaryKey != null && property.Name.Equals(primaryKey.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        // Deteksi apakah properti adalah auto-increment (tipe int atau long dan tidak nullable)
                        if (property.PropertyType == typeof(int) || property.PropertyType == typeof(long))
                        {
                            continue; // Lewati pengaturan nilai untuk primary key
                        }
                    }

                    var value = request.NewRow[key];
                    if (value is System.Text.Json.JsonElement jsonElement)
                    {
                        if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.String)
                        {
                            value = jsonElement.GetString();
                        }
                        else if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.Number)
                        {
                            value = jsonElement.GetRawText();
                        }
                        else
                        {
                            return BadRequest(new { success = false, message = "Invalid value type." });
                        }
                    }

                    var convertedValue = Convert.ChangeType(value, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                    property.SetValue(newEntity, convertedValue);
                }
            }

            var dbSet = tableProperty.GetValue(_context);
            dbSet.GetType().GetMethod("Add").Invoke(dbSet, new[] { newEntity });
            _context.SaveChanges();

            return Json(new { success = true, message = "Row created successfully." });
        }

        public class CreateRequest
        {
            public string TableName { get; set; }
            public Dictionary<string, object> NewRow { get; set; }
        }


        public class DeleteRequest
        {
            public string TableName { get; set; }
            public Dictionary<string, object> RowToDelete { get; set; }
        }
    }
}
