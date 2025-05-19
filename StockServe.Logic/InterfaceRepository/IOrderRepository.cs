using Stockserve.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServe.Logic.InterfaceRepository
{
    public interface IOrderRepository
    {
        List<OrderDto> GetAllOrders();
        void AddOrder(OrderDto order);
        void UpdatePaymentStatus(int tableId, string payStatus);
    }
}