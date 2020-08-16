using System;

namespace Users.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException()
            : base("User not found")
        {
        }

        public UserNotFoundException(int id)
            : base($"User not found: {id}")
        {
        }
    }
}