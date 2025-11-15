using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using censudex_products.src.Dtos;
using censudex_products.src.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Grpc.Core;
using productProto;

namespace censudex_products.src.Services
{
    public class ProductGrpcService : ProductService.ProductServiceBase
    {
        private readonly IProductRepository _productRepository;
        private readonly Cloudinary _cloudinary;
        public ProductGrpcService(IProductRepository productRepositorypo, Cloudinary cloudinary)
        {   
            _cloudinary = cloudinary;
            _productRepository = productRepositorypo;
        }

        public override async Task<ProductList> GetAllProducts(Empty request, ServerCallContext context)
        {
            var products = await _productRepository.GetAll();

            var response = new ProductList();
            response.Products.AddRange(products.Select(p => new Product
            {
                Id = p.Id,
                Name = p.name,
                Description = p.description,
                Price = (double)p.price,
                Category = p.category,
                ImageUrl = p.imageUrl,
                IsActive = p.isActive
            }));

            return response;
        }

        public override async Task<Product> GetProductById(ProductIdRequest request, ServerCallContext context)
        {
            // Validar ID
            if (string.IsNullOrWhiteSpace(request.Id))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "El ID es requerido."));

            var p = await _productRepository.GetById(request.Id);

            return new Product
            {
                Id = p.Id,
                Name = p.name,
                Description = p.description,
                Price = (double)p.price,
                Category = p.category,
                ImageUrl = p.imageUrl,
                IsActive = p.isActive
            };
        }
        
        public override async Task<ProductResponse> CreateProduct(NewProductRequest request, ServerCallContext context)
        {
            
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Description) || string.IsNullOrEmpty(request.Category))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Nombre, descripcion y categoria son requeridos."));

            if (request.Image.Length == 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "La imagen es requerida."));

            if (request.Image.Length > 5 * 1024 * 1024)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "La imagen debe ser menor a 5MB."));

            if (request.Price <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "El precio debe ser mayor a 0."));

            // Convertir bytes a stream
            using var stream = new MemoryStream(request.Image.ToByteArray());
            string nameFolder = Environment.GetEnvironmentVariable("CLODINARY_FOLDER_NAME") ?? "censudesProduct";
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(request.Name, stream),
                Folder = nameFolder
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                throw new RpcException(new Status(StatusCode.Internal, uploadResult.Error.Message));

            var newProduct = new Models.Product
            {
                name = request.Name,
                description = request.Description,
                price = (decimal)request.Price,
                category = request.Category,
                imageUrl = uploadResult.SecureUrl.AbsoluteUri
            };

            await _productRepository.Add(newProduct);

            return new ProductResponse
            {
                Success = true,
                Message = "Producto creado con éxito",
                Product = new Product
                {
                    Id = newProduct.Id,
                    Name = newProduct.name,
                    Description = newProduct.description,
                    Price = (double)newProduct.price,
                    Category = newProduct.category,
                    ImageUrl = newProduct.imageUrl
                }
            };
        }


        public override async Task<ProductResponse> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
        {
            // Validar ID
            if (string.IsNullOrWhiteSpace(request.Id))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "El ID es requerido."));

            // Construir DTO
            var updateProduct = new UpdateProductDto
            {
                name = request.Name,
                description = request.Description,
                category = request.Category,
                price = request.Price == 0 ? null : (decimal?)request.Price,
                imageUrl = null 
            };

            // Si hay imagen
            if (request.Image.Length > 0)
            {
                if (request.Image.Length > 5 * 1024 * 1024)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "La imagen no puede exceder 5MB."));

                using var stream = new MemoryStream(request.Image.ToByteArray());

                var folder = Environment.GetEnvironmentVariable("CLOUDINARY_FOLDER_NAME") ?? "censudexProduct";
                
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(request.Name ?? "prod_img", stream),
                    Folder = folder
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                    throw new RpcException(new Status(StatusCode.Internal, uploadResult.Error.Message));

                updateProduct.imageUrl = uploadResult.SecureUrl.ToString();
            }

            
            bool result = await _productRepository.Update(request.Id, updateProduct);

            if (!result)
            {
                return new ProductResponse
                {
                    Success = false,
                    Message = "No se actualizó ningún producto."
                };
            }

            // Obtener producto actualizado para enviarlo de vuelta
            var updatedEntity = await _productRepository.GetById(request.Id);

            return new ProductResponse
            {
                Success = true,
                Message = "Producto actualizado con éxito",
                Product = new Product
                {
                    Id = updatedEntity.Id,
                    Name = updatedEntity.name,
                    Description = updatedEntity.description,
                    Price = (double)updatedEntity.price,
                    Category = updatedEntity.category,
                    ImageUrl = updatedEntity.imageUrl,
                    IsActive = updatedEntity.isActive
                }
            };
        }
        
        

        public override async Task<ProductResponse> SoftDeleteProduct(ProductIdRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Id))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "El ID es requerido."));

            // Verificar si el producto existe
            var product = await _productRepository.GetById(request.Id);

            if (product == null)
            {
                return new ProductResponse
                {
                    Success = false,
                    Message = "El producto no existe."
                };
            }

            // Aplicar soft delete
            var result = await _productRepository.DeleteSoft(request.Id);

            if (!result)
            {
                return new ProductResponse
                {
                    Success = false,
                    Message = "No se pudo desactivar el producto."
                };
            }

            // Obtener producto actualizado
            var updated = await _productRepository.GetById(request.Id);

            return new ProductResponse
            {
                Success = true,
                Message = "Producto desactivado correctamente.",
                Product = new Product
                {
                    Id = updated.Id,
                    Name = updated.name,
                    Description = updated.description,
                    Price = (double)updated.price,
                    Category = updated.category,
                    ImageUrl = updated.imageUrl,
                    IsActive = updated.isActive
                }
            };
        }


    }
}