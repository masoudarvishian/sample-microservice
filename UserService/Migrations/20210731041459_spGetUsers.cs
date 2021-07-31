using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Migrations
{
    public partial class spGetUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedure = @"CREATE PROCEDURE spGetUsers
                                 AS
                                 BEGIN
	                                 select * from Users
                                 END";

            migrationBuilder.Sql(procedure);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedure = @"DROP PROCEDURE spGetUsers";

            migrationBuilder.Sql(procedure);
        }
    }
}
