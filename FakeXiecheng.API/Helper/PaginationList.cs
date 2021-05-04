using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Helper
{
    public class PaginationLis<T> : List<T>
    {
        /// <summary>
        /// 页面总量
        /// </summary>
        public int TotalPage { get; set; }
        /// <summary>
        /// 数据库总数据量
        /// </summary>
        public int TotalCount{ get; set; }
        /// <summary>
        /// 判断是否有上一页
        /// </summary>
        public bool HasPrevious => CurrentPage > 1;
        /// <summary>
        /// 判断是否有下一页
        /// </summary>
        public bool HasNext => CurrentPage < TotalPage;
       
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 一页显示数量
        /// </summary>
        public int PageSize { get; set; }

        public PaginationLis(int totalCount, int currentPage,int pageSize,List<T> intems)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            AddRange(intems);
            TotalCount = totalCount;
            TotalPage = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        public  static async Task<PaginationLis<T>> CreateAync(int currentPage, int pageSize,IQueryable<T> result)
        {
            var totalCount = await result.CountAsync();
            //pagination
            //skip
            var skip = (currentPage - 1) * pageSize;
            result = result.Skip(skip);
            //以pagesize为标准显示一定量的数据
            result = result.Take(pageSize);
;
            var items = await result.ToListAsync();
            return new PaginationLis<T>(totalCount,currentPage, pageSize, items);
        }
    }
}
