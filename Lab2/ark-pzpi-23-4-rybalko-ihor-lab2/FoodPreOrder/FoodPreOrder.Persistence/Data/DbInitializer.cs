using FoodPreOrder.Domain.Entities;
using FoodPreOrder.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPreOrder.Persistence.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine("База вже існує або помилка міграції: " + ex.Message);
            }

            if (context.Users.Any())
            {
                return;
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword("1234567890");

            var admin = new User
            {
                FullName = "Головний Власник",
                Email = "admin@glowee.com",
                Phone = "+380670000001",
                Role = UserRole.Admin,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            var client = new User
            {
                FullName = "Іван Клієнт",
                Email = "client@gmail.com",
                Phone = "+380670000002",
                Role = UserRole.Customer,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            var owner2 = new User
            {
                FullName = "Марія Піцайоло",
                Email = "maria@glowee.com",
                Phone = "+380670000004",
                Role = UserRole.Admin,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.AddRange(admin, client, owner2);
            context.SaveChanges();

            var restaurant = new Restaurant
            {
                NameUA = "Glowee Бургер",
                NameEN = "Glowee Burger",
                Address = "м. Харків, пр. Науки 14",
                IsActive = true,
                OwnerId = admin.Id,
                Latitude = 50.005,
                Longitude = 36.230,
                RegisteredAt = DateTime.UtcNow,
                PaidUntil = DateTime.UtcNow.AddMonths(1)
            };

            context.Restaurants.Add(restaurant);
            context.SaveChanges();

            var cook = new User
            {
                FullName = "Петро Кухар",
                Email = "cook@glowee.com",
                Phone = "+380670000003",
                Role = UserRole.KitchenStaff,
                PasswordHash = passwordHash,
                RestaurantId = restaurant.Id,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(cook);
            context.SaveChanges();

            var catBurgers = new Category { NameUA = "Бургери", NameEN = "Burgers", RestaurantId = restaurant.Id };
            var catDrinks = new Category { NameUA = "Напої", NameEN = "Drinks", RestaurantId = restaurant.Id };
            context.Categories.AddRange(catBurgers, catDrinks);
            context.SaveChanges();

            var dishes1 = new[]
            {
                new Dish
                {
                    NameUA = "Чізбургер", NameEN = "Cheeseburger",
                    DescriptionUA = "Соковита яловичина, сир чеддер",
                    Price = 150, CategoryId = catBurgers.Id, PreparationTimeMinutes = 15, IsAvailable = true
                },
                new Dish
                {
                    NameUA = "Кола Zero", NameEN = "Coke Zero",
                    DescriptionUA = "0.5л",
                    Price = 40, CategoryId = catDrinks.Id, PreparationTimeMinutes = 2, IsAvailable = true
                }
            };
            context.Dishes.AddRange(dishes1);
            context.SaveChanges();

            var terminal1 = new IoTDevice
            {
                SerialNumber = "DEV-KITCHEN-001",
                LocationName = "Гарячий цех",
                IsActive = true,
                RestaurantId = restaurant.Id,
                LastPing = DateTime.UtcNow
            };
            context.IoTDevices.Add(terminal1);
            context.SaveChanges();

            var restaurant2 = new Restaurant
            {
                NameUA = "Glowee Піца & Паста",
                NameEN = "Glowee Pizza & Pasta",
                Address = "м. Харків, вул. Сумська 10",
                IsActive = true,
                OwnerId = owner2.Id, 
                Latitude = 50.000, 
                Longitude = 36.235,
                RegisteredAt = DateTime.UtcNow,
                PaidUntil = DateTime.UtcNow.AddMonths(2)
            };

            context.Restaurants.Add(restaurant2);
            context.SaveChanges();

            var catPizza = new Category { NameUA = "Піца", NameEN = "Pizza", RestaurantId = restaurant2.Id };
            var catSalads = new Category { NameUA = "Салати", NameEN = "Salads", RestaurantId = restaurant2.Id };
            context.Categories.AddRange(catPizza, catSalads);
            context.SaveChanges();

            var dishes2 = new[]
            {
                new Dish
                {
                    NameUA = "Піца Пепероні", NameEN = "Pepperoni Pizza",
                    DescriptionUA = "Гостра ковбаса, моцарела, томатний соус",
                    DescriptionEN = "Spicy sausage, mozzarella, tomato sauce",
                    Price = 220,
                    CategoryId = catPizza.Id,
                    PreparationTimeMinutes = 20,
                    IsAvailable = true
                },
                new Dish
                {
                    NameUA = "Піца Маргарита", NameEN = "Margherita Pizza",
                    DescriptionUA = "Класична піца з томатами та базиліком",
                    DescriptionEN = "Classic pizza with tomatoes and basil",
                    Price = 180,
                    CategoryId = catPizza.Id,
                    PreparationTimeMinutes = 15,
                    IsAvailable = true
                },
                new Dish
                {
                    NameUA = "Салат Цезар", NameEN = "Caesar Salad",
                    DescriptionUA = "Курка, пармезан, сухарики, соус",
                    DescriptionEN = "Chicken, parmesan, croutons, sauce",
                    Price = 160,
                    CategoryId = catSalads.Id,
                    PreparationTimeMinutes = 10,
                    IsAvailable = true
                }
            };
            context.Dishes.AddRange(dishes2);
            context.SaveChanges();

            var terminal2 = new IoTDevice
            {
                SerialNumber = "DEV-PIZZA-002",
                LocationName = "Піч",
                IsActive = true,
                RestaurantId = restaurant2.Id,
                LastPing = DateTime.UtcNow
            };
            context.IoTDevices.Add(terminal2);
            context.SaveChanges();
        }
    }
}
