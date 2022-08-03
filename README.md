[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/likvido/Likvido.Test.Api/Publish%20to%20nuget)](https://github.com/Likvido/Likvido.Test.Api/actions?query=workflow%3A%22Publish+to+nuget%22)
[![Nuget](https://img.shields.io/nuget/v/Likvido.Test.Api)](https://www.nuget.org/packages/Likvido.Test.Api/)

# Likvido.Test.Api

Likvido.Test.Api adds support for ASP.NET Core integration tests using a test web host and an in-memory test server.

Integration tests ensure that an app's components function correctly at a level that includes the app's supporting infrastructure, such as the database and network. 

## API app prerequisites
* `TargetFramework net6.0`
* `Microsoft.EntityFrameworkCore`

## How to add integration tets

Create a new test project and reference packages:
* `Likvido.Test.Api`
* `xunit`

Add a text fixture interited from `ConfigurableTestFixture`.
```c#
public class MyTestFixture : ConfigurableTestFixture<Startup, MyDesignDbContext, MyRuntimeDbContext>
{
    public MyTestFixture(DatabaseFixture<MyDesignDbContext, MyRuntimeDbContext> databaseFixture)
        : base(new FixtureOptions<MyDesignDbContext, MyRuntimeDbContext>
        {
            DatabaseFixture = databaseFixture,
            ConfigureNamedHttpClients = new()
            {
                // Configure clients with different settings.
                ["Auth"] = (httpClient, configuration) => httpClient.DefaultRequestHeaders.Add("ApiKey", configuration["ApiKey"])
            };
        })
    {
    }

    // Get configured clients by name:
    public HttpClient AuthClient => HttpClientFactory.GetNamedHttpClient("Auth");

    // Add mocks to overwrite services.
    public Mock<IMyService> MyService { get; } = new();
}
```

Add db context fixture and configuration.
```c#
public class MySqliteContextFixture : SqliteContextFixture<PayablesContext, ApplicationDbContext>
{
    private ITestDataHelper? _testDataHelper;

    protected override ITestDataHelper GetTestDataHelper() =>
        _testDataHelper ??= new MyDbSqliteTestDataHelper(() => DesignContext, Quote);
}

public class MyDbSqliteTestDataHelper : BaseTestDataHelper<MyDesignContext>
{
    public MyDbSqliteTestDataHelper(
        Func<MyDesignContext> context,
        Func<string, string> quote) : base(context, quote)
    {
    }

    public override async Task GenerateTestDataAsync()
    {
        // Add test data to Context.
    }

    public override string[] GetCleanupTables()
    {
        return new[]
        {
            // Names of tables to cleanup after test execution.
        };
    }
}
```

Add an xunit collection. This will share test fixture between tests.
```c#
[CollectionDefinition(Name)]
public class MyTestFixtureCollection : ICollectionFixture<MyTestFixture>
{
    public const string Name = nameof(MyTestFixtureCollection);
}
```

Add controller tests and a collection attribute.

```c#
[Collection(MyTestFixtureCollection.Name)]
public class MyControllerTests : TestsBase<MyTestFixture>
{
  [Fact]
  public async Task Should_Get_Something()
  {
      // Db context is awailable via Fixture.Context. Add extensions to easily work with test data.
      var something = await Fixture.Context.CreateSomething();
      
      var response = await Fixture.Client.GetAsJsonAsync($"/something/{something.Id}", request);
      response.EnsureSuccessStatusCode();

      var actualSomething = Fixture.Context.Something.Find(1);
      var content = await response.Content.ReadFromJsonAsync<JObject>()
      
      // Assert response.
      content.Value<int>("id", 1);
  }
}
```

## Note

[More on integration tests in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-5.0)
