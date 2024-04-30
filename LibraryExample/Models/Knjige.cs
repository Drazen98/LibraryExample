namespace LibraryExample.Models
{
    public class Knjige(int iD, string? name, AutorKnjige bookAuthor, Zanr bookGenre, DateTime createTime)
    {
        public int ID { get; set; } = iD;
        public string? Name { get; set; } = name;

        public AutorKnjige Author { get; set; } = bookAuthor;

        public Zanr Zanr { get; set; } = bookGenre;

        public DateTime CreateTime { get; set; } = createTime;
    }

    public class KnjigeView(string? name, AutorKnjigeView bookAuthor, ZanrView bookGenre, DateTime createTime)
    {
        public string? Name { get; set; } = name;

        public AutorKnjigeView Author { get; set; } = bookAuthor;

        public ZanrView Zanr { get; set; } = bookGenre;

        public DateTime CreateTime { get; set; } = createTime;
    }

}
