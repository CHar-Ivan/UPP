using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Pages
{
    public class AddExhibitModel : PageModel
    {
        public List<Exhibit> Exhibits { get; set; } = new List<Exhibit>();
        public List<Country> Countries { get; set; } = new List<Country>();

        [BindProperty]
        public Exhibit NewExhibit { get; set; }

        public void OnGet()
        {
            using (var context = new Datab())
            {
                Exhibits = context.Exhibits.ToList();
                Countries = context.Countries.ToList();
            }
        }

        public IActionResult OnPost()
        {
            using (var context = new Datab())
            {
                context.Exhibits.Add(NewExhibit);
                context.SaveChanges();
            }
            return RedirectToPage("./Index");
        }
    }
}