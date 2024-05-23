using TianyiNetwork.Web.AppsApi.Controllers;

namespace TianyiNetwork.Web.AppsApi.RateLimiting
{
    /// <summary>
    /// Limiting 8 req/hour for <see cref="CardController.CreateCard"/>
    /// </summary>
    public class CardCreateRateLimitingPolicy() : IpBasedRateLimitingPolicy(
        policyName: nameof(CardController.CreateCard),
        permitLimit: 8,
        window: TimeSpan.FromHours(1))
    {
        public static string PolicyName = nameof(CardController.CreateCard);
    }
}
