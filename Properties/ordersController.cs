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
    public class ordersController : Controller
    {
        private readonly ApplicationDbContext _context; // Для взаимодействия с БД через Entity Framework Core

        // Констркутор
        public ordersController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Если объёкт не передан сразу ругаемся
        }

        // GET: orders
        // Страница Индекс
        public async Task<IActionResult> Index()
        {
            // Извлекаем заказы и связанные с ними книги и авторы
            var applicationDbContext = _context.orders.OrderBy(o => o.id).Include(o => o.Book).Include(o => o.Reader);

            // Делаем из этого список и передаём
            return View(await applicationDbContext.ToListAsync());
        }

        

        // GET: orders/Create
        public IActionResult Create()
        {

            // Получаем список читателей из базы данных
            List<readers> readers = _context.readers.ToList();
            // Создаём SelectList для выпадающего списка
            ViewBag.ReaderList = new SelectList(readers, "id", "FullName");

            // Получаем список книг из базы данных
            List<books> books = _context.books.ToList();
            // Создаём SelectList для выпадающего списка
            ViewBag.BookList = new SelectList(books, "id", "title");

            return View();
        }

        // POST: orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("readerid,bookid,orderdate,returndate")] orders order)
        {
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            // Если данные корректны
            if (ModelState.IsValid)
            {
                try
                {
                    // Добавляем новую запись в базу данных 
                    _context.Add(order);
                    // Если есть что сохранять
                    if (_context.ChangeTracker.HasChanges())
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
                            ModelState.AddModelError("", "Ошибка при сохранении заказа. Проверьте данные и попробуйте еще раз.");
                            return View(order);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Нет изменений для сохранения.");
                        return View(order);
                    }
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"DbUpdateException: {ex.Message}");
                    ModelState.AddModelError("", $"Ошибка при сохранении заказа: {ex.InnerException?.Message ?? ex.Message}");
                    return View(order);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    ModelState.AddModelError("", $"Произошла непредвиденная ошибка: {ex.Message}");
                    return View(order);
                }
            }
            else
            {
                foreach (var modelStateValue in ModelState.Values)
                {
                    foreach (var error in modelStateValue.Errors)
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                }
            }
            // Если что-то пошло не так, возвращаем ту же страницу

            // Заполняем выпадающие списки
            List<readers> readers = _context.readers.ToList();
            ViewBag.ReaderList = new SelectList(readers, "id", "FullName", order.readerid);

            List<books> books = _context.books.ToList();
            ViewBag.BookList = new SelectList(books, "id", "title", order.bookid);

            return View(order);
        }



        // GET: orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Если ID не был передан
            if (id == null)
            {
                return NotFound();
            }

            // Ищем заказ по ID
            var order = await _context.orders.FindAsync(id);
            // Проверяем, существует ли заказ
            if (order == null)
            {
                return NotFound();
            }

            
            // Получаем список читателей из базы данных
            List<readers> readers = _context.readers.ToList();
            // Создаём SelectList для выпадающего списка
            ViewBag.ReaderList = new SelectList(readers, "id", "FullName", order.readerid);

            // Получаем список книг из базы данных
            List<books> books = _context.books.ToList();
            // Создаём SelectList для выпадающего списка
            ViewBag.BookList = new SelectList(books, "id", "title", order.bookid);



            return View(order);
        }

        // POST: orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,readerid,bookid,orderdate,returndate")] orders order)
        {
            // Соответствует ли ID
            if (id != order.id)
            {
                return NotFound();
            }

            // Проверка корректности данных
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order); // Обновляем объект
                    await _context.SaveChangesAsync(); // Сохраняем изменения
                    return RedirectToAction(nameof(Index)); // Возвращаемся в Индекс
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine($"DbUpdateConcurrencyException: {ex.Message}");
                    // Если вдруг объект удалён
                    if (!OrderExists(order.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", $"Ошибка при сохранении заказа: {ex.InnerException?.Message ?? ex.Message}");
                        
                        List<readers> readers = _context.readers.ToList();
                        ViewBag.ReaderList = new SelectList(readers, "id", "FullName", order.readerid);

                        List<books> books = _context.books.ToList();
                        ViewBag.BookList = new SelectList(books, "id", "title", order.bookid);

                        return View(order);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    ModelState.AddModelError("", $"Произошла непредвиденная ошибка: {ex.Message}");
                    
                    List<readers> readers = _context.readers.ToList();
                    ViewBag.ReaderList = new SelectList(readers, "id", "FullName", order.readerid);

                    List<books> books = _context.books.ToList();
                    ViewBag.BookList = new SelectList(books, "id", "title", order.bookid);

                    return View(order);
                }
            }
            else
            // Если данные некорректны
            {
                foreach (var modelStateValue in ModelState.Values)
                {
                    foreach (var error in modelStateValue.Errors)
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                }
                
                List<readers> readers = _context.readers.ToList();
                ViewBag.ReaderList = new SelectList(readers, "id", "FullName", order.readerid);

                List<books> books = _context.books.ToList();
                ViewBag.BookList = new SelectList(books, "id", "title", order.bookid);

                return View(order);
            }
        }

        private bool OrderExists(int id)
        {
            return _context.orders.Any(e => e.id == id);
        }

        public IActionResult Delete()
        {
            return View();
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Ищем по ID
            var order = await _context.orders.FindAsync(id);
            if (order != null)
            {
                // Удаляем заказ
                _context.orders.Remove(order);
                await _context.SaveChangesAsync();// Сохраняем изменения
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ordersExists(int id)
        {
            return _context.orders.Any(e => e.id == id);
        }


       
    }
}
