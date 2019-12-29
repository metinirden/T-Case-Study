namespace TCS.Domain
{
    public interface IShoppingCart
    {
        int NumberOfCategories { get; }
        int NumberOfProducts { get; }
        double TotalAmount { get; }

        void AddItem(Product product, int quantity);
        void ApplyCoupon(Coupon coupon);
        void ApplyDiscounts(params Campaign[] campaigns);
        double GetCampaignDiscount();
        double GetCouponDiscount();
        double GetDeliveryCost();
        double GetTotalAmountAfterDiscounts();
        string Print();
    }
}