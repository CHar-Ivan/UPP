namespace WebApplication1
{
    public class Exhibit
    {
        public int Id { get; set; }
        public string Name { get; set; } // Название экспоната
        public string Material { get; set; } // Материал (Холст, Мрамор, Золото)
        public int Country_Id { get; set; } // Страна происхождения
    }
}