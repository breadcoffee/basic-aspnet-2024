using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MyPortfolio.Data;
using MyPortfolio.Models;

namespace MyPortfolio.Controllers
{
    public class ProjectController : Controller
    {
        private readonly AppDbContext _context;

        // 파일 업로드 웹환경 
        private readonly IWebHostEnvironment _environment;

        public ProjectController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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
        public async Task<IActionResult> Create(TempProject temp)
        {
            if (ModelState.IsValid)
            {
                // 파일 업로드
                string upFileName = UploadImageFile(temp);

                // 파일명을 받아서 TempProject 내용을 Project로 이전
                Project project = new Project
                {
                    ProjectName = temp.ProjectName,
                    Description = temp.Description,
                    FilePath = upFileName,
                };

                _context.Add(project);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(temp);
        }


        // GET: Project/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) { return NotFound(); }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            // 이미지 때문에 추가작업
            TempProject temp = new TempProject
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                Description = project.Description,
                FilePath = project.FilePath,
            };

            return View(temp);
        }

        // POST: Project/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TempProject temp)
        {
            if (ModelState.IsValid)
            {
                // 파일 업로드
                string upFileName = UploadImageFile(temp);

                // 새로 업로드된 파일이 없고, 이전 파일명이 있으면
                // 그 파일명을 그대 사용!!!
                if (upFileName == string.Empty && temp.FilePath != string.Empty)
                {
                    upFileName = temp.FilePath;
                }

                try
                {
                    // 파일명을 받아서 TempProject 내용을 Project로 이전
                    Project project = new Project
                    {
                        Id = temp.Id,
                        ProjectName = temp.ProjectName,
                        Description = temp.Description,
                        FilePath = upFileName,
                    };

                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(temp.Id))
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
            return View(temp);
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

        #region `업로드메서드 - Routing에 관련없음`

        /// <summary>
        /// 파일업로드 메서드!!
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        private string UploadImageFile(TempProject temp)
        {
            var resultFileName = string.Empty;

            if (temp.ProjectImage != null)
            {
                string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");
                resultFileName = Guid.NewGuid() + "_" + temp.ProjectImage.FileName;
                string filePath = Path.Combine(uploadFolder, resultFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    temp.ProjectImage.CopyTo(fileStream);
                }
            }

            return resultFileName;
        }

        #endregion
    }
}
