using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Eazly.CommonKit.Module.TemplateURL.Migrations.EntityBuilders;
using Eazly.CommonKit.Module.TemplateURL.Repository;

namespace Eazly.CommonKit.Module.TemplateURL.Migrations
{
    [DbContext(typeof(TemplateURLContext))]
    [Migration("Eazly.CommonKit.Module.TemplateURL.01.00.00.00")]
    public class InitializeModule : MultiDatabaseMigration
    {
        public InitializeModule(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var entityBuilder = new TemplateURLEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Create();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var entityBuilder = new TemplateURLEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Drop();
        }
    }
}
