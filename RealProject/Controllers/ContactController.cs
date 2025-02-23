using Microsoft.AspNetCore.Mvc;
using RealProject.Models;
using RealProject.Models.DapperGenericRepo;

namespace PharmaProject.Controllers
{
    public class ContactController : Controller
    {
        IRepository<Contact> _contactRepository;
        public ContactController(IRepository<Contact> contactRepository) {
            _contactRepository = contactRepository;
        }
        public IActionResult Contact()
        {
            return View();
        }


        [HttpPost]
        public IActionResult GetContact()
        {
            //var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;


            Contact contact = new Contact
            {
                //UserID = userId,
                FirstName = Request.Form["c_fname"],
                LastName = Request.Form["c_lname"],
                Email = Request.Form["c_email"],
                Subject = Request.Form["c_subject"],
                Message = Request.Form["c_message"]
            };


            //IRepository<Contact> repository = new GenericRepository<Contact>();
            //repository.Add(contact); 

            IRepository<Contact> contactRepository = _contactRepository;
            _contactRepository.Add(contact);
            TempData["SuccessMessage"] = "Contact information added successfully!";
            
            return RedirectToAction("Contact");
        }

        
    }
}
