using database;
using DtoModels;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Businesslayer
{
    public class Repository : IRepository
    {
        private Db db { get; set; }

        public Repository()
        {
            db = new Db();
        }

        public async Task<List<DtoProductInventory>> GetAll()
        {
            return await db.GetAll();
        }

        public async Task<DtoProductInventory> GetProductByIdAsync(int id)
        {
            return await db.GetProductByIdAsync(id);
        }
        public async Task<int> CreateCustomerAsync(Customer customer)
        {
            return await db.CreateCustomerAsync(customer);
        }
    }
}
