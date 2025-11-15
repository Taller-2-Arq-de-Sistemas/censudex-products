using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using censudex_products.src.Dtos;
using censudex_products.src.Models;

namespace censudex_products.src.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> Add(Product product);
        Task<List<Product>> GetAll();
        Task<Product> GetById(string id);
        Task<bool> Update(string id,UpdateProductDto  product);
        Task<bool> DeleteSoft(string id); 
    }
}