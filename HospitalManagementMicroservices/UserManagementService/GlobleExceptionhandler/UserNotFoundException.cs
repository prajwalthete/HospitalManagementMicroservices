﻿namespace UserManagementService.GlobleExceptionhandler
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message) : base(message) { }

    }
}
