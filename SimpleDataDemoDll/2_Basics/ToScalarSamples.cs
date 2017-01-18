using System.Collections.Generic;

namespace SimpleDataDemoDll
{
    internal class ToScalarSamples
    {
        internal void RunAll()
        {
            // ToScalar http://simplefx.org/simpledata/docs/pages/Retrieve/ScalarValues.htm
            // Throws RuntimeBinderException. 
            ExampleRunner.RunQuery(
                "Get a single album. Return null. Cast with ToScalar()",
                db => db.Albums.Get(1000).ToScalar(),
                new List<string>(), "string");
            //表里没有AlbumId=1000的数据，返回null

            ExampleRunner.RunQuery(
                "Get a single album. Cast with ToScalar()",
                db => db.Albums.Get(1).ToScalar(),
                new List<string>(), "string");
            //Calling ToScalar on this will return the first property in the SimpleRecord as a scalar value
            //返回1

            // Throws InvalidCastException
            ExampleRunner.RunQuery(
                "Get a single album title. Cast with ToScalar<int>(). Throws InvalidCastException",
                db => db.Albums.Select(db.Albums.Title).Get(1).ToScalar<int>(),
                new List<string>(), "string");
            //title为string

            ExampleRunner.RunQuery(
                "Get a single album title. Cast with ToScalar<string>()",
                db => db.Albums.Select(db.Albums.Title).Get(1).ToScalar<string>(),
                new List<string>(), "string");

            ExampleRunner.RunQuery(
                "Get a list of album titles. Return null. Cast with ToScalar(). Throws Exception",
                db => db.Albums.FindAllByGenreId(100).Select(db.Albums.Title).ToScalar());
            //表里没有GenreId=100的数据，返回null

            ExampleRunner.RunQuery(
                "Get a list of many album titles. Cast with ToScalar(). Throws Exception",
                db => db.Albums.FindAllByGenreId(1).Select(db.Albums.Title).ToScalar());
            //没有抛异常，返回第一条数据的第一个字段——Title

            ExampleRunner.RunQuery(
                "Get a list of one album title. Cast with ToScalar(). Returns album title as string",
                db => db.Albums.FindAllByAlbumId(1).Select(db.Albums.Title).ToScalar());

            ExampleRunner.RunQuery(
                "Get a list of one album. Cast with ToScalar(). Returns albumid as int",
                db => db.Albums.FindAllByAlbumId(1).ToScalar());

            ExampleRunner.RunQuery(
                "Get a list of one album title. Cast with ToScalar<string>(). Returns album title as string",
                db => db.Albums.FindAllByAlbumId(1).Select(db.Albums.Title).ToScalar<string>());

            ExampleRunner.RunQuery(
                "Get a list of one album. Cast with ToScalar<string>(). Throws InvalidCastException",
                db => db.Albums.FindAllByAlbumId(1).ToScalar<string>());

            // ToScalarOrDefault
            ExampleRunner.RunQuery(
                "Get a single album. Return null. Cast with ToScalarOrDefault(). Throws RuntimeBinderException",
                db => db.Albums.Get(1000).ToScalarOrDefault(),
                new List<string>(), "string");

            ExampleRunner.RunQuery(
                "Get a single album. Cast with ToScalarOrDefault(). Throws UnresolvableObjectException",
                db => db.Albums.Get(1).ToScalarOrDefault(),
                new List<string>(), "string");
            //ToScalarOrDefault and ToScalarOrDefault<T> will throw an UnresolvableObjectException if the query you apply them to returns 
            //a single result as a SimpleRecord instance rather than a SimpleQuery.

            ExampleRunner.RunQuery(
                "Get a single album title. Cast with ToScalarOrDefault<int>(). Throws UnresolvableObjectException",
                db => db.Albums.Select(db.Albums.Title).Get(1).ToScalarOrDefault<int>(),
                new List<string>(), "string");

            ExampleRunner.RunQuery(
                "Get a single album title. Cast with ToScalarOrDefault<string>(). Throws UnresolvableObjectException",
                db => db.Albums.Select(db.Albums.Title).Get(1).ToScalarOrDefault<string>(),
                new List<string>(), "string");

            ExampleRunner.RunQuery(
                "Get a list of zero album titles. Return null. Cast with ToScalarOrDefault(). Returns null",
                db => db.Albums.FindAllByGenreId(100).Select(db.Albums.Title).ToScalarOrDefault());
            //return null 表里没有GenreId=100的数据

            ExampleRunner.RunQuery(
                "Get a list of many album titles. Cast with ToScalarOrDefault(). Throws SimpleDataException",
                db => db.Albums.FindAllByGenreId(1).Select(db.Albums.Title).ToScalarOrDefault());
            //返回title值，没有抛异常

            ExampleRunner.RunQuery(
                "Get a list of one album title. Cast with ToScalarOrDefault(). Returns album title as string",
                db => db.Albums.FindAllByAlbumId(1).Select(db.Albums.Title).ToScalarOrDefault());

            ExampleRunner.RunQuery(
                "Get a list of one album. Cast with ToScalarOrDefault(). Returns albumid as int",
                db => db.Albums.FindAllByAlbumId(1).ToScalarOrDefault());

            ExampleRunner.RunQuery(
                "Get a list of one album title. Cast with ToScalarOrDefault<string>(). Returns album title as string",
                db => db.Albums.FindAllByAlbumId(1).Select(db.Albums.Title).ToScalarOrDefault<string>());

            ExampleRunner.RunQuery(
                "Get a list of one album. Cast with ToScalarOrDefault<string>(). Throws InvalidCastException",
                db => db.Albums.FindAllByAlbumId(1).ToScalarOrDefault<string>());
            //Microsoft.CSharp.RuntimeBinder.RuntimeBinderException
            //无法将类型“int”隐式转换为“string”
        }
    }
}