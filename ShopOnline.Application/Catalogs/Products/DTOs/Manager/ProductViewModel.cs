﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ShopOnline.Application.Catalogs.Products.DTOs
{
    //Data stranfer object
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string SeoDescription { get; set; }
        public string SeoTitle { get; set; }
        public DateTime DateCreated { get; set; }
        public int ViewCount { get; set; }
        public string SeoAlias { get; set; }
        public string LanguageId { get; set; }
        public Decimal Price { get; set; }
        public Decimal OriginalPrice { get; set; }
        public int Stock { get; set; }
    }
}
