using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using TCS.Domain;

namespace TCS.Test
{
    public class ShoppingCartTests
    {
        private Mock<IDeliveryCostCalculator> _deliveryCostCalculator;

        [SetUp]
        public void Setup()
        {
            _deliveryCostCalculator = new Mock<IDeliveryCostCalculator>();
        }

        [Test]
        public void Construction_WithNullCalculator_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new ShoppingCart(null);
            });
        }

        [Test]
        public void AddItem_WithNullProductPositiveQuantity_Throws()
        {
            IShoppingCart shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
            Assert.Throws<ArgumentNullException>(() =>
            {
                shoppingCart.AddItem(null, 3);
            });
        }

        [Test]
        public void AddItem_WithProperProductNegativeQuantity_Throws()
        {
            IShoppingCart shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
            Assert.Throws<ArgumentException>(() =>
            {
                shoppingCart.AddItem(new Product("S8", 900, new Category("Smart Phone")), -1);
            });
        }

        [Test]
        public void AddItem_WithProperProductPositiveQuantity_ShouldAdd()
        {
            ShoppingCart shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
            Product product = new Product("S8", 900, new Category("Smart Phone"));
            shoppingCart.AddItem(product, 3);
            Assert.AreEqual(1, shoppingCart._shoppingCartItems.Count);
            Assert.AreEqual(3, shoppingCart._shoppingCartItems[product]);
        }

        [Test]
        public void AddItem_ExsistingProduct_ShouldIncreaseQuantity()
        {
            ShoppingCart shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
            Product product = new Product("S8", 900, new Category("Smart Phone"));
            shoppingCart.AddItem(product, 1);
            Assert.AreEqual(1, shoppingCart._shoppingCartItems[product]);
            shoppingCart.AddItem(product, 1);
            Assert.AreEqual(2, shoppingCart._shoppingCartItems[product]);
        }

        [Test]
        public void ApplyDiscounts_OneCampaign_ShouldAddCampaign()
        {
            ShoppingCart shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
            Campaign campaign = new Campaign(new Category("Smart Phone"), 5, DiscountType.Amount, 3);
            shoppingCart.ApplyDiscounts(campaign);
            Assert.AreEqual(1, shoppingCart._campaigns.Count);
        }

        [Test]
        public void ApplyCoupon_OneCoupon_ShouldAddCoupon()
        {
            ShoppingCart shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
            Coupon coupon = new Coupon(50, 5, DiscountType.Amount);
            shoppingCart.ApplyCoupon(coupon);
            Assert.That(shoppingCart._coupon == coupon);
        }

        [Test]
        public void GetCouponDiscount_WithNoCouponWithOneProduct_ReturnsZero()
        {
            ShoppingCart shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
            shoppingCart.AddItem(new Product("S8", 900, new Category("Smart Phone")), 1);
            Assert.AreEqual(0, shoppingCart.GetCouponDiscount());
        }

        [Test]
        public void GetCouponDiscount_WithOneCouponCartLessThanCouponLimit_ReturnsZero()
        {
            ShoppingCart shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
            shoppingCart.AddItem(new Product("3310", 99.99, new Category("Phone")), 1);
            shoppingCart.ApplyCoupon(new Coupon(250, 50, DiscountType.Amount));
            Assert.AreEqual(0, shoppingCart.GetCouponDiscount());
        }

        [Test]
        public void GetCouponDiscount_WithOneCouponCartGreaterThanCouponLimit_ReturnsCouponAmount()
        {
            double couponAmount = 50;

            ShoppingCart shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
            shoppingCart.AddItem(new Product("S8", 900, new Category("Smart Phone")), 1);
            shoppingCart.ApplyCoupon(new Coupon(500, couponAmount, DiscountType.Amount));
            Assert.AreEqual(couponAmount, shoppingCart.GetCouponDiscount());
        }

        [Test]
        public void GetCouponDiscount_WithOneCouponCartGreaterThanCouponLimit_ReturnsCouponRate()
        {
            double couponRate = 50;
            ShoppingCart shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
            shoppingCart.AddItem(new Product("S8", 900, new Category("Smart Phone")), 1);
            shoppingCart.ApplyCoupon(new Coupon(500, couponRate, DiscountType.Rate));

            double couponAmount = shoppingCart.TotalAmount * (couponRate / 100);

            Assert.AreEqual(couponAmount, shoppingCart.GetCouponDiscount());
        }

        [Test]
        public void GetCampaignDiscount_WithZeroCampaign_ReturnsZero()
        {
            ShoppingCart shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
            shoppingCart.AddItem(new Product("S8", 900, new Category("Smart Phone")), 1);
            Assert.AreEqual(0, shoppingCart.GetCampaignDiscount());
        }

        [Test]
        public void GetCampaignDiscount_WithOneCampaignCartLessThenItemCount_ReturnsZero()
        {
            ShoppingCart shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
            Category category = new Category("Smart Phone");
            shoppingCart.AddItem(new Product("S8", 900, category), 1);
            shoppingCart.ApplyDiscounts(new Campaign(category, 50, DiscountType.Amount, 3));
            Assert.AreEqual(0, shoppingCart.GetCampaignDiscount());
        }

        [Test]
        public void GetCampaignDiscount_WithOneCampaignCartGreaterThenItemCount_ReturnsCampaignAmount()
        {
            ShoppingCart shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
            Category category = new Category("Smart Phone");
            shoppingCart.AddItem(new Product("S8", 900, category), 5);
            const int campaignAmount = 50;
            shoppingCart.ApplyDiscounts(new Campaign(category, campaignAmount, DiscountType.Amount, 3));
            Assert.AreEqual(campaignAmount * shoppingCart.NumberOfProducts, shoppingCart.GetCampaignDiscount());
        }

        [Test]
        public void GetCampaignDiscount_WithOneCampaignCartGreaterThenItemCount_ReturnsCampaignRate()
        {
            ShoppingCart shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
            Category category = new Category("Smart Phone");
            shoppingCart.AddItem(new Product("S8", 900, category), 5);
            const int campaignRate = 10;
            shoppingCart.ApplyDiscounts(new Campaign(category, campaignRate, DiscountType.Rate, 3));
            Assert.AreEqual(shoppingCart._shoppingCartItems.Sum(sci => (sci.Key.Price * campaignRate / 100) * sci.Value), shoppingCart.GetCampaignDiscount());
        }

        [Test]
        public void GetTotalAmountAfterDiscounts__WithNoCampaignAndNoCoupon_ReturnsTotalAmount()
        {
            ShoppingCart shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
            Category category = new Category("Smart Phone");
            shoppingCart.AddItem(new Product("S8", 900, category), 5);
            Assert.AreEqual(shoppingCart.TotalAmount, shoppingCart.GetTotalAmountAfterDiscounts());
        }

        [Test]
        public void GetDeliveryCost_ForPropertCart_()
        {
            ShoppingCart shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
            Category category = new Category("Smart Phone");
            shoppingCart.AddItem(new Product("S8", 900, category), 5);

            _deliveryCostCalculator.Setup(dcc => dcc.CalculateFor(shoppingCart)).Returns(50);

            Assert.AreEqual(50, shoppingCart.GetDeliveryCost());
        }
    }
}