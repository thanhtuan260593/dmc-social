using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ThanhTuan.Blogs.Entities;

namespace ThanhTuan.Blogs.Repositories
{
  public static class Helper
  {
    public static IQueryable<T> ApplyPaging<T>(IQueryable<T> query, PagingParameter<T> pagingParam)
    {
      var chain = query;
      if (pagingParam.OrderBy != null)
      {
        if (pagingParam.OrderDirection == PagingParameter<T>.OrderDirections.ASC)
        {
          chain = chain.OrderBy(pagingParam.OrderBy);
        }
        else
        {
          chain = chain.OrderByDescending(pagingParam.OrderBy);
        }
      }
      chain = chain.Skip(pagingParam.Offset).Take(pagingParam.Limit);
      return chain;
    }

    public static IQueryable<T> ApplyFilters<T>(IQueryable<T> query, Filterable<T> listParams)
    {
      if (listParams == null || listParams.FilterQueries == null) return query;
      foreach (var filter in listParams.FilterQueries)
      {
        query = query.Where(filter);
      }
      return query;
    }
    public static IQueryable<T> ApplyListParam<T>(IQueryable<T> query, ListParameter<T> parameter)
    {
      query = ApplyFilters(query, parameter.Filter);
      query = ApplyPaging(query, parameter.Paging);
      return query;
    }
    public static string NormalizeString(string str)
    {
      if (string.IsNullOrEmpty(str))
        return str;
      var normalized = str;
      normalized = normalized.ToLower();
      normalized = Regex.Replace(normalized, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ", "a");
      normalized = Regex.Replace(normalized, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ", "e");
      normalized = Regex.Replace(normalized, "ì|í|ị|ỉ|ĩ", "i");
      normalized = Regex.Replace(normalized, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ", "o");
      normalized = Regex.Replace(normalized, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ", "u");
      normalized = Regex.Replace(normalized, "ỳ|ý|ỵ|ỷ|ỹ", "y");
      normalized = Regex.Replace(normalized, "đ", "d");
      normalized = Regex.Replace(normalized, "[^a-z0-9]", "_");
      return normalized;
    }

    public static string NormalizeTag(string str)
    {
      return NormalizeString(str).Replace("_", "");
    }
  }
}