using Entities;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person Entity
    /// </summary>
    public interface IPersonsSorterService
    {
        /// <summary>
        /// returns Sorted list of persons
        /// </summary>
        /// <param name="allPersons"> Represens list of persons to sort</param>
        /// <param name="sortBy">Name of the property(key),based on which the persons should sorted</param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns> Returns sorted persons as PersonResponse</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);
    }
}
