namespace easy_shop.Models
{
    public class ProductViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public IFormFile ImageFile { get; set; }
        public int Available { get; set; }
        public string Category { get; set; }
    }
}
