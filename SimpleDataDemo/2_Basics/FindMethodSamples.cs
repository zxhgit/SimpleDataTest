namespace SimpleDataDemo
{
    internal class FindMethodSamples
    {
        internal void RunAll()
        {
            ExampleRunner.RunQuery(
                "Run Find with no arguments. Throws BadExpressionException",
                db => db.Albums.Find());

            ExampleRunner.RunQuery(
                "Run Find(db.Albums.GenreId) - just the column name. Throws BadExpressionException",
                db => db.Albums.Find(db.Albums.GenreId));

            ExampleRunner.RunQuery(
                "Run Find(1) - no column names. Throws BadExpressionException",
                db => db.Albums.Find(1));

            ExampleRunner.RunQuery(
                "Albums.Find(db.Albums.GenreId = null). Malformed Simple Expression. Throws BadExpressionException",
                db => db.Albums.Find(db.Albums.GenreId = null));

            ExampleRunner.RunQuery(
                "Albums.Find(db.Albums.GenreId == \"a\"). Malformed Simple Expression. Throws FormatException",
                db => db.Albums.Find(db.Albums.GenreId == "a"));

            ExampleRunner.RunQuery(
                "Albums.Find(db.Albums.GenreId != null).",
                db => db.Albums.Find(db.Albums.GenreId != null));

            ExampleRunner.RunQuery(
                "Albums.Find(db.Albums.GenreId == 1)",
                db => db.Albums.Find(db.Albums.GenreId == 1));

            ExampleRunner.RunQuery(
                "Albums.Find(db.Albums.GenreId > 3)",
                db => db.Albums.Find(db.Albums.GenreId > 3));

            ExampleRunner.RunQuery(
                "Albums.Find(db.Albums.GenreId == 1 && db.Albums.ArtistId == 120)",
                db => db.Albums.Find(db.Albums.GenreId == 1 && db.Albums.ArtistId == 120));

            ExampleRunner.RunQuery(
                "Albums.Find(db.Albums.GenreId == 1 & db.Albums.ArtistId == 120)",
                db => db.Albums.Find(db.Albums.GenreId == 1 & db.Albums.ArtistId == 120));

            ExampleRunner.RunQuery(
                "Albums.Find(db.Albums.GenreId == 2 || db.Albums.ArtistId == 160)",
                db => db.Albums.Find(db.Albums.GenreId == 2 || db.Albums.ArtistId == 160));

            ExampleRunner.RunQuery(
                "Albums.Find(db.Albums.GenreId == 2 | db.Albums.ArtistId == 160)",
                db => db.Albums.Find(db.Albums.GenreId == 2 | db.Albums.ArtistId == 160));

            ExampleRunner.RunQuery(
                "Albums.Find(db.Albums.GenreId == 1, db.Albums.ArtistId == 120). Throws BadExpressionException as only one SimpleExpression allowed in Find",
                db => db.Albums.Find(db.Albums.GenreId == 1, db.Albums.ArtistId == 120));
        }
    }
}