
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Models;

namespace MyMvcApp.Controllers
{  
    [Authorize(Roles = "Admin")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Student list database se fetch hogi
        // public async Task<IActionResult> Index()
        // {
        //     var students = await _context.Students.ToListAsync();
        //     return View(students);
        // }


        //searh filter 
        // public async Task<IActionResult> Index(string? searchString, string? course, string? name)
        // {
        //     var query = _context.Students.AsQueryable();

        //     if (!string.IsNullOrWhiteSpace(searchString))
        //     {
        //         query = query.Where(s =>
        //             s.Name.Contains(searchString) ||
        //             s.Email.Contains(searchString));
        //     }

        //     if (!string.IsNullOrWhiteSpace(course))
        //     {
        //         query = query.Where(s => s.Course == course);
        //     }
        //     if (!string.IsNullOrWhiteSpace(name))
        //     {
        //         query = query.Where(s =>
        //             s.Name.Contains(name));
        //     }

        //     ViewBag.SearchString = searchString;
        //     ViewBag.Course = course;
        //     ViewBag.Name = name;

        //     var students = await query.ToListAsync();

        //     return View(students);
        // }




        // Create form open karna


        public async Task<IActionResult> Index(
            string? searchString,
            string? course, string? name,
            int page = 1)
        {
            int pageSize = 5;

            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(s =>
                    s.Name.Contains(searchString) ||
                    s.Email.Contains(searchString));
            }

            if (!string.IsNullOrWhiteSpace(course))
            {
                query = query.Where(s => s.Course == course);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(s => s.Name.Contains(name));
            }

            int totalStudents = await query.CountAsync();

            var students = await query
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.Course = course;
            ViewBag.Name = name;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(
                totalStudents / (double)pageSize);

            return View(students);
        }


        public IActionResult Create()
        {
            return View();
        }

        // Form submit hone par student database mein save karna
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (string.IsNullOrWhiteSpace(student.Name))
                ModelState.AddModelError(nameof(student.Name), "Name cannot be blank.");
            if (string.IsNullOrWhiteSpace(student.Email))
                ModelState.AddModelError(nameof(student.Email), "Email cannot be blank.");
            if (string.IsNullOrWhiteSpace(student.Course))
                ModelState.AddModelError(nameof(student.Course), "Course cannot be blank.");

            if (!ModelState.IsValid)
            {
                return View(student);
            }

            student.Name = student.Name.Trim();
            student.Email = student.Email.Trim();
            student.Course = student.Course.Trim();
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Existing student ka data fetch karna
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // Updated student ko database mein save karna
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student updatedStudent)
        {
            if (id != updatedStudent.Id)
            {
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(updatedStudent.Name))
                ModelState.AddModelError(nameof(updatedStudent.Name), "Name cannot be blank.");
            if (string.IsNullOrWhiteSpace(updatedStudent.Email))
                ModelState.AddModelError(nameof(updatedStudent.Email), "Email cannot be blank.");
            if (string.IsNullOrWhiteSpace(updatedStudent.Course))
                ModelState.AddModelError(nameof(updatedStudent.Course), "Course cannot be blank.");

            if (!ModelState.IsValid)
            {
                return View(updatedStudent);
            }

            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            student.Name = updatedStudent.Name.Trim();
            student.Email = updatedStudent.Email.Trim();
            student.Course = updatedStudent.Course.Trim();

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Student ko database se delete karna
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
