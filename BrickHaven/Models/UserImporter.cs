using BrickHaven.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.IO;
using System.Threading.Tasks;

public class UserImporter
{
    private readonly UserManager<Customer> _userManager;

    public UserImporter(UserManager<Customer> userManager)
    {
        _userManager = userManager;
    }

    public async Task ImportUsersFromCsvAsync(string csvFilePath)
    {
        int userId = 1; // Starting ID number
        using (var reader = new StreamReader(csvFilePath))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                var username = $"user{userId}"; // Generate username based on ID
                var password = "Hello123!"; // Default password

                var user = new Customer { UserName = username };
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    Console.WriteLine($"Created user: {username}");
                }
                else
                {
                    Console.WriteLine($"Failed to create user: {username}");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                    }
                }

                userId++; // Increment ID for the next user
            }
        }
    }
}
