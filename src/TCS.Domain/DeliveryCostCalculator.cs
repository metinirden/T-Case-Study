namespace TCS.Domain
{
    using System;

    public class DeliveryCostCalculator : IDeliveryCostCalculator
    {
        public double CostPerDelivery { get; private set; }
        public double CostPerProduct { get; private set; }
        public double FixedCost { get; private set; }

        public DeliveryCostCalculator(double costPerDelivery, double costPerProduct, double fixedCost)
        {
            if (costPerDelivery < 0 || costPerProduct < 0 || fixedCost < 0)
            {
                throw new ArgumentException("Delivery costs should have positive values.");
            }

            CostPerDelivery = costPerDelivery;
            CostPerProduct = costPerProduct;
            FixedCost = fixedCost;
        }

        public double CalculateFor(IShoppingCart cart)
        {
            if (cart is null)
            {
                throw new ArgumentNullException(nameof(cart));
            }

            int numberOfDeliveries = cart.NumberOfCategories;
            int numberOfProducts = cart.NumberOfProducts;

            if (numberOfDeliveries == 0 || numberOfProducts == 0)
            {
                return 0;
            }

            return (CostPerDelivery * numberOfDeliveries) + (CostPerProduct * numberOfProducts) + FixedCost;
        }
    }
}