using AutoFixture;
using AutoMapper;
using FakeItEasy;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Services;
using Homeverse.Domain.Entities;
using Homeverse.Domain.Interfaces;
using Homeverse.Infrastructure.Data;
using Homeverse.UnitTest.Mocks;

namespace Homeverse.UnitTest.Services;

public class ContactServiceTests
{
    private readonly Fixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IContactRepository _contactRepository;
    private readonly IContactService _sut;

    public ContactServiceTests()
    {
        _fixture = new Fixture();
        var context = MockDbContext.CreateMockDbContext();
        _unitOfWork = new UnitOfWork(context);
        _mapper = A.Fake<IMapper>();
        _contactRepository = A.Fake<IContactRepository>();
        _sut = new ContactService(_unitOfWork, _mapper, _contactRepository);
    }


    [Fact]
    public async Task GetContactsAsync_WhenSuccessful_ShouldReturnContacts()
    {
        // Arrange
        var contacts = _fixture.CreateMany<Contact>(3).ToList();
        var response = _fixture.CreateMany<ContactResponse>(3).ToList();
        A.CallTo(() => _contactRepository.GetContactsAsync()).Returns(contacts);
        A.CallTo(() => _mapper.Map<IEnumerable<ContactResponse>>(A<IEnumerable<Contact>>._)).Returns(response);

        // Act
        var actual = await _sut.GetContactsAsync();

        // Assert
        A.CallTo(() => _contactRepository.GetContactsAsync()).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<IEnumerable<ContactResponse>>(actual);
        Assert.Equal(contacts.Count(), actual.Count());
    }

    [Fact]
    public async Task GetContactByIdAsync_WhenSuccessful_ShouldReturnContact()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var contact = _fixture.Create<Contact>();
        var response = _fixture.Create<ContactResponse>();
        A.CallTo(() => _contactRepository.GetContactByIdAsync(A<int>._)).Returns(contact);
        A.CallTo(() => _mapper.Map<ContactResponse>(A<Contact>._)).Returns(response);

        // Act
        var actual = await _sut.GetContactByIdAsync(id);

        // Assert
        A.CallTo(() => _contactRepository.GetContactByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<ContactResponse>(actual);
    }

    [Fact]
    public async Task AddContactAsync_WhenSuccessful_ShouldAddAndReturnContact()
    {
        // Arrange
        var request = _fixture.Create<ContactRequest>();
        var contact = _fixture.Create<Contact>();
        var response = _fixture.Create<ContactResponse>();
        A.CallTo(() => _mapper.Map<Contact>(A<ContactRequest>._)).Returns(contact);
        A.CallTo(() => _mapper.Map<ContactResponse>(A<Contact>._)).Returns(response);

        // Act
        var actual = await _sut.AddContactAsync(request);

        // Assert
        A.CallTo(() => _contactRepository.AddContactAsync(A<Contact>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<ContactResponse>(actual);
    }

    [Fact]
    public async Task DeleteContactAsync_WhenSuccessful_ShouldDeleteContact()
    {
        // Arrange
        var id = _fixture.Create<int>();

        // Act
        await _sut.DeleteContactAsync(id);

        // Assert
        A.CallTo(() => _contactRepository.DeleteContactAsync(A<int>._)).MustHaveHappenedOnceExactly();
    }
}
