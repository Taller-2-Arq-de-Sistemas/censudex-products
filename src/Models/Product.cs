using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace censudex_products.src.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string name { get; set; }  = string.Empty;
        public string description { get; set; }  = string.Empty;
        public decimal price { get; set; } 
        public string category { get; set; } = string.Empty;
        public string imageUrl { get; set; } = string.Empty;
        public bool isActive { get; set; } = true;
        public DateTime registrationDate { get; set; } = DateTime.Now;

    }
}