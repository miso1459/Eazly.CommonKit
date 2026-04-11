using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Eazly.CommonKit.Module.TemplateURL.Migrations.EntityBuilders
{
    public class TemplateURLEntityBuilder : AuditableBaseEntityBuilder<TemplateURLEntityBuilder>
    {
        private const string _entityTableName = "Eazly.CommonKitTemplateURL";
        private readonly PrimaryKey<TemplateURLEntityBuilder> _primaryKey = new("PK_Eazly.CommonKitTemplateURL", x => x.TemplateURLId);
        private readonly ForeignKey<TemplateURLEntityBuilder> _moduleForeignKey = new("FK_Eazly.CommonKitTemplateURL_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public TemplateURLEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override TemplateURLEntityBuilder BuildTable(ColumnsBuilder table)
        {
            TemplateURLId = AddAutoIncrementColumn(table,"TemplateURLId");
            ModuleId = AddIntegerColumn(table,"ModuleId");
            Name = AddMaxStringColumn(table,"Name");
            AddAuditableColumns(table);
            return this;
        }

        public OperationBuilder<AddColumnOperation> TemplateURLId { get; set; }
        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
        public OperationBuilder<AddColumnOperation> Name { get; set; }
    }
}
