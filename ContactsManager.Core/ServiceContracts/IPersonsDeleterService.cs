using Entities;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person Entity
    /// </summary>
    public interface IPersonsDeleterService
    {
        
        /// <summary>
        /// Delete  a person based on the given person id 
        /// </summary>
        /// <param name="personID">Person ID to delete</param>
        /// 
        /// <returns>Returns true, if the deletion successful otherwise false</returns>
        Task<bool> DeletePerson(Guid? personID);
    }
}
