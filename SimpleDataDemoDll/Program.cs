using System;

namespace SimpleDataDemoDll
{
    internal class Program
    {
        private static void Main()
        {
            do
            {
                ShowOptions();
                switch (Console.ReadLine())
                {
                    case "1"://test
                        var openDbDemo = new OpenDatabaseSamples();
                        openDbDemo.RunAll();
                        break;
                    case "2"://test
                        var allMethodDemo = new AllMethodSamples();
                        allMethodDemo.RunAll();
                        break;
                    case "3"://FindByXX取top 1
                        var findByMethodDemo = new FindByMethodSamples();
                        findByMethodDemo.RunAll();
                        break;
                    case "4"://FindAllByXX取全部
                        var findAllByMethodDemo = new FindAllByMethodSamples();
                        findAllByMethodDemo.RunAll();
                        break;
                    case "5"://sql中都包含top 1
                        var getMethodDemo = new GetMethodSamples();
                        getMethodDemo.RunAll();
                        break;
                    case "6"://用表达式查询，sql无top 1(控制台输出sql有误，应该有top 1)
                        var findMethodDemo = new FindMethodSamples();
                        findMethodDemo.RunAll();
                        break;
                    case "7"://用表达式查询，sql无top 1
                        var findAllMethodDemo = new FindAllMethodSamples();
                        findAllMethodDemo.RunAll();
                        break;
                    case "8":
                        var columnSelectionDemo = new ColumnSelectionSamples();
                        columnSelectionDemo.RunAll();
                        break;
                    case "9":
                        var basicWhereDemo = new WhereMethodSamples();
                        basicWhereDemo.RunAll();
                        break;
                    case "a":
                    case "A":
                        var commonSearchesDemo = new WhereConditionSamples();
                        commonSearchesDemo.RunAll();
                        break;
                    case "b":
                    case "B":
                        var countDistinctDemo = new CountDistinctSamples();
                        countDistinctDemo.RunAll();
                        break;
                    case "c":
                    case "C":
                        var getCountDemo = new GetCountMethodSamples();
                        getCountDemo.RunAll();
                        break;
                    case "d":
                    case "D":
                        var getCountByDemo = new GetCountByMethodSamples();
                        getCountByDemo.RunAll();
                        break;
                    case "e":
                    case "E":
                        var pocoDemo = new PocoMethodSamples();
                        pocoDemo.RunAll();
                        break;
                    case "f":
                    case "F"://http://simplefx.org/simpledata/docs/pages/Retrieve/ScalarValues.htm
                        var scalarDemo = new ToScalarSamples();
                        scalarDemo.RunAll();
                        break;
                    case "g":
                    case "G":
                        var scalarCollectionDemo = new ToScalarCollectionSamples();
                        scalarCollectionDemo.RunAll();
                        break;
                    case "h":
                    case "H"://http://simplefx.org/simpledata/docs/pages/Retrieve/LazyLoadingJoins.htm
                        var naturalJoinDemo = new NaturalJoinSamples();
                        naturalJoinDemo.RunAll();
                        break;
                    case "i":
                    case "I"://http://simplefx.org/simpledata/docs/pages/Retrieve/LazyLoadingJoins.htm
                        var explicitJoinDemo = new ExplicitJoinSamples();
                        explicitJoinDemo.RunAll();
                        break;
                    case "j":
                    case "J":
                        var outerJoinDemo = new OuterJoinSamples();
                        outerJoinDemo.RunAll();
                        break;
                    case "k":
                    case "K"://类似OuterJoinSamples，可参照
                        var leftJoinDemo = new LeftJoinSamples();
                        leftJoinDemo.RunAll();
                        break;
                    case "l":
                    case "L":
                        var lazyEagerDemo = new LazyVsEagerLoadingSamples();
                        lazyEagerDemo.RunAll();
                        break;
                    case "m":
                    case "M":
                        var existsDemo = new ExistsMethodSamples();
                        existsDemo.RunAll();
                        break;
                    case "n":
                    case "N":
                        var existsByDemo = new ExistsByMethodSamples();
                        existsByDemo.RunAll();
                        break;
                    case "o":
                    case "O":
                        var orderByDemo = new OrderByMethodSamples();
                        orderByDemo.RunAll();
                        var orderByDescDemo = new OrderByDescendingMethodSamples();//类似OrderByMethodSamples
                        orderByDescDemo.RunAll();
                        break;
                    case "p":
                    case "P":
                        var thenByDemo = new ThenByMethodSamples();
                        thenByDemo.RunAll();
                        break;
                    case "q":
                    case "Q":
                        var withDemo = new WithMethodSamples();
                        withDemo.RunAll();
                        break;
                    case "r":
                    case "R":
                        var havingDemo = new HavingMethodSamples();
                        havingDemo.RunAll();
                        break;
                    case "x":
                    case "X":
                        Console.WriteLine("Fin");
                        return;
                    default:
                        Console.WriteLine("Not a valid option. Choose again");
                        break;
                }
            } while (true);
        }

        private static void ShowOptions()
        {
            Console.WriteLine();
            Console.WriteLine("--------");
            Console.WriteLine();
            Console.WriteLine("Simple.Data Samples");
            Console.WriteLine("Choose from the following options");
            Console.WriteLine();
            Console.WriteLine("1. Open Database Demos");
            Console.WriteLine("2. Basic Selection with All()");
            Console.WriteLine("3. Basic Selection with FindBy()");
            Console.WriteLine("4. Basic Selection with FindAllBy()");
            Console.WriteLine("5. Basic Selection with Get()");
            Console.WriteLine("6. Basic Selection with Find()");
            Console.WriteLine("7. Basic Selection with FindAll()");
            Console.WriteLine("8. Column Selection and Aliasing");
            Console.WriteLine("9. Basic use of Where()");
            Console.WriteLine("A. Common Search Conditions");
            Console.WriteLine("B. Count \\ Distinct Demos");
            Console.WriteLine("C. GetCount Demos");
            Console.WriteLine("D. GetCountBy Demos");
            Console.WriteLine("E. Poco Demos");
            Console.WriteLine("F. ToScalar Demos");
            Console.WriteLine("G. ToScalar Collection Demos");
            Console.WriteLine("H. Natural Join Demos");
            Console.WriteLine("I. Explicit Join Demos");
            Console.WriteLine("J. Outer Join Demos");
            Console.WriteLine("K. Left Join Demos");
            Console.WriteLine("L. Lazy vs Eager Loading Demos");
            Console.WriteLine("M. Any \\ Exists Demos");
            Console.WriteLine("N. AnyBy \\ ExistsBy Demos");
            Console.WriteLine("O. OrderBy(Descending) Demos");
            Console.WriteLine("P. ThenBy(Descending) Demos");
            Console.WriteLine("Q. With Demos");
            Console.WriteLine("R. Having Demos");
            Console.WriteLine("X. Quit");
            Console.WriteLine();
        }
    }
}