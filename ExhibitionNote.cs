namespace WebApplication1
{
    public class ExhibitionNote
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public int Exhibitor_Id { get; set; }
        public int Exhibit_Id { get; set; }
        public string Date { get; set; }

        public ExhibitionNote() { }

        public ExhibitionNote(int exhibitId, int userId, int exhibitorId)
        {
            Exhibit_Id = exhibitId;
            User_Id = userId;
            Exhibitor_Id = exhibitorId;
            Date = System.DateTime.Now.ToString("dd.MM.yyyy");
        }
    }
}