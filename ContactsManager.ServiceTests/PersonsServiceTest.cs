using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using Xunit.Abstractions;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;
using Serilog.Extensions.Hosting;
using Serilog;
using Microsoft.Extensions.Logging;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personRepositoryMock.Object;
            var diagnosticContextMock = new Mock<IDiagnosticContext>();
            var loggerMock = new Mock<ILogger<PersonsGetterService>>();

            //var countriesInitialData = new List<Country>()
            //{ };
            //var personsInitialData = new List<Person>()
            //{ };

            //DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            //ApplicationDbContext dbContext = dbContextMock.Object;
            //dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            //dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

           // _countriesService = new CountriesService(null);

            _personsGetterService = new PersonsGetterService(_personsRepository, loggerMock.Object,diagnosticContextMock.Object);

           
            _personsAdderService = new PersonsAdderService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object);

            _personsDeleterService = new PersonsDeleterService(_personsRepository,loggerMock.Object,diagnosticContextMock.Object);

            _personsSorterService = new PersonsSorterService(_personsRepository, loggerMock.Object ,diagnosticContextMock.Object);

            _personsDeleterService = new PersonsDeleterService(_personsRepository,loggerMock.Object,diagnosticContextMock.Object);

            _testOutputHelper = testOutputHelper;
        }
        #region AddPerson
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange 
            PersonAddRequest? personAddRequest = null;

            Func<Task> action = async () =>
            {
                await _personsAdderService.AddPerson(personAddRequest);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange 
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();

            Person person = personAddRequest.ToPerson();

            // When PersonsRepository.AddPerson is called, it has to return the same person object 
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _personsAdderService.AddPerson(personAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();

        }

        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            //Arrange 
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someonse@example").Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();

            //if we supply any argumnent value to the AddPerson method, it should return the same return value

            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            //Act
            PersonResponse person_Response_from_add = await _personsAdderService.AddPerson(personAddRequest);
            person_response_expected.PersonID = person_Response_from_add.PersonID;

            //Assert
            //Assert.True(person_Response_from_add.PersonID != Guid.Empty);
            person_Response_from_add.PersonID.Should().NotBe(Guid.Empty);

            //Assert.Contains(person_Response_from_add, persons_list);
            //persons_list.Should().Contain(person_Response_from_add);

            person_Response_from_add.Should().Be(person_response_expected);
        }
        #endregion

        #region GetPersonByPersonID

        //If we supply null as Person ID, it should return null as PersonResponse 

        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
        {
            //Arrange
            Guid? personID = null;

            //Act 
            PersonResponse? person_response_from_get = await _personsGetterService.GetPersonByPersonID(personID);

            //Assert
            //Assert.Null(person_response_from_get);
            person_response_from_get.Should().BeNull();

        }

        //if we supply a valid person id, it should return the valid person details as PersonResponse object
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID_ToBeSuccessful()
        {
            // Arrange 
            //CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
            //CountryResponse country_response = await _countriesService.AddCountry(country_request);

            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "email@sample.com")
                .With(temp=>temp.Country,null as Country)
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            PersonResponse? person_response_from_get = await _personsGetterService.GetPersonByPersonID(person.PersonID);


            //Assert
            //Assert.Equal(person_response_from_add, person_response_from_get);
            person_response_from_get.Should().Be(person_response_expected);
        }
        #endregion

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_ToBeEmptyList()
        {
            //Arrange
            var persons = new List<Person>();
            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //Act 
            List<PersonResponse> persons_from_get = await _personsGetterService.GetAllPersons();

            //Assert
            //Assert.Empty(persons_from_get);   old way
            persons_from_get.Should().BeEmpty();
        }

        //First, we will add few persons and then when we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetAllPerson_WithFewPersons_ToBeSuccessful()
        {
            //Arrange

            List<Person> persons = new List<Person>() { _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp=>temp.Country ,null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected...");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_list_from_get = await _personsGetterService.GetAllPersons();

            _testOutputHelper.WriteLine("Actual...");
            foreach (PersonResponse person_response_from_get in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            /* foreach (PersonResponse person_response_from_add in person_respones_list_from_add)
             {
                 Assert.Contains(person_response_from_add, persons_list_from_get);

             }*/
            persons_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion
        // If the search text is empty and search by "PersonName", it should return all persons

        #region GetFilteredPersons

        //First, we will add few persons and then when we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            //Arrange

            List<Person> persons = new List<Person>() { _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp=>temp.Country ,null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected...");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);
            //Act
            List<PersonResponse> persons_list_from_search = await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "");

            _testOutputHelper.WriteLine("Actual...");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            //foreach (PersonResponse person_response_from_add in person_respones_list_from_add)
            //{
            //    Assert.Contains(person_response_from_add, persons_list_from_search);
            //}
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }
        //First we add few persons and then we will search based on person name with some search string. It should return the matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            //Arrange
            //Arrange

            List<Person> persons = new List<Person>() { _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp=>temp.Country ,null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected...");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_list_from_search = await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "sa");

            _testOutputHelper.WriteLine("Actual...");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            //foreach (PersonResponse person_response_from_add in person_respones_list_from_add)
            //{
            //    Assert.Contains(person_response_from_add, persons_list_from_search);
            //}
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion

        #region GetSortedPersons

        // when we sort based on PersonName in DESC,it should return persons list in descendig on PersonName
        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            //Arrange

            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp=>temp.Email,
                "someone_1@example.com")
                .With(temp=>temp.Country,null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp=>temp.Email,
                "someone_2@example.com")
                .With(temp=>temp.Country,null as Country)
                .Create(),
                _fixture.Build<Person>()

                .With(temp=>temp.Email,
                "someone_2@example.com")
                .With(temp=>temp.Country,null as Country)
                .Create(),
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personRepositoryMock.
                Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);

            //print person_response_list_from_add

            _testOutputHelper.WriteLine("Expected...");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            List<PersonResponse> allPersons = await _personsGetterService.GetAllPersons();

            //Act

            List<PersonResponse> persons_list_from_sort = await _personsSorterService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC); // incase sensitive

            _testOutputHelper.WriteLine("Actual...");
            foreach (PersonResponse person_response_from_get in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }
            //person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

            //Assert
            //for (int i = 0; i < person_response_list_from_add.Count; i++)
            //{
            //    Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
            //}

            //persons_list_from_sort.Should().BeEquivalentTo(person_response_list_from_add);

            persons_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
        }

        #endregion

        #region UpdatePerson
        // When we supply null as personUpdateRequest,it should throw ArgumentNullException
        [Fact]
        public async Task UpatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;

            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _personsUpdaterService.UpdatePerson(person_update_request);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();


        }

        //when wee supply invalid person id, it should throw ArgumentException
        [Fact]
        public async Task UpatePerson_InValidPersonID_ToBeArgumnetException()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = _fixture.Build<PersonUpdateRequest>()
                .Create();
            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _personsUpdaterService.UpdatePerson(person_update_request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        //when personNAme is null, it should  throw ArgumentException
        [Fact]
        public async Task UpatePerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange

            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Email,
                "someone@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            
             var action = async () =>
            {
                
                await _personsUpdaterService.UpdatePerson(person_update_request);
            };
            await action.Should().ThrowAsync<ArgumentException>();


        }


        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpatePerson_PersonFullDetails_ToBeSuccessful()
        {

            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.com")
                .With(temp=>temp.Country,null as Country)
                .With(temp=>temp.Gender,"Male")
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();

            _personRepositoryMock.Setup
                (temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);

            _personRepositoryMock.Setup
                (temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            person_update_request.PersonName = "William";
            person_update_request.Email = "William@example.com";

            //Act
            PersonResponse person_response_from_update = await _personsUpdaterService.UpdatePerson(person_update_request);


            //Assert
            //Assert.Equal(person_response_from_get, person_response_from_update);

            person_response_from_update.Should().Be(person_response_expected);
        }
        #endregion

        #region DeletePerson
        //if you supply an valid PersonID, it should return true 
        [Fact]

        public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
        {
            //Arrange

            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.com")
                .With(temp=>temp.Country,null as Country)
                .With(temp=>temp.Gender,"Female")
                .Create();

            _personRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(true);

            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            bool isDeleted = await _personsDeleterService.DeletePerson(person.PersonID);

            isDeleted.Should().BeTrue();
        }

        //if you supply an invalid PersonID, it should return false 
        [Fact]

        public async Task DeletePerson_InValidPersonID()
        {

            //Act
            bool isDeleted = await _personsDeleterService.DeletePerson(Guid.NewGuid());

            //Assert
            //Assert.False(isDeleted);
            isDeleted.Should().BeFalse();
        }
        #endregion
    }
}
