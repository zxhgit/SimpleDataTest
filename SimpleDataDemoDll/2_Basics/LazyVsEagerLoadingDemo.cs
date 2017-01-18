using System;
using SimpleDataDemoDll.POCO;

namespace SimpleDataDemoDll
{
    internal class LazyVsEagerLoadingSamples
    {
        internal void RunAll()
        {
            JoinRunner.RunDemo(
                "db.Artists.Get(22) - SimpleRecord lazy loaded accessed as dynamic object",
                db => db.Artists.Get(22),
                String.Empty, false);

            JoinRunner.RunDemo(
                "db.Artists.Get(22) - SimpleRecord lazy loaded accessed as artist POCO",
                db => db.Artists.Get(22),
                "singleArtist", false);
            //artist.AlbumsΪnull��δ�������,��Ϊת�����Զ���ľ�̬����Artist��ֻ��SimpleData��SimpleRecord��SimpleQuery�Ż����

            JoinRunner.RunDemo(
                "db.Artists.All() - SimpleQuery lazy loaded accessed as dynamic object",
                db => db.Artists.All(),
                String.Empty, false);
            //�����������ѯAlbums��sql

            JoinRunner.RunDemo(
                "db.Artists.All() - SimpleQuery lazy loaded with explicit join accessed as dynamic object",
                db =>
                    db.Artists.All()
                        .Join(db.Albums)
                        .On(db.Artists.ArtistId == db.Albums.ArtistId)
                        .Select(db.Albums.Title, db.Artists.Name, db.Artists.ArtistId),
                String.Empty, false);
            //�����������ѯAlbums��sql

            JoinRunner.RunDemo(
                "db.Artists.All() - SimpleQuery lazy loaded accessed as dynamic object, cast to IEnumerable<Artist>",
                db => db.Artists.All().Cast<Artist>(),
                "IEnumerableArtist", false);
            //ͬ�ڶ���Demo

            JoinRunner.RunDemo(
                "db.Artists.All() - SimpleQuery lazy loaded accessed as dynamic object, cast to List<dynamic>",
                db => db.Artists.All().ToList(),
                "ListDynamic", false);
            //�����������ѯAlbums��sql

            //----------------------����Ϊlazy load----------------------------
            //----------------------����Ϊeager load----------------------------

            JoinRunner.RunDemo(
                "db.Artists.FindAllByArtistId(22).WithAlbums().FirstOrDefault(). SimpleRecord eager loaded accessed as dynamic object.",
                db => db.Artists.FindAllByArtistId(22).WithAlbums().FirstOrDefault(),
                String.Empty, true);
            //if (!isEager || (artist.Albums != null))
            //isEager��Ϊtrue�������artist.Albums != null
            //with __Data AS(
            //SELECT[dbo].[Artists].[ArtistId], ROW_NUMBER() over(order by[dbo].[Artists].[ArtistId]) as [_#_]
            //  FROM[dbo].[Artists] left join[dbo].[Albums]
            //  ON([dbo].[Artists].[ArtistId] =[dbo].[Albums].[ArtistId])
            //  where[dbo].[Artists].[ArtistId] = 22
            //  )
            //  select[dbo].[Artists].[ArtistId],[dbo].[Artists].[Name],
            //  [dbo].[Albums].[AlbumId] as [__withn__Albums__AlbumId],
            //  [dbo].[Albums].[GenreId] as [__withn__Albums__GenreId],
            //  [dbo].[Albums].[ArtistId] as [__withn__Albums__ArtistId],
            //  [dbo].[Albums].[Title] as [__withn__Albums__Title],
            //  [dbo].[Albums].[Price] as [__withn__Albums__Price],
            //  [dbo].[Albums].[AlbumArtUrl] as [__withn__Albums__AlbumArtUrl]
            //  from __Data join[dbo].[Artists]
            //  on[dbo].[Artists].[ArtistId]=__Data.ArtistId
            //  left join[dbo].[Albums]
            //  on[dbo].[Artists].[ArtistId]=[dbo].[Albums].[ArtistId]
            //  where[dbo].[Artists].[ArtistId]=22 and[_#_] between 1 and 1

            JoinRunner.RunDemo(
                "db.Artists.FindAllByArtistId(22).WithAlbums().FirstOrDefault(). SimpleRecord eager loaded accessed as Artist POCO. ",
                db => db.Artists.FindAllByArtistId(15).WithAlbums().FirstOrDefault(),
                "singleArtist", true);
            //WITH __Data AS(
            //SELECT[dbo].[Artists].[ArtistId], ROW_NUMBER() OVER(ORDER BY[dbo].[Artists].[ArtistId]) AS[_#_]
            //from[dbo].[Artists] LEFT JOIN[dbo].[Albums]
            //ON([dbo].[Artists].[ArtistId] = [dbo].[Albums].[ArtistId])
            //WHERE[dbo].[Artists].[ArtistId] = 15
            //)
            //SELECT[dbo].[Artists].[ArtistId],[dbo].[Artists].[Name],
            //[dbo].[Albums].[AlbumId] AS[__withn__Albums__AlbumId],
            //[dbo].[Albums].[GenreId] AS[__withn__Albums__GenreId],
            //[dbo].[Albums].[ArtistId] AS[__withn__Albums__ArtistId],
            //[dbo].[Albums].[Title] AS[__withn__Albums__Title],
            //[dbo].[Albums].[Price] AS[__withn__Albums__Price],
            //[dbo].[Albums].[AlbumArtUrl] AS[__withn__Albums__AlbumArtUrl]
            //FROM __Data JOIN[dbo].[Artists]
            //ON [dbo].[Artists].[ArtistId] = __Data.[ArtistId]
            //LEFT JOIN[dbo].[Albums]
            //ON([dbo].[Artists].[ArtistId] = [dbo].[Albums].[ArtistId])
            //WHERE[dbo].[Artists].[ArtistId] = 15 AND[_#_] BETWEEN 1 AND 1

            JoinRunner.RunDemo(
                "db.Artists.All().WithAlbums() - SimpleQuery eaqer loaded & accessed as dynamic object",
                db => db.Artists.All().WithAlbums(),
                String.Empty, true);
            //��lazy load��ͬ��������demo�������������sql��
            //select[dbo].[Artists].[ArtistId],[dbo].[Artists].[Name],
            //[dbo].[Albums].[AlbumId] AS[__withn__Albums__AlbumId],
            //[dbo].[Albums].[GenreId] AS[__withn__Albums__GenreId],
            //[dbo].[Albums].[ArtistId] AS[__withn__Albums__ArtistId],
            //[dbo].[Albums].[Title] AS[__withn__Albums__Title],
            //[dbo].[Albums].[Price] AS[__withn__Albums__Price],
            //[dbo].[Albums].[AlbumArtUrl] AS[__withn__Albums__AlbumArtUrl]
            //from[dbo].[Artists] LEFT JOIN[dbo].[Albums]
            //ON([dbo].[Artists].[ArtistId] = [dbo].[Albums].[ArtistId])

            JoinRunner.RunDemo(
                "db.Artists.All().WithAlbums().Cast<Artist>() - SimpleQuery lazy loaded accessed as dynamic object, cast to IEnumerable<Artist>",
                db => db.Artists.All().WithAlbums().Cast<Artist>(),
                "IEnumerableArtist", true);
            //slqͬ��

            JoinRunner.RunDemo(
                "db.Artists.All().WithAlbums().ToList() - SimpleQuery lazy loaded accessed as dynamic object, cast to List<dynamic>",
                db => db.Artists.All().WithAlbums().ToList(),
                "ListDynamic", true);
            //slqͬ��
        }
    }
}