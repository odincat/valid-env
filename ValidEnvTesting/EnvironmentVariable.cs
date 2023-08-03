namespace ValidEnvTesting;

using ValidEnv;

public class TestEnvironmentVariable
{
    [Theory]
    [InlineData("TEST", "ABC")]
    public void LoadAssert(string inputKey, string inputValue)
    {
        Environment.SetEnvironmentVariable(inputKey, inputValue);

        EnvironmentVariable<string> envVar = new(inputKey, new BuiltIn.StringConverter());
        envVar.Load();

        Assert.Equal(inputKey, envVar.Key);
        Assert.Equal(inputValue, envVar.Value);
    }

    [Theory]
    [InlineData("KLaASUDkalsd")]
    public void EnvVarNull(string inputKey)
    {
        EnvironmentVariable<string> envVar = new(inputKey, new BuiltIn.StringConverter());

        Assert.Throws<Exceptions.ValidEnvMissingException<string>>(() => envVar.Load());
    }

}
