namespace ValidEnvTesting;

using ValidEnv;
using System.Net.Mail;

public class TestBuiltIn
{
    // === Strings ===
    private static readonly BuiltIn.StringConverter stringConverter = new();
    private static readonly EnvironmentVariable<string> stringVar = new("", stringConverter);

    [Theory]
    [InlineData("abc", 1, 2)]
    [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", 10, 120)]
    [InlineData("", 5, 10)]
    public void StringConverterInvalidLength(string input, int minLength, int maxLength)
    {
        stringConverter.WithMinLength(minLength);
        stringConverter.WithMaxLength(maxLength);
        Assert.Throws<Exceptions.ValidEnvCustomException>(() => stringConverter.FromString(input, stringVar));
    }
    

    // === Booleans ===
    private static readonly BuiltIn.BoolConverter boolConverter = new();
    private static readonly EnvironmentVariable<bool> boolVar = new("", boolConverter);

    [Theory]
    [InlineData("true", true)]
    [InlineData("TRUE", true)]
    [InlineData("1", true)]
    [InlineData("t", true)]
    [InlineData("yes", true)]
    [InlineData("YES", true)]
    [InlineData("false", false)]
    [InlineData("FALSE", false)]
    [InlineData("0", false)]
    [InlineData("f", false)]
    [InlineData("no", false)]
    [InlineData("NO", false)]
    public void BooleanConverter(string input, bool expected)
    {
        Assert.Equal(expected, boolConverter.FromString(input, boolVar));
    }

    [Theory]
    [InlineData("")]
    [InlineData("wrong")]
    [InlineData("JKLÖDas")]
    public void BooleanConverterInvalid(string input)
    {
        Assert.Throws<Exceptions.ValidEnvParseException<bool>>(() => boolConverter.FromString(input, boolVar));
    }


    // === Integers ===
    private static readonly BuiltIn.IntConverter intConverter = new();
    private static readonly EnvironmentVariable<int> intVar = new("", intConverter);

    [Theory]
    [InlineData("0", 0)]
    [InlineData("-12", -12)]
    public void IntConverter(string input, int expected)
    {
        Assert.Equal(expected, intConverter.FromString(input, intVar));
    }

    [Theory]
    [InlineData("F")]
    [InlineData("")]
    [InlineData("0.1")]
    public void IntConverterInvalid(string input)
    {
        Assert.Throws<Exceptions.ValidEnvParseException<int>>(() => intConverter.FromString(input, intVar));
    }


    // === Ports ===
    private static readonly BuiltIn.PortConverter portConverter = new();
    private static readonly EnvironmentVariable<int> portVar = new("", portConverter);

    [Theory]
    [InlineData("65536")] // Maximum allowed port number is 65535
    public void PortConverterInvalid(string input)
    {
        Assert.Throws<Exceptions.ValidEnvCustomException>(() => portConverter.FromString(input, intVar));
    }


    // === Email ===
    private static readonly BuiltIn.EmailConverter emailConverter = new();
    private static readonly EnvironmentVariable<MailAddress> emailVar = new("", emailConverter);
    public static TheoryData<string, MailAddress> TestMails => new()
    {
        { "test@example.com", new MailAddress("test@example.com") },
        { "123@gmx.de", new MailAddress("123@gmx.de") },
    };

    [Theory]
    [MemberData(nameof(TestMails))]
    public void EmailConverter(string input, MailAddress expected)
    {
        Assert.Equal(expected, emailConverter.FromString(input, emailVar));
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("@gmail.com")]
    [InlineData("123")]
    [InlineData(".com")]
    public void EmailConverterInvalid(string input)
    {
        Assert.Throws<Exceptions.ValidEnvParseException<MailAddress>>(() => emailConverter.FromString(input, emailVar));
    }


    // === Uri ===
    private static readonly BuiltIn.UriConverter uriConverter = new();
    private static readonly EnvironmentVariable<Uri> uriVar = new("", uriConverter);
    public static TheoryData<string, Uri> TestUris => new()
    {
        { "https://example.com", new Uri("https://example.com") },
    };

    [Theory]
    [MemberData(nameof(TestUris))]
    public void UriConverter(string input, Uri expected)
    {
        Assert.Equal(expected, uriConverter.FromString(input, uriVar));
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("123")]
    [InlineData("@gmail.com")]
    [InlineData(".com")]
    public void UriConverterInvalid(string input)
    {
        Assert.Throws<Exceptions.ValidEnvParseException<Uri>>(() => uriConverter.FromString(input, uriVar));
    }
}
