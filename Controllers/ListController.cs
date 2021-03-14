using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using To_Do.Data;
using To_Do.Models;

namespace To_Do.Controllers
{
    public class ListController : Controller
    {
        private readonly ListItemContext _context;

        public ListController(ListItemContext context)
        {
            _context = context;
        }

        // GET: List
        public async Task<IActionResult> Index(string sort
            , string filter
            , string searchString
            , int? pageNumber)
        {
            ViewData["Sort"] = sort;
            ViewData["TextSort"] = string.IsNullOrEmpty(sort) ? "text_desc" : "";
            ViewData["CompletedSort"] = StringComparer.OrdinalIgnoreCase.Equals(sort, "completed") ? "completed_desc" : "completed";
            filter = searchString ?? filter;
            ViewData["Filter"] = filter;

            var items = _context.ListItems.AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                items = items.Where(i => i.Text.Contains(filter));
            }

            switch (sort?.ToLower())
            {
                case "text_desc":
                    items = items.OrderByDescending(i => i.Text);
                    break;
                case "completed":
                    items = items.OrderBy(i => i.Completed);
                    break;
                case "completed_desc":
                    items = items.OrderByDescending(i => i.Completed);
                    break;
                default:
                    items = items.OrderBy(i => i.Text);
                    break;
            }

            int pageSize = 5;
            return View(await PaginatedList<ListItem>.CreateAsync(items.AsNoTracking(), GetEffectivePageNumber(searchString, pageNumber), pageSize));
        }

        private static int GetEffectivePageNumber(string searchString, int? pageNumber)
        {
            //If we have a search string to process, reset the page number to 1
            if (null != searchString)
            { return 1; }
            
            //If a page number was specified, return that, otherwise default to 1
            return pageNumber ?? 1;
        }

        // GET: List/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listItem = await _context.ListItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listItem == null)
            {
                return NotFound();
            }

            return View(listItem);
        }

        // GET: List/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: List/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Text,Completed")] ListItem listItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(listItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(listItem);
        }

        // GET: List/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listItem = await _context.ListItems.FindAsync(id);
            if (listItem == null)
            {
                return NotFound();
            }
            return View(listItem);
        }

        // POST: List/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Text,Completed")] ListItem listItem)
        {
            if (id != listItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(listItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListItemExists(listItem.Id))
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
            return View(listItem);
        }

        // GET: List/Complete/5
        public async Task<IActionResult> Complete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listItem = await _context.ListItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listItem == null)
            {
                return NotFound();
            }

            return View(listItem);
        }

        // POST: List/Complete/5
        [HttpPost, ActionName("Complete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteConfirmed(int id)
        {
            var listItem = await _context.ListItems.FindAsync(id);
            listItem.Completed = !listItem.Completed;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ListItemExists(int id)
        {
            return _context.ListItems.Any(e => e.Id == id);
        }
    }
}
