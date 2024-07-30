using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models;

namespace MyPortfolio.Controllers
{
    public class ProjectController : Controller
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Project
        public async Task<IActionResult> Index()
        {
            return View(await _context.Project.ToListAsync());
        }

        // GET: Project/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Project/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Project/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectName,Description")] Project project, IFormFile FilePath)
        {
            if (ModelState.IsValid)
            {
                var fileUrl = ""; // if (ModelState.IsValid)에 포함되는 지역변수로 변경
                if (FilePath != null && FilePath.Length > 0)
                {
                    //var fileUrl = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", FilePath.FileName);

                    // 이미지 파일 이름이 중복되지 않도록 랜덤으로 변경
                    var newFileName = "Mp_" + DateTime.Now.ToString("yyMMdd_HHmmssfff");

                    var extension = System.IO.Path.GetExtension(FilePath.FileName);

                    // FileStream으로 위 경로에 파일 저장
                    fileUrl = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", newFileName + extension);

                    //버퍼 사용
                    using (var stream = System.IO.File.Create(fileUrl))
                    {
                        await FilePath.CopyToAsync(stream);
                    }
                }

                // 새로 변경된 이미지파일 경로를 Project 객체에 할당
                project.FilePath = fileUrl;
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Project/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Project/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectName,Description")] Project project, IFormFile FilePath)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var fileUrl = "";
                try
                {
                    if (FilePath != null && FilePath.Length > 0)
                    {
                        //var fileUrl = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", FilePath.FileName);

                        // 이미지 파일 이름이 중복되지 않도록 랜덤으로 변경
                        var newFileName = "Mp_" + DateTime.Now.ToString("yyMMdd_HHmmssfff");

                        var extension = System.IO.Path.GetExtension(FilePath.FileName);

                        // FileStream으로 위 경로에 파일 저장
                        fileUrl = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", newFileName + extension);

                        //버퍼 사용
                        using (var stream = System.IO.File.Create(fileUrl))
                        {
                            await FilePath.CopyToAsync(stream);
                        }
                    }
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
            return View(project);
        }

        // GET: Project/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Project.FindAsync(id);
            if (project != null)
            {
                _context.Project.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.Id == id);
        }
    }
}
