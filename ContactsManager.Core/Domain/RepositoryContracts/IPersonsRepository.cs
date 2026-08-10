using Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;


namespace RepositoryContracts
{
    /// <summary>
    /// Reprsents data access logic for managing person entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        ///Adds a person object to the data store
        /// </summary>
        /// <param name="person">person object to add </param>
        /// <returns>Returns the person object after adding it to the table</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Returns all person in the data store
        /// </summary>
        /// <returns> List of person objects from table</returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// Returns a person object based on the fiven person id 
        /// </summary>
        /// <param name="Person">PersonID (guid) to search</param>
        /// <returns>A person object or null</returns>
        Task<Person?> GetPersonByPersonID(Guid personID);


        /// <summary>
        /// Returns all person objects based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>matching persons with given condition</returns>
        Task<List<Person>>GetFilteredPersons(Expression<Func<Person,bool>>predicate);

        /// <summary>
        /// Deletes a person object based  on the person id 
        /// </summary>
        /// <param name="personID">Person ID (guid) to search</param>
        /// <returns> Returns true if the deletion is successful otherwise false</returns>
        Task<bool> DeletePersonByPersonID(Guid personID);

        /// <summary>
        /// Updates a person object(person name and other details) based on the given person id
        /// </summary>
        /// <param name="person">Person object to update</param>
        /// <returns>Returns yje updared person object</returns>
        Task<Person> UpdatePerson(Person person);
    }
}
