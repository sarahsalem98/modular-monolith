using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Customers.Core.Entities
{
    public class User : IEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string? Address { get; private set; }

        private User() { }


        public static User Create(string name, string email, string? address = null)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                Address = address
            };
        }

        public void UpdateName(string name)
        {
            Name = name;
        }
        public void UpdateEmail(string email) { 
         Email = email;

        }

        public void UpdateAddress(string? address) { 
            Address = address;
        }

    }
}
