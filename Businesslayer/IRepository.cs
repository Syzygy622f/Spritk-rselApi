using DtoModels;
using Models;

namespace Businesslayer
{
    public interface IRepository
    {
        Task<int> CreateCustomerAsync(Customer customer);
        Task<List<DtoProductInventory>> GetAll();
        Task<DtoProductInventory> GetProductByIdAsync(int id);
    }
}