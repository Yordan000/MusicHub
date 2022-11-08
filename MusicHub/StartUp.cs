using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace MusicHub
{
    using System;

    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
            string result = ExportSongsAboveDuration(context , 4);
            Console.WriteLine(result);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var allAlbums = context.Albums
                .Where( a=> a.ProducerId == producerId)
                .Include(p=> p.Producer)
                .Include(s => s.Songs)
                .ThenInclude( w=> w.Writer)
                .ToArray()
                .Select(a=> new 
                {
                    TotalPrice = a.Price,
                    AlbumName = a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy"),
                    ProducerName = a.Producer.Name,
                    AllAlbumSongs = a.Songs
                                     .Select( s=> new
                                     {
                                         SongName = s.Name,
                                         Price = s.Price,
                                         SongWriterName = s.Writer.Name
                                     })
                                     .OrderByDescending(s => s.SongName)
                                     .ThenBy( s=> s.SongWriterName)
                })
                .OrderByDescending(a => a.TotalPrice)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var a in allAlbums)
            {
                int count = 0;
                sb.AppendLine($"-AlbumName: {a.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {a.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {a.ProducerName}");
                sb.AppendLine($"-Songs:");
                foreach (var s in a.AllAlbumSongs)
                {
                    sb.AppendLine($"---#{++count}");
                    sb.AppendLine($"---SongName: {s.SongName}");
                    sb.AppendLine($"---Price: {s.Price:F2}");
                    sb.AppendLine($"---Writer: {s.SongWriterName}");
                }

                sb.AppendLine($"-AlbumPrice: {a.TotalPrice:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context.Songs
                .Include( w=> w.Writer)
                .Include(sp => sp.SongPerformers)
                .ThenInclude(p => p.Performer)
                .Include(a=> a.Album)
                .ThenInclude( p=> p.Producer)
                .ToArray()
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    SongName = s.Name,
                    PerformerFullName = s.SongPerformers.Select( sp => $"{sp.Performer.FirstName} {sp.Performer.LastName}").FirstOrDefault(),
                    WriterName = s.Writer.Name,
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.WriterName)
                .ThenBy(s => s.PerformerFullName)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (var s in songs)
            {
                sb.AppendLine($"-Song #{++count}");
                sb.AppendLine($"---SongName: {s.SongName}");
                sb.AppendLine($"---Writer: {s.WriterName}");
                sb.AppendLine($"---Performer: {s.PerformerFullName}");
                sb.AppendLine($"---AlbumProducer: {s.AlbumProducer}");
                sb.AppendLine($"---Duration: {s.Duration}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
