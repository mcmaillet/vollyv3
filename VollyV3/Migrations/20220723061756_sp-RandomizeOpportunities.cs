using Microsoft.EntityFrameworkCore.Migrations;

namespace VollyV3.Migrations
{
    public partial class spRandomizeOpportunities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"
CREATE PROCEDURE [dbo].[RandomizeOpportunities]
AS
BEGIN
    PRINT 'Randomizing opportunities'

    SET NOCOUNT ON

    DECLARE @tracking_table TABLE (id int)
    DECLARE @tracking_element int
    DECLARE @total_count int
    DECLARE @current_random int

    PRINT 'DELETE [OpportunitiesOrdering]'

    DELETE FROM [dbo].[OpportunitiesOrdering]

    SELECT @total_count = COUNT(*)
    FROM [dbo].[Opportunities] op
    INNER JOIN [dbo].[Organizations] og ON og.Id = op.OrganizationId
    WHERE IsArchived = 0
    AND og.Enabled = 1

    DECLARE open_cursor CURSOR FOR
    SELECT op.Id
    FROM [dbo].[Opportunities] op
    INNER JOIN [dbo].[Organizations] og ON og.Id = op.OrganizationId
    WHERE IsArchived = 0
    AND og.Enabled = 1

    OPEN open_cursor

    FETCH NEXT FROM open_cursor
    INTO @tracking_element

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SELECT @current_random = CEILING(RAND()*@total_count)
	    PRINT 'Initial @current_random' + CAST(@current_random AS VARCHAR)
	    WHILE @current_random IN (SELECT id FROM @tracking_table)
	    BEGIN
		    PRINT '@current_random' + CAST(@current_random AS VARCHAR)
		    SELECT @current_random = CEILING(RAND()*@total_count)
	    END

	    PRINT 'INSERT [OpportunitiesOrdering]'

	    INSERT INTO [dbo].[OpportunitiesOrdering] (OpportunityId,Ordering)
	    VALUES (@tracking_element,@current_random)

	    INSERT INTO @tracking_table (id)
	    VALUES (@current_random)

        FETCH NEXT FROM open_cursor
        INTO @tracking_element
    END
    CLOSE open_cursor
    DEALLOCATE open_cursor
END";

            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
