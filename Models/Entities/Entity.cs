using Microsoft.EntityFrameworkCore;

namespace TianyiNetwork.Web.AppsApi.Models.Entities
{
    public class Entity(DbContextOptions<Entity> options) : DbContext(options)
    {
        public required DbSet<CardEntity> Cards { get; set; }
    }
}
