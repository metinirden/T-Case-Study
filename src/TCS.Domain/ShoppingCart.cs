using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TCS.Test")]

namespace TCS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ShoppingCart : IShoppingCart
    {
        internal Dictionary<Product, int> _shoppingCartItems;
        internal Coupon? _coupon;
        internal List<Campaign> _campaigns;
        private readonly IDeliveryCostCalculator _deliveryCostCalculator;

        public double TotalAmount => _shoppingCartItems.Sum(sci => sci.Key.Price * sci.Value);
        public int NumberOfProducts => _shoppingCartItems.Sum(sci => sci.Value);
        public int NumberOfCategories => _shoppingCartItems.GroupBy(sci => sci.Key.Category.Title).Count();

        public ShoppingCart(IDeliveryCostCalculator deliveryCostCalculator)
        {
            _shoppingCartItems = new Dictionary<Product, int>();
            _campaigns = new List<Campaign>();

            if (deliveryCostCalculator is null)
            {
                throw new ArgumentNullException(nameof(deliveryCostCalculator));
            }
            _deliveryCostCalculator = deliveryCostCalculator;
        }

        public void AddItem(Product product, int quantity)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            if (quantity < 1)
            {
                throw new ArgumentException(nameof(quantity));
            }

            bool itemExistInCart = _shoppingCartItems.TryGetValue(product, out int currentQuatity);
            if (itemExistInCart)
            {
                _shoppingCartItems[product] = (currentQuatity + quantity);
            }
            else
            {
                _shoppingCartItems[product] = quantity;
            }
        }

        public void ApplyDiscounts(params Campaign[] campaigns)
        {
            _campaigns.AddRange(campaigns);
        }

        public void ApplyCoupon(Coupon coupon)
        {
            _coupon = coupon;
        }

        public double GetTotalAmountAfterDiscounts()
        {
            double amountAfterDiscounts = TotalAmount;
            amountAfterDiscounts -= GetCampaignDiscount();
            amountAfterDiscounts -= GetCouponDiscount(amountAfterDiscounts);
            return amountAfterDiscounts;
        }

        public double GetCouponDiscount()
        {
            return GetCouponDiscount(TotalAmount);
        }

        public double GetCampaignDiscount()
        {
            double discountAmount = 0;
            foreach (var camp in _campaigns)
            {
                Dictionary<Product, int> products = GetProductsByCategory(camp.Category);
                int itemCount = products.Sum(sci => sci.Value);
                if (itemCount >= camp.MinimumItemCount)
                {
                    if (camp.DiscountType == DiscountType.Amount)
                    {
                        discountAmount = camp.DiscountValue * itemCount;
                    }
                    else if (camp.DiscountType == DiscountType.Rate)
                    {
                        discountAmount = products.Sum(sci => sci.Key.Price * (camp.DiscountValue / 100) * sci.Value);
                    }
                }
            }
            return discountAmount;
        }

        public double GetDeliveryCost()
        {
            return _deliveryCostCalculator.CalculateFor(this);
        }

        public string Print()
        {
            return $"{TotalAmount} {_deliveryCostCalculator.CalculateFor(this)}";
        }

        #region Helper Functions

        private Dictionary<Product, int> GetProductsByCategory(Category category)
        {
            return _shoppingCartItems.Where(sci => sci.Key.Category == category || IsSubCategory(category, sci.Key.Category))
                .ToDictionary(sci => sci.Key, sci => sci.Value);
        }

        private double GetCouponDiscount(double totalAmount)
        {
            double discountAmount = 0;
            if (_coupon != null)
            {
                if (totalAmount >= _coupon.MinimumAmount)
                {
                    if (_coupon.DiscountType == DiscountType.Amount)
                    {
                        discountAmount = _coupon.DiscountValue;
                    }
                    else if (_coupon.DiscountType == DiscountType.Rate)
                    {
                        discountAmount = totalAmount * (_coupon.DiscountValue / 100);
                    }
                }
            }
            return discountAmount;
        }

        private bool IsSubCategory(Category parent, Category sub)
        {
            Category temp = sub.Parent;
            while (temp != null)
            {
                if (temp == parent)
                {
                    return true;
                }
                temp = temp.Parent;
            }
            return false;
        }

        #endregion Helper Functions
    }
}