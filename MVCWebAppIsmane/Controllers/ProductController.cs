﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCWebAppIsmane.Data;
using MVCWebAppIsmane.Models;

namespace MVCWebAppIsmane.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var products = _dataContext.Products.ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            var categories = _dataContext.Categories.ToList(); // Replace with your data retrieval logic.

            ViewBag.Categories = new SelectList(categories, "Id", "Name"); // Populate ViewBag with the list of categories.
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model, IFormFile posterFile)
        {
            if (ModelState.IsValid)
            {
                if (posterFile != null && posterFile.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + posterFile.FileName;
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "pics");
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await posterFile.CopyToAsync(fileStream);
                    }   

                    model.Poster = "pics/" + uniqueFileName;
                }

                _dataContext.Products.Add(model);
                _dataContext.SaveChanges();
                return RedirectToAction("Index");

                //whateer you want
            }

            return View(model);        
        }




        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var categories = _dataContext.Categories.ToList(); // Replace with your data retrieval logic.


            Product? product = _dataContext.Products.FirstOrDefault(u => u.Id == Id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product model, IFormFile posterFile)
        {
            if (ModelState.IsValid)
            {
                Product? existingProduct = _dataContext.Products.FirstOrDefault(u => u.Id == model.Id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                if (posterFile != null && posterFile.Length > 0)
                {
                    // A new image has been uploaded, so proceed with the new image.
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + posterFile.FileName;
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "pics");
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await posterFile.CopyToAsync(fileStream);
                    }

                    // Remove the old image (if it exists) and update with the new one.
                    if (!string.IsNullOrEmpty(existingProduct.Poster))
                    {
                        string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduct.Poster);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    model.Poster = "pics/" + uniqueFileName;
                }
                else
                {
                    // No new image uploaded, retain the existing image.
                    model.Poster = existingProduct.Poster;
                }

                // Update other properties and save the product.
                existingProduct.Name = model.Name;
                existingProduct.Description = model.Description;
                existingProduct.Price = model.Price;
                existingProduct.IdCategory = model.IdCategory;
                existingProduct.Poster = model.Poster;

                _dataContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }






    }
}
