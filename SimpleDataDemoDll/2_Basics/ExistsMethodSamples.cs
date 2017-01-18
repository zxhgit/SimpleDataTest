using System.Collections.Generic;

namespace SimpleDataDemoDll
{
    internal class ExistsMethodSamples
    {
        internal void RunAll()
        {
            ExampleRunner.RunQuery(
                "Run db.Albums.Exists - returns true",
                db => db.Albums.Exists());
            //select distinct 1 from [dbo].[Albums] 

            ExampleRunner.RunQuery(
                "Run db.Albums.Exists.Exists - returns true",
                db => db.Albums.Exists().Exists());
            //RuntimeBinderException
            //“bool”未包含“Exists”的定义

            ExampleRunner.RunQuery(
                "Run db.AlbumCovers.Exists table exists. Throws UnresolvableObjectException",
                db => db.AlbumCovers.Exists());
            //没有表AlbumCovers

            ExampleRunner.RunQuery(
                 "Run Exists(db.Albums.GenreId) - just the column name. Throws BadExpressionException",
                 db => db.Albums.Exists(db.Albums.GenreId));
            //未抛异常
            //select distinct 1 from [dbo].[Albums] 

            ExampleRunner.RunQuery(
                "Run Exists(1) - no column names. Throws BadExpressionException",
                db => db.Albums.Exists(1));
            //同上

            ExampleRunner.RunQuery(
                "Run Exists(1) - no column names. Throws BadExpressionException",
                db => db.Albums.Exists(true));
            //同上

            ExampleRunner.RunQuery(
                 "Run Exists(two expressions). Throws BadExpressionException",
                 db => db.Albums.Exists(db.Albums.GenreId == 1, db.Albums.AlbumId == 1));
            //同上

            ExampleRunner.RunQuery(
                "Albums.Exists(db.Albums.GenreId == \"a\"). Malformed Simple Expression. Throws FormatException",
                db => db.Albums.Exists(db.Albums.GenreId == "a"));
            //转型失败

            ExampleRunner.RunQuery(
                "Albums.Exists(db.Albums.GenreId != null).",
                db => db.Albums.Exists(db.Albums.GenreId != null));
            //select distinct 1 from [dbo].[Albums] where [dbo].[Albums].[GenreId] is not null

            ExampleRunner.RunQuery(
                "Albums.Exists(db.Albums.GenreId == 1)",
                db => db.Albums.Exists(db.Albums.GenreId == 1));
            //select distinct 1 from [dbo].[Albums] where [dbo].[Albums].[GenreId] = @p1

            ExampleRunner.RunQuery(
                "Albums.Exists(db.Albums.GenreId > 3)",
                db => db.Albums.Exists(db.Albums.GenreId > 3));
            //select distinct 1 from [dbo].[Albums] where [dbo].[Albums].[GenreId] > @p1

            ExampleRunner.RunQuery(
                "Albums.Exists(db.Albums.GenreId == 1 && db.Albums.ArtistId == 120)",
                db => db.Albums.Exists(db.Albums.GenreId == 1 && db.Albums.ArtistId == 120));
            //select distinct 1 from [dbo].[Albums] where [dbo].[Albums].[GenreId] = @p1 and [dbo].[Albums].[ArtistId] = @p2

            ExampleRunner.RunQuery(
                "Albums.Exists(db.Albums.GenreId == 1 & db.Albums.ArtistId == 120)",
                db => db.Albums.Exists(db.Albums.GenreId == 1 & db.Albums.ArtistId == 120));
            //select distinct 1 from [dbo].[Albums] where [dbo].[Albums].[GenreId] = @p1 and [dbo].[Albums].[ArtistId] = @p2

            ExampleRunner.RunQuery(
                "Albums.Exists(db.Albums.GenreId == 2 || db.Albums.ArtistId == 160)",
                db => db.Albums.Exists(db.Albums.GenreId == 2 || db.Albums.ArtistId == 160));
            //select distinct 1 from [dbo].[Albums] where [dbo].[Albums].[GenreId] = @p1 or [dbo].[Albums].[ArtistId] = @p2

            ExampleRunner.RunQuery(
                "Albums.Exists(db.Albums.GenreId == 2 | db.Albums.ArtistId == 160)",
                db => db.Albums.Exists(db.Albums.GenreId == 2 | db.Albums.ArtistId == 160));
            //select distinct 1 from [dbo].[Albums] where [dbo].[Albums].[GenreId] = @p1 or [dbo].[Albums].[ArtistId] = @p2

        }
    }
}