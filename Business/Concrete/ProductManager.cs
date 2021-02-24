using Business.Abstract;
using Business.CCS;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Validation;
using Core.Results;
using Core.Utilities;
using Core.Utilities.Business;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using DataAccess.Concrete.InMemory;
using Entities.Concrete;
using Entities.DTOs;
using FluentValidation;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Result = Core.Results.Result;

namespace Business.Concrete
{
	public class ProductManager : IProductService
	{
		IProductDal _productDal;
		ICategoryService _categoryService;
		private EfProductDal efProductDal;
		private CategoryManager categoryManager;

		public ProductManager(IProductDal productDal,ILogger logger,ICategoryService categoryService)
		{
			_productDal = productDal;
			_categoryService = categoryService;
			
		}

		public ProductManager(EfProductDal efProductDal, CategoryManager categoryManager)
		{
			this.efProductDal = efProductDal;
			this.categoryManager = categoryManager;
		}

		[ValidationAspect(typeof(ProductValidator))]
		public IResult Add(Product product)
		{
			IResult result=BusinessRules.Run(CheckIfProductNameExists(product.ProductName), CheckIfProductCountOfCategoryCorrect(product.CategoryId),CheckIfCategoryLimitExceded());
			if (result!=null)
			{
				return result;

			}
			if (CheckIfProductCountOfCategoryCorrect(product.CategoryId).Success)
			{
				if (CheckIfProductNameExists(product.ProductName).Success)
				{
					_productDal.Add(product);
					return new SuccessResult(Messages.ProductAdded);

				}
				
			}
			
			_productDal.Add(product);
			return new SuccessResult(Messages.ProductAdded);
		}

		public IDataResult<List<Product>> GetAll()
		{
			return new SuccessDataResult<List<Product>>(_productDal.GetAll(),Messages.ProductsListed);
			
			
		}

		public IDataResult<List<Product>> GetAllByCategoryId(int Id)
		{
			return new SuccessDataResult<List<Product>>(_productDal.GetAll(p => p.CategoryId == Id));
		}

		public IDataResult<Product> GetById(int productId)
		{
			return new SuccessDataResult<Product>( _productDal.Get(p => p.ProductId == productId));
		}

		public IDataResult<List<Product>> GetByUnitPrice(decimal min, decimal max)
		{
			return new SuccessDataResult<List<Product>>(_productDal.GetAll(P => P.UnitPrice>=min && P.UnitPrice <= max));
		}

		public IDataResult<List<ProductDetailDto>> GetProductDetails()
		{
			if (DateTime.Now.Hour == 23)
			{
				return new ErrorDataResult<List<ProductDetailDto>>(Messages.MaintenanceTime);
			}
			return new SuccessDataResult<List<ProductDetailDto>>(_productDal.GetProductDetails());

		}

		[ValidationAspect(typeof(ProductValidator))]
		public IResult Update(Product product)
		{

			var result = _productDal.GetAll(p => p.CategoryId == product.CategoryId).Count;
			if (result >= 10)
			{
				return new ErrorResult(Messages.ProductCountsError);

			}
			throw new NotImplementedException();
		}
		private IResult CheckIfProductCountOfCategoryCorrect(int categoryId)
		{
			var result = _productDal.GetAll(p => p.CategoryId ==categoryId).Count;
			if (result >= 15)
			{
				return new ErrorResult(Messages.ProductCountsError);

			}
			return new SuccessResult();
		}
		private IResult CheckIfProductNameExists(string productname)
		{
			var result = _productDal.GetAll(p => p.ProductName == productname).Any();
			if (result)
			{
				return new ErrorResult(Messages.ProductNameAllReadyExists);

			}
			return new SuccessResult();
		}
		private IResult CheckIfCategoryLimitExceded()
		{
			var result = _categoryService.GetAll();
			if (result.Data.Count>15)
			{
				return new ErrorResult(Messages.CategoryLimitExceded);

			}
			return new SuccessResult();
			;
		}

	}
}
