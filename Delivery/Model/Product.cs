//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Delivery.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.OrderProduct = new HashSet<OrderProduct>();
        }
    
        public int ProductArticle { get; set; }
        public string ProductName { get; set; }
        public int ProductUnit { get; set; }
        public double ProductCost { get; set; }
        public double ProductDiscount { get; set; }
        public int ProductCategory { get; set; }
        public string ProductDescription { get; set; }
        public int ProductProvider { get; set; }
        public string ProductImage { get; set; }

        public int ProductCount { get; set; }

        public double ProductCostWithDiscount { get; set; }

        public string ProductImagePath
        {
            get
            {
                string path = Environment.CurrentDirectory + @"\..\..\Resources\Images\" + this.ProductImage + ".png";

                if (File.Exists(path))
                {
                    return path;
                }
                else
                {
                    return Environment.CurrentDirectory + @"\..\..\Resources\plug.png";
                }
            }
        }

        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderProduct> OrderProduct { get; set; }
        public virtual Provider Provider { get; set; }
        public virtual Unit Unit { get; set; }
    }
}
