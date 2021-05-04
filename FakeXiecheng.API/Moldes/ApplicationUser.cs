using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Moldes
{
    public class ApplicationUser:IdentityUser
    {
        /// <summary>
        /// 地址信息
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 购物车
        /// </summary>
        public ShoppingCart ShoppingCart { get; set; }
        /// <summary>
        /// 订单
        /// </summary>
        public ICollection<Order> Orders { get; set; }
        /// <summary>
        ///用户角色
        /// </summary>
        public virtual ICollection<IdentityUserRole<string>> UserRoles{ get; set; }
        /// <summary>
        /// 用户权限声明
        /// </summary>
        public virtual ICollection<IdentityUserClaim<string>> Claims{ get; set; }
        /// <summary>
        /// 用户第三方登录信息
        /// </summary>
        public virtual ICollection<IdentityUserLogin<string>> Logins{ get; set; }
        /// <summary>
        /// 用户登录的session
        /// </summary>
        public virtual ICollection<IdentityUserToken<string>> Tokens{ get; set; }

    }
}
