namespace TCS.Domain
{
    public class Product
    {
        public string Title { get; private set; }
        public double Price { get; private set; }
        public Category Category { get; private set; }

        public Product(string title, double price, Category category)
        {
            Title = title;
            Price = price;
            Category = category;
        }
    }
}