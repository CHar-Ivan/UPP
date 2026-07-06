namespace WebApplication1.Pages
{
    public class Pr
    {
        public int Id { get; set; }
        public string Exhibit { get; set; }
        public string Exhibitor { get; set; }
        public string Date { get; set; }

        private static int i = 0;

        public Pr() { }

        public Pr(string exhibitName, string exhibitorFio, string date)
        {
            Id = ++i;
            Exhibit = exhibitName;
            Exhibitor = exhibitorFio;
            Date = date;
        }
    }
}