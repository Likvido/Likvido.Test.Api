namespace Likvido.Test.Api
{
    public static class HttpClientFactoryBuilder<TStartup>
        where TStartup : class
    {
        public static HttpClientFactory<TStartup> Build(HttpClientFactoryBuilderOptions options) =>
            new(options);
    }
}
