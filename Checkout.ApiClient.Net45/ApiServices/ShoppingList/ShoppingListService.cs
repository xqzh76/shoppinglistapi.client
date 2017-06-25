namespace Checkout.ApiServices.ShoppingList
{
    using Checkout.ApiServices.SharedModels;
    using Checkout.Helpers;

    public class ShoppingListService
    {
        private readonly AccessTokenClient tokenClient = new AccessTokenClient();

        public HttpResponse<Drink> GetDrink(string name)
        {
            var getDrinkUri = string.Format(ApiUrls.Drink, name);
            return new ApiHttpClient().GetRequest<Drink>(getDrinkUri, this.tokenClient.GetBearerToken());
        }

        public HttpResponse<DrinkList> GetDrinkList()
        {
            var getDrinkListUri = ApiUrls.Drinks;
            return new ApiHttpClient().GetRequest<DrinkList>(getDrinkListUri, this.tokenClient.GetBearerToken());
        }

        public HttpResponse<Drink> AddDrink(Drink drink)
        {
            var addDrinkUri = ApiUrls.Drinks;
            return new ApiHttpClient().PostRequest<Drink>(addDrinkUri, this.tokenClient.GetBearerToken(), drink);
        }

        public HttpResponse<Drink> UpdateDrink(string name, Drink drink)
        {
            var updateDrinkUri = string.Format(ApiUrls.Drink, name);
            return new ApiHttpClient().PutRequest<Drink>(updateDrinkUri, this.tokenClient.GetBearerToken(), drink);
        }

        public HttpResponse<OkResponse> DeleteDrink(string name)
        {
            var deleteDrinkUri = string.Format(ApiUrls.Drink, name);
            return new ApiHttpClient().DeleteRequest<OkResponse>(deleteDrinkUri, this.tokenClient.GetBearerToken());
        }
    }
}