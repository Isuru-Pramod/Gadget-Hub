namespace GadgetHub.WebAPI.Models;

public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
