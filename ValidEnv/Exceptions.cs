namespace ValidEnv;

public class Exceptions
{
    public class ValidEnvParseException<T> : Exception
    {
        private static string CreateExceptionMessage(string value, EnvironmentVariable<T> ctx, Exception? exception)
        {
            string message = $"\nUnable to convert environment variable {ctx.Key} = '{value}' (String) to '{typeof(T).Name}'. ❌";

            if (ctx.Description != null)
            {
                message += $"\nVariable description: {ctx.Description}";
            }

            if (exception != null)
            {
                message += $"\nException: {exception.Message}";
            }

            return message;
        }

        public ValidEnvParseException(string value, EnvironmentVariable<T> ctx, Exception? exception) : base(CreateExceptionMessage(value, ctx, exception))
        {}
    }


    public class ValidEnvMissingException<T> : Exception
    {
        private static string CreateExceptionMessage(string key, string? description)
        {
            string message = $"\nEnvironment variable '{key}' ({typeof(T).Name}) is not declared. 💨";

            if (description != null)
            {
                message += $"\nVariable description: {description}";
            }

            return message;
        }

        public ValidEnvMissingException(string name, string? description) : base(CreateExceptionMessage(name, description))
        {}
    }


    public class ValidEnvCustomException : Exception
    {
        public ValidEnvCustomException(string errorMessage) : base(errorMessage)
        {}
    }
}
