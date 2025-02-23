namespace RealProject.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductID{ get; set; }
        public int Quantity {  get; set; }
        public int Price { get; set; }
    }
}
