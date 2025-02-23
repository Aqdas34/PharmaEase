using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using RealProject.Models;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;
using RealProject.Models.DapperGenericRepo;

namespace PharmaProject.Controllers
{

    //[Authorize (Policy ="BusinessPolicy")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment env;
        IRepository<Product> productRepository;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env,IRepository<Product> productRepository)
        {
            _logger = logger;
            this.env = env;
            this.productRepository = productRepository;

        }

        public IActionResult Index()
        {



            //IRepository<Product> repository = new GenericRepository<Product>();
            //List<Product> products = repository.GetAll();
            IRepository<Product> productRepo = this.productRepository;
            List<Product> products = productRepo.GetAll();
            foreach (var p in products)
            {
                string path = p.ImagePath;

                int index = path.IndexOf("uploaded");

                if (index != -1)
                {
              
                    string extractedString = path.Substring(index);

                    extractedString = extractedString.Replace('\\', '/');
                    string initialPath = "/";
                    extractedString = initialPath + extractedString;


                    p.ImagePath = extractedString;

                }
                else
                {
                    Console.WriteLine("Substring '\\uploaded' not found in the path.");
                }
            }





            return View(products);
             
        }



       




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
