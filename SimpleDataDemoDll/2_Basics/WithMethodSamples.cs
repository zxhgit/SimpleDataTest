namespace SimpleDataDemoDll
{
    internal class WithMethodSamples
    {
        internal void RunAll()
        {
            WithRunner.RunDemo(
                "Run With without a base command",
                db => db.Albums.WithArtists());
            /*
             select [dbo].[Albums].[AlbumId],
            [dbo].[Albums].[GenreId],
            [dbo].[Albums].[ArtistId],
            [dbo].[Albums].[Title],
            [dbo].[Albums].[Price],
            [dbo].[Albums].[AlbumArtUrl],
            [dbo].[Artists].[ArtistId] AS [__with1__Artists__ArtistId],
            [dbo].[Artists].[Name] AS [__with1__Artists__Name] 
            from [dbo].[Albums] LEFT JOIN [dbo].[Artists] 
            ON ([dbo].[Artists].[ArtistId] = [dbo].[Albums].[ArtistId])

            返回SimpleQuery 
             */

            WithRunner.RunDemo(
                "Run With returning a SimpleRecord - fluid style",
                db => db.Albums.WithArtists().Get(1));
            /*
             WITH __Data AS (
            SELECT [dbo].[Albums].[AlbumId], 
            ROW_NUMBER() OVER(ORDER BY [dbo].[Albums].[AlbumId]) AS [_#_]
            from [dbo].[Albums] LEFT JOIN [dbo].[Artists] 
            ON ([dbo].[Artists].[ArtistId] = [dbo].[Albums].[ArtistId]) 
            WHERE [dbo].[Albums].[AlbumId] = 1
            )
            SELECT [dbo].[Albums].[AlbumId],
            [dbo].[Albums].[GenreId],
            [dbo].[Albums].[ArtistId],
            [dbo].[Albums].[Title],
            [dbo].[Albums].[Price],
            [dbo].[Albums].[AlbumArtUrl],
            [dbo].[Artists].[ArtistId] AS [__with1__Artists__ArtistId],
            [dbo].[Artists].[Name] AS [__with1__Artists__Name] 
            FROM __Data JOIN [dbo].[Albums] 
            ON [dbo].[Albums].[AlbumId] = __Data.[AlbumId] 
            LEFT JOIN [dbo].[Artists] 
            ON ([dbo].[Artists].[ArtistId] = [dbo].[Albums].[ArtistId]) 
            WHERE [dbo].[Albums].[AlbumId] = 1 AND [_#_] BETWEEN 1 AND 1

            返回SimpleRecord
             */

            WithRunner.RunDemo(
                "Run With returning a SimpleRecord - named style",
                db => db.Albums.With(db.Albums.Artists).Get(1));
            //同上

            WithRunner.RunDemo(
                "Run With returning a SimpleQuery - fluid style",
                db => db.Albums.All().WithArtists().FirstOrDefault());
            /*
             WITH __Data AS (
            SELECT [dbo].[Albums].[AlbumId], 
            ROW_NUMBER() OVER(ORDER BY [dbo].[Albums].[AlbumId]) AS [_#_]
            from [dbo].[Albums] LEFT JOIN [dbo].[Artists] 
            ON ([dbo].[Artists].[ArtistId] = [dbo].[Albums].[ArtistId])
            )
            SELECT [dbo].[Albums].[AlbumId],
            [dbo].[Albums].[GenreId],
            [dbo].[Albums].[ArtistId],
            [dbo].[Albums].[Title],
            [dbo].[Albums].[Price],
            [dbo].[Albums].[AlbumArtUrl],
            [dbo].[Artists].[ArtistId] AS [__with1__Artists__ArtistId],
            [dbo].[Artists].[Name] AS [__with1__Artists__Name] 
            FROM __Data JOIN [dbo].[Albums] 
            ON [dbo].[Albums].[AlbumId] = __Data.[AlbumId] 
            LEFT JOIN [dbo].[Artists] 
            ON ([dbo].[Artists].[ArtistId] = [dbo].[Albums].[ArtistId]) AND [_#_] BETWEEN 1 AND 1
             */

            WithRunner.RunDemo(
                "Run With returning a SimpleQuery - named style",
                db => db.Albums.All().With(db.Albums.Artists).FirstOrDefault());
            //同上

            WithRunner.RunDemo(
                "Run With but no arguments",
                db => db.Albums.All().With());

            WithRunner.RunDemo(
                "Run With with args in fluid and named style",
                db => db.Albums.All().WithArtists(db.Albums.Genre).FirstOrDefault());
            /*
             WITH __Data AS (
            SELECT [dbo].[Albums].[AlbumId], 
            ROW_NUMBER() OVER(ORDER BY [dbo].[Albums].[AlbumId]) AS [_#_]
            from [dbo].[Albums] LEFT JOIN [dbo].[Artists] 
            ON ([dbo].[Artists].[ArtistId] = [dbo].[Albums].[ArtistId])
            )
            SELECT [dbo].[Albums].[AlbumId],
            [dbo].[Albums].[GenreId],
            [dbo].[Albums].[ArtistId],
            [dbo].[Albums].[Title],
            [dbo].[Albums].[Price],
            [dbo].[Albums].[AlbumArtUrl],
            [dbo].[Artists].[ArtistId] AS [__with1__Artists__ArtistId],
            [dbo].[Artists].[Name] AS [__with1__Artists__Name] 
            FROM __Data JOIN [dbo].[Albums] 
            ON [dbo].[Albums].[AlbumId] = __Data.[AlbumId] 
            LEFT JOIN [dbo].[Artists] 
            ON ([dbo].[Artists].[ArtistId] = [dbo].[Albums].[ArtistId]) AND [_#_] BETWEEN 1 AND 1
             */

            WithRunner.RunDemo(
                "Run With against a table with no FK relationship to primary",
                db => db.Albums.All().WithOrders().FirstOrDefault());//★

            WithRunner.RunDemo(
                "Run With against three child tables",
                db => db.Albums.All().WithArtists().WithGenre().FirstOrDefault());

            WithRunner.RunDemo(
                "Run With going two tables deep",
                db => db.OrderDetails.All().With(db.OrderDetails.Albums).With(db.OrderDetails.Albums.Artists),
                "OrderDetailList");
            /*
             select [dbo].[OrderDetails].[OrderDetailId],
            [dbo].[OrderDetails].[OrderId],
            [dbo].[OrderDetails].[AlbumId],
            [dbo].[OrderDetails].[Quantity],
            [dbo].[OrderDetails].[UnitPrice],
            [dbo].[Albums].[AlbumId] AS [__with1__Albums__AlbumId],
            [dbo].[Albums].[GenreId] AS [__with1__Albums__GenreId],
            [dbo].[Albums].[ArtistId] AS [__with1__Albums__ArtistId],
            [dbo].[Albums].[Title] AS [__with1__Albums__Title],
            [dbo].[Albums].[Price] AS [__with1__Albums__Price],
            [dbo].[Albums].[AlbumArtUrl] AS [__with1__Albums__AlbumArtUrl],
            [dbo].[Artists].[ArtistId] AS [__with1__Artists__ArtistId],
            [dbo].[Artists].[Name] AS [__with1__Artists__Name] 
            from [dbo].[OrderDetails] LEFT JOIN [dbo].[Albums] 
            ON ([dbo].[Albums].[AlbumId] = [dbo].[OrderDetails].[AlbumId]) 
            LEFT JOIN [dbo].[Artists] 
            ON ([dbo].[Artists].[ArtistId] = [dbo].[Albums].[ArtistId])
             */
        }
    }
}