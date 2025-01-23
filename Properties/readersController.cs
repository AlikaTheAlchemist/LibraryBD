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
    public class readersController : Controller
    {
        private readonly ApplicationDbContext _context; // Для взаимодействия с БД через Entity Framework Core

        // Конструктор
        public readersController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Если объёкт не передан сразу ругаемся
        }

        // GET: readers
        // Страница Индекс
        public async Task<IActionResult> Index()
        {
            // Извлекаем читателей и отправляем на страницу, но в одну строчку и с асинхроном
            return View(await _context.readers.OrderBy(r => r.id).ToListAsync());
        }

        

        // GET: readers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: readers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("firstname,lastname,middlename,birthdate,registrationdate")] readers reader)
        {
            // Если данные корректны
            if (ModelState.IsValid)
            {
                try
                {
                    // Добавляем новую запись в базу данных
                    _context.Add(reader);
                    await _context.SaveChangesAsync();

                    // После успешного добавления перенаправляем на главную страницу или список
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Показываем ошибку
                    ModelState.AddModelError("", $"Ошибка добавления записи: {ex.Message}");
                }
            }

            // Данные некорректны, выводим что именно не так
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
            }

            // Если что-то пошло не так, возвращаем ту же страницу с ошибками
            return View(reader);
        }

        // GET: readers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Если ID не был передан
            if (id == null)
            {
                return NotFound();
            }

            // Ищем читателя по ID
            var reader = await _context.readers.FindAsync(id);
            // Проверяем, существует ли ачитатель
            if (reader == null)
            {
                return NotFound();
            }
            return View(reader);
        }

        // POST: readers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,firstname,lastname,middlename,birthdate,registrationdate")] readers reader)
        {
            // Соответствует ли ID
            if (id != reader.id)
            {
                return NotFound();
            }

            // Проверка корректности данных
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reader);// Обновляем объект
                    await _context.SaveChangesAsync(); // Сохраняем изменения
                    return RedirectToAction(nameof(Index)); // Возвращаемся в Индекс
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Если вдруг объект удалён
                    if (!_context.readers.Any(e => e.id == reader.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(reader);
        }

        // GET: readers/Delete/5
        public IActionResult Delete()
        {
            return View();
        }

        // POST: readers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Ищем по ID
            var reader = await _context.readers.FindAsync(id);
            if (reader != null)
            {
                // Ищем все заказы читателя и удаляем с ним
                var orders = _context.orders.Where(b => b.readerid == id);
                _context.orders.RemoveRange(orders);
                _context.readers.Remove(reader);
            }
            await _context.SaveChangesAsync(); // Сохраняем изменения
            return RedirectToAction(nameof(Index));
        }

        private bool readersExists(int id)
        {
            return _context.readers.Any(e => e.id == id);
        }


       
    }
}
