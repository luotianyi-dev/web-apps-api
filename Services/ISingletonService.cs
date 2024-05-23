namespace TianyiNetwork.Web.AppsApi.Services
{
    public interface ISingletonService<out T> where T : class
    {
        public static abstract T Create(WebApplicationBuilder builder);
    }
}
