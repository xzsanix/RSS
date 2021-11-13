using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RSS.Models;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace RSS.Controllers
{
    public class CategoryController : Controller
    {
        Data data = new Data();
        public IActionResult Index()
        {
            return View(data.Categories);
        }

        public ActionResult Export()
        {
            SqlConnection con = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=C:\\USERS\\ASUST\\ONEDRIVE\\DOKUMENTUMOK\\DATABASE.MDF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            string strSQL = "Select Title from Category";
            SqlDataAdapter dt = new SqlDataAdapter(strSQL, con);

            DataSet ds = new DataSet();
            dt.Fill(ds, "Category");
            ds.WriteXml("categories.opml"); //project mappajaba menti le

            return View("Views/Category/Index.cshtml", data.Categories);
        }

        public ActionResult Import([Bind("Id,Title")] Category category)
        {
            Random rnd = new Random();
            XmlTextReader reader = new XmlTextReader("cat.opml");

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Title")
                {
                    reader.Read();
                    if ((reader.NodeType == XmlNodeType.Text) && (reader.HasValue))
                    {
                        category.Title = reader.Value;

                        data.Add(category);
                        data.SaveChanges();

                    }

                }
            }

            return View("Views/Category/Index.cshtml", data.Categories);
        }


        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] Category category)
        {
            if (ModelState.IsValid)
            {
                data.Add(category);
                await data.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await data.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    data.Update(category);
                    await data.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await data.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await data.Categories.FindAsync(id);
            data.Categories.Remove(category);
            await data.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return data.Categories.Any(e => e.Id == id);
        }
    }
}
