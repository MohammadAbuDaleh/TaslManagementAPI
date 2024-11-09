using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaslManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE TaskManagementDB.GetTasksByStatus
                 @Status Int,
                 @UserId NVARCHAR(450) = NULL
             AS
             BEGIN
                 IF @UserId IS NULL
                     SELECT * FROM UserTasks WHERE Status = @Status;
                 ELSE
                     SELECT * FROM Tasks WHERE Status = @Status AND UserId = @UserId;
             END;");


            migrationBuilder.Sql(@"CREATE PROCEDURE TaskManagementDB.GetTasksDueToday
             AS
             BEGIN
                 SELECT * FROM UserTasks  WHERE CAST(DueDate AS DATE) = CAST(GETDATE() AS DATE)
                  ORDER BY  DueDate ASC;
             END;");


            migrationBuilder.Sql(@"CREATE PROCEDURE TaskManagementDB.GetUserTaskCounts
            AS
            BEGIN
                SELECT u.UserName, COUNT(*) AS TaskCount
                 FROM UserTasks t
                 JOIN AspNetUsers u ON t.UserId = u.Id
                 GROUP BY u.UserName
                 ORDER BY TaskCount DESC;
            END;");

            migrationBuilder.Sql(@"CREATE PROCEDURE TaskManagementDB.SearchUserTasks
    @Title NVARCHAR(100) = NULL,
    @Status INT = NULL,
    @Priority INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

SELECT 
    ut.Id,
    ut.Title,
    ut.Description,
    ut.Status,
    ut.Priority,
    ut.DueDate,
    ut.UserId,
    u.UserName
FROM 
    UserTasks ut
Inner JOIN 
    AspNetUsers u ON ut.UserId = u.Id
WHERE
    (@Title IS NULL OR ut.Title LIKE '%' + @Title + '%') AND
    (@Status IS NULL OR ut.Status = @Status) AND
    (@Priority IS NULL OR ut.Priority = @Priority);
END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS TaskManagementDB.GetTasksByStatus");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS TaskManagementDB.GetTasksDueToday");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS TaskManagementDB.GetUserTaskCounts");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS TaskManagementDB.GetUserTaskCounts");
        }
    }
}
