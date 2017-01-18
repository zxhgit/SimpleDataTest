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
            //��bool��δ������Exists���Ķ���

            ExampleRunner.RunQuery(
                "Run db.AlbumCovers.Exists table exists. Throws UnresolvableObjectException",
                db => db.AlbumCovers.Exists());
            //û�б�AlbumCovers

            ExampleRunner.RunQuery(
                 "Run Exists(db.Albums.GenreId) - just the column name. Throws BadExpressionException",
                 db => db.Albums.Exists(db.Albums.GenreId));
            //δ���쳣
            //select distinct 1 from [dbo].[Albums] 

            ExampleRunner.RunQuery(
                "Run Exists(1) - no column names. Throws BadExpressionException",
                db => db.Albums.Exists(1));
            //ͬ��

            ExampleRunner.RunQuery(
                "Run Exists(1) - no column names. Throws BadExpressionException",
                db => db.Albums.Exists(true));
            //ͬ��

            ExampleRunner.RunQuery(
                 "Run Exists(two expressions). Throws BadExpressionException",
                 db => db.Albums.Exists(db.Albums.GenreId == 1, db.Albums.AlbumId == 1));
            //ͬ��

            ExampleRunner.RunQuery(
                "Albums.Exists(db.Albums.GenreId == \"a\"). Malformed Simple Expression. Throws FormatException",
                db => db.Albums.Exists(db.Albums.GenreId == "a"));
            //ת��ʧ��

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