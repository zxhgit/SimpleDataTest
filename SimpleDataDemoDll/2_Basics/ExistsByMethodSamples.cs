namespace SimpleDataDemoDll
{
    internal class ExistsByMethodSamples
    {
        internal void RunAll()
        {
            ExampleRunner.RunQuery(
                "Run ExistsBy(). No parameters. Throws System.ArgumentException",
                db => db.Albums.ExistsBy());
            //No parameters specified.

            ExampleRunner.RunQuery(
                "Run ExistsByGenreId() - just the column name. Throws System.ArgumentException",
                db => db.Albums.ExistsByGenreId());
            //No parameters specified.

            ExampleRunner.RunQuery(
                "Run ExistsBy(1) - no column names. Throws System.ArgumentException",
                db => db.Albums.ExistsBy(1));
            //No columns specified.

            ExampleRunner.RunQuery(
                "Albums.ExistsByGenreId(null).",
                db => db.Albums.ExistsByGenreId(null));
            //select distinct 1 from [dbo].[Albums] where [dbo].[Albums].[GenreId] is null

            ExampleRunner.RunQuery(
                "Albums.ExistsBy(GenreId: null).",
                db => db.Albums.ExistsBy(GenreId: null));
            //No columns specified.

            ExampleRunner.RunQuery(
                "Albums.ExistsByGenreId(\"a\"). Malformed Simple Expression. Throws System.FormatException",
                db => db.Albums.ExistsByGenreId("a"));
            //转型失败

            ExampleRunner.RunQuery(
                "Albums.ExistsBy(GenreId:\"a\"). Malformed Simple Expression. Throws System.FormatException",
                db => db.Albums.ExistsBy(GenreId: "a"));
            //转型失败

            ExampleRunner.RunQuery(
                "Albums.ExistsByGenreId(1)",
                db => db.Albums.ExistsByGenreId(1));
            //select distinct 1 from [dbo].[Albums] where [dbo].[Albums].[GenreId] = @p1

            ExampleRunner.RunQuery(
                "Albums.ExistsBy(GenreId :1)",
                db => db.Albums.ExistsBy(GenreId: 1));
            //No columns specified.

            ExampleRunner.RunQuery(
                "Albums.ExistsByGenreIdAndArtistId(1, 120)",
                db => db.Albums.ExistsByGenreIdAndArtistId(1, 120));
            //select distinct 1 from [dbo].[Albums] where [dbo].[Albums].[GenreId] = @p1 and [dbo].[Albums].[ArtistId] = @p2

            ExampleRunner.RunQuery(
                "Albums.ExistsBy(GenreId: 1, ArtistId: 120)",
                db => db.Albums.ExistsBy(GenreId: 1, ArtistId: 120));
            //No columns specified.
        }
    }
}