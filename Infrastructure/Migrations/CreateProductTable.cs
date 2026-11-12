using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Migrations
{
    [Migration(20261111193500, "create product table")]
    public class CreateProductTable : Migration
    {
        public override void Up()
        {
            Create.Table("product_model")
                .WithColumn("id").AsGuid().PrimaryKey()
                .WithColumn("name").AsString(255).NotNullable()
                .WithColumn("price").AsDecimal(18, 2).NotNullable()
                .WithColumn("weight").AsDecimal(10, 3).NotNullable()
                .WithColumn("type").AsString(32).NotNullable()
                .WithColumn("created_at").AsDateTimeOffset().NotNullable()
                    .WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("warehouse_number").AsInt32().NotNullable();
        }

        public override void Down()
        {
            if (Schema.Table("product_model").Exists())
                Delete.Table("product_model");
        }
    }
}
