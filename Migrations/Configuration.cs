namespace OODProject.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<OODProject.PlayerData>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "OODProject.PlayerData";
        }

        protected override void Seed(OODProject.PlayerData context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
