using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace censudex_products.src.Dtos
{
    public class UpdateProductDto
    {
        public string? name { get; set; }  = string.Empty;
        public string? description { get; set; }  = string.Empty;
        public decimal? price { get; set; } 
        public string? category { get; set; } = string.Empty;
        public string? imageUrl { get; set; } = string.Empty;
        

    }
}