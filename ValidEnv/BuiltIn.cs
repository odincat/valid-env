namespace ValidEnv;

using System.Net.Mail;

public class BuiltIn {
    public class StringConverter : IEnvVarConverter<string>
    {
        private int? MinLength { get; set; }
        private int? MaxLength { get; set; }

        public StringConverter WithMinLength(int minLength)
        {
            MinLength = minLength;
            return this;
        }

        public StringConverter WithMaxLength(int maxLength)
        {
            MaxLength = maxLength;
            return this;
        }
        
        public string FromString(string input, EnvironmentVariable<string> ctx)
        {
            if (MinLength != null && input.Length < MinLength) {
                string errorMessage = $"Variable '{ctx.Key}' ({input}) is too short. Min length is {MaxLength} characters.";
                throw new Exceptions.ValidEnvCustomException(errorMessage);
            }

            if (MaxLength != null && input.Length > MaxLength) {
                string errorMessage = $"Variable '{ctx.Key}' is too long. Max length is {MaxLength} characters.";
                throw new Exceptions.ValidEnvCustomException(errorMessage);
            }

            return input;
        }
    }

    public class BoolConverter : IEnvVarConverter<bool>
    {
        public bool FromString(string input, EnvironmentVariable<bool> ctx)
        {
            return input.ToLower() switch
            {
                "true" or "1" or "t" or "yes" => true,
                "false" or "0" or "f" or "no" => false,
                _ => throw new Exceptions.ValidEnvParseException<bool>(input, ctx, null),
            };
        }
    }

    public class IntConverter : IEnvVarConverter<int>
    {
        public int FromString(string input, EnvironmentVariable<int> ctx)
        {
            try {
                return int.Parse(input);
            } catch (Exception e)
            {
                throw new Exceptions.ValidEnvParseException<int>(input, ctx, e);
            }
        }
    }

    public class PortConverter : IEnvVarConverter<int>
    {
        private static readonly int MAX_PORT_NUMBER = 65535;

        public int FromString(string input, EnvironmentVariable<int> ctx)
        {
            try {
                var port = int.Parse(input);

                if (port > MAX_PORT_NUMBER)
                {
                    string errorMessage = $"Port number '{ctx.Key}' ({port}) is too large. The maximum possible port number is {MAX_PORT_NUMBER}";
                    throw new Exceptions.ValidEnvCustomException(errorMessage);
                }

                return port;
            } catch (FormatException e)
            {
                throw new Exceptions.ValidEnvParseException<int>(input, ctx, e);
            }
        }
    }

    public class EmailConverter : IEnvVarConverter<MailAddress>
    {
        public MailAddress FromString(string input, EnvironmentVariable<MailAddress> ctx)
        {
            try {
                var address = new MailAddress(input);
                return address;
            } catch (FormatException e) {
                throw new Exceptions.ValidEnvParseException<MailAddress>(input, ctx, e);
            }
        }
    }

    public class UriConverter : IEnvVarConverter<Uri>
    {
        public Uri FromString(string input, EnvironmentVariable<Uri> ctx)
        {
            try {
                var uri = new Uri(input);
                return uri;
            } catch (FormatException e) {
                throw new Exceptions.ValidEnvParseException<Uri>(input, ctx, e);
            }
        }
    }
}
