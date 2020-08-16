using System;

namespace Users.Exceptions
{
    public class UsernameNotUniqueException : Exception
    {
        public UsernameNotUniqueException()
            : base("Username was not unique")
        {
        }

        public UsernameNotUniqueException(string username)
            : base($"Username was not unique: {username}")
        {
        }
    }
}