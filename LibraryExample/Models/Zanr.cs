namespace LibraryExample.Models
{
    public class Zanr(int iD, string? name)
    {
        public int ID { get; set; } = iD;
        public string? Name { get; set; } = name;
    }
    public class ZanrView(string? name)
    {
        public string? Name { get; set; } = name;
    }
}

