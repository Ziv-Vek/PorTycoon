using System;
using System.Collections.Generic;
using System.Linq;

namespace SupersonicWisdomSDK
{
    [Serializable]
    public class SwProductDescriptor
    {
        #region --- Members ---

        public string productID;
        public SwProductType productType;

        #endregion


        #region --- Private Methods ---

        internal static List<SwProduct> Convert(List<SwProductDescriptor> products)
        {
            var convertedProducts = new List<SwProduct>(products.Count);
            convertedProducts.AddRange(products.Where(product => !string.IsNullOrEmpty(product.productID)).Select(Convert));

            return convertedProducts;
        }

        internal static SwProduct Convert(SwProductDescriptor product)
        {
            return new SwProduct
            {
                productId = product.productID,
                productType = product.productType,
                isNoAds = true
            };
        }

        #endregion
    }
}