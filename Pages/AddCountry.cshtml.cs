using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Pages
{
    public class AddCountryModel : PageModel
    {
        public List<Country> Countries { get; set; } = new List<Country>();

        [BindProperty]
        public string Name { get; set; }

        public void OnGet()
        {
            using (var context = new Datab())
            {
                Countries = context.Countries.ToList();
            }
        }

        public IActionResult OnPost()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                using (var context = new Datab())
                {
                    context.Countries.Add(new Country { Name = Name });
                    context.SaveChanges();
                }
            }
            return RedirectToPage("./AddCountry");
        }
    }
}