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
    }
}
