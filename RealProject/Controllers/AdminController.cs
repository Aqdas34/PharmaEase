using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PharmaProject.Controllers;
using RealProject.Hubs;
using RealProject.Models;
using RealProject.Models.AdoRepository;
using RealProject.Models.DapperGenericRepo;

namespace RealProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IWebHostEnvironment env;

        IRepository<Product> productRepository;
        private readonly IHubContext<MedicineHub> _hubContext;
        public AdminController(ILogger<AdminController> logger, IWebHostEnvironment env, IRepository<Product> productRepository, IHubContext<MedicineHub> hubContext)
        {
            _logger = logger;
            this.env = env;
            this.productRepository = productRepository;
            _hubContext = hubContext;
        }
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Index()
        {

            //IRepository<Product> repository = new GenericRepository<Product>();
            //List<Product> products = repository.GetAll();
            IRepository<Product> productRepo = productRepository;
            List<Product> products = productRepo.GetAll();
            foreach (var p in products)
            {

                string path = p.ImagePath;

                int index = path.IndexOf("uploaded");

                if (index != -1)
                {
                    // Extract the substring from "\uploaded" to the end
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



        [Authorize(Policy = "AdminPolicy")]
        public IActionResult AddForm()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            string wwwFolder = env.WebRootPath;
            string path = Path.Combine(wwwFolder, "uploaded");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            
            string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(product.Image.FileName)}";


            
            var pathWithFileName = Path.Combine(path, uniqueFileName);
            FileStream filestream = new FileStream(pathWithFileName, FileMode.Create);
            product.Image.CopyTo(filestream);
            filestream.Close();

            product.ImagePath = pathWithFileName;

            IRepository<Product> repository = new GenericRepository<Product>();
            repository.Add(product);
            _hubContext.Clients.All.SendAsync("ReceiveNewMedicine", product.Name, product.Price);
            TempData["SuccessMessage"] = "Medicine Added successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult Show()
        {
            List<Product> products = new List<Product>();
            //ProductRepository productRepository = new ProductRepository();

            //products = productRepository.GetProducts();

            IRepository<Product> productRepo = productRepository;
            products = productRepo.GetAll();


            foreach (var p in products)
            {
                //Console.WriteLine($"p --> {p}");
                string path = p.ImagePath;

                int index = path.IndexOf("uploaded");

                if (index != -1)
                {
                    // Extract the substring from "\uploaded" to the end
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


        [Route("/admin/deleteAction/{id}")]
        public IActionResult DeleteAction(int id)
        {
            //ProductRepository productRepository = new ProductRepository();
            //productRepository.DeleteProduct(id);

            //IRepository<Product> repository = new GenericRepository<Product>();
            //repository.Delete(id);

            IRepository<Product> productRepo = productRepository;
            productRepo.Delete(id);
            return RedirectToAction("Index");
        }

        [Route("/admin/Edit/{id}")]
        public IActionResult Edit(int id)
        {


            //IRepository<Product> repository = new GenericRepository<Product>();
            //Product product = repository.GetById(id);


            IRepository<Product> productRepo = productRepository;
            Product product = productRepo.GetById(id);
            return View(product);
        }

        [HttpPost]
        public IActionResult EditProduct(Product product)
        {

            string wwwFolder = env.WebRootPath;
            string path = Path.Combine(wwwFolder, "uploaded");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = Path.GetFileName(product.Image.FileName);

            string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(product.Image.FileName)}";



            var pathWithFileName = Path.Combine(path, uniqueFileName);
            FileStream filestream = new FileStream(pathWithFileName, FileMode.Create);
            product.Image.CopyTo(filestream);
            filestream.Close();

            product.ImagePath = pathWithFileName;

            //IRepository<Product> repository = new GenericRepository<Product>();
            //repository.Update(product);


            IRepository<Product> productRepo = productRepository;
            productRepo.Update(product);

            TempData["SuccessMessage"] = "Medicine Updated successfully.";
            return RedirectToAction("Index");
        }

    }
}
