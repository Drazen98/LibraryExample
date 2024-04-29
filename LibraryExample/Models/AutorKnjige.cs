namespace LibraryExample.Models
{
    public class AutorKnjige(int iD, string? name, int birthDate)
    {
        public int ID { get; set; } = iD;
        public string? Name { get; set; } = name;

        public int BirthDate { get; set; } = birthDate;
    }
    public class AutorKnjigeView(string? name, int birthDate)
    {
        public string? Name { get; set; } = name;

        public int BirthDate { get; set; } = birthDate;
    }
}
