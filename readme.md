# ValidEnv - Trust your environment variables
<a href="https://www.nuget.org/packages/ValidEnv/" target="_blank"><img src="https://img.shields.io/badge/nuget.org-ValidEnv-blue"></a>

ValidEnv is a small library that makes it easy to access and enforce environment variables when developing .NET applications.
Fully inspired by [envsafe](https://github.com/KATT/envsafe) (which is made for TypeScript) and thereby [envalid](https://github.com/af/envalid).

## Getting started
Install it via the .NET-CLI: `dotnet add package ValidEnv`.

First, we declare a static class, which will hold all of our environment variables and can be accessed anywhere in your application.
In there, we declare a field (also static) with the desired type used by the application.

Next, we assign this field a new instance of an `EnvironmentVariable` of our wanted type (EnvironmentVariable&lt;**TYPE**&gt;), here it's a `string`.

This constructor takes two arguments:
1. The name of the environment variable.
2. A so-called converter, which is essentially a class that converts a string (the env var) to the desired type. [To create your own](#create-your-own), simply create a class that inherits from `IEnvVarConverter<T>`. Now implement the `FromString` method, which takes in a string as input and returns the parsed value of type `T`.

You can also call two additional methods on an `EnvironmentVariable`:
- `WithDescription(string description)`: Describes the purpose of an environment variable
- `WithFallback(T fallback)`: Allows you to specify a default value, which is used if the environment variable isn't declared. (A message will also be printed.)

```c#
using SafeEnv;
//    ^: In this example, this uses: "EnvironmentVariable" and "BuiltIn"

public static class Env
{
    public static string MyPreciousKey = new EnvironmentVariable<string>("MY_PRECIOUS_KEY", new BuiltIn.String()).Load();
}
```

## Converters
Converters are a key concept in this library. They transform the string value of an environment variable to the desired type.
To see which ones come by default, simply look under `ValidEnv.BuiltIn` namespace. But for your convenience, here's also a table:

| Converter (BuiltIn.*) | Output type | Description |
| --- | --- | ---|
| StringConverter | string | Simple enough. Optionally, it allows you to specify a minimum and/or maximum length. (`.WithMinLength(int minLength)` / `.WithMaxLength(int maxLength)`)|
| BooleanConverter | bool | Parses "true", "1", "t" or "yes" as `true` and "false", "0", "f" or "no" as `false`. Case independant.|
| IntConverter | int | Parses an integer. |
| PortConverter | int | Ensures, that the parsed integer does not exceed the maximum TCP port number (65535). |
| EmailConverter | System.Net.Mail.MailAddress | Parses the environment variable into an EmailAddress. |
| UriConverter | Uri | Parses into a Uri. |

### Create your own
This library is designed to be extendable. Creating your own converters enables you to implement parsing into custom classes or defining new constraints on existing types.

In order to create a new converter, create a class that implements `ValidEnv.IEnvVarConverter<T>`. `T` should be the type / class of your choice.

Let's assume you want to parse this custom string `"yes;10cm;30cm;10.2%"` into the following class:

```c#
public class FluxCapacitor
{
    public bool Active { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double PlutoniumLevel { get; set; }

    public FluxCapacitor(bool active, int width, int height, double plutoniumLevel)
    {
        Active = active;
        Width = width;
        Height = height;
        PlutoniumLevel = plutoniumLevel;
    }
}
```

Now let's create our converter class and implement `IEnvVarConverter<FluxCapacitor>`, including the `FromString` function from IEnvVarConverter.

```c#
public class FluxCapacitorConverter : IEnvVarConverter<FluxCapacitor>
{
    public FluxCapacitor FromString(string input, EnvironmentVariable<FluxCapacitor> ctx)
    {
        // TODO!
    }
}
```

So far, so good. This is probably a good time to *throw* in that ValidEnv comes with a few built-in exceptions that ease the development of custom converters.
There is `ValidEnvParseException<T>` and `ValidEnvCustomException`. We will be using these two in the converter we are about to write.

(Technically there's also `ValidEnvMissingException<T>`, but that's not used in the converters, because when a converter is called, it is already insured that the value passed in isn't null.)

First, we make sure that after splitting our string input by semicolons, we have four "parts" to parse.

```c#
// ...
    public FluxCapacitor FromString(string input, EnvironmentVariable<FluxCapacitor> ctx)
    {
        string[] inputParts = input.Split(";");

        // Something is wrong here!
        if (inputParts.Length != 4)
        {
            throw new Exceptions.ValidEnvParseException<FluxCapacitor>(input, ctx, new Exception("String must contain four parts, separated by a semicolon."));
        }
    }
// ...
```

Let's add the rest of the parsing, right? (On a quick note: This isn't a "good" solution and is quite hacky).

You can probably do it better! Last but not least, we need to return a new instance of (a) `FluxCapacitor` with the values we parsed.

**Complete converter:**
```c#
public class FluxCapacitorConverter : IEnvVarConverter<FluxCapacitor>
{
    // reference: "yes;10cm;30cm;10.2%"
    public FluxCapacitor FromString(string input, EnvironmentVariable<FluxCapacitor> ctx)
    {
        string[] inputParts = input.Split(";");

        // Something is wrong here!
        if (inputParts.Length != 4)
        {
            throw new Exceptions.ValidEnvParseException<FluxCapacitor>(input, ctx, new Exception("String must contain four parts, separated by a semicolon."));
        }

        string activeRaw = inputParts[0].ToLower();
        Exception activeParseException = new("First part can only be 'yes' or 'no'.");
        bool active = activeRaw == "yes" ? true : activeRaw == "no" ? false : throw new Exceptions.ValidEnvParseException<FluxCapacitor>(input, ctx, activeParseException);

        string widthRaw = inputParts[1].ToLower();
        widthRaw.Replace("cm", "");
        int width;

        try
        {
            width = int.Parse(widthRaw);
        } catch (FormatException e)
        {
            throw new Exceptions.ValidEnvParseException<FluxCapacitor>(input, ctx, e);
        }


        string heightRaw = inputParts[2].ToLower();
        widthRaw.Replace("cm", "");
        int height;

        try
        {
            height = int.Parse(heightRaw);
        } catch (FormatException e)
        {
            throw new Exceptions.ValidEnvParseException<FluxCapacitor>(input, ctx, e);
        }


        string plutoniumLevelRaw = inputParts[3];
        plutoniumLevelRaw.Replace("%", "");
        double plutoniumLevel;

        try
        {
            plutoniumLevel = double.Parse(plutoniumLevelRaw);
        } catch (FormatException e)
        {
            throw new Exceptions.ValidEnvParseException<FluxCapacitor>(input, ctx, e);
        }

        return new FluxCapacitor(active, width, height, plutoniumLevel);
    }
}
```

Now, it can be used in your static environment variable class as follows:
```c#
// ...
    public static FluxCapacitor FLUX = new EnvironmentVariable<FluxCapacitor>("FLUX", new FluxCapacitorConverter()).Load();
// ...
```

Congrats, that's actually all it takes, given that this is a quite unrealistic use case.

## Design choices
You might notice that this isn't exactly like the [envsafe](https://github.com/KATT/envsafe) library for TypeScript.
In [envsafe](https://github.com/KATT/envsafe) you simply call `str()` or `port()` when initializing a variable. You can look at the ReadMe of [envsafe](https://github.com/KATT/envsafe) to get an idea.

However, I found this approach not quite suitable for C#. It surely would have been possible, but I don't feel like this would have added any value.
Yes, this adds an extra level of abstraction — the converter stuff. But that's the way it is. ;)
But more importantly, it keeps the actual custom implementation separate from the `EnvironmentVariable` class, which is a lot more descriptive in my humble opinion.
