using System.Text;

namespace SimpleDataDemoDll
{
    internal class WhereMethodSamples
    {
        internal void RunAll()
        {
            ExampleRunner.RunQuery(
                "Where() called without query command. db.Albums.Where(db.Albums.GenreId == 1)",
                db => db.Albums.Where(db.Albums.GenreId == 1));
            //Simple.Data.BadExpressionException
            //Where methods refquire a single criteria expression.

            ExampleRunner.RunQuery(
                "Where called without no parameters. db.Albums.All().Where()",
                db => db.Albums.All().Where());
            //Simple.Data.BadExpressionException
            //Where methods refquire a single criteria expression.

            ExampleRunner.RunQuery(
                "Where called with invalid SimpleExpression as parameter. FIeld name wrong",
                db => db.Albums.All().Where(db.Albums.NonExistentColumn == 1));
            //Simple.Data.UnresolvableObjectException
            //Column not found

            ExampleRunner.RunQuery(
                "Where called with two valid SimpleExpressions as parameters.",
                db => db.Albums.All().Where(db.Albums.GenreId == 1, db.Albums.ArtistId == 120));
            //Simple.Data.BadExpressionException
            //Where methods refquire a single criteria expression.

            ExampleRunner.RunQuery(
                "db.Albums.All().Where(db.Albums.GenreId == 1). Where clause added",
                db => db.Albums.All().Where(db.Albums.GenreId == 1));
            //ok WHERE [dbo].[Albums].[GenreId]=@p1

            ExampleRunner.RunQuery(
                "db.Albums.All().Where(db.Albums.GenreId == 1).Where(db.Albums.ArtistId == 120). Where clauses added and concatenated with AND",
                db => db.Albums.All().Where(db.Albums.GenreId == 1).Where(db.Albums.ArtistId == 120));
            //ok WHERE [dbo].[Albums].[GenreId]=@p1 AND [dbo].[Albums].[ArtistId]=@p2

            ExampleRunner.RunQuery(
                "db.Albums.FindAllByGenreId(1).Where(db.Albums.ArtistId == 120). Where clause concatenated with AND",
                db => db.Albums.FindAllByGenreId(1).Where(db.Albums.ArtistId == 120));
            //ok WHERE [dbo].[Albums].[GenreId]=@p1 AND [dbo].[Albums].[ArtistId]=@p2

            ExampleRunner.RunQuery(
                "Concatenate two SimpleExpressions using OR",
                db => OrTwoExpressions(db));
            //ok WHERE [dbo].[Albums].[GenreId]=@p1 OR [dbo].[Albums].[ArtistId]=@p2

            ExampleRunner.RunQuery(
                "Concatenate two ORed SimpleExpressions with a third. Use Where to concatenate",
                db =>
                db.Albums.FindAll(db.Albums.GenreId == 1 || db.Albums.GenreId == 2).Where(db.Albums.ArtistId == 120));
            //ok WHERE (([dbo].[Albums].[GenreId]=@p1 OR [dbo].[Albums].[GenreId]=@p2) AND [dbo].[Albums].[ArtistId]=@p3)

            ExampleRunner.RunQuery(
                "Concatenate two ORed SimpleExpressions with a third. Build SimpleExpression first",
                db => OrAndThreeExpressions(db));
            //ok WHERE (([dbo].[Albums].[GenreId]=@p1 OR [dbo].[Albums].[GenreId]=@p2) AND [dbo].[Albums].[ArtistId]=@p3)

            ExampleRunner.RunQuery(
                "db.Albums.All().Where(db.Albums.Title == \"Dark Side of the Moon\"); String Comparison",
                db => db.Albums.All().Where(db.Albums.Title == "Dark Side Of The Moon"));
            //ok WHERE [dbo].[Albums].[Title]=@p1

            ExampleRunner.RunQuery(
                "Compare string field to production of StringBuilder. db.Albums.Title == sb.ToString()",
                db => CompareStringBuilderProduction(db));
            //ok WHERE [dbo].[Albums].[Title]=@p1
        }

        private dynamic CompareStringBuilderProduction(dynamic db)
        {
            var sb = new StringBuilder("Dark Side ");
            sb.Append("Of The Moon");
            return db.Albums.All().Where(db.Albums.Title == sb.ToString());
        }

        private dynamic OrTwoExpressions(dynamic db)
        {
            dynamic expr1 = db.Albums.GenreId == 1;
            dynamic expr2 = db.Albums.ArtistId == 120;
            return db.Albums.All().Where(expr1 || expr2);
        }

        private dynamic OrAndThreeExpressions(dynamic db)
        {
            dynamic expr1 = db.Albums.GenreId == 1;
            dynamic expr2 = db.Albums.GenreId == 1;
            dynamic expr3 = db.Albums.ArtistId == 120;
            return db.Albums.All().Where((expr1 || expr2) && expr3);
        }
    }
}