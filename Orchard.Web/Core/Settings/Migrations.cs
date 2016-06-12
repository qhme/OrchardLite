using Orchard.Data.Migration;

namespace Orchard.Core.Settings
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable("ShellDescriptorRecord",
               table => table
                   .Column<int>("Id", column => column.PrimaryKey().Identity())
                   .Column<int>("SerialNumber")
               );

            SchemaBuilder.CreateTable("ShellFeatureRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name")
                    .Column<int>("ShellDescriptorRecord_id"));


            SchemaBuilder.CreateTable("ShellParameterRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Component")
                    .Column<string>("Name")
                    .Column<string>("Value")
                    .Column<int>("ShellDescriptorRecord_id")
                );

            SchemaBuilder.CreateTable("SettingRecord",
             table => table
                    .ContentPartRecord()
                    .Column<string>("Name", col => col.WithLength(200))
                    .Column<string>("Value", col => col.WithLength(2000)));

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.CreateTable("ShellFeatureStateRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name")
                    .Column<string>("InstallState")
                    .Column<string>("EnableState")
                    .Column<int>("ShellStateRecord_Id")
                );

            SchemaBuilder.CreateTable("ShellStateRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Unused")
                );
            return 2;
        }

        public int UpdateFrom2()
        {
            //SchemaBuilder.CreateTable("ContentPartDefinitionRecord",
            // table => table
            //     .Column<int>("Id", column => column.PrimaryKey().Identity())
            //     .Column<string>("Name")
            //     .Column<bool>("Hidden")
            //     .Column<string>("Settings", column => column.Unlimited())
            // );

            SchemaBuilder.CreateTable("ContentTypeDefinitionRecord",
               table => table
                   .Column<int>("Id", column => column.PrimaryKey().Identity())
                   .Column<string>("Name")
                   .Column<string>("DisplayName")
                   .Column<bool>("Hidden")
                   .Column<string>("Settings", column => column.Unlimited())
              );

            SchemaBuilder.CreateTable("ContentTypePartDefinitionRecord",
              table => table
                  .Column<int>("Id", column => column.PrimaryKey().Identity())
                  .Column<string>("Settings", column => column.Unlimited())
                  .Column<int>("ContentPartDefinitionRecord_id")
                  .Column<int>("ContentTypeDefinitionRecord_Id")
              );
            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.DropTable("ContentPartDefinitionRecord");
            SchemaBuilder.AlterTable("ContentTypePartDefinitionRecord", table =>
            {
                table.DropColumn("ContentPartDefinitionRecord_id");
                table.AddColumn<string>("PartName");
            });
            return 4;
        }
    }
}
