using TCS.Domain;

namespace TCS.App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Category fruit = new Category("Fruit");
            Category shoe = new Category("Shoe");
            Category menShoe = new Category("Men Shoe");
            menShoe.AssignParent(shoe);

            Product apple = new Product("Apple", 100, fruit);
            Product orange = new Product("Orange", 50, fruit);
            Product sportShoe = new Product("Sport Shoe", 250, menShoe);

            ShoppingCart cart = new ShoppingCart(new DeliveryCostCalculator(1.5, 10, 2.99));
            cart.AddItem(apple, 10);
            cart.AddItem(orange, 5);
            cart.AddItem(sportShoe, 1);

            Campaign c1 = new Campaign(fruit, 10, DiscountType.Rate, 5);
            Campaign c2 = new Campaign(menShoe, 30, DiscountType.Rate, 150);
            cart.ApplyDiscounts(c1, c2);

            Coupon coupon = new Coupon(1, 10, DiscountType.Amount);
            cart.ApplyCoupon(coupon);

            System.Console.WriteLine(cart.Print());
        }
    }
}