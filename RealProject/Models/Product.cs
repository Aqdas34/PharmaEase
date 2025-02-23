namespace RealProject.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Company { get; set; }
        public string? Description { get; set; }
        public string? Dosage { get; set; }

        public IFormFile? Image { get; set; }

        public string? ImagePath { get; set; }


    }
}
