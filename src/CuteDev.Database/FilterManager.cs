using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CuteDev.Database
{
    /// <summary>
    /// Database filtre işlemlerini yönetir. (volkansendag - 13.01.2016)
    /// </summary>
    public static class FilterManger
    {
        #region Properties

        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains");
        private static MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        private static MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

        #endregion

        #region Functions

        /// <summary>
        /// pList nesnesi icindeki filtre ve orderby ifadesini olusturur. IQueryable nesneye uygular. (volkansendag - 13.01.2016)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="prms"></param>
        /// <returns></returns>
        public static IQueryable<T> GetFilter<T>(this IQueryable<T> query, pList prms)
        {
            var deleg = GetFilter<T>(prms);
            if (prms.sort != null && prms.sort.Count > 0 && !String.IsNullOrEmpty(prms.sort.First().field))
            {
                query = query.OrderBy(prms.sort.First().field, prms.sort.First().dir);
            }
            if (deleg != null)
            {
                return query.Where(deleg);
            }
            return query;
        }

        /// <summary>
        /// Linq ile hazırlanmis sorgunun filtrelerini uygular.
        /// Order By islemini uygular.
        /// Sayfalama yaparak rList olarak donus yapar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="prms"></param>
        /// <returns>rList<typeparamref name="T"/></returns>
        public static rList<T> Filter<T>(this IQueryable<T> query, pList prms)
        {
            query = query.GetFilter<T>(prms);

            return new rList<T>(query, prms);
        }

        /// <summary>
        /// Linq ile hazırlanmis sorguyu calistirir. rList olarak dondurur.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static rList<T> Filter<T>(this IQueryable<T> query)
        {
            return new rList<T>(query);
        }

        /// <summary>
        /// pList nesnesi icindeki filtre ifadesini olusturur. (volkansendag - 13.01.2016)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prms"></param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> GetFilter<T>(pList prms)
        {
            if (prms == null || prms.filter == null || prms.filter.filters.Count <= 0)
                return null;

            List<Filter> filters = GetFilterList<T>(prms.filter.filters);

            return GetFilter<T>(filters);
        }

        /// <summary>
        /// filteritems listesini okur ve Filer listesi dondurur. (volkansendag - 13.01.2016)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filterItems"></param>
        /// <returns></returns>
        private static List<Filter> GetFilterList<T>(List<filterItem> filterItems)
        {
            List<Filter> filters = new List<Filter>();

            foreach (var item in filterItems)
            {
                var filter = new Filter()
                {
                    Operation = GetOperation(item.@operator),
                    PropertyName = item.field,
                    Value = item.value
                };

                ParameterExpression param = Expression.Parameter(typeof(T), "t");

                MemberExpression member = Expression.Property(param, filter.PropertyName);

                if ((member.Type == typeof(DateTime) || member.Type == typeof(DateTime?)) && filter.Value != null && filter.Value.IsDate())
                {
                    AddDateFilters(filters, filter);
                }
                else
                {
                    filters.Add(filter);
                }
            }

            return filters;
        }

        /// <summary>
        /// Filter listesinden filtre ifadesi olusturur. (volkansendag - 13.01.2016)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prms"></param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> GetFilter<T>(List<Filter> prms)
        {
            return GetExpression<T>(prms);
        }

        /// <summary>
        /// Filter listesinden filtre ifadesi olusturur. (volkansendag - 13.01.2016)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filters"></param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> GetExpression<T>(IList<Filter> filters)
        {
            if (filters.Count == 0)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(T), "t");
            Expression exp = null;

            if (filters.Count == 1)
                exp = GetExpression<T>(param, filters[0]);
            else if (filters.Count == 2)
                exp = GetExpression<T>(param, filters[0], filters[1]);
            else
            {
                while (filters.Count > 0)
                {
                    var f1 = filters[0];
                    var f2 = filters[1];

                    if (exp == null)
                        exp = GetExpression<T>(param, filters[0], filters[1]);
                    else
                        exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0], filters[1]));

                    filters.Remove(f1);
                    filters.Remove(f2);

                    if (filters.Count == 1)
                    {
                        exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0]));
                        filters.RemoveAt(0);
                    }
                }
            }

            return Expression.Lambda<Func<T, bool>>(exp, param);
        }

        /// <summary>
        /// iki kosullu Filter den binary ifade (BinaryExpression) olusturur. (volkansendag - 13.01.2016)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="filter1"></param>
        /// <param name="filter2"></param>
        /// <returns></returns>
        private static BinaryExpression GetExpression<T>(ParameterExpression param, Filter filter1, Filter filter2)
        {
            Expression bin1 = GetExpression<T>(param, filter1);
            Expression bin2 = GetExpression<T>(param, filter2);

            return Expression.AndAlso(bin1, bin2);
        }

        /// <summary>
        /// iki kosullu Filter den ifade (Expression) olusturur. (volkansendag - 13.01.2016)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static Expression GetExpression<T>(ParameterExpression param, Filter filter)
        {
            MemberExpression member = Expression.Property(param, filter.PropertyName);
            if (filter.Value != null && filter.Value.GetType() != member.Type)
                filter.Value = filter.Value.TryTypeConvert(member.Type);

            Expression constant = Expression.Constant(filter.Value);

            return GetExpression(member, constant, filter.Operation);
        }

        /// <summary>
        /// sag veya sol degerlerden birinin typei null olmasi durumunda esitleme yapar. (volkansendag - 09.03.2016)
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        private static Expression GetExpression(Expression e1, Expression e2, Op op)
        {
            if (IsNullableType(e1.Type) && !IsNullableType(e2.Type))
                e2 = Expression.Convert(e2, e1.Type);
            else if (!IsNullableType(e1.Type) && IsNullableType(e2.Type))
                e1 = Expression.Convert(e1, e2.Type);

            switch (op)
            {
                case Op.Equals:
                    return Expression.Equal(e1, e2);

                case Op.NotEquals:
                    return Expression.NotEqual(e1, e2);

                case Op.GreaterThan:
                    return Expression.GreaterThan(e1, e2);

                case Op.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(e1, e2);

                case Op.LessThan:
                    return Expression.LessThan(e1, e2);

                case Op.LessThanOrEqual:
                    return Expression.LessThanOrEqual(e1, e2);

                case Op.Contains:
                    return Expression.Call(e1, containsMethod, e2);

                case Op.NotContains:
                    return Expression.Not(Expression.Call(e1, containsMethod, e2));

                case Op.StartsWith:
                    return Expression.Call(e1, startsWithMethod, e2);

                case Op.EndsWith:
                    return Expression.Call(e1, endsWithMethod, e2);
            }

            return null;
        }

        /// <summary>
        /// type null olmasi durumunu kontrol eder. (volkansendag - 09.03.2016)
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static bool IsNullableType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// sort edilmesi gereken fieldi ifadeye ekler. (volkansendag - 13.01.2016)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <param name="sort"></param>
        /// <param name="sortType"></param>
        /// <returns></returns>
        private static IQueryable<T> OrderBy<T>(this IQueryable<T> q, string sort, string sortType)
        {
            var classPara = Expression.Parameter(typeof(T), "t");
            var pi = typeof(T).GetProperty(sort);
            q = q.Provider.CreateQuery<T>(
                                Expression.Call(
                                    typeof(Queryable),
                                    sortType == "asc" ? "OrderBy" : "OrderByDescending",
                                    new Type[] { typeof(T), pi.PropertyType },
                                    q.Expression,
                                    Expression.Lambda(Expression.Property(classPara, pi), classPara))
                                );
            return q;
        }

        /// <summary>
        /// String degerden filtre operatorunu getirir. (volkansendag - 13.01.2016)
        /// </summary>
        /// <param name="opStr"></param>
        /// <returns></returns>
        private static Op GetOperation(string opStr)
        {
            if (opStr == "eq")
                return Op.Equals;
            if (opStr == "neq")
                return Op.NotEquals;
            if (opStr == "startswith")
                return Op.StartsWith;
            if (opStr == "contains")
                return Op.Contains;
            if (opStr == "doesnotcontain")
                return Op.NotContains;
            if (opStr == "endswith")
                return Op.EndsWith;
            if (opStr == "gte")
                return Op.GreaterThanOrEqual;
            if (opStr == "gt")
                return Op.GreaterThan;
            if (opStr == "lt")
                return Op.LessThan;
            if (opStr == "lte")
                return Op.LessThanOrEqual;

            return Op.Equals;
        }

        /// <summary>
        /// tarih nesnelerinin filtreye dogru eklenmesi icin calisir. (volkansendag - 13.01.2016)
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="filter"></param>
        private static void AddDateFilters(List<Filter> filters, Filter filter)
        {
            DateTime date;
            DateTime.TryParse(filter.Value.ToString(), out date);

            if (filter.Operation == Op.Equals)
            {
                filters.Add(new Filter()
                {
                    Operation = Op.GreaterThan,
                    PropertyName = filter.PropertyName,
                    Value = date.StartOfDay().ToString()
                });

                filters.Add(new Filter()
                {
                    Operation = Op.LessThan,
                    PropertyName = filter.PropertyName,
                    Value = date.EndOfDay().ToString()
                });
            }
            else if ((filter.Operation == Op.GreaterThan || filter.Operation == Op.LessThanOrEqual))
            {
                filters.Add(new Filter()
                {
                    Operation = filter.Operation,
                    PropertyName = filter.PropertyName,
                    Value = date.EndOfDay().ToString()
                });
            }
            else
            {
                filters.Add(filter);
            }
        }

        #endregion
    }
}
