namespace SimpleDataDemoDll
{
    internal class GetCountByMethodSamples
    {
        internal void RunAll()
        {
            ExampleRunner.RunQuery(
                "Run GetCountBy(). No parameters. Throws System.ArgumentException",
                db => db.Albums.GetCountBy());
            //System.ArgumentException
            //GetCountByrequires arguments.

            ExampleRunner.RunQuery(
                "Run GetCountByGenreId() - just the column name. Throws System.ArgumentException",
                db => db.Albums.GetCountByGenreId());
            //System.ArgumentException
            //No parameters specified.

            ExampleRunner.RunQuery(
                "Run GetCountBy(1) - no column names. Throws System.ArgumentException",
                db => db.Albums.GetCountBy(1));
            //System.ArgumentException
            //列表初始值设定项必须至少包含一个初始值设定项

            // Runs select COUNT(*) from [dbo].[Albums] WHERE [dbo].[Albums].[GenreId] IS NULL
            ExampleRunner.RunQuery(
                "Albums.GetCountByGenreId(null).",
                db => db.Albums.GetCountByGenreId(null));

            // Throws System.ArgumentException.  No columns specified. Expeceted to run
            ExampleRunner.RunQuery(
                "Albums.GetCountBy(GenreId: null).",
                db => db.Albums.GetCountBy(GenreId: null));

            // Throws System.FormatException. Expected BadExpressionException
            ExampleRunner.RunQuery(
                "Albums.GetCountByGenreId(\"a\"). Malformed Simple Expression. Throws System.FormatException",
                db => db.Albums.GetCountByGenreId("a"));
            //System.FormatException
            //将参数值从string转换到int32失败。

            // Throws System.FormatException. Expected BadExpressionException
            ExampleRunner.RunQuery(
                "Albums.GetCountBy(GenreId:\"a\"). Malformed Simple Expression. Throws System.FormatException",
                db => db.Albums.GetCountBy(GenreId: "a"));
            //System.FormatException
            //将参数值从string转换到int32失败。

            // select COUNT(*) from [dbo].[Albums] WHERE [dbo].[Albums].[GenreId] = @p1
            // @p1 (Int32) = 1
            ExampleRunner.RunQuery(
                "Albums.GetCountByGenreId(1)",
                db => db.Albums.GetCountByGenreId(1));

            // Throws System.ArgumentException.  No columns specified. Expeceted to run
            //未抛异常，结果同上
            ExampleRunner.RunQuery(
                "Albums.GetCountBy(GenreId :1)",
                db => db.Albums.GetCountBy(GenreId: 1));

            // select COUNT(*) from [dbo].[Albums] WHERE ([dbo].[Albums].[GenreId] = @p1 AND [dbo].[Albums].[ArtistId] = @p2)
            //@p1 (Int32) = 1
            //@p2 (Int32) = 120
            ExampleRunner.RunQuery(
                "Albums.GetCountByGenreIdAndArtistId(1, 120)",
                db => db.Albums.GetCountByGenreIdAndArtistId(1, 120));

            // select COUNT(*) from [dbo].[Albums] WHERE ([dbo].[Albums].[GenreId] = @p1 AND [dbo].[Albums].[ArtistId] = @p2)
            //@p1 (Int32) = 1
            //@p2 (Int32) = 120
            ExampleRunner.RunQuery(
                "Albums.GetCountBy(GenreId: 1, ArtistId: 120)",
                db => db.Albums.GetCountBy(GenreId: 1, ArtistId: 120));
        }
    }
}