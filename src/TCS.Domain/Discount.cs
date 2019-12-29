namespace TCS.Domain
{
    public abstract class Discount
    {
        public double DiscountValue { get; protected set; }
        public DiscountType DiscountType { get; protected set; }

        protected Discount(double discountValue, DiscountType discountType)
        {
            DiscountType = discountType;
            DiscountValue = discountValue;
        }
    }
}