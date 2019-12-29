namespace TCS.Domain
{
    public class Coupon : Discount
    {
        public double MinimumAmount { get; private set; }

        public Coupon(double minimumAmount, double discountValue, DiscountType discountType) : base(discountValue, discountType)
        {
            MinimumAmount = minimumAmount;
        }
    }
}