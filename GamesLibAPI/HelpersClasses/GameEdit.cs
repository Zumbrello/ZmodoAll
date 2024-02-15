using GamesLibAPI.Models;

namespace GamesLibAPI.HelpersClasses;

public class GameEdit
{
    public int Id { get; set; }

    public string GameName { get; set; }

    public int IdDeveloper { get; set; }

    public int IdPublisher { get; set; }

    public string Description { get; set; }

    public string SystemRequestMin { get; set; }

    public string SystemRequestRec { get; set; }

    public string? ReleaseDate { get; set; }

    public string? MainImage { get; set; }
    //public virtual ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
}