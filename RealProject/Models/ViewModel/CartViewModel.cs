namespace RealProject.Models.ViewModel
{
    public class CartViewModel
    {
            public List<Product> Products { get; set; }
        public List<Cart> CartDetails { get; set; }
        public decimal Total { get; set; }
    }
}
