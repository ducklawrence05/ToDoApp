using System.Linq.Expressions;

namespace ToDoApp.Application.Extentions
{
    public static class QueryExtentions
    {
        public static IQueryable<T> Search<T>(this IQueryable<T> query, string? searchProperty, string? searchValue) where T : class
        {
            if (string.IsNullOrEmpty(searchProperty) || string.IsNullOrEmpty(searchValue))
            {
                return query;
            }

            var property = typeof(T).GetProperties()
                .FirstOrDefault(p => p.Name.Equals(searchProperty, StringComparison.OrdinalIgnoreCase));
            if (property != null)
            {
                // Tạo tham số x có kiểu T (giống như x => x.Property)
                var parameter = Expression.Parameter(typeof(T), "x");

                // Tạo truy cập property: x.Property
                var propertyAccess = Expression.Property(parameter, property.Name);

                // ép property về ToString()
                var toStringMethod = typeof(object).GetMethod("ToString");
                var toStringCall = Expression.Call(propertyAccess, toStringMethod);

                // Gọi ToLower()
                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                var loweredProperty = Expression.Call(toStringCall, toLowerMethod);

                // Lấy method Contains(string)
                var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);

                // Tạo giá trị truyền vào: "searchValue"
                var searchExpression = Expression.Constant(searchValue.ToLower());

                // Tạo biểu thức gọi hàm: x.Property.Contains(searchValue)
                var containsExpression = Expression.Call(loweredProperty, containsMethod, searchExpression);

                // Tạo lambda: x => x.Property.Contains(searchValue)
                var lambda = Expression.Lambda<Func<T, bool>>(containsExpression, parameter);

                // Apply filter vào query
                query = query.Where(lambda);
            }

            return query;
        }

        public static IQueryable<T> Sort<T>(this IQueryable<T> query, string? sortBy, bool isAscending) where T : class
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return query;
            }

            var property = typeof(T).GetProperties()
                .FirstOrDefault(p => p.Name.Equals(sortBy, StringComparison.OrdinalIgnoreCase));

            if (property != null)
            {
                // Khởi tạo một tham số đại diện cho từng phần tử trong query, ví dụ: x =>
                var parameter = Expression.Parameter(typeof(T), "x");

                // Lấy property cần sort từ tham số, ví dụ: x.Name
                var propertyAccess = Expression.Property(parameter, property.Name);

                // Tạo lambda từ expression trên: x => x.Name
                var orderByExpression = Expression.Lambda(propertyAccess, parameter);

                // Chọn method là "OrderBy" hay "OrderByDescending" tuỳ theo biến truyền vào
                var methodName = isAscending ? "OrderBy" : "OrderByDescending";

                // Gọi hàm OrderBy/OrderByDescending
                var resultExpression = Expression.Call
                (
                    typeof(Queryable),                           // Gọi phương thức từ class Queryable
                    methodName,                                  // Tên hàm: "OrderBy" hoặc "OrderByDescending"
                    [typeof(T), property.PropertyType],          // Kiểu generic: T là entity, property.PropertyType là kiểu của property sort
                    query.Expression,                            // Biểu thức query hiện tại
                    Expression.Quote(orderByExpression)          // Biểu thức lambda cần truyền vào OrderBy
                );

                // Tạo lại một query mới từ expression vừa tạo
                query = query.Provider.CreateQuery<T>(resultExpression);
            }

            return query;
        }

        public static IQueryable<T> Paging<T>(this IQueryable<T> query, int pageIndex, int pageSize) where T : class
        {
            if (pageIndex >= 1 && pageSize >= 1)
            {
                query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }

            return query;
        }

        public static IQueryable<T> ApplyQuery<T>(
            this IQueryable<T> query,
            string? searchProperty, string? searchValue,
            string? sortBy, bool isAscending,
            int pageIndex, int pageSize
        ) where T : class
        {
            query = query.Search(searchProperty, searchValue);
            query = query.Sort(sortBy, isAscending);
            query = query.Paging(pageIndex, pageSize);
            return query;
        }
    }
}
