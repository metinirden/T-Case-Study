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
        private ShoppingCart shoppingCart;

        [SetUp]
        public void Setup()
        {
            _deliveryCostCalculator = new Mock<IDeliveryCostCalculator>();
            shoppingCart = new ShoppingCart(_deliveryCostCalculator.Object);
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
            Assert.Throws<ArgumentNullException>(() =>
            {
                shoppingCart.AddItem(null, 3);
            });
        }

        [Test]
        public void AddItem_WithProperProductNegativeQuantity_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                shoppingCart.AddItem(new Product("S8", 900, new Category("Smart Phone")), -1);
            });
        }

        [Test]
        public void AddItem_WithProperProductZeroQuantity_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                shoppingCart.AddItem(new Product("S8", 900, new Category("Smart Phone")), 0);
            });
        }

        [Test]
        public void AddItem_WithProperProductPositiveQuantity_ShouldAdd()
        {
            Product product = new Product("S8", 900, new Category("Smart Phone"));
            shoppingCart.AddItem(product, 3);
            Assert.AreEqual(1, shoppingCart._shoppingCartItems.Count);
            Assert.AreEqual(3, shoppingCart._shoppingCartItems[product]);
        }

        [Test]
        public void AddItem_ExsistingProduct_ShouldIncreaseQuantity()
        {
            Product product = new Product("S8", 900, new Category("Smart Phone"));
            shoppingCart.AddItem(product, 1);
            Assert.AreEqual(1, shoppingCart._shoppingCartItems[product]);
            shoppingCart.AddItem(product, 1);
            Assert.AreEqual(2, shoppingCart._shoppingCartItems[product]);
        }

        [Test]
        public void ApplyDiscounts_OneCampaign_ShouldAddCampaign()
        {
            Campaign campaign = new Campaign(new Category("Smart Phone"), 5, DiscountType.Amount, 3);
            shoppingCart.ApplyDiscounts(campaign);
            Assert.AreEqual(1, shoppingCart._campaigns.Count);
        }

        [Test]
        public void ApplyCoupon_OneCoupon_ShouldAddCoupon()
        {
            Coupon coupon = new Coupon(50, 5, DiscountType.Amount);
            shoppingCart.ApplyCoupon(coupon);
            Assert.That(shoppingCart._coupon == coupon);
        }

        [Test]
        public void GetCouponDiscount_WithNoCouponWithOneProduct_ReturnsZero()
        {
            shoppingCart.AddItem(new Product("S8", 900, new Category("Smart Phone")), 1);
            Assert.AreEqual(0, shoppingCart.GetCouponDiscount());
        }

        [Test]
        public void GetCouponDiscount_WithOneCouponCartLessThanCouponLimit_ReturnsZero()
        {
            shoppingCart.AddItem(new Product("3310", 99.99, new Category("Phone")), 1);
            shoppingCart.ApplyCoupon(new Coupon(250, 50, DiscountType.Amount));
            Assert.AreEqual(0, shoppingCart.GetCouponDiscount());
        }

        [Test]
        public void GetCouponDiscount_WithOneCouponCartGreaterThanCouponLimit_ReturnsCouponAmount()
        {
            double couponAmount = 50;

            shoppingCart.AddItem(new Product("S8", 900, new Category("Smart Phone")), 1);
            shoppingCart.ApplyCoupon(new Coupon(500, couponAmount, DiscountType.Amount));
            Assert.AreEqual(couponAmount, shoppingCart.GetCouponDiscount());
        }

        [Test]
        public void GetCouponDiscount_WithOneCouponCartGreaterThanCouponLimit_ReturnsCouponRate()
        {
            double couponRate = 50;

            shoppingCart.AddItem(new Product("S8", 900, new Category("Smart Phone")), 1);
            shoppingCart.ApplyCoupon(new Coupon(500, couponRate, DiscountType.Rate));

            double couponAmount = shoppingCart.TotalAmount * (couponRate / 100);

            Assert.AreEqual(couponAmount, shoppingCart.GetCouponDiscount());
        }

        [Test]
        public void GetCampaignDiscount_WithZeroCampaign_ReturnsZero()
        {
            shoppingCart.AddItem(new Product("S8", 900, new Category("Smart Phone")), 1);
            Assert.AreEqual(0, shoppingCart.GetCampaignDiscount());
        }

        [Test]
        public void GetCampaignDiscount_WithOneCampaignCartLessThenItemCount_ReturnsZero()
        {
            Category category = new Category("Smart Phone");
            shoppingCart.AddItem(new Product("S8", 900, category), 1);
            shoppingCart.ApplyDiscounts(new Campaign(category, 50, DiscountType.Amount, 3));
            Assert.AreEqual(0, shoppingCart.GetCampaignDiscount());
        }

        [Test]
        public void GetCampaignDiscount_WithOneCampaignCartGreaterThenItemCount_ReturnsCampaignAmount()
        {
            Category category = new Category("Smart Phone");
            shoppingCart.AddItem(new Product("S8", 900, category), 5);
            const int campaignAmount = 50;
            shoppingCart.ApplyDiscounts(new Campaign(category, campaignAmount, DiscountType.Amount, 3));
            Assert.AreEqual(250, shoppingCart.GetCampaignDiscount());
        }

        [Test]
        public void GetCampaignDiscount_WithOneCampaignCartGreaterThenItemCount_ReturnsCampaignRate()
        {
            Category category = new Category("Smart Phone");
            shoppingCart.AddItem(new Product("S8", 900, category), 5);
            const int campaignRate = 10;
            shoppingCart.ApplyDiscounts(new Campaign(category, campaignRate, DiscountType.Rate, 3));
            Assert.AreEqual(shoppingCart._shoppingCartItems.Sum(sci => (sci.Key.Price * campaignRate / 100) * sci.Value), shoppingCart.GetCampaignDiscount());
        }

        [Test]
        public void GetTotalAmountAfterDiscounts_WithEmptyCard_ReturnsZero()
        {
            Assert.AreEqual(0, shoppingCart.GetTotalAmountAfterDiscounts());
        }

        [Test]
        public void GetTotalAmountAfterDiscounts_CartWithOneProductOneCampaignNoCoupon_ReturnsTotalMinusCampaignDiscount()
        {
            Category pcParts = new Category("PC Parts");
            Product keyboard = new Product("Keyboard", 50, pcParts);
            shoppingCart.AddItem(keyboard, 5);

            Campaign camp = new Campaign(pcParts, 5, DiscountType.Amount, 4);
            shoppingCart.ApplyDiscounts(camp);

            double expected = shoppingCart.TotalAmount - shoppingCart.GetCampaignDiscount();
            Assert.AreEqual(expected, shoppingCart.GetTotalAmountAfterDiscounts());
        }

        [Test]
        public void GetTotalAmountAfterDiscounts_CartWithOneProductOneRateCampaignNoCoupon_ReturnsTotalMinusCampaignDiscount()
        {
            Category pcParts = new Category("PC Parts");
            Product keyboard = new Product("Keyboard", 50, pcParts);
            shoppingCart.AddItem(keyboard, 5);

            Campaign camp = new Campaign(pcParts, 5, DiscountType.Rate, 3);
            shoppingCart.ApplyDiscounts(camp);

            Assert.That((shoppingCart.TotalAmount - shoppingCart.GetCampaignDiscount()) == shoppingCart.GetTotalAmountAfterDiscounts());
        }

        [Test]
        public void GetTotalAmountAfterDiscounts_CartWithOneProductNoCampaignOneCoupon_ReturnsTotalMinusCampaignDiscount()
        {
            Category pcParts = new Category("PC Parts");
            Product keyboard = new Product("Keyboard", 50, pcParts);
            shoppingCart.AddItem(keyboard, 5);

            Coupon coupon = new Coupon(200, 50, DiscountType.Amount);
            shoppingCart.ApplyCoupon(coupon);

            double expected = shoppingCart.TotalAmount - shoppingCart.GetCouponDiscount();
            Assert.AreEqual(expected, shoppingCart.GetTotalAmountAfterDiscounts());
        }

        [Test]
        public void GetTotalAmountAfterDiscounts_CartWithOneProductNoCampaignOneRateCoupon_ReturnsTotalMinusCampaignDiscount()
        {
            Category pcParts = new Category("PC Parts");
            Product keyboard = new Product("Keyboard", 50, pcParts);
            shoppingCart.AddItem(keyboard, 5);

            Coupon coupon = new Coupon(200, 10, DiscountType.Rate);
            shoppingCart.ApplyCoupon(coupon);

            double expected = shoppingCart.TotalAmount - shoppingCart.GetCouponDiscount();
            Assert.AreEqual(expected, shoppingCart.GetTotalAmountAfterDiscounts());
        }

        [Test]
        public void GetTotalAmountAfterDiscounts_WithNoCampaignAndNoCoupon_ReturnsTotalAmount()
        {
            Category category = new Category("Smart Phone");
            shoppingCart.AddItem(new Product("S8", 900, category), 5);
            Assert.AreEqual(shoppingCart.TotalAmount, shoppingCart.GetTotalAmountAfterDiscounts());
        }

        [Test]
        public void GetTotalAmountAfterDiscounts_OneProductOneCampaignOneCoupon_ReturnsTotalMinusAmountAfterDiscounts()
        {
            Category category = new Category("Watch");
            Product seiko = new Product("Seiko Miltary", 500, category);
            shoppingCart.AddItem(seiko, 2);

            Campaign camp = new Campaign(category, 50, DiscountType.Amount, 2);
            shoppingCart.ApplyDiscounts(camp);

            Coupon coupon = new Coupon(50, 10, DiscountType.Amount);
            shoppingCart.ApplyCoupon(coupon);

            double expected = shoppingCart.TotalAmount - shoppingCart.GetCouponDiscount() - shoppingCart.GetCampaignDiscount();
            Assert.AreEqual(expected, shoppingCart.GetTotalAmountAfterDiscounts());
        }

        [Test]
        public void GetDeliveryCost_ForPropertCart_ShouldReturnCalculatedAmount()
        {
            Category category = new Category("Smart Phone");
            shoppingCart.AddItem(new Product("S8", 900, category), 5);

            _deliveryCostCalculator.Setup(dcc => dcc.CalculateFor(shoppingCart)).Returns(50);

            Assert.AreEqual(50, shoppingCart.GetDeliveryCost());
        }

        [Test]
        public void NumberOfProduct_EmptyCart_ReturnZero()
        {
            int expected = 0;
            Assert.AreEqual(expected, shoppingCart.NumberOfProducts);
        }

        [Test]
        public void NumberOfProduct_CartWithOneProduct_ReturnOne()
        {
            shoppingCart.AddItem(new Product("Thinkpad", 600, new Category("Notebook")), 3);
            int expected = 1;
            Assert.AreEqual(expected, shoppingCart.NumberOfProducts);
        }

        [Test]
        public void NumberOfProduct_CartWithTwoProduct_ReturnTwo()
        {
            shoppingCart.AddItem(new Product("Thinkpad", 600, new Category("Notebook")), 3);
            shoppingCart.AddItem(new Product("Fahrenheit 451", 600, new Category("Book")), 1);
            int expected = 2;
            Assert.AreEqual(expected, shoppingCart.NumberOfProducts);
        }

        [Test]
        public void NumberOfCategories_EmptyCart_ReturnZero()
        {
            int expected = 0;
            Assert.AreEqual(expected, shoppingCart.NumberOfCategories);
        }

        [Test]
        public void NumberOfCategories_CartWithOneProduct_ReturnOne()
        {
            shoppingCart.AddItem(new Product("Thinkpad", 600, new Category("Notebook")), 3);
            int expected = 1;
            Assert.AreEqual(expected, shoppingCart.NumberOfCategories);
        }

        [Test]
        public void NumberOfCategories_CartWithTwoProductDifferentCategories_ReturnTwo()
        {
            shoppingCart.AddItem(new Product("Thinkpad", 600, new Category("Notebook")), 3);
            shoppingCart.AddItem(new Product("Fahrenheit 451", 600, new Category("Book")), 1);
            int expected = 2;
            Assert.AreEqual(expected, shoppingCart.NumberOfCategories);
        }

        [Test]
        public void NumberOfCategories_CartWithTwoProductSameCategories_ReturnOne()
        {
            shoppingCart.AddItem(new Product("Thinkpad T450", 600, new Category("Notebook")), 3);
            shoppingCart.AddItem(new Product("Thinkpad X1", 700, new Category("Notebook")), 2);
            int expected = 1;
            Assert.AreEqual(expected, shoppingCart.NumberOfCategories);
        }
    }
}