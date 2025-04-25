using StockServe.Data;

namespace StockServe.Logic
{
    public class TableService
    {
        public List<Table> GetAllTables()
        {
            List<TableDto> tableDtos = new TableRepository().GetAllTables();
            List<Table> tables = new List<Table>();
            foreach (var tableDto in tableDtos)
            {
                tables.Add(new Table
                {
                    Id = tableDto.Id,
                    TableNumber = tableDto.TableNumber
                });
            }
            return tables; 
        }
    }
}
