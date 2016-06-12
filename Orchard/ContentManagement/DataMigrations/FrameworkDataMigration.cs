using Orchard.Data.Migration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.DataMigrations
{
    public class FrameworkDataMigration : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable("ContentItemRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Data", c => c.Unlimited())
                    //.Column<int>("ContentType_id")
                );


            //SchemaBuilder.CreateTable("ContentTypeRecord",
            //    table => table
            //        .Column<int>("Id", column => column.PrimaryKey().Identity())
            //        .Column<string>("Name")
            //    );

            SchemaBuilder.CreateTable("CultureRecord",
                        table => table
                            .Column<int>("Id", column => column.PrimaryKey().Identity())
                            .Column<string>("Culture")
                        );
            return 1;
        }

        public int UpdateFrom1()
        {
            //SchemaBuilder.AlterTable("ContentItemRecord",
            //   table => table
            //       .CreateIndex("IDX_ContentType_id", "ContentType_id")
            //   );

            //SchemaBuilder.AlterTable("ContentTypeRecord",
            //     table => table
            //    .CreateIndex("IDX_ContentType_Name", "Name")
            //);

            return 2;
        }
    }
}
