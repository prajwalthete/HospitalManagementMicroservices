﻿namespace UserManagementService.GlobleExceptionhandler
{
    public class DuplicateEmailException : Exception
    {
        public DuplicateEmailException(string message) : base(message) { }

    }
}
