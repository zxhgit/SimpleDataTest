using System.Collections.Generic;

namespace SimpleDataDemoDll
{
    internal class OuterJoinSamples
    {
        internal void RunAll()
        {
            #region Valid calls

            ExampleRunner.RunQuery(
                "Explicit OuterJoin between two tables using OuterJoin only (before Select method)",
                db => db.Albums.FindAllByGenreId(1)
                        .OuterJoin(db.Genre, GenreId: db.Albums.GenreId)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name),
                new List<string> {"Title", "Name"});
            //[dbo].[Albums] LEFT JOIN [dbo].[Genres] ON ([dbo].[Genres].[GenresId]=[dbo].[Albums].[GenresId])

            ExampleRunner.RunQuery(
                "Explicit OuterJoin between two tables using OuterJoin only (after Select method)",
                db => db.Albums.FindAllByGenreId(1)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name)
                        .OuterJoin(db.Genre, GenreId: db.Albums.GenreId),
                new List<string> {"Title", "Name"});
            //[dbo].[Albums] LEFT JOIN [dbo].[Genres] ON ([dbo].[Genres].[GenresId]=[dbo].[Albums].[GenresId])

            ExampleRunner.RunQuery(
                "Explicit OuterJoin between two tables using OuterJoin and On",
                db => db.Albums.FindAllByGenreId(1)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name)
                        .OuterJoin(db.Genre).On(db.Genre.GenreId == db.Albums.GenreId),
                new List<string> {"Title", "Name"});
            //[dbo].[Albums] LEFT JOIN [dbo].[Genres] ON ([dbo].[Genres].[GenresId]=[dbo].[Albums].[GenresId])

            ExampleRunner.RunQuery(
                "Explicit OuterJoin between two tables using OuterJoin only (indexer style)",
                db => db.Albums.FindAllByGenreId(1)
                        .Select(
                            db["Albums"]["Title"],
                            db["Genre"]["Name"])
                        .OuterJoin(db["Genre"], GenreId: db["Albums"]["GenreId"]),
                new List<string> {"Title", "Name"});

            ExampleRunner.RunQuery(
                "Explicit OuterJoin between two tables using OuterJoin only and aliases using expression in the On clause",
                db => TwoTableOuterJoinWithAliasUsingExpressions(db),
                new List<string> {"Title", "Name"});
            //[dbo].[Albums] LEFT JOIN [dbo].[Genres] [g] ON ([g].[GenresId]=[dbo].[Albums].[GenresId])

            ExampleRunner.RunQuery(
                "Explicit OuterJoin between two tables using OuterJoin only and aliases using named parameters in the On clause",
                db => TwoTableOuterJoinWithAliasUsingNamedParameters(db),
                new List<string> {"Title", "Name"});
            //[dbo].[Albums] LEFT JOIN [dbo].[Genres] [g] ON ([g].[GenresId]=[dbo].[Albums].[GenresId])

            ExampleRunner.RunQuery(
                "Explicit OuterJoin between three tables: orderDetails, albums and genre",
                db => db.OrderDetails.FindAllByOrderId(1)
                        .Select(
                            db.OrderDetails.OrderId,
                            db.OrderDetails.Albums.Title,
                            db.OrderDetails.Albums.Genre.Name)
                        .OuterJoin(db.Albums, AlbumId: db.OrderDetails.AlbumId)
                        .OuterJoin(db.Genre, GenreId: db.Albums.GenreId),
                new List<string> {"Title", "Name"});

            ExampleRunner.RunQuery(
                "Explicit OuterJoin between three tables using OuterJoin only and aliases using expression in the On clause",
                db => ThreeTableOuterJoinWithAliasUsingExpressions(db),
                new List<string> {"OrderId", "Title", "Name"});

            #endregion

            #region Invalid Calls

            //Throws NullReferenceException
            ExampleRunner.RunQuery(
                "Explicit OuterJoin between two tables using OuterJoin only (trying to OuterJoin the wrong table)",
                db => db.Albums.FindAllByGenreId(1)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name)
                        .OuterJoin(db.Albums, GenreId: db.Genre.GenreId),//db.Albums与db.Genre.Ge..不匹配
                new List<string> {"Title", "Name"});

            ExampleRunner.RunQuery(
                "Explicit OuterJoin between two tables using OuterJoin only (target table key column name doesn't exist)",
                db => db.Albums.FindAllByGenreId(1)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name)
                        .OuterJoin(db.Genre, NotPresent: db.Albums.GenreId),//NotPresent列在表Genres中不存在；target table指的是OuterJoin(db.Genre,..
                new List<string> {"Title", "Name"});

            ExampleRunner.RunQuery(
                "Explicit OuterJoin between two tables using OuterJoin only (start table key column name doesn't exist)",
                db => db.Albums.FindAllByGenreId(1)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name)
                        .OuterJoin(db.Genre, GenreId: db.Albums.NotPresent),//start table 指的是OuterJoin(......db.Albums.N...)左表
                new List<string> {"Title", "Name"});

            ExampleRunner.RunQuery(
                "Explicit OuterJoin between two tables using OuterJoin only (key column types don't match)",
                db => db.Albums.FindAllByGenreId(1)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name)
                        .OuterJoin(db.Genre, GenreId: db.Albums.Title),//GenreId与Title类型不匹配
                new List<string> {"Title", "Name"});

            // Throws System.InvalidOperationException
            ExampleRunner.RunQuery(
                "Explicit OuterJoin between two tables using OuterJoin and On. On not immediately after OuterJoin",
                db => db.Albums.FindAllByGenreId(1)
                        .OuterJoin(db.Genre)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name)
                        .On(db.Genre.GenreId == db.Albums.GenreId),
                new List<string> {"Title", "Name"});
            //Call to On must be preceded by call to JoinInfo

            // Throws IndexOutOfRangeException
            ExampleRunner.RunQuery(
                "Run OuterJoin with no arguments",
                db => db.Albums.FindAllByGenreId(1)
                        .OuterJoin()
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name),
                new List<string> {"Title", "Name"});

            // Throws Simple.Data.Ado.AdapterException
            // The multi-part identifier "dbo.Genres.Name" could not be bound.
            ExampleRunner.RunQuery(
                "Run OuterJoin with only targetTable named",//OuterJoin方法只有目标表名称
                db => db.Albums.FindAllByGenreId(1)
                        .OuterJoin(db.Genre)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name),
                new List<string> {"Title", "Name"});

            // Throws Simple.Data.Ado.AdapterException
            // The multi-part identifier "dbo.Genres.Name" could not be bound.
            ExampleRunner.RunQuery(
                "Run OuterJoin with only targetTable and column named wrongly",
                db => db.Albums.FindAllByGenreId(1)
                        .OuterJoin(db.Genre.GenreId)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name),
                new List<string> {"Title", "Name"});

            // Throws Simple.Data.Ado.AdapterException
            // The multi-part identifier "dbo.Genres.Name" could not be bound.
            ExampleRunner.RunQuery(
                "Run OuterJoin with only startTable and column named",
                db => db.Albums.FindAllByGenreId(1)
                        .OuterJoin(db.Albums.GenreId)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name),
                new List<string> {"Title", "Name"});

            //System.ArgumentOutOfRangeException
            //Index was out of range. Must be non-negative and less than the size of the collection.
            //Parameter name: index
            ExampleRunner.RunQuery(
                "Run OuterJoin with no arguments for OuterJoin or On",
                db => db.Albums.FindAllByGenreId(1)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name)
                        .OuterJoin(db.Genre)
                        .On(),
                new List<string> {"Title", "Name"});

            //System.ArgumentOutOfRangeException
            //Index was out of range. Must be non-negative and less than the size of the collection.
            //Parameter name: index
            ExampleRunner.RunQuery(
                "Run OuterJoin with no arguments for On",
                db => db.Albums.FindAllByGenreId(1)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name)
                        .OuterJoin(db.Genre)
                        .On(),
                new List<string> {"Title", "Name"});

            //System.ArgumentOutOfRangeException
            //Index was out of range. Must be non-negative and less than the size of the collection.
            //Parameter name: index
            ExampleRunner.RunQuery(
                "Run OuterJoin with just column name as argument for On",
                db => db.Albums.FindAllByGenreId(1)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name)
                        .OuterJoin(db.Genre)
                        .On(db.Albums.GenreId),
                new List<string> {"Title", "Name"});

            //System.ArgumentOutOfRangeException
            //Index was out of range. Must be non-negative and less than the size of the collection.
            //Parameter name: index
            ExampleRunner.RunQuery(
                "Run OuterJoin with just literal as argument for On",
                db => db.Albums.FindAllByGenreId(1)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name)
                        .OuterJoin(db.Genre)
                        .On(true),
                new List<string> {"Title", "Name"});


            ExampleRunner.RunQuery(
                "Run OuterJoin with simpleexpression for On but not between two columns",
                db => db.Albums.FindAllByGenreId(1)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name)
                        .OuterJoin(db.Genre)
                        .On(db.Albums.GenreId == null),
                new List<string> {"Title", "Name"});
            //[dbo].[Albums] LEFT JOIN [dbo].[Genres] ON ([dbo].[Albums].[GenresId] IS NULL)
            //未抛异常 

            //Microsoft.CSharp.RuntimeBinder.RuntimeBinderException
            //'Simple.Data.DynamicTable' does not contain a definition for 'GenreId'
            ExampleRunner.RunQuery(
                "Run OuterJoin with malformed simpleexpression for On",
                db => db.Albums.FindAllByGenreId(1)
                        .Select(
                            db.Albums.Title,
                            db.Genre.Name)
                        .OuterJoin(db.Genre)
                        .On(db.Albums.GenreId = db.Genre.GenreId),//=应该为==
                new List<string> {"Title", "Name"});

            #endregion
        }

        private static dynamic TwoTableOuterJoinWithAliasUsingExpressions(dynamic db)
        {
            dynamic genreAlias;
            return db.Albums.FindAllByGenreId(1)
                     .OuterJoin(db.Genre.As("g"), out genreAlias).On(genreAlias.GenreId == db.Albums.GenreId)
                     .Select(
                         db.Albums.Title,
                         genreAlias.Name);
        }

        private static dynamic TwoTableOuterJoinWithAliasUsingNamedParameters(dynamic db)
        {
            dynamic genreAlias;
            return db.Albums.FindAllByGenreId(1)
                     .OuterJoin(db.Genre.As("g"), out genreAlias).On(GenreId: db.Albums.GenreId)
                     .Select(
                         db.Albums.Title,
                         genreAlias.Name);
        }

        private static dynamic ThreeTableOuterJoinWithAliasUsingExpressions(dynamic db)
        {
            dynamic genreAlias;
            dynamic albumsAlias;
            return db.OrderDetails.FindAllByOrderId(1)
                     .OuterJoin(db.Albums.As("a"), out albumsAlias).On(albumsAlias.AlbumId == db.OrderDetails.AlbumId)
                     .OuterJoin(db.Genre.As("g"), out genreAlias).On(genreAlias.GenreId == albumsAlias.GenreId)
                     .Select(
                         db.OrderDetails.OrderId,
                         albumsAlias.Title,
                         genreAlias.Name);
        }
    }
}