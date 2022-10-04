using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Business.Interfaces;
using System.Threading.Tasks;
using Business.Models;
using Data.Interfaces;
using AutoMapper;
using Data.Entities;
using Business.Validation;

namespace Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddAsync(ProductModel model)
        {
            ProductModelVaild(model);

            var category = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(model.ProductCategoryId);
            if (category == null)
            {
                category = new ProductCategory()
                {
                    Id = model.ProductCategoryId,
                    CategoryName = model.CategoryName
                };
                await _unitOfWork.ProductCategoryRepository.AddAsync(category);
            }
            var product = _mapper.Map<Product>(model);
            product.Category = category;

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveAsync();
        }

        public async Task AddCategoryAsync(ProductCategoryModel categoryModel)
        {
            ProductCategoryModelVaild(categoryModel);

            await _unitOfWork.ProductCategoryRepository.AddAsync(_mapper.Map<ProductCategory>(categoryModel));
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            if (modelId < 0) throw new MarketException("modelId must be greater than 0");

            await _unitOfWork.ProductRepository.DeleteByIdAsync(modelId);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<ProductModel>> GetAllAsync()
        {
            var products = await _unitOfWork.ProductRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<ProductModel>>(products);
        }

        public async Task<IEnumerable<ProductCategoryModel>> GetAllProductCategoriesAsync()
        {
            var productCategories = await _unitOfWork.ProductCategoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductCategoryModel>>(productCategories);
        }

        public async Task<IEnumerable<ProductModel>> GetByFilterAsync(FilterSearchModel filterSearch)
        {
            if (filterSearch == null) throw new MarketException("filterSearch is null");

            var products = await _unitOfWork.ProductRepository.GetAllWithDetailsAsync();
            var filterProducts = filterSearch.CategoryId != null ? products.Where(p => p.ProductCategoryId == filterSearch.CategoryId) : products;
            filterProducts = filterSearch.MinPrice != null ? filterProducts.Where(p => p.Price >= filterSearch.MinPrice) : filterProducts;
            filterProducts = filterSearch.MaxPrice != null ? filterProducts.Where(p => p.Price <= filterSearch.MaxPrice) : filterProducts;
            return _mapper.Map<IEnumerable<ProductModel>>(filterProducts);
        }

        public async Task<ProductModel> GetByIdAsync(int id)
        {
            if (id < 0) throw new MarketException("id must be greater than 0");

            var product = await _unitOfWork.ProductRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<ProductModel>(product);
        }

        public async Task RemoveCategoryAsync(int categoryId)
        {
            if (categoryId < 0) throw new MarketException("categoryId must be greater than 0");

            await _unitOfWork.ProductCategoryRepository.DeleteByIdAsync(categoryId);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(ProductModel model)
        {
            ProductModelVaild(model);

            var product = _mapper.Map<Product>(model);
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateCategoryAsync(ProductCategoryModel categoryModel)
        {
            ProductCategoryModelVaild(categoryModel);

            var productCategory = _mapper.Map<ProductCategory>(categoryModel);
            _unitOfWork.ProductCategoryRepository.Update(productCategory);
            await _unitOfWork.SaveAsync();
        }

        private void ProductModelVaild(ProductModel model)
        {
            if (model == null) throw new MarketException("model is null");
            if (model.Id < 0) throw new MarketException("model.Id must be greater than 0");
            if (model.ProductCategoryId < 0) throw new MarketException("model.ProductCategoryId must be greater than 0");
            //if (string.IsNullOrEmpty(model.CategoryName)) throw new MarketException("model.CategoryName is null or empty");
            if (string.IsNullOrEmpty(model.ProductName)) throw new MarketException("model.ProductName is null or empty");
            if (model.Price < 0) throw new MarketException("model.Price must be greater than 0");
        }

        private void ProductCategoryModelVaild(ProductCategoryModel model)
        {
            if (model == null) throw new MarketException("model is null");
            if (model.Id < 0) throw new MarketException("model.Id must be greater than 0");
            if (string.IsNullOrEmpty(model.CategoryName)) throw new MarketException("model.CategoryName is null or empty");
        }
    }
}
