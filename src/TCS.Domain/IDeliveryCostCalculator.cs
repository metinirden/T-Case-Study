namespace TCS.Domain
{
    public interface IDeliveryCostCalculator
    {
        double CostPerDelivery { get; }
        double CostPerProduct { get; }
        double FixedCost { get; }

        double CalculateFor(IShoppingCart shoppingCart);
    }
}