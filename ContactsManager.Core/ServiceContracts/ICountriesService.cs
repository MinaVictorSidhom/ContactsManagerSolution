using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;
namespace ServiceContracts
{
    /// <summary>
    /// Represents a buisness logic for a manipulating country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add</param>
        /// <returns> Country object after adding it(including newly generated id)</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        ///  Returns All Countries from the list
        /// </summary>
        /// <returns> All countries from the list as a list of countries</returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        ///     Returns a countryResponse object based on countryID
        /// </summary>
        /// <param name="CountryID">CounntryID(guid) to search</param>
        /// <returns> Mathching country as countryresponse object</returns>
        Task<CountryResponse?> GetCountryByCountryID(Guid? CountryID);

        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}
