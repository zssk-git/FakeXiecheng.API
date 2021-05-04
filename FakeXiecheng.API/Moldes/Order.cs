using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Moldes
{
    public enum OrderStateEnum
    {
        Peding,//订单已生成
        Processing,//支付处理中
        Completed,//交易成功
        Declined,//交易失败
        Cancelled,//订单取消
        Refund,//已退款
    }
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public  ICollection<LineItem> OrderItems { get; set; }

        public OrderStateEnum State { get; set; }
        public DateTime CraeteDateUTC { get; set; }
        public string TransactionMetadata { get; set; }
    }
}
