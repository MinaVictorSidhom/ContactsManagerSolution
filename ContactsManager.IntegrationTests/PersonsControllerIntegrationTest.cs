using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Fizzler;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
namespace CRUDTests
{
    public class PersonsControllerIntegrationTest:IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }
        #region Index
        [Fact]
        public async Task Index_ToReturnView()
        {
            //Act
            HttpResponseMessage response = await _client.GetAsync("/Persons/Index");

            //Assert
            response.IsSuccessStatusCode.Should().BeTrue();

            string responseBody= await response.Content.ReadAsStringAsync();

            HtmlDocument html = new HtmlDocument();

            html.LoadHtml(responseBody);

            var documnet = html.DocumentNode;

            documnet.QuerySelectorAll("table.persons").Should().NotBeNull();
        }
        #endregion
    }
}
