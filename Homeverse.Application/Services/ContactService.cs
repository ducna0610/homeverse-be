using AutoMapper;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Domain.Entities;
using Homeverse.Domain.Interfaces;

namespace Homeverse.Application.Services;

public interface IContactService
{
    Task<IEnumerable<ContactResponse>> GetContactsAsync();
    Task<ContactResponse> GetContactByIdAsync(int id);
    Task<ContactResponse> AddContactAsync(ContactRequest request);
    Task DeleteContactAsync(int id);
}

public class ContactService : IContactService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IContactRepository _contactRepository;

    public ContactService(IUnitOfWork unitOfWork, IMapper mapper, IContactRepository contactRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _contactRepository = contactRepository;
    }

    public async Task<IEnumerable<ContactResponse>> GetContactsAsync()
    {
        var contacts = await _contactRepository.GetContactsAsync();

        return _mapper.Map<IEnumerable<ContactResponse>>(contacts);
    }

    public async Task<ContactResponse> GetContactByIdAsync(int id)
    {
        var response = await _contactRepository.GetContactByIdAsync(id);

        return _mapper.Map<ContactResponse>(response);
    }

    public async Task<ContactResponse> AddContactAsync(ContactRequest request)
    {
        var contact = _mapper.Map<Contact>(request);

        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _contactRepository.AddContactAsync(contact);
        });

        return _mapper.Map<ContactResponse>(contact);
    }

    public async Task DeleteContactAsync(int id)
    {
        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _contactRepository.DeleteContactAsync(id);
        });
    }
}
