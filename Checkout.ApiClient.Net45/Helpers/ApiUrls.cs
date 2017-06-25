namespace Checkout
{
    using Checkout.Helpers;

    public class ApiUrls
    {
        private static string _drinksApiUri;
        private static string _drinkApiUri;

        public static void ResetApiUrls()
        {
            _drinksApiUri = null;
            _drinkApiUri = null;
        }


        public static string Drinks
            => _drinksApiUri ?? (_drinksApiUri = string.Concat(AppSettings.BaseApiUri, "/shoppinglist/drinks"));

        public static string Drink
            => _drinkApiUri ?? (_drinkApiUri = string.Concat(AppSettings.BaseApiUri, "/shoppinglist/drinks/{0}"));
    }
}