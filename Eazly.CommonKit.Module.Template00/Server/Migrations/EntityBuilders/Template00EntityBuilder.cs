using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Eazly.CommonKit.Module.Template00.Migrations.EntityBuilders
{
    public class Template00EntityBuilder : AuditableBaseEntityBuilder<Template00EntityBuilder>
    {
        private const string _entityTableName = "Eazly.CommonKitTemplate00";
        private readonly PrimaryKey<Template00EntityBuilder> _primaryKey = new("PK_Eazly.CommonKitTemplate00", x => x.Template00Id);
        private readonly ForeignKey<Template00EntityBuilder> _moduleForeignKey = new("FK_Eazly.CommonKitTemplate00_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public Template00EntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override Template00EntityBuilder BuildTable(ColumnsBuilder table)
        {
            Template00Id = AddAutoIncrementColumn(table,"Template00Id");
            ModuleId = AddIntegerColumn(table,"ModuleId");
            Name = AddMaxStringColumn(table,"Name");
            AddAuditableColumns(table);
            return this;
        }

        public OperationBuilder<AddColumnOperation> Template00Id { get; set; }
        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
        public OperationBuilder<AddColumnOperation> Name { get; set; }
    }
}
