using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PharmaProject.Controllers;
using RealProject.Models;
using RealProject.Models.DapperGenericRepo;
using RealProject.Models.ViewModel;


namespace PharmaProject.Controllers
{
    public class OrderController : Controller
    {

        private readonly IRepository<BillingDetails> _billingDetailsRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Order> _orderRepository;

        public OrderController(IRepository<BillingDetails> billingDetailsRepository, IRepository<Product> productRepository, IRepository<Order> orderRepository)
        {
            this._billingDetailsRepository = billingDetailsRepository;
            this._productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        //[Route("/order/AddToCart/{id}")]
        //public IActionResult AddToCart(int id)
        //{
        //    IRepository<Product> repository = new GenericRepository<Product>();
        //    Product product = repository.GetById(id);

        //    string cartJson = string.Empty;
        //    List<Product> cart = new List<Product>();
        //    try
        //    {
        //        if (!HttpContext.Request.Cookies.ContainsKey("Cart"))
        //        {
        //            cart = new List<Product>();

        //        }
        //        else
        //        {
        //            var cartCookie = HttpContext.Request.Cookies["Cart"];
        //            cart = JsonConvert.DeserializeObject<List<Product>>(cartCookie) ?? new List<Product>();
        //        }
        //        cart.Add(product);
        //        cartJson = JsonConvert.SerializeObject(cart);
        //        CookieOptions options = new CookieOptions
        //        {
        //            HttpOnly = true,
        //            Secure = HttpContext.Request.IsHttps // Set Secure to true only if HTTPS is used
        //        };
        //        HttpContext.Response.Cookies.Append("Cart", cartJson, options);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    return RedirectToAction("Cart");
        //}


        public IActionResult Cart()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Message"] = "Please log in to view your cart.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                TempData["Message"] = "User ID not found.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            string cartSessionKey = $"Cart_{userId}";
            string detailSessionKey = $"Detail_{userId}";


            var products = HttpContext.Session.GetString(cartSessionKey) != null
                           ? JsonConvert.DeserializeObject<List<Product>>(HttpContext.Session.GetString(cartSessionKey))
                           : new List<Product>();
            var cartDetails = HttpContext.Session.GetString(detailSessionKey) != null
                              ? JsonConvert.DeserializeObject<List<Cart>>(HttpContext.Session.GetString(detailSessionKey))
                              : new List<Cart>();

            decimal total = 0;
            for(int i=0;i<products.Count;i++)
            {
                total += products[i].Price * cartDetails[i].Quantity;
            }
            var cartViewModel = new CartViewModel
            {
                Products = products,
                CartDetails = cartDetails,
                Total = total
            };

            return View(cartViewModel);
        }


        [Route("/order/Delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Message"] = "Please log in to view your cart.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                TempData["Message"] = "User ID not found.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            string cartSessionKey = $"Cart_{userId}";
            string detailSessionKey = $"Detail_{userId}";

            var products = HttpContext.Session.GetString(cartSessionKey) != null
                           ? JsonConvert.DeserializeObject<List<Product>>(HttpContext.Session.GetString(cartSessionKey))
                           : new List<Product>();

            var cartDetails = HttpContext.Session.GetString(detailSessionKey) != null
                              ? JsonConvert.DeserializeObject<List<Cart>>(HttpContext.Session.GetString(detailSessionKey))
                              : new List<Cart>();

            // Find the product index
            int productIndex = products.FindIndex(p => p.Id == id);
            if (productIndex != -1)
            {
                // Remove product and its corresponding cart detail
                products.RemoveAt(productIndex);
                cartDetails.RemoveAt(productIndex);

                // Save the updated lists back to the session
                HttpContext.Session.SetString(cartSessionKey, JsonConvert.SerializeObject(products));
                HttpContext.Session.SetString(detailSessionKey, JsonConvert.SerializeObject(cartDetails));
            }

            return RedirectToAction("Cart");
        }


        [HttpPost]
        public IActionResult AddToCart(int id, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Message"] = "Please log in to add items to your cart.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                TempData["Message"] = "User ID not found.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            //IRepository<Product> repository = new GenericRepository<Product>();
            //Product product = repository.GetById(id);

            IRepository<Product> repository = _productRepository;
            Product product = repository.GetById(id);
           
            string path = product.ImagePath;
            int index = path.IndexOf("uploaded");

            if (index != -1)
            {
                string extractedString = path.Substring(index).Replace('\\', '/');
                string initialPath = "/";
                extractedString = initialPath + extractedString;
                product.ImagePath = extractedString;
            }
            else
            {
                Console.WriteLine("Substring '\\uploaded' not found in the path.");
            }

            List<Product> cart = new List<Product>();
            List<Cart> cartDetail = new List<Cart>();

            try
            {
                string cartSessionKey = $"Cart_{userId}";
                string detailSessionKey = $"Detail_{userId}";

                if (HttpContext.Session.GetString(cartSessionKey) != null)
                {
                    cart = JsonConvert.DeserializeObject<List<Product>>(HttpContext.Session.GetString(cartSessionKey)) ?? new List<Product>();
                }

                if (HttpContext.Session.GetString(detailSessionKey) != null)
                {
                    cartDetail = JsonConvert.DeserializeObject<List<Cart>>(HttpContext.Session.GetString(detailSessionKey)) ?? new List<Cart>();
                }

                bool productExistsInCart = cart.Any(p => p.Id == product.Id);
                if (productExistsInCart)
                {
                    foreach (var p in cartDetail)
                    {
                        if (p.ProductId == id)
                        {
                            p.Quantity = quantity;
                        }
                    }
                }
                else
                {
                    cart.Add(product);
                    cartDetail.Add(new Cart { ProductId = id, Quantity = quantity });
                }

                HttpContext.Session.SetString(cartSessionKey, JsonConvert.SerializeObject(cart));
                HttpContext.Session.SetString(detailSessionKey, JsonConvert.SerializeObject(cartDetail));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Cart");
        }






                public IActionResult Update(List<Cart> products)
        {
            return RedirectToAction("Cart");
        }




       
        public IActionResult OrderProceed()
        {
            return View();  
        }
        public IActionResult Checkout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Message"] = "Please log in to proceed to checkout.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                TempData["Message"] = "User ID not found.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            string cartSessionKey = $"Cart_{userId}";
            string detailSessionKey = $"Detail_{userId}";

            var products = GetSessionData<List<Product>>(cartSessionKey) ?? new List<Product>();
            var cartDetails = GetSessionData<List<Cart>>(detailSessionKey) ?? new List<Cart>();

            decimal total = products.Zip(cartDetails, (product, detail) => product.Price * detail.Quantity).Sum();

            var checkoutViewModel = new CartViewModel
            {
                Products = products,
                CartDetails = cartDetails,
                Total = total
            };

            return View(checkoutViewModel);
        }

        private T GetSessionData<T>(string key)
        {
           
            var value = HttpContext.Session.GetString(key);
            return value != null ? JsonConvert.DeserializeObject<T>(value) : default;
        }


        [HttpPost]
        public IActionResult PlaceOrder()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            BillingDetails billingDetails = new BillingDetails {
                UserID = userId,
                Country = Request.Form["c_country"],
                FirstName = Request.Form["c_fname"],
                LastName = Request.Form["c_lname"],
                Address = Request.Form["c_address"],
                State = Request.Form["c_state"],
                Email = Request.Form["c_email_address"],
                Phone = Request.Form["c_phone"],
                Note = Request.Form["c_order_notes"]
            };
            //IRepository<BillingDetails> repository = new GenericRepository<BillingDetails>();
            //repository.Add(billingDetails);
            IRepository<BillingDetails> repository = _billingDetailsRepository;
            repository.Add(billingDetails);


            



            return RedirectToAction("OrderProceed");
            
        }
    }
}
