using System.Collections.Generic;

namespace SimpleDataDemoDll
{
    /// <summary>
    /// NaturalJoinSamples
    /// </summary>
    /// <remarks>
    /// Simple.Data allows you to join two or more tables implicitly, rather than use the Join, OuterJoin or With commands, provided the tables have a 
    /// foreign key constraint set up between them so referential integrity is enforced.
    /// 外键的引用完整性是强制的★
    /// 
    /// There are two points to note about Natural joins:
    /// 1. Simple.Data translates natural joins into LEFT JOIN statements.
    /// 2. If you cast the results of an natural join query into a POCO, Simple.Data will not ‘gather up’ any 1:n join data into collections for you to 
    ///    query as enumerables. You’ll need to use the With or WithOne statements to achieve this.
    /// 
    /// Exceptions
    /// UnresolvableObjectException: TableN doesn’t have a column called ‘Field’.
    /// [AdoAdapter only]Simple.Data.Ado.SchemaResolutionException: Table2 and Table3 do not have a foreign key relationship defined in the 
    /// database.★
    /// [AdoAdapter only]Simple.Data.Ado.AdoAdapterException: StartTable is not the same table being queried in the main selection command.
    /// </remarks>
    internal class NaturalJoinSamples
    {
        internal void RunAll()
        {
            ExampleRunner.RunQuery(
                "Implicit Inner Join between two tables: albums and genre",
                db => db.Albums.FindAllByGenreId(1)
                        .Select(
                            db.Albums.Title,
                            db.Albums.Genre.Name),//外键为GenreId，表为Genres，如何通过Genre与二者关联上
                new List<string> {"Title", "Name"});
            //left join Genres

            ExampleRunner.RunQuery(
                "Implicit Inner Join between three tables: orderDetails, albums and genre",
                db => db.OrderDetails.FindAllByOrderId(1)
                        .Select(
                            db.OrderDetails.OrderId,
                            db.OrderDetails.Albums.Title,
                            db.OrderDetails.Albums.Genre.Name),
                new List<string> {"Title", "Name"});
            //left join Albums,Genres

            ExampleRunner.RunQuery(
                "Implicit 1:n Join between two tables: genre and albums",
                db => db.Genre.FindAllByGenreId(1)
                        .Select(
                            db.Genre.Name,
                            db.Genre.Albums.Title),
                new List<string> {"Name", "Title"});
            //left join Albums

            ExampleRunner.RunQuery(
                "Implicit m:n Join between two tables: order and albums",
                db => db.Orders.FindAllByOrderId(4)
                        .Select(
                            db.Orders.OrderId,
                            db.Orders.OrderDetails.Albums.Title),
                new List<string> {"OrderId", "Title"});
            //left join OrderDetails,Albums
        }
    }
}