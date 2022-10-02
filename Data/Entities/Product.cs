using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entities
{
    public class Product : BaseEntity
    {
        public int ProductCategoryId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public virtual ProductCategory Category { get; set; }

        public virtual ICollection<ReceiptDetail> ReceiptDetails { get; set; }
    }
}
