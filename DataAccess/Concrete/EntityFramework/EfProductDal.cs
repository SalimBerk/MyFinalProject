using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Concrete.EntityFramework
{
	public class EfproductDal : IProductDal
	{
		public void Add(Product product)
		{
			throw new NotImplementedException();
		}

		public void Delete(Product product)
		{
			throw new NotImplementedException();
		}

		public List<Product> GetAll()
		{
			return new List<Product> { new Product { ProductName = "Kedi" }, new Product { ProductName = "Köpek" }, new Product { ProductName = "Maymun" } };
		}

		public List<Product> GetAllByCategory(int categoryId)
		{
			throw new NotImplementedException();
		}

		public void Update(Product product)
		{
			throw new NotImplementedException();
		}
	}
}
