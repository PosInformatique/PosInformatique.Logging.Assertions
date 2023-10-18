# PosInformatique.Logging.Assertions
PosInformatique.Logging.Assertions is a library to mock and assert easily the logs generated by the ILogger interface.

## Installing from NuGet
The [PosInformatique.Logging.Assertions](https://www.nuget.org/packages/PosInformatique.Logging.Assertions/)
library is available directly on the
[![Nuget](https://img.shields.io/nuget/v/PosInformatique.Logging.Assertions)](https://www.nuget.org/packages/PosInformatique.Logging.Assertions/)
official website.

To download and install the library to your Visual Studio unit test projects use the following NuGet command line 

```
Install-Package PosInformatique.Logging.Assertions
```

## How it is work?

Imagine that you have the following class which contains various log:

```csharp
public class CustomerManager
{
    private readonly IEmailProvider emailProvider;

    private readonly ILogger<CustomerManager> logger;

    public CustomerManager(IEmailProvider emailProvider, ILogger<CustomerManager> logger)
    {
        this.emailProvider = emailProvider;
        this.logger = logger;
    }

    public async Task SendEmailAsync(int id, string name)
    {
        this.logger.LogInformation($"Starting to send an email to the customer '{Name}' with the identifier '{Id}'", name, id);

        using (this.logger.BeginScope(new { Id = id }))
        {
            try
            {
                this.logger.LogDebug($"Call the SendAsync() method");

                await this.emailProvider.SendAsync(name);

                this.logger.LogDebug($"SendAsync() method has been called.");

                this.logger.LogInformation($"Email provider has been called.");
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Unable to send the email !");
            }
        }
    }
}

public interface IEmailProvider
{
    Task SendAsync(string name);
}
```

As a good developer (like me), who always do unit tests with 100% of code coverage, you have to write the unit tests
to test the `SendEmailAsync()` method and mock the `IEmailProvider` and `ILogger<T>` interfaces with your favorite mocking library.
([Moq](https://github.com/devlooped/moq) for me...).

> Some developers consider that log information should not be test with unit tests... :laughing: :laughing: 
`ILogger` interface and his methods calls should be test as any normal code and specially the scope to be sure we
inject the right data. Most often, developpers discover that their own logs don't log things correctly in production
environments... :laughing: :laughing: 

So the unit test to write for the previous example should look like something like that:

```csharp
[Fact]
public async Task SendEmailAsync()
{
    // Arrange
    var emailProvider = new Mock<IEmailProvider>(MockBehavior.Strict);
    emailProvider.Setup(ep => ep.SendAsync("Gilles TOURREAU"))
        .Returns(Task.CompletedTask);

    var logger = new Mock<ILogger<CustomerManager>>(MockBehavior.Strict);
    logger.Setup(l => l.Log(LogLevel.Information, "Starting to send an email to the customer '{Name}' with the identifier '{Id}'", ..., ..., ... )) // WTF???
        ...
    logger.Setup(l => l.BeginScope<...>(...))   // WTF???

    var manager = new CustomerManager(emailProvider.Object, logger.Object);

    // Act
    await manager.SendEmailAsync(1234, "Gilles TOURREAU");

    // Assert
    emailProvider.VerifyAll();
    logger.VerifyAll();
}

```

As your can see, it is very hard to mock the [Log()](https://learn.microsoft.com/fr-fr/dotnet/api/microsoft.extensions.logging.ilogger.log)
method of the [ILogger](https://learn.microsoft.com/fr-fr/dotnet/api/microsoft.extensions.logging.ilogger)
interface which have the following signature:

```csharp
void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
```

And also to check the scope usage with the [ILogger](https://learn.microsoft.com/fr-fr/dotnet/api/microsoft.extensions.logging.ilogger)
interface it can be hard!

The [PosInformatique.Logging.Assertions](https://www.nuget.org/packages/PosInformatique.Logging.Assertions/) library
allows the developpers to mock the [ILogger](https://learn.microsoft.com/fr-fr/dotnet/api/microsoft.extensions.logging.ilogger)
easily and setup the sequence of the expected logs using a fluent style code. For the previous example,
this is how the unit test look like for the previous example.

```csharp
[Fact]
public async Task SendEmailAsync()
{
    // Arrange
    var emailProvider = new Mock<IEmailProvider>(MockBehavior.Strict);
    emailProvider.Setup(ep => ep.SendAsync("Gilles TOURREAU"))
        .Returns(Task.CompletedTask);

    var logger = new LoggerMock<CustomerManager>();
    logger.SetupSequence()
        .LogInformation("Starting to send an email to the customer '{Name}' with the identifier '{Id}'")
            .WithArguments("Gilles TOURREAU", 1234)
        .BeginScope(new { Id = 1234 })
            .LogDebug("Call the SendAsync() method")
            .LogDebug("SendAsync() method has been called.")
            .LogInformation("Email provider has been called.")
        .EndScope();

    var manager = new CustomerManager(emailProvider.Object, logger.Object);

    // Act
    await manager.SendEmailAsync(1234, "Gilles TOURREAU");

    // Assert
    emailProvider.VerifyAll();
    logger.VerifyLogs();
}
```

:heart_eyes: :heart_eyes: Sexy isn't it??? The unit test is more easy to read and write!

> Do not forget to call the `VerifyLogs()` at the end of your unit test like a `VerifyAll()` call with the
[Moq](https://github.com/devlooped/moq) library. The `VerifyLogs()` will check that all methods setup (*Arrange*) are called
by the code under test (*Act*).

Do not hesitate to use the indentation to make the fluent code more readable specially when
using nested scopes.

For example to check nested log scopes write the following code with the following indented code:
```csharp
var logger = new LoggerMock<CustomerManager>();
logger.SetupSequence()
    .LogInformation("Starting to send an email to the customer '{Name}' with the identifier '{Id}'")
        .WithArguments("Gilles TOURREAU", 1234)
    .BeginScope(new { Id = 1234 })
        .BeginScope(new { Name = "Gilles" })
            .LogError("Error in the scope 1234 + Gilles")
        .EndScope()
        .LogInformation("Log between the 2 nested scopes.")
        .BeginScope(new { Name = "Aiza" })
            .LogError("Error in the scope 1234 + Aiza")
        .EndScope()
    .EndScope();
```

### Test the error logs with an exception.
To test an `Exception` with specified in the `LogError()` method of the `ILogger` interface use the
`WithException()` method an set the instance expected:

```csharp
[Fact]
public async Task SendEmailAsync_WithException()
{
    // Arrange
    var theException = new FormatException("An exception");

    var emailProvider = new Mock<IEmailProvider>(MockBehavior.Strict);
    emailProvider.Setup(ep => ep.SendAsync("Gilles TOURREAU"))
        .ThrowsAsync(theException);

    var logger = new LoggerMock<CustomerManager>();
    logger.SetupSequence()
        .LogInformation("Starting to send an email to the customer '{Name}' with the identifier '{Id}'")
            .WithArguments("Gilles TOURREAU", 1234)
        .BeginScope(new { Id = 1234 })
            .LogDebug("Call the SendAsync() method")
            .LogError("Unable to send the email !")
                .WithException(theException)
        .EndScope();

    var manager = new CustomerManager(emailProvider.Object, logger.Object);

    // Act
    await manager.Invoking(m => m.SendEmailAsync(1234, "Gilles TOURREAU"))
        .Should().ThrowExactlyAsync<FormatException>();

    // Assert
    emailProvider.VerifyAll();
    logger.VerifyLogs();
}
```

In the case the exception is throw by the code (It is mean the exception is not produced by the unit test
during the *Arrange* phase), use the version with a delegate to check the content of the Exception:

```csharp
var logger = new LoggerMock<CustomerManager>();
logger.SetupSequence()
    .LogInformation("Starting to send an email to the customer '{Name}' with the identifier '{Id}'")
        .WithArguments("Gilles TOURREAU", 1234)
    .BeginScope(new { Id = 1234 })
        .LogDebug("Call the SendAsync() method")
        .LogError("Unable to send the email !")
            .WithException(e =>
            {
                e.Message.Should().Be("An exception");
                e.InnerException.Should().BeNull();
            })
    .EndScope();
```

### Test log message templates
The power of this library is the ability to assert the
[log message templates](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/#log-message-template)
including the arguments. (*You know the kind of log messages
which contains the vital identifiers to search in emergency in production environment and are often bad logged by the developpers...*
:laughing: :laughing:).

To assert the log message templates parameters use the `WithArguments()` method which is available with 2 overloads:
- `WithArguments(params object[] expectedArguments)`: Allows to specify the expected arguments of the log message template.
- `WithArguments(int expectedCount, Action<LogMessageTemplateArguments> expectedArguments)`: Allows to specify
an delegate to assert complex arguments.

For example, to assert the following log message:
```csharp
this.logger.LogInformation($"Starting to send an email to the customer '{Name}' with the identifier '{Id}'", name, id);
```

Using the first way with the `WithArguments(params object[] expectedArguments)` method:

```csharp
var logger = new LoggerMock<CustomerManager>();
logger.SetupSequence()
    .LogInformation("Starting to send an email to the customer '{Name}' with the identifier '{Id}'")
        .WithArguments("Gilles TOURREAU", 1234)

    ... // Continue the setup expected log sequence
```

Using the second way with the `WithArguments(int expectedCount, Action<LogMessageTemplateArguments> expectedArguments)` method
which give you more control of the assertions:

```csharp
var logger = new LoggerMock<CustomerManager>();
logger.SetupSequence()
    .LogInformation("Starting to send an email to the customer '{Name}' with the identifier '{Id}'")
        .WithArguments(2, args =>
        {
            args["Name"].Should().Be("Gilles TOURREAU");
            args["Id"].Should().Be(1234);
        })

    ... // Continue the setup expected log sequence
```

> Here we use the FluentAssertions library to check the arguments values of the log message template, but of course you can use your
favorite assertion framework to check it.

The second way allows also to check the arguments of the log template message by there index (*it is not what I recommand,
because if the trainee developper in your team change the name of the arguments name in the log message template, you will not
see the impacts in your unit tests execution...*):

```csharp
var logger = new LoggerMock<CustomerManager>();
logger.SetupSequence()
    .LogInformation("Starting to send an email to the customer '{Name}' with the identifier '{Id}'")
        .WithArguments(2, args =>
        {
            args[0].Should().Be("Gilles TOURREAU");
            args[1].Should().Be(1234);
        })

    ... // Continue the setup expected log sequence
```

### Test the BeginScope() state
If you use the `BeginScope` method in your logging process, you can assert the content of state
specified in argument using two methods.

For example, to assert the following code:
```
using (this.logger.BeginScope(new StateInfo() { Id = 1234 }))
{
    ... // Other Log
}
```

With the `StateInfo` class as simple like like that:
```csharp
public class StateInfo
{
    public int Id { get; set; }
}
```

You can assert the `BeginScope()` method call using an anonymous object:

```csharp
var logger = new LoggerMock<CustomerManager>();
logger.SetupSequence()
    .BeginScope(new { Id = 1234 })
       ... // Other Log() assertions
    .EndScope();
```

> The `BeginScope()` assertion check the equivalence (property by property and not the reference itself)
between the actual object in the code and the expected object in the assertion.

Or you can assert the `BeginScope()` method call using a delegate if your state object is complex:

```csharp
var logger = new LoggerMock<CustomerManager>();
logger.SetupSequence()
    .BeginScope<State>(state =>
    {
        state.Id.Should().Be(1234);
    })
       ... // Other Log() assertions
    .EndScope();
```

### Application Insights dictionary state
If you use Application Insights as output of your logs, the `BeginScope()` state argument must take a dictionary of string/object as the following code sample:

```
using (this.logger.BeginScope(new Dictionary<string, object>() { { "Id", 1234 } }))
{
    ... // Other Log
}
```

To assert the `BeginScope()` in the previous sample code, you can use the `SetupSequence().BeginScope(Object)` method assertion as pass the expected
dictionary as argument.

```csharp
var logger = new LoggerMock<CustomerManager>();
logger.SetupSequence()
    .BeginScope(new Dictionary<string, object>() { { "Id", 1234 } })
       ... // Other Log() assertions
    .EndScope();
```

The [PosInformatique.Logging.Assertions](https://www.nuget.org/packages/PosInformatique.Logging.Assertions/) library provides a
`SetupSequence().BeginScopeAsDictionary(Object)` method which allows to assert the content of the dictionary using an object (Each property and his value of the expected
object is considered as a key/value couple of the dictionary). Do not hesitate to use anonymous object in your unit test to make the code more easy to read.

The following example have the same behavior as the previous example, but is more easy to read by removing the dictionary instantiation and some extract brackets:

```csharp
var logger = new LoggerMock<CustomerManager>();
logger.SetupSequence()
    .BeginScopeAsDictionary(new { Id = 1234 })
       ... // Other Log() assertions
    .EndScope();
```

## Assertion fail messages
The [PosInformatique.Logging.Assertions](https://www.nuget.org/packages/PosInformatique.Logging.Assertions/) library
try to make the assert fail messages the most easy to understand for the developers:

![Assertion Failed Too Many Calls](https://raw.githubusercontent.com/PosInformatique/PosInformatique.Logging.Assertions/main/docs/AssertionFailedTooManyCalls.png)
![Assertion Missing Logs](https://raw.githubusercontent.com/PosInformatique/PosInformatique.Logging.Assertions/main/docs/AssertionMissingLogs.png)

## Library dependencies
- The [PosInformatique.Logging.Assertions](https://www.nuget.org/packages/PosInformatique.Logging.Assertions/) target the .NET Standard 2.0
and the version 2.0.0 of the [Microsoft.Extensions.Logging.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Abstractions) NuGet package. So this library can be used with
different .NET architecture projects (.NET Framework, .NET Core, Xamarin,...) and also with old versions of the
[Microsoft.Extensions.Logging.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Abstractions) NuGet package.

- The [PosInformatique.Logging.Assertions](https://www.nuget.org/packages/PosInformatique.Logging.Assertions/) library
depends of the [FluentAssertions](https://github.com/fluentassertions/fluentassertions) library
for internal assertions which is more pretty to read in the exceptions message. It is use the version 6.0.0 and of course it is compatible
with the earlier version of it.

- Also, the [PosInformatique.Logging.Assertions](https://www.nuget.org/packages/PosInformatique.Logging.Assertions/) library
use the internal [FluentAssertions](https://github.com/fluentassertions/fluentassertions) unit test
provider engine detection to throw an exception (when an assertion is false) depending of the engine used to execute
the unit test. For example, `XunitException` if the unit test engine used is `Xunit`.
