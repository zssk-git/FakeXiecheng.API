using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Helper
{
    public class PaginationLis<T>:List<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public PaginationLis(int currentPage,int pageSize,List<T> intems)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            AddRange(intems);
        }

        public  static async Task<PaginationLis<T>> CreateAync(int currentPage, int pageSize,IQueryable<T> result)
        {
            //pagination
            //skip
            var skip = (currentPage - 1) * pageSize;
            result = result.Skip(skip);
            //以pagesize为标准显示一定量的数据
            result = result.Take(pageSize);
;
            var items = await result.ToListAsync();
            return new PaginationLis<T>(currentPage, pageSize, items);
        }
    }
}
