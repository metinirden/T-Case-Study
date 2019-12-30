using System;
using TCS.Domain;

namespace TCS.App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Category furnitureCat = new Category("Furniture");
            Category deskCat = new Category("Desk");
            Category smartPhoneCat = new Category("Smart Phone");
            deskCat.AssignParent(furnitureCat);

            Product cabinet = new Product("Dolap", 450, furnitureCat);
            Product pcDesk = new Product("Bilgisayar Masası", 350, deskCat);
            Product samsungS8 = new Product("Samsung S8", 1250, smartPhoneCat);

            ShoppingCart cart = new ShoppingCart(new DeliveryCostCalculator(5, 15, 2.99));
            cart.AddItem(cabinet, 2);
            cart.AddItem(pcDesk, 3);
            cart.AddItem(samsungS8, 1);

            Coupon coupon = new Coupon(2500, 100, DiscountType.Amount);
            Campaign furnitureCampaign = new Campaign(furnitureCat, 10, DiscountType.Rate, 5);
            Campaign smartPhoneCampaign = new Campaign(smartPhoneCat, 30, DiscountType.Rate, 2);

            cart.ApplyCoupon(coupon);
            cart.ApplyDiscounts(furnitureCampaign, smartPhoneCampaign);

            Console.WriteLine(cart.Print());
        }
    }
}