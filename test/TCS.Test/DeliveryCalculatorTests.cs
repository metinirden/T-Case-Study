using System;
using System.Collections;
using Moq;
using NUnit.Framework;
using TCS.Domain;

namespace TCS.Test
{
    [TestFixture]
    public class DeliveryCalculatorTests
    {
        private Mock<IShoppingCart> _shoppingCart;

        [SetUp]
        public void Setup()
        {
            _shoppingCart = new Mock<IShoppingCart>();
        }

        [Test]
        public void Construction_WithNegativeDeliveryPrices_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                IDeliveryCostCalculator deliveryCostCalculator = new DeliveryCostCalculator(-1, -1, -3);
            });
        }

        [Test]
        public void CalculateFor_NullShoppingCart_Throws()
        {
            IDeliveryCostCalculator deliveryCostCalculator = new DeliveryCostCalculator(1, 1, 1);
            Assert.Throws<ArgumentNullException>(() =>
            {
                deliveryCostCalculator.CalculateFor(null);
            });
        }

        [TestCaseSource(typeof(CalculateForDataClass), "TestCases")]
        public double CalculateFor_OneProduct_ReturnsCorrectPrice(double costPerDelivery, double costPerProduct, double fixedCost,
            int numberOfCategories, int numberOfProducts)
        {
            IDeliveryCostCalculator deliveryCostCalculator = new DeliveryCostCalculator(costPerDelivery, costPerProduct, fixedCost);
            _shoppingCart.Setup(sc => sc.NumberOfCategories).Returns(numberOfCategories);
            _shoppingCart.Setup(sc => sc.NumberOfProducts).Returns(numberOfProducts);
            return deliveryCostCalculator.CalculateFor(_shoppingCart.Object);
        }

        public class CalculateForDataClass
        {
            public static IEnumerable TestCases
            {
                get
                {
                    yield return new TestCaseData(10, 5, 2.99, 2, 3).Returns(37.99);
                    yield return new TestCaseData(1, 1, 1, 1, 1).Returns(3);
                    yield return new TestCaseData(1, 1, 1, 0, 1).Returns(0);
                    yield return new TestCaseData(1, 1, 1, 1, 0).Returns(0);
                    yield return new TestCaseData(1, 1, 1, 0, 0).Returns(0);
                }
            }
        }
    }
}