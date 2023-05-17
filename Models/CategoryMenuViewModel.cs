using System.Collections;

namespace NextCommerce.Models
{
    public class CategoryMenuViewModel : IEnumerable<CategoryMenuItem>
    {
        public List<CategoryMenuItem> Items { get; set; }

        public IEnumerator<CategoryMenuItem> GetEnumerator()
        {
            foreach (var item in Items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class CategoryMenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CategoryMenuItem> Children { get; set; }
    }
}
