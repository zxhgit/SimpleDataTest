using System;
using System.Data.SqlTypes;
using Simple.Data;

namespace SimpleDataDemoDll
{
    internal class WhereConditionSamples
    {
        internal void RunAll()
        {
            ExampleRunner.RunQuery(
                  "Comparison Operators. 1.Equals : Where(db.Albums.ArtistId == 120)",
                  db => db.Albums.All().Where(db.Albums.ArtistId == 120));
            //ok WHERE [dbo].[Albums].[ArtistId]=@p1

            ExampleRunner.RunQuery(
                  "Comparison Operators. 2.Not Equals : Where(db.Albums.GenreId != 1)",
                  db => db.Albums.All().Where(db.Albums.GenreId != 1));
            //ok WHERE [dbo].[Albums].[GenreId]!=@p1

            ExampleRunner.RunQuery(
                  "Comparison Operators. 3.Less Than : Where(db.Albums.Price < 8.99)",
                  db => db.Albums.All().Where(db.Albums.Price < 8.99));
            //ok WHERE [dbo].[Albums].[Price]<@p1

            ExampleRunner.RunQuery(
                  "Comparison Operators. 4.Less Than Or Equals : Where(db.Albums.Price <= 8.99)",
                  db => db.Albums.All().Where(db.Albums.Price <= 8.99));
            //ok WHERE [dbo].[Albums].[Price]<=@p1

            ExampleRunner.RunQuery(
                  "Comparison Operators. 5.Greater Than : Where(db.Albums.Price > 7.99)",
                  db => db.Albums.All().Where(db.Albums.Price > 7.99));
            //ok WHERE [dbo].[Albums].[Price]>@p1

            ExampleRunner.RunQuery(
                  "Comparison Operators. 6.Greaer Than Or Equals : Where(db.Albums.Price >= 7.99)",
                  db => db.Albums.All().Where(db.Albums.Price >= 7.99));
            //ok WHERE [dbo].[Albums].[Price]>=@p1

            ExampleRunner.RunQuery(
                "Math Operators. 1.Add : Where(db.Albums.AlbumId + db.Albums.ArtistId > 120)",
                db => db.Albums.All().Where(db.Albums.AlbumId + db.Albums.ArtistId > 120));
            //ok WHERE ([dbo].[Albums].[AlbumId]+[dbo].[Albums].[ArtistId])>@p1

            ExampleRunner.RunQuery(
                "Math Operators. 2.Subtract : Where(db.Albums.AlbumId - db.Albums.ArtistId < 130)",
                db => db.Albums.All().Where(db.Albums.AlbumId - db.Albums.ArtistId < 130));
            //ok WHERE ([dbo].[Albums].[AlbumId]-[dbo].[Albums].[ArtistId])<@p1

            ExampleRunner.RunQuery(
              "Math Operators. 3. Multiply : Where(db.OrderDetails.Quantity * db.OrderDetails.UnitPrice >= 50)",
              db => db.Albums.All().Where(db.OrderDetails.Quantity * db.OrderDetails.UnitPrice >= 50));
            //Simple.Data.Ado.AdoAdapterException
            //无法绑定由多个部分组成的标识符"dbo.OrderDetails.Quantity"
            //无法绑定由多个部分组成的标识符"dbo.OrderDetails.UnitPrice"

            ExampleRunner.RunQuery(
              "Math Operators. 4.Divide : Where(db.OrderDetails.UnitPrice / db.OrderDetails.Quantity <= 3)",
              db => db.Albums.All().Where(db.OrderDetails.UnitPrice / db.OrderDetails.Quantity <= 3));
            //Simple.Data.Ado.AdoAdapterException
            //无法绑定由多个部分组成的标识符"dbo.OrderDetails.Quantity"
            //无法绑定由多个部分组成的标识符"dbo.OrderDetails.UnitPrice"

            ExampleRunner.RunQuery(
              "Math Operators. 5.Modulo : Where(db.OrderDetails.UnitPrice % db.OrderDetails.Quantity != 4)",
              db => db.Albums.All().Where(db.OrderDetails.UnitPrice % db.OrderDetails.Quantity != 4));
            //Simple.Data.Ado.AdoAdapterException
            //无法绑定由多个部分组成的标识符"dbo.OrderDetails.UnitPrice"
            //无法绑定由多个部分组成的标识符"dbo.OrderDetails.Quantity"       

            ExampleRunner.RunQuery(
              "Using LIKE : Where(db.Albums.Title.Like(\"%Side Of The%\")",
              db => db.Albums.All().Where(db.Albums.Title.Like("%Side Of The%")));
            //ok WHERE [dbo].[Albums].[Title] LIKE @p1

            ExampleRunner.RunQuery(
                "Using NOT LIKE : Where(db.Albums.Title.NotLike(\"%a%\")",
                db => db.Albums.All().Where(db.Albums.Title.NotLike("%a%")));
            //ok WHERE [dbo].[Albums].[Title] NOT LIKE @p1

            ExampleRunner.RunQuery(
              "Using IN. 1.Embedded : db.Albums.FindAllByTitle(new[] {\"Nevermind\", \"Ten\"})",
              db => db.Albums.FindAllByTitle(new[] { "Nevermind", "Ten" }));
            //ok WHERE [dbo].[Albums].[Title] IN (@p1_0,@p1_1)

            ExampleRunner.RunQuery(
              "Using IN. 2.In FindAll : db.Albums.FindAll(db.Albums.Title == new[] {\"Nevermind\", \"Ten\"})",
              db => db.Albums.FindAll(db.Albums.Title == new[] { "Nevermind", "Ten" }));
            //ok WHERE [dbo].[Albums].[Title] IN (@p1_0,@p1_1)

            ExampleRunner.RunQuery(
              "Using IN. 3.In Where method : db.Albums.All().Where(db.Albums.Title == new[] {\"Nevermind\", \"Ten\"})",
              db => db.Albums.All().Where(db.Albums.Title == new[] { "Nevermind", "Ten" }));
            //ok WHERE [dbo].[Albums].[Title] IN (@p1_0,@p1_1)

            ExampleRunner.RunQuery(
              "Using NOT IN. 1.In FindAll : db.Albums.FindAll(db.Albums.GenreId != new[] {1,3,7})",
              db => db.Albums.FindAll(db.Albums.GenreId != new[] { 1, 3, 7 }));
            //ok WHERE [dbo].[Albums].[GenreId] NOT IN (@p1_0,@p1_1,@p1_2)

            ExampleRunner.RunQuery(
              "Using NOT IN. 2.In Where method : db.Albums.All().Where(db.Albums.GenreId != new[] {1,3,7})",
              db => db.Albums.All().Where(db.Albums.GenreId != new[] { 1, 3, 7 }));
            //ok WHERE [dbo].[Albums].[GenreId] NOT IN (@p1_0,@p1_1,@p1_2)

            ExampleRunner.RunQuery(
                "Using BETWEEN. 1. Embedded : db.Albums.FindAllByAlbumId(400.to(410))",
                db => db.Albums.FindAllByAlbumId(400.to(410)));
            //ok WHERE [dbo].[Albums].[AlbumId] BETWEEN @p1_start AND @p1_end

            ExampleRunner.RunQuery(
              "Using BETWEEN. 2. In FIndAll : db.Albums.FindAll(db.Albums.AlbumId == 400.to(410))",
              db => db.Albums.FindAll(db.Albums.AlbumId == 400.to(410)));
            //ok WHERE [dbo].[Albums].[AlbumId] BETWEEN @p1_start AND @p1_end

            ExampleRunner.RunQuery(
              "Using BETWEEN. 3. In Where : db.Albums.All().Where(db.Albums.AlbumId == 400.to(410))",
              db => db.Albums.All().Where(db.Albums.AlbumId == 400.to(410)));
            //ok WHERE [dbo].[Albums].[AlbumId] BETWEEN @p1_start AND @p1_end

            ExampleRunner.RunQuery(
              "Using NOT BETWEEN. 1. In FindAll : db.Orders.FindAll(db.Orders.OrderDate != DateTime.MinValue.to(DateTime.Now))",
              db => db.Orders.FindAll(db.Orders.OrderDate != SqlDateTime.MinValue.Value.to(DateTime.Now)));
            //ok WHERE [dbo].[Orders].[OrderDate] NOT BETWEEN @p1_start AND @p1_end 

            ExampleRunner.RunQuery(
              "Using NOT BETWEEN. 2. In Where : db.Orders.All().Where(db.Orders.OrderDate != DateTime.MinValue.to(DateTime.Now))",
              db => db.Orders.All().Where(db.Orders.OrderDate != SqlDateTime.MinValue.Value.to(DateTime.Now)));
            //ok WHERE [dbo].[Orders].[OrderDate] NOT BETWEEN @p1_start AND @p1_end 

            ExampleRunner.RunQuery(
                "Using IS NULL. 1. Embedded : db.Albums.FindAllByGenreId(null);",
                db => db.Albums.FindAllByGenreId(null));
            //ok WHERE [dbo].[Albums].[GenreId] IS NULL

            ExampleRunner.RunQuery(
                "Using IS NULL. 2. Embedded : db.Albums.FindAllBy(GenreId:null);",
                db => db.Albums.FindAllBy(GenreId: null));
            //ok WHERE [dbo].[Albums].[GenreId] IS NULL

            ExampleRunner.RunQuery(
                "Using IS NULL. 3. In FindAll : db.Albums.FindAll(db.Albums.GenreId == null);",
                db => db.Albums.FindAll(db.Albums.GenreId == null));
            //ok WHERE [dbo].[Albums].[GenreId] IS NULL

            ExampleRunner.RunQuery(
                "Using IS NULL. 4. In Where : db.Albums.All().Where(db.Albums.GenreId == null);",
                db => db.Albums.All().Where(db.Albums.GenreId == null));
            //ok WHERE [dbo].[Albums].[GenreId] IS NULL

            ExampleRunner.RunQuery(
                "Using IS NOT NULL. 1. In FindAll : db.Albums.FindAll(db.Albums.GenreId != null);",
                db => db.Albums.FindAll(db.Albums.GenreId != null));
            //ok WHERE [dbo].[Albums].[GenreId] IS NOT NULL

            ExampleRunner.RunQuery(
                "Using IS NOT NULL. 2. In Where : db.Albums.All().Where(db.Albums.GenreId != null);",
                db => db.Albums.All().Where(db.Albums.GenreId != null));
            //ok WHERE [dbo].[Albums].[GenreId] IS NOT NULL
        }
    }
}