using ValidEnv;

// Easily access them from anywhere in your application
Console.WriteLine(Env.EXAMPLE_API_KEY);

// Declare them once... ^
public static class Env
{
    public static string EXAMPLE_API_KEY = new EnvironmentVariable<string>("EXAMPLE_API_KEY", new BuiltIn.StringConverter())
        // Make it clear, what a variable is doing.
        .WithDescription("This is a very important key, that allows us to fetch X. Can be obtained from Z.")
        .Load(); // Don't forget to call 'Load'.

    private static BuiltIn.StringConverter CookieSecretConverter = new BuiltIn.StringConverter()
        .WithMinLength(50); // We want the secret to be looong
    public static string COOKIE_SECRET = new EnvironmentVariable<string>("EXAMPLE_API_KEY", new BuiltIn.StringConverter())
        .WithDescription("Used for authentication")
        .Load();

    public static bool BYPASS_EMAILS = new EnvironmentVariable<bool>("BYPASS_MAILS", new BuiltIn.BoolConverter())
        .WithDescription("Stops sending out emails and logs them to the console")
        .WithFallback(false) // Avoid exceptions, by setting a default value :)
        .Load();

    public static int SERVER_PORT_NUMBER = new EnvironmentVariable<int>("SERVER_PORT", new BuiltIn.PortConverter())
        .WithDescription("Allows you to specify a custom port for XY.")
        .WithFallback(3000) 
        .Load();

    public static System.Net.Mail.MailAddress FROM_EMAIL = new EnvironmentVariable<System.Net.Mail.MailAddress>("FROM_EMAIL", new BuiltIn.EmailConverter())
        .Load();

    public static Uri BASE_URI = new EnvironmentVariable<Uri>("BASE_URI", new BuiltIn.UriConverter())
        .Load();
}
