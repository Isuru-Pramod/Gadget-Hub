using GadgetHub.WebAPI.Models;

namespace GadgetHub.WebAPI.Services;

public class CustomerService
{
    private readonly List<Customer> _customers = new();

    public Customer Register(Customer customer)
    {
        _customers.Add(customer);
        return customer;
    }

    public Customer Login(string email, string password)
    {
        return _customers.FirstOrDefault(c => c.Email == email && c.Password == password);
    }

    public Customer GetById(Guid id)
    {
        return _customers.FirstOrDefault(c => c.Id == id);
    }
}
