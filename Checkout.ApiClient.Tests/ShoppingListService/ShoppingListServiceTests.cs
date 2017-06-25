namespace Tests.ShoppingListService
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Checkout.ApiServices.ShoppingList;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture(Category = "ShoppingListApi")]
    public class ShoppingListServiceTests : BaseServiceTests
    {
        [Test]
        public void AddDrink()
        {
            var drink = new Drink {Name = Path.GetRandomFileName(), Quantity = 1};
            var response = this.CheckoutClient.ShoppingListService.AddDrink(drink);

            response.Should().NotBeNull();
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.Model.Name.Should().Be(drink.Name);
            response.Model.Quantity.Should().Be(drink.Quantity);
        }

        [Test]
        public void DeleteDrink()
        {
            var drink = new Drink { Name = Path.GetRandomFileName(), Quantity = 1 };
            var drinkAdded = this.CheckoutClient.ShoppingListService.AddDrink(drink).Model;
            var response = this.CheckoutClient.ShoppingListService.DeleteDrink(drinkAdded.Name);

            response.Should().NotBeNull();
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.Model.Message.Should().BeEquivalentTo("Ok");
        }

        [Test]
        public void GetDrink()
        {
            var drink = new Drink { Name = Path.GetRandomFileName(), Quantity = 1 };
            var drinkAdded = this.CheckoutClient.ShoppingListService.AddDrink(drink).Model;
            var response = this.CheckoutClient.ShoppingListService.GetDrink(drinkAdded.Name);

            response.Should().NotBeNull();
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.Model.Name.Should().Be(drink.Name);
            response.Model.Quantity.Should().Be(drink.Quantity);
        }

        [Test]
        public void GetDrinkList()
        {
            var drink1 = new Drink { Name = Path.GetRandomFileName(), Quantity = 1 };
            var drink2 = new Drink { Name = Path.GetRandomFileName(), Quantity = 1 };
            this.CheckoutClient.ShoppingListService.AddDrink(drink1);
            this.CheckoutClient.ShoppingListService.AddDrink(drink2);

            var response = this.CheckoutClient.ShoppingListService.GetDrinkList();

            response.Should().NotBeNull();
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.Model.Data.Select(x => x.Name).Should().Contain(drink1.Name);
            response.Model.Data.Select(x => x.Name).Should().Contain(drink2.Name);
        }

        [Test]
        public void UpdateDrink()
        {
            var drink = new Drink { Name = Path.GetRandomFileName(), Quantity = 1 };
            var drinkAdded = this.CheckoutClient.ShoppingListService.AddDrink(drink).Model;
            var updatedDrink = new Drink{Name = drinkAdded.Name, Quantity = drinkAdded.Quantity + 1};
            var response = this.CheckoutClient.ShoppingListService.UpdateDrink(updatedDrink.Name, updatedDrink);

            response.Should().NotBeNull();
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.Model.Name.Should().Be(updatedDrink.Name);
            response.Model.Quantity.Should().Be(updatedDrink.Quantity);
        }
    }
}