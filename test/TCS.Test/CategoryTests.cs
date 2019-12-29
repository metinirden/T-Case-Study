using System;
using NUnit.Framework;
using TCS.Domain;

namespace TCS.Test
{
    public class CategoryTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AssignParent_DiffrentCategory_ShoudBeEqual()
        {
            Category nike = new Category("nike");
            Category shoes = new Category("shoes");
            nike.AssignParent(shoes);
            Assert.AreEqual(shoes, nike.Parent);
        }

        [Test]
        public void AssignParent_SameCategory_Throws()
        {
            Category nike = new Category("nike");
            Assert.Throws<ArgumentException>(() => { nike.AssignParent(nike); });
        }

        [Test]
        public void AssignParent_NullCategory_Throws()
        {
            Category nike = new Category("nike");
            Assert.Throws<ArgumentNullException>(() => { nike.AssignParent(null); });
        }

        [Test]
        public void IsParentOf_SubCategory_ReturnsTrue()
        {
            Category nike = new Category("nike");
            Category shoes = new Category("shoes");
            nike.AssignParent(shoes);

            Assert.True(shoes.IsParentOf(nike));
        }

        [Test]
        public void IsParentOf_DoubleSubCategory_ReturnsTrue()
        {
            Category nike = new Category("nike");
            Category casual = new Category("casual");
            Category shoes = new Category("shoes");
            nike.AssignParent(casual);
            casual.AssignParent(shoes);
            Assert.True(shoes.IsParentOf(nike));
        }

        [Test]
        public void IsParentOf_ParentCategory_ReturnsFalse()
        {
            Category nike = new Category("nike");
            Category shoes = new Category("shoes");
            nike.AssignParent(shoes);

            Assert.False(nike.IsParentOf(shoes));
        }
    }
}