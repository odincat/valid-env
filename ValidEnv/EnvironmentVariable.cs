namespace ValidEnv;

public class EnvironmentVariable<T>
{
    private IEnvVarConverter<T> Converter { get; }

    public string Key { get; }
    public T? Value { get; set; }
    public T? Fallback { get; set; }
    public string? Description { get; set; }

    public EnvironmentVariable(string key, IEnvVarConverter<T> converter)
    {
        Key = key;
        Converter = converter;
    }

    public EnvironmentVariable<T> WithDescription(string description)
    {
        Description = description;
        return this;
    }

    public EnvironmentVariable<T> WithFallback(T fallback)
    {
        Fallback = fallback;
        return this;
    }
    
    public T Load()
    {
        var inputVariable = Environment.GetEnvironmentVariable(Key);

        if (inputVariable == null)
        {
            if (Fallback != null)
            {
                Console.WriteLine($"Environment variable '{Key}' is not available. Using fallback '{Fallback}' instead.");
                Value = Fallback;
            }
            else
            {
                throw new Exceptions.ValidEnvMissingException<T>(Key, Description);
            }
        }
        else
        {
            try
            {
                Value = Converter.FromString(inputVariable, this);
            }
            catch (Exception)
            {
                if (Fallback != null)
                {
                    Value = Fallback;
                    Console.WriteLine($"Failed to parse environment variable '{Key}'. Using fallback '{Fallback}' instead.");

                    return Value;
                }

                throw;
            }
        }

        return Value;
    }
}
