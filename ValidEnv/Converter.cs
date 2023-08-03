namespace ValidEnv;

public interface IEnvVarConverter<T>
{
    T FromString(string input, EnvironmentVariable<T> ctx);
}
