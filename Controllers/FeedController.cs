using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RSS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RSS.Controllers
{
    public class FeedController : Controller
    {
        Data data = new Data();


        public IActionResult Index()
        {
            var query = from f in data.Feeds.Include
                (feed => feed.Category)
                        select f;

            return View(query.ToList());
        }

        public async Task<IActionResult> FeedOpen(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var feed = data.Feeds.Where(f => f.Id == id).Single();
            var feedUrl = feed.Url;


            XmlTextReader reader = new XmlTextReader(feedUrl);

            Dictionary<string, string> OneItem = new Dictionary<string, string>();

            var title = "";
            var link = "";
            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "title")
                {
                    reader.Read();
                    if ((reader.NodeType == XmlNodeType.Text) && (reader.HasValue))
                    {
                        title = reader.Value;
                    }
                }

                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "link")
                {
                    reader.Read();
                    if ((reader.NodeType == XmlNodeType.Text) && (reader.HasValue))
                    {
                        link = reader.Value;
                    }
                }
                
                Response.Cookies.Append("Viewed", link);

                if (title != "" && link != "" && !OneItem.ContainsKey(title))
                {
                    OneItem.Add(title, link);
                    title = "";
                    link = "";
                }

            }

            ViewBag.Dictionary = OneItem;
            ViewBag.Title = feed.Url;

            return View();

        }

        // GET: Feed/Create
        public IActionResult Create()
        {
            ViewBag.Category = new SelectList(data.Categories, "Id", "Title");

            return View();
        }

        // POST: Feed/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Url,CategoryId")] Feed feed)
        {
            ViewBag.Category = new SelectList(data.Categories, "Id", "Title");

            if (ModelState.IsValid)
            {
                data.Add(feed);
                await data.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(feed);
        }

        // GET: Feed/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Category = new SelectList(data.Categories, "Id", "Title");

            if (id == null)
            {
                return NotFound();
            }

            var feed = await data.Feeds.FindAsync(id);
            if (feed == null)
            {
                return NotFound();
            }
            return View(feed);
        }

        // POST: Feed/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Url, CategoryId")] Feed feed)
        {
            ViewBag.Category = new SelectList(data.Categories, "Id", "Title");

            if (id != feed.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    data.Update(feed);
                    await data.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedExists(feed.Id))
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
            return View(feed);
        }

        // GET: Feed/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feed = await data.Feeds
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feed == null)
            {
                return NotFound();
            }

            return View(feed);
        }

        // POST: Feed/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feed = await data.Feeds.FindAsync(id);
            data.Feeds.Remove(feed);
            await data.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedExists(int id)
        {
            return data.Feeds.Any(e => e.Id == id);
        }

    }

}
