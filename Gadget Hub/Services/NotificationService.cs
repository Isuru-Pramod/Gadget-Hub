using GadgetHub.WebAPI.Models;

namespace GadgetHub.WebAPI.Services;

public class NotificationService
{
    public void Send(Notification notification)
    {
        Console.WriteLine("=== Email Notification ===");
        Console.WriteLine($"To: {notification.Email}");
        Console.WriteLine($"Subject: {notification.Subject}");
        Console.WriteLine($"Message: {notification.Message}");
    }
}
