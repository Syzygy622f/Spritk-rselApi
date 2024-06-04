using DtoModels;
using Models;

namespace Businesslayer
{
    public interface IRepository
    {
        Task<int> CreateCustomerAsync(Customer customer);
        Task<int> CreateInvoice(List<int> productsId, string mail);
        Task<List<DtoProductInventory>> GetAll();
        Task<bool> getCostumerAsynce(string mail);
        Task<DtoProductInventory> GetProductByIdAsync(int id);
    }
}