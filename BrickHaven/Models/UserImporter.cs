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
        using (var reader = new StreamReader(csvFilePath))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                // Extracting values from CSV columns
                var customerId = int.Parse(values[0]); // Assuming customerid is an INT in the CSV
                var firstName = values[1];
                var lastName = values[2];
                var birthday = DateTime.Parse(values[3]); // Assuming birthday is in a valid date format in the CSV
                var residenceCountry = values[4];
                var gender = values[5];
                var age = float.Parse(values[6]); // Assuming age is represented as an integer in the CSV

                // Dynamic username generation (but still brickmaster#)
                var username = $"brickmaster{customerId}";
                var email = $"user{customerId}@brickhaven.com";
                var emailConfirmed = true;
                var lockoutEnabled = false;
                var cookieEnabled = false;

                var user = new Customer
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = emailConfirmed,
                    LockoutEnabled = lockoutEnabled,
                    CustomerID = customerId,
                    FirstName = firstName,
                    LastName = lastName,
                    Birthday = birthday,
                    ResidenceCountry = residenceCountry,
                    Gender = gender,
                    Age = age
                };

                // Create user with password "BrickHaven#.123!"
                var result = await _userManager.CreateAsync(user, $"BrickHaven{customerId}.123!");

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
            }
        }
    }
}