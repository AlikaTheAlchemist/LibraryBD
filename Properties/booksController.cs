using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Properties
{
    public class booksController : Controller
    {
        private readonly ApplicationDbContext _context; // Для взаимодействия с БД через Entity Framework Core

        // Конструктор
        public booksController(ApplicationDbContext context) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Если объёкт не передан сразу ругаемся
        }

        // GET: books
        // Страница Индекс
        public IActionResult Index()
        {
            // Получаем все книги из базы данных
            var books = _context.books.OrderBy(b => b.id).Include(b => b.author).ToList();

            // Передаем список книг
            return View(books);
        }

        
        // GET: books/Create
        public IActionResult Create()
        {
            // Получаем список авторов из базы данных
            List<authors> authors = _context.authors.ToList();
            // Создаём SelectList для выпадающего списка
            ViewBag.AuthorList = new SelectList(authors, "id", "FullName");

            return View();
        }



        // POST: books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("title,publicationyear,authorid")] books books)
        {
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            // Если данные корректны
            if (ModelState.IsValid)
            {
                
                if (string.IsNullOrWhiteSpace(books.title))
                {
                    ModelState.AddModelError("title", "Поле Название не может быть пустым.");
                    return View(books);
                }

                try
                {
                    // Добавляем новую запись в базу данных 
                    _context.Add(books);

                    if (_context.ChangeTracker.HasChanges()) // Если есть что сохранять
                    {
                        // Количество затронутых строк (изменений)
                        int affectedRows = await _context.SaveChangesAsync();
                        Console.WriteLine($"Rows affected: {affectedRows}");

                        // Если изменения есть возвращаемся в индекс
                        if (affectedRows > 0)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ModelState.AddModelError("", "Ошибка при сохранении книги. Проверьте данные и попробуйте еще раз.");
                            return View(books);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Нет изменений для сохранения.");
                        return View(books);
                    }
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"DbUpdateException: {ex.Message}");
                    ModelState.AddModelError("", $"Ошибка при сохранении книги: {ex.InnerException?.Message ?? ex.Message}");
                    return View(books);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    ModelState.AddModelError("", $"Произошла непредвиденная ошибка: {ex.Message}");
                    return View(books);
                }
            }
            else
            {
                // Если данные некорректны выводим в консоль все ошибки
                foreach (var modelStateValue in ModelState.Values)
                {
                    foreach (var error in modelStateValue.Errors)
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                }
            }
            return View(books);
        }




        // GET: books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Если ID не был передан
            if (id == null)
            {
                return NotFound();
            }

            // Ищем книгу по ID
            var book = await _context.books.FindAsync(id);
            // Проверяем, существует ли книга
            if (book == null)
            {
                return NotFound();
            }

            // Подгружаем список авторов для выпадающего списка
            ViewData["AuthorId"] = new SelectList(_context.authors, "id", "FullName", book.authorid);
            return View(book);
        }

        // POST: books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,title,publicationyear,authorid")] books book)
        {
            // Соответствует ли ID
            if (id != book.id)
            {
                return NotFound();
            }

            // Проверка корректности данных
            if (ModelState.IsValid)
            {
                try
                {
                    // Обновляем объект
                    _context.Update(book);
                    await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных

                    _context.Attach(book);
                    _context.Update(book);

                    Console.WriteLine("Изменения сохранены успешно."); // Отладочное сообщение

                }
                catch (DbUpdateConcurrencyException)
                {
                    Console.WriteLine("Ошибка при сохранении изменений.");
                    // Если вдруг объект удалён
                    if (!_context.books.Any(e => e.id == book.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // Возвращаемся в Индекс
                return RedirectToAction(nameof(Index));
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

            Console.WriteLine("ModelState недействителен.");
            return View(book);
        }




        private bool BookExists(int id)
        {
            return _context.books.Any(e => e.id == id);
        }



        // GET: books/Delete/5
        public IActionResult Delete()
        {
            return View();
        }


        // POST: books/DeleteById
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Ищем по ID
            var book = await _context.books.FindAsync(id);
            if (book != null)
            {
                // Ищем заказы связанные с этой книгой и удаляем
                var orders = _context.orders.Where(o => o.bookid == id);
                _context.orders.RemoveRange(orders);
                _context.books.Remove(book);
            }
            await _context.SaveChangesAsync();// Сохраняем изменения
            return RedirectToAction(nameof(Index));
        }




        private bool booksExists(int id)
        {
            return _context.books.Any(e => e.id == id);
        }
    }
}
