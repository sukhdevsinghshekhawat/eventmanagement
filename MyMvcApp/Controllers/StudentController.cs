using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Models;

namespace MyMvcApp.Controllers;

[Authorize(Roles = "Admin")]
public sealed class StudentController(ApplicationDbContext context) : Controller
{
    private const int PageSize = 5;

    public async Task<IActionResult> Index(string? searchString, string? course, string? name, int page = 1)
    {
        page = Math.Max(page, 1);
        var query = context.Students.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
            query = query.Where(student => student.Name.Contains(searchString) || student.Email.Contains(searchString));
        if (!string.IsNullOrWhiteSpace(course))
            query = query.Where(student => student.Course == course);
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(student => student.Name.Contains(name));

        var totalStudents = await query.CountAsync();
        var students = await query.OrderBy(student => student.Id)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewBag.SearchString = searchString;
        ViewBag.Course = course;
        ViewBag.Name = name;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalStudents / (double)PageSize);
        return View(students);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Student student)
    {
        ValidateStudent(student);
        if (!ModelState.IsValid) return View(student);

        NormalizeStudent(student);
        context.Students.Add(student);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var student = await context.Students.FindAsync(id);
        return student is null ? NotFound() : View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Student updatedStudent)
    {
        if (id != updatedStudent.Id) return BadRequest();

        ValidateStudent(updatedStudent);
        if (!ModelState.IsValid) return View(updatedStudent);

        var student = await context.Students.FindAsync(id);
        if (student is null) return NotFound();

        student.Name = updatedStudent.Name.Trim();
        student.Email = updatedStudent.Email.Trim();
        student.Course = updatedStudent.Course.Trim();
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await context.Students.FindAsync(id);
        if (student is null) return NotFound();

        context.Students.Remove(student);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private void ValidateStudent(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.Name))
            ModelState.AddModelError(nameof(student.Name), "Name cannot be blank.");
        if (string.IsNullOrWhiteSpace(student.Email))
            ModelState.AddModelError(nameof(student.Email), "Email cannot be blank.");
        if (string.IsNullOrWhiteSpace(student.Course))
            ModelState.AddModelError(nameof(student.Course), "Course cannot be blank.");
    }

    private static void NormalizeStudent(Student student)
    {
        student.Name = student.Name.Trim();
        student.Email = student.Email.Trim();
        student.Course = student.Course.Trim();
    }
}
