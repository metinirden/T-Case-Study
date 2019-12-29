namespace TCS.Domain
{
    public class Campaign : Discount
    {
        public int MinimumItemCount { get; private set; }
        public Category Category { get; private set; }

        public Campaign(Category category, double discountValue, DiscountType discountType, int minimumItemCount)
        : base(discountValue, discountType)
        {
            Category = category;
            MinimumItemCount = minimumItemCount;
        }
    }
}