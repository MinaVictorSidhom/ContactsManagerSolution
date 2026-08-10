using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using RepositoryContracts;
using ServiceContracts.DTO;
using ServiceContracts;
using Services;
using Xunit;
using Moq;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesRepository _countriesRepository;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;

            _countriesService = new CountriesService(_countriesRepository);
        }
        #region AddCountry
        // when ContryAddRequest is null , it should throw ArgumentNullExeption

        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _countriesService.AddCountry(request);
            });
        }

        //when the countryName is  null, it should throw ArgumentExeption
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest { CountryName = null };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(request);
            });
        }

        //when the countryName is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest { CountryName = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest { CountryName = "USA" };

            Country existingCountry = new Country() { CountryID = Guid.NewGuid(), CountryName = "USA" };

            _countriesRepositoryMock
                .SetupSequence(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync((Country?)null)
                .ReturnsAsync(existingCountry);

            _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .Returns<Country>(country => Task.FromResult(country));

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            });
        }

        //when you supply proper countryName, it should insert the country to existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest { CountryName = "Japan" };
            Country? addedCountry = null;

            _countriesRepositoryMock
                .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync((Country?)null);

            _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .Callback<Country>(country => addedCountry = country)
                .Returns<Country>(country => Task.FromResult(country));

            //Act
            CountryResponse response = await _countriesService.AddCountry(request);

            _countriesRepositoryMock
                .Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(new List<Country>() { addedCountry! });

            List<CountryResponse> countries_from_GetAllCountries = await _countriesService.GetAllCountries();

            //Assert
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, countries_from_GetAllCountries);
        }
        #endregion


        #region GetAllCountries
        //the list of countries should be empty by default(before adding any countries)
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            //Arrange
            _countriesRepositoryMock
                .Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(new List<Country>());

            //Act
            List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

            //Assert
            Assert.Empty(actual_country_response_list);
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest>()
            {
                new CountryAddRequest()
                {
                    CountryName="USA"
                },
                new CountryAddRequest(){CountryName="UK"}
            };

            List<Country> addedCountries = new List<Country>();

            _countriesRepositoryMock
                .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync((Country?)null);

            _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .Callback<Country>(country => addedCountries.Add(country))
                .Returns<Country>(country => Task.FromResult(country));

            //Act 
            List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();
            foreach (CountryAddRequest country_request in country_request_list)
            {
                countries_list_from_add_country.Add(await _countriesService.AddCountry(country_request));
            }

            _countriesRepositoryMock
                .Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(addedCountries);

            List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

            // read each element from countries_lsit_from_add_country
            foreach (CountryResponse expectedCountry in countries_list_from_add_country)
            {
                Assert.Contains(expectedCountry, actualCountryResponseList);
            }
        }
        #endregion

        #region GetCountryByCountryID

        [Fact]
        // if we supply a null as CountryID, it should return null as countryResponse
        public async Task GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;

            //Act
            CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryID(countryID);

            //Assert
            Assert.Null(country_response_from_get_method);
        }

        [Fact]
        // if we supply valid country id, it should return the matching country details as country response object
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest? country_add_request = new CountryAddRequest()
            {
                CountryName = "China"
            };
            Country? addedCountry = null;

            _countriesRepositoryMock
                .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync((Country?)null);

            _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .Callback<Country>(country => addedCountry = country)
                .Returns<Country>(country => Task.FromResult(country));

            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            _countriesRepositoryMock
                .Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
                .ReturnsAsync(addedCountry);

            //Act
            CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryID(country_response_from_add.CountryID);

            //Assert
            Assert.Equal(country_response_from_add, country_response_from_get);
        }
        #endregion
    }
}