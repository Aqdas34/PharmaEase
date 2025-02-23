using Microsoft.AspNetCore.Mvc;
using RealProject.Models;
using System.Collections.Generic;
using System.Linq;

namespace PharmaProject.Controllers
{
    public class StoreController : Controller
    {
        private readonly IRepository<Product> _productRepository;

        public StoreController(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }


        public IActionResult MyView(string searchQuery, string searchBy)
        {
            List<Product> products = _productRepository.GetAll();

            // If searchQuery is not null or empty, filter the products
            if (!string.IsNullOrEmpty(searchQuery))
            {
                if (searchBy == "name")
                {
                    products = products.Where(p => p.Name.StartsWith(searchQuery, StringComparison.InvariantCultureIgnoreCase)).ToList();
                }
                else
                {
                    if (int.TryParse(searchQuery, out int priceSearch))
                    {

                        products = products.Where(p => (p.Price <= priceSearch)).ToList();
                    }
                }
                //products = products.Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                //                            || p.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            }




            // Modify ImagePath as before
            foreach (var p in products)
            {
                string path = p.ImagePath;
                int index = path.IndexOf("uploaded");

                if (index != -1)
                {
                    string extractedString = path.Substring(index).Replace('\\', '/');
                    p.ImagePath = "/" + extractedString;
                }
                else
                {
                    Console.WriteLine("Substring '\\uploaded' not found in the path.");
                }
            }

            return PartialView(products);
        }

        public IActionResult Store(string searchQuery, string searchBy)
        {
            // Get all products
            List<Product> products = _productRepository.GetAll();

            // If searchQuery is not null or empty, filter the products
            if (!string.IsNullOrEmpty(searchQuery))
            {
                if (searchBy == "name")
                {
                    products = products.Where(p => p.Name.Contains(searchQuery, StringComparison.InvariantCultureIgnoreCase)).ToList();
                }
                else
                {
                    if(int.TryParse(searchQuery, out int priceSearch))
                    {

                    products = products.Where(p => (p.Price ==  priceSearch)).ToList();
                    }
                }
                //products = products.Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                //                            || p.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            

            

            // Modify ImagePath as before
            foreach (var p in products)
            {
                string path = p.ImagePath;
                int index = path.IndexOf("uploaded");

                if (index != -1)
                {
                    string extractedString = path.Substring(index).Replace('\\', '/');
                    p.ImagePath = "/" + extractedString;
                }
                else
                {
                    Console.WriteLine("Substring '\\uploaded' not found in the path.");
                }
            }

            return View(products);
        }
    }
}
