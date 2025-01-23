using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Properties
{
    public class authorsController : Controller
    {
        private readonly ApplicationDbContext _context; // Для взаимодействия с БД через Entity Framework Core

        // Конструктор
        public authorsController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Если объёкт не передан сразу ругаемся
        }





        // GET: authors
        // Страница Индекс
        public async Task<IActionResult> Index()
        {
            // Получаем список авторов
            var authors = _context.authors.OrderBy(a => a.id).ToList();

            // Отправляем авторов на страницу
            return View(authors);
        }



        // GET: authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("firstname,lastname,middlename,birthdate")] authors author)
        {
            // Если данные корректны
            if (ModelState.IsValid)
            {
                try
                {
                    // Добавляем новую запись в базу данных
                    _context.Add(author);
                    await _context.SaveChangesAsync();

                    // После успешного добавления перенаправляем на главную страницу или список
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Показываем ошибку если вдруг что
                    ModelState.AddModelError("", $"Ошибка добавления записи: {ex.Message}");
                }
            }

            // Если что-то пошло не так, возвращаем ту же страницу с ошибками
            return View(author);
        }

        // GET: authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Если ID не был передан
            if (id == null)
            {
                return NotFound();
            }

            // Ищем автора по ID
            var author = await _context.authors.FindAsync(id);
            // Проверяем, существует ли автор
            if (author == null)
            {
                return NotFound();
            }

            // Показываем
            return View(author);
        }

        // POST: authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,firstname,lastname,middlename,birthdate")] authors author)
        {
            // Соответствует ли ID
            if (id != author.id)
            {
                return NotFound();
            }

            // Проверка корректности данных
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(author); // Обновляем существующего автора
                    await _context.SaveChangesAsync(); // Сохраняем изменения
                    return RedirectToAction(nameof(Index)); // Возвращаемся в Индекс
                }
                catch (DbUpdateConcurrencyException)
                {
                    Console.WriteLine("Ошибка при сохранении изменений.");
                    // Если вдруг объект удалён
                    if (!AuthorExists(author.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            // Если данные некорректны
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Ошибка: {error.ErrorMessage}");
                    }
                }
            }
            return View(author);
        }

        private bool AuthorExists(int id)
        {
            return _context.authors.Any(e => e.id == id);
        }

        // GET: authors/Delete/5
        public IActionResult Delete()
        {
            return View();
        }

        // POST: authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Ищем по ID
            var author = await _context.authors.FindAsync(id);
            if (author != null)
            {
                // Ищем все книги автора и удаляем с ним
                var books = _context.books.Where(b => b.authorid == id);
                _context.books.RemoveRange(books); 
                _context.authors.Remove(author);
            }

            await _context.SaveChangesAsync(); // Сохраняем изменения
            return RedirectToAction(nameof(Index));
        }
    

        private bool authorsExists(int id)
        {
            return _context.authors.Any(e => e.id == id);
        }
    }
}
