using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bidding.API.Models;
using Bidding.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bidding.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[AuthorizePermission]
    public class CategoryController : Controller
    {
        private readonly CategoryService categoryService;

        public CategoryController(CategoryService service)
        {
            categoryService = service;
        }
        [AuthorizePermission]
        [HttpGet]
        public ActionResult<Category> Get()
        {
            var categoryData = categoryService.Get();
            return Json(new { categoryData });
        }

        [Route("controller/{id}")]
        [HttpGet("{id:length(24)}", Name = "GetCategory")]
        public ActionResult<Category> Get(string id)
        {
            var category = categoryService.Get(id);

            if (category == null)
            {
                return NotFound();
            }
            
            return Json(new { category });
        }
        [AuthorizePermission]
        [Route("GetSubcategory/{categoryId}")]
        public ActionResult GetSubcategory(string categoryId)
        {
            var subcategory = categoryService.GetSubcategory(categoryId);
            return Json(new { subcategory });
        }

        //To list types corresponding to a particular category and sub-category
        [AuthorizePermission]
        [Route("GetTypes/{categoryId}/{subcategoryId}")]
        public ActionResult GetTypes(string categoryId, string subcategoryId)
        {
            var types = categoryService.GetTypes(categoryId, subcategoryId);
            return Json(new { types });
        }

        //To return the list of attributes based on the category, sub-category and type
        [AuthorizePermission]
        [Route("GetAttributes/{categoryId}/{subcategoryId}/{typeId}")]
        public ActionResult<Dictionary<string, List<Models.Attribute>>> GetAttributes(string categoryId, string subcategoryId, string typeId)
        {
            return categoryService.GetAttributes(categoryId, subcategoryId, typeId);
            //var attributes = categoryService.GetAttributes(categoryId, subcategoryId, typeId);
            //return Json(new { attributes });
        }

        //To return the list of rating fields based on the category, sub-category and type
        [Route("GetRating/{categoryId}/{subcategoryId}/{typeId}")]
        public ActionResult<Dictionary<string, List<RatingField>>> GetRating(string categoryId, string subcategoryId, string typeId)
        {
            return categoryService.GetRating(categoryId, subcategoryId, typeId);
        }

        [HttpPost]
        public ActionResult<Category> Create(Category category)
        {
            categoryService.Create(category);
            return CreatedAtRoute("GetCategory", new { id = category.Id.ToString() }, category);
        }

        [HttpPost("EditCategory")]
        public ActionResult<Category> EditCategory(Category model)
        {
            var category = categoryService.Edit(model);
            return category;
        }

        [Route("controller/{id}")]
        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Category category)
        {
            if (categoryService.Get(id) == null)
            {
                return NotFound();
            }
            categoryService.Update(id, category);
            return NoContent();
        }

        [Route("controller/{id}")]
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var category = categoryService.Get(id);
            if (category == null)
            {
                return NotFound();
            }
            categoryService.Remove(category.Id);
            return NoContent();
        }
    }
}
