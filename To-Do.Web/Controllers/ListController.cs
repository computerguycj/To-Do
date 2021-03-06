using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using To_Do.Data;
using To_Do.Models;

namespace To_Do.Controllers
{
    public class ListController : Controller
    {
        private readonly ListItemContext _context;
        private readonly ILogger<ListController> _logger;

        public ListController(ListItemContext context, ILogger<ListController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: List
        public async Task<IActionResult> Index(string sort
            , string filter
            , string searchString
            , int? pageNumber)
        {
            ViewData["Sort"] = sort;
            ViewData["TextSort"] = StringComparer.OrdinalIgnoreCase.Equals(sort, "text") ? "text_desc" : "text";
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
                case "text":
                    items = items.OrderBy(i => i.Text);
                    break;
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
                    items = items.OrderBy(i => i.Id);
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
                _logger.Log(LogLevel.Information, "Successfully created item", listItem);
                return RedirectToAction(nameof(Index));
            }
            return View(listItem);
        }

        // GET: List/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.Log(LogLevel.Error, "Edit method called without an id");
                return NotFound();
            }

            var listItem = await _context.ListItems.FindAsync(id);
            if (listItem == null)
            {
                _logger.Log(LogLevel.Error, $"Unable to find item with id: {id}");
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
                _logger.Log(LogLevel.Warning, "Edit POST method called with a different id than the object's id");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(listItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.Log(LogLevel.Error, "Edit POST method failed", ex);
                    if (!ListItemExists(listItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                _logger.Log(LogLevel.Information, "Successfully edited item", listItem);
                return RedirectToAction(nameof(Index));
            }
            return View(listItem);
        }

        private bool ListItemExists(int id)
        {
            return _context.ListItems.Any(e => e.Id == id);
        }

        [HttpPut]
        public ActionResult ToggleComplete(int? id)
        {
            try
            {
                var item = _context.ListItems.FindAsync(id).Result;
                if (null == item)
                {
                    _logger.Log(LogLevel.Warning, $"ToggleComplete method couldn't find an item with id: {id}");
                    return BadRequest(new { error = $"Can't find item with id: {id}." });
                }

                item.Completed = !item.Completed;
                _context.SaveChangesAsync();

                _logger.Log(LogLevel.Information, $"ToggleComplete method succeeded for id: {id}");
                return Json(new { msg = $"Successfully toggled item id: {id}." });
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "ToggleComplete method failed", ex);
                throw ex;
            }
        }
    }
}
