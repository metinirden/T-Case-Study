namespace TCS.Domain
{
    using System;

    public class Category
    {
        public string Title { get; private set; }
        public Category Parent { get; private set; } = null;

        public Category(string title)
        {
            Title = title;
        }

        public void AssignParent(Category parentCategory)
        {
            if (parentCategory is null)
            {
                throw new ArgumentNullException(nameof(parentCategory));
            }
            else if (parentCategory.Equals(this))
            {
                throw new ArgumentException(nameof(parentCategory));
            }
            else
            {
                Parent = parentCategory;
            }
        }
    }
}