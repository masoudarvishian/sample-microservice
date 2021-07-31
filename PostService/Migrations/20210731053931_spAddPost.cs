using Microsoft.EntityFrameworkCore.Migrations;

namespace PostService.Migrations
{
    public partial class spAddPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedure = @"CREATE PROCEDURE spAddPost

                                @title nvarchar(100),
                                @content nvarchar(100),
                                @userid int

                                AS
                                BEGIN
                                  INSERT INTO Posts (Title, Content, UserId)
                                  VALUES (@title, @content, @userid);
                                END";

            migrationBuilder.Sql(procedure);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedure = @"DROP PROCEDURE spAddPost";

            migrationBuilder.Sql(procedure);
        }
    }
}
