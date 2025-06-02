using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Stockserve.Domain.Dto;
using StockServe.Logic.InterfaceRepository;


namespace Stockserve.UnitTest.FakeRepository
{
    public class FakeTableRepository : ITableRepository 
    {
        public List<TableDto> GetAllTables()
        {
            return new List<TableDto>
            {
                new TableDto { Id = 5, TableNumber = 1 }, // Linked to 3 active dishes (OrderId 1 & 2)
                new TableDto { Id = 6, TableNumber = 2 }  // Linked to 1 active dish (OrderId 4)
            };
        }
    }
}
