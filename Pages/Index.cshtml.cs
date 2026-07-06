using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        public List<Pr> pr { get; set; } = new List<Pr>();
        public List<Exhibitor> exhibitors { get; set; } = new List<Exhibitor>();
        public List<Exhibit> exhibits { get; set; } = new List<Exhibit>();
        public List<Country> countries { get; set; } = new List<Country>();

        // Свойства для хранения редактируемого экспоната
        [BindProperty]
        public Exhibit EditingExhibit { get; set; }

        public void OnGet()
        {
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new Datab())
            {
                // Получаем форматированные строки для таблицы выставки
                pr = (from n in context.ExhibitionNotes
                      join ex in context.Exhibits on n.Exhibit_Id equals ex.Id
                      join autor in context.Exhibitors on n.Exhibitor_Id equals autor.Id
                      where n.User_Id == 1
                      select new Pr(ex.Name, autor.FIO, n.Date) { Id = n.Id }).ToList(); // Передаем ID записи выставки

                // Загружаем авторов, которые еще не участвуют в выставке
                var allExhibitors = context.Exhibitors.ToList();
                exhibitors = allExhibitors.Where(a => !a.In_Exhibition).ToList();

                exhibits = context.Exhibits.ToList();
                countries = context.Countries.ToList();
            }
        }

        public IActionResult OnPost(string action, int selectedExhibitorId, int selectedExhibitId)
        {
            if (action == "addExhibit") return RedirectToPage("./AddExhibit");
            else if (action == "addCountry") return RedirectToPage("./AddCountry");
            else if (action == "addNote")
            {
                using (var context = new Datab())
                {
                    var note = new ExhibitionNote(selectedExhibitId, 1, selectedExhibitorId);
                    var author = context.Exhibitors.FirstOrDefault(e => e.Id == selectedExhibitorId);

                    if (author != null)
                    {
                        author.In_Exhibition = true;
                    }

                    context.ExhibitionNotes.Add(note);
                    context.SaveChanges();
                }
            }
            return RedirectToPage("./Index");
        }

        // 1. МЕТОД УДАЛЕНИЯ ЗАПИСИ ИЗ ВЫСТАВКИ
        public IActionResult OnPostDelete(int id)
        {
            using (var context = new Datab())
            {
                var note = context.ExhibitionNotes.FirstOrDefault(n => n.Id == id);
                if (note != null)
                {
                    // Освобождаем автора, чтобы он снова мог выставляться
                    var author = context.Exhibitors.FirstOrDefault(e => e.Id == note.Exhibitor_Id);
                    if (author != null)
                    {
                        author.In_Exhibition = false;
                    }

                    context.ExhibitionNotes.Remove(note);
                    context.SaveChanges();
                }
            }
            return RedirectToPage("./Index");
        }

        // 2. МЕТОД СОХРАНЕНИЯ ИЗМЕНЕНИЙ ПРИ РЕДАКТИРОВАНИИ ЭКСПОНАТА
        public IActionResult OnPostEditExhibit()
        {
            if (EditingExhibit != null && EditingExhibit.Id > 0)
            {
                using (var context = new Datab())
                {
                    var dbExhibit = context.Exhibits.FirstOrDefault(e => e.Id == EditingExhibit.Id);
                    if (dbExhibit != null)
                    {
                        dbExhibit.Name = EditingExhibit.Name;
                        dbExhibit.Material = EditingExhibit.Material;
                        dbExhibit.Country_Id = EditingExhibit.Country_Id;
                        context.SaveChanges();
                    }
                }
            }
            return RedirectToPage("./Index");
        }
    }
}