using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbaKidsShop.Common
{
    public static class EntityValidationConstants
    {
        public static class ApplicationUserConstants
        {
            public const int FullNameMinLength = 1;
            public const int FullNameMaxLength = 100;
        }

        public static class ProductConstants
        {
            public const int ProductNameMinLength = 2;
            public const int ProductNameMaxLength = 60;

            public const int DescriptionMinLength = 5;
            public const int DescriptionMaxLength = 255;

            public const int MinQuantity = 1;
            public const int MaxQuantity = 10;
        }

        public static class CategoryConstants
        {
            public const int CategoryNameMinLength = 3;
            public const int CategoryNameMaxLength = 50;
        }

        public static class ShippingDetailConstants
        {
            public const int AddressMinLength = 1;
            public const int AddressMaxLength = 255;

            public const int CityMinLength = 1;
            public const int CityMaxLength = 120;

            public const int PostalCodeMinLength = 1;
            public const int PostalCodeMaxLength = 20;

            public const int CountryMinLength = 1;
            public const int CountryMaxLength = 50;
        }
    }
}
