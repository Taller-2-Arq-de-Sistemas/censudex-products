using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using censudex_products.src.Dtos;
using censudex_products.src.Interfaces;
using censudex_products.src.Models;
using MongoDB.Driver;

namespace censudex_products.src.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;

        public ProductRepository(IMongoDatabase database)
        {
            
            string nameColection = Environment.GetEnvironmentVariable("COLECTION_NAME") ?? "product";
            _products = database.GetCollection<Product>(nameColection);



            var indexName = Builders<Product>.IndexKeys.Ascending(p => p.name);

            var nameIndexOptions = new CreateIndexOptions
            {
                Unique = true,
                Name = "ux_product_name"
            };

            var nameIndexModel = new CreateIndexModel<Product>(indexName, nameIndexOptions);

            _products.Indexes.CreateOne(nameIndexModel);


        }

        public async Task<Product> Add(Product product)
        {
            try
            {
                await _products.InsertOneAsync(product);
                return product;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new Exception("Error: Ya existe un producto con ese nombre.");
            }
        }

        public async Task<bool> DeleteSoft(string id)
        {
            var filter = Builders<Product>.Filter.Eq(t => t.Id, id);

            var combinedUpdate = Builders<Product>.Update.Combine(
                Builders<Product>.Update.Set("isActive", false)
            );
            
            var result = await _products.UpdateOneAsync(filter, combinedUpdate);

            return result.ModifiedCount > 0;
        }

        public async Task<List<Product>> GetAll(string? category, string? name)
        {
            var filters = new List<FilterDefinition<Product>>();
            var builder = Builders<Product>.Filter;


            if (!string.IsNullOrEmpty(category))
            {
                filters.Add(builder.Eq(t => t.category, category));
            }

            if (!string.IsNullOrEmpty(name))
            {
                filters.Add(builder.Eq(t => t.name, name)); 
                // Nota: .Date asegura que no compare con la hora
            }

         

            var filter = filters.Any() ? builder.And(filters) : builder.Empty;

            return await _products.Find(filter).ToListAsync();
        }

        public async Task<Product> GetById(string id)
        {
            var filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Eq(t => t.Id, id),
                Builders<Product>.Filter.Eq(t => t.isActive, true)
            );
            var result =  await _products.Find(filter).FirstOrDefaultAsync();
            return result;
        }

        public async Task<bool> Update(string id, UpdateProductDto product)
        {
            var filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Eq(p => p.Id, id),
                Builders<Product>.Filter.Eq(p => p.isActive, true)
            );



            var updates = new List<UpdateDefinition<Product>>();

            if (product.name != null)
                updates.Add(Builders<Product>.Update.Set(p => p.name, product.name));

            if (product.description != null)
                updates.Add(Builders<Product>.Update.Set(p => p.description, product.description));

            if (product.category != null)
                updates.Add(Builders<Product>.Update.Set(p => p.category, product.category));

            if (product.price.HasValue)
                updates.Add(Builders<Product>.Update.Set(p => p.price, product.price));

            if (!updates.Any())
                return false; // No hay nada que actualizar

            var combinedUpdate = Builders<Product>.Update.Combine(updates);

            try
            {
                var result = await _products.UpdateOneAsync(filter, combinedUpdate);
                return result.ModifiedCount > 0;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new InvalidOperationException("Error: Ya existe un producto con ese nombre.", ex);
            }
        }
    }
}