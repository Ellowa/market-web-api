using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entities
{
    public class Customer : BaseEntity
    {
        public int PersonId { get; set; }

        public int DiscountValue { get; set; }

        public virtual Person Person { get; set; }

        public virtual ICollection<Receipt> Receipts { get; set; }
    }
}
