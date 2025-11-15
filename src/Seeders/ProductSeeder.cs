using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using censudex_products.src.Models;
using MongoDB.Driver;

namespace censudex_products.src.Seeders
{
    public class ProductSeeder
    {
        private readonly IMongoCollection<Product> _products;

        public ProductSeeder(IMongoDatabase database)
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

        public async Task SeedAsync()
        {
            // Verificar si ya existen productos
            long count = await _products.CountDocumentsAsync(_products => true);

            if (count > 0)
                return;

            var products = new List<Product>
            {
                new Product
                {
                    name = "Teclado Mecánico RGB",
                    description = "Teclado mecánico retroiluminado con switches azules.",
                    price = 39990,
                    category = "Periféricos",
                    imageUrl = "https://example.com/teclado.jpg"
                },
                new Product
                {
                    name = "Mouse Gamer 7200 DPI",
                    description = "Mouse ergonómico con 7 botones programables.",
                    price = 19990,
                    category = "Periféricos",
                    imageUrl = "https://example.com/mouse.jpg"
                },
                new Product
                {
                    name = "Monitor 27 144Hz",
                    description = "Monitor Full HD de 27 pulgadas con 144Hz.",
                    price = 189990,
                    category = "Monitores",
                    imageUrl = "https://example.com/monitor.jpg"
                },
                new Product {
                    name = "Audífonos Bluetooth Pro",
                    description = "Audífonos inalámbricos con cancelación de ruido.",
                    price = 39990,
                    category = "Tecnología",
                    imageUrl = "https://example.com/audifonos.jpg"
                },
                new Product {
                    name = "Teclado Mecánico 2 RGB",
                    description = "Teclado mecánico con switches rojos y retroiluminación.",
                    price = 54990,
                    category = "Tecnología",
                    imageUrl = "https://example.com/teclado.jpg"
                },
                new Product {
                    name = "Silla Gamer ErgoX",
                    description = "Silla gamer ergonómica con soporte lumbar.",
                    price = 129990,
                    category = "Hogar",
                    imageUrl = "https://example.com/silla.jpg"
                },
                new Product {
                    name = "Polera Oversize Negra",
                    description = "Polera básica oversize de algodón.",
                    price = 9990,
                    category = "Ropa",
                    imageUrl = "https://example.com/polera.jpg"
                },
                new Product {
                    name = "Zapatillas UrbanStreet",
                    description = "Zapatillas urbanas ligeras y cómodas.",
                    price = 45990,
                    category = "Ropa",
                    imageUrl = "https://example.com/zapatillas.jpg"
                },
                new Product {
                    name = "Mesa de Centro Minimalista",
                    description = "Mesa de centro de madera estilo nórdico.",
                    price = 74990,
                    category = "Hogar",
                    imageUrl = "https://example.com/mesa.jpg"
                },
                new Product {
                    name = "Lampara LED Smart",
                    description = "Lámpara inteligente compatible con Google Home.",
                    price = 19990,
                    category = "Hogar",
                    imageUrl = "https://example.com/lampara.jpg"
                },
                new Product {
                    name = "Botella Térmica 1L",
                    description = "Botella de acero inoxidable mantiene temperatura por 12h.",
                    price = 12990,
                    category = "Accesorios",
                    imageUrl = "https://example.com/botella.jpg"
                },
                new Product {
                    name = "Mochila Anti-Robo Pro",
                    description = "Mochila con cierre oculto y puerto USB.",
                    price = 34990,
                    category = "Accesorios",
                    imageUrl = "https://example.com/mochila.jpg"
                },
                new Product {
                    name = "Mouse Gamer 2 7200 DPI",
                    description = "Mouse gamer con iluminación RGB y 7 botones.",
                    price = 15990,
                    category = "Tecnología",
                    imageUrl = "https://example.com/mouse.jpg"
                },
                new Product {
                    name = "Monitor 27'' Full HD",
                    description = "Monitor con panel IPS y alta fidelidad de color.",
                    price = 139990,
                    category = "Tecnología",
                    imageUrl = "https://example.com/monitor.jpg"
                },
                new Product {
                    name = "Hoodie Classic Gris",
                    description = "Sudadera con capucha, unisex, 100% algodón.",
                    price = 24990,
                    category = "Ropa",
                    imageUrl = "https://example.com/hoodie.jpg"
                },
                new Product {
                    name = "Toalla Microfibra XL",
                    description = "Toalla ultra absorbente ideal para deporte.",
                    price = 8990,
                    category = "Accesorios",
                    imageUrl = "https://example.com/toalla.jpg"
                },
                new Product {
                    name = "Colchoneta Yoga Premium",
                    description = "Colchoneta antideslizante de alta densidad.",
                    price = 16990,
                    category = "Deporte",
                    imageUrl = "https://example.com/yoga.jpg"
                },
                new Product {
                    name = "Set Mancuernas Ajustables",
                    description = "Mancuernas de 2 a 24kg con sistema ajustable.",
                    price = 89990,
                    category = "Deporte",
                    imageUrl = "https://example.com/mancuernas.jpg"
                },
                new Product {
                    name = "Cámara Web Full HD",
                    description = "Cámara para streaming con micrófono integrado.",
                    price = 22990,
                    category = "Tecnología",
                    imageUrl = "https://example.com/camara.jpg"
                },
                new Product {
                    name = "Smartwatch FitBand X",
                    description = "Smartwatch con medidor cardíaco y GPS.",
                    price = 49990,
                    category = "Tecnología",
                    imageUrl = "https://example.com/smartwatch.jpg"
                }

            };

            await _products.InsertManyAsync(products);
        }
    }
}