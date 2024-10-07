using System;
using System.Collections.Generic;

namespace Thunders.Domain.Base
{
    public class BasicException : Exception
    {
        public int Code { get; set; }
        public Exception? Ex { get; set; }

        public BasicException(string message, Exception? ex = null) : base(message)
        {
            this.Ex = ex;
        }
    }

    public class NotFoundException : BasicException
    {
        public NotFoundException(string message) : base(message)
        {
            Code = 404;
        }
    }

    public class BadRequestException : BasicException
    {
        public IList<string> Messages { get; set; } = new List<string>();

        public BadRequestException(string message) : base(message)
        {
            Code = 400;
        }

        public BadRequestException(string message, Exception ex) : base(message, ex)
        {
            Code = 400;
        }

        public BadRequestException(IList<string> messages) : base(string.Join("<br/>", messages))
        {
            Code = 400;
            Messages = messages;
        }

        public BadRequestException(IList<string> messages, Exception ex) : base(string.Join("<br/>", messages), ex)
        {
            Code = 400;
            Messages = messages;
        }
    }

    public class ConflictException : BasicException
    {
        public ConflictException(string message) : base(message)
        {
            Code = 409;
        }
    }
    public class NotAcceptableException : BasicException
    {
        public NotAcceptableException(string message) : base(message)
        {
            Code = 406;
        }
    }

    public class NotAuthorizedException : BasicException
    {
        public NotAuthorizedException(string message) : base(message)
        {
            Code = 401;
        }
    }

    public class ForbiddenException : BasicException
    {
        public ForbiddenException(string message) : base(message)
        {
            Code = 403;
        }
    }

    public class RequestTimeoutException : BasicException
    {
        public RequestTimeoutException(string message) : base(message)
        {
            Code = 408;
        }
    }

    public class UnsupportedMediaTypeException : BasicException
    {
        public UnsupportedMediaTypeException(string message) : base(message)
        {
            Code = 415;
        }
    }

    public class InternalServerErroException : Exception
    {
        public InternalServerErroException(string message) : base(message) { }
        public InternalServerErroException(string message, Exception ex) : base(message, ex) { }
    }

    public class GatewayTimeoutException : BasicException
    {
        public GatewayTimeoutException(string message) : base(message)
        {
            Code = 504;
        }
    }
}

