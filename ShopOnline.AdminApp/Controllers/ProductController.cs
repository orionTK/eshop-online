﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using ShopOnline.AdminApp.Services;
using ShopOnline.Utilies.Constants;
using ShopOnline.ViewModel.Catalog.Products;
using ShopOnline.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopOnline.AdminApp.Controllers
{
    [Authorize]

    public class ProductController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IProductApiClient _productApiClient;
        private readonly ICategoryApiClient _categoryApiClient;
        public ProductController(IConfiguration configuration, IProductApiClient productApiClient, ICategoryApiClient categoryApiClient)
        {
            _configuration = configuration;
            _productApiClient = productApiClient;
            _categoryApiClient = categoryApiClient;
        }
        public async Task<IActionResult> Index(string keyword, int? categoryId, int pageIndex = 1, int pageSize = 5)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);
            //var session = HttpContext.Session.GetString(SystemConstants.AppSettings.Token);
            var rq = new GetManageProductPagingRequest()
            {
                //BearerToken = session,
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                LanguageId = languageId,
                CategoryId = categoryId
            };
            var data = await _productApiClient.GetProductsPaging(rq);
            ViewBag.Keyword = keyword;
            var categories = await _categoryApiClient.GetAll(languageId);
            
            ViewBag.Categories = categories.Select(x => new SelectListItem()
            {
                Text = x.CategoryName,
                Value = x.Id.ToString(),
                Selected = categoryId.HasValue && categoryId.Value == x.Id
            });
           
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data.ResultObj);
        }

        [HttpGet]
        public IActionResult Create()
        {
           return View();
        }

       

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {

           if(!ModelState.IsValid)
                return View();

            var result = await _productApiClient.CreateProduct(request);
            if (result)
            {
                TempData["result"] = "Thêm mới sản phẩm dùng thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Thêm sản phẩm thất bại");
            return View(request);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)  
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);
            var result = await _productApiClient.GetById(id, languageId);
            if (result.IsSuccessed)
            {
                var product = result.ResultObj;
                var updateRequest = new ProductUpdateRequest()
                {
                    ProductId = id,
                    Description = product.Description,
                    Details = product.Details,
                    ProductName = product.ProductName,
                    SeoAlias = product.SeoAlias,
                    SeoDescription = product.SeoDescription,
                    SeoTitle = product.SeoTitle,
                    Price = product.Price,
                    OriginalPrice = product.OriginalPrice,
                    Stock = product.Stock
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Edit([FromForm] ProductUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _productApiClient.UpdateProduct(request);
            if (result.IsSuccessed)
            {
                TempData["result"] = "Cập nhật sản phẩm dùng thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Cập nhật sản phẩm thất bại");
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return View(new ProductDeleteRequest()
            {
                Id = id
            });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);
            var result = await _productApiClient.GetById(id, languageId);
            if (result.IsSuccessed)
            {
                var product = result.ResultObj;
                var p = new ProductViewModel()
                {
                    ProductId = product.ProductId,
                    OriginalPrice = product.OriginalPrice,
                    Price = product.Price,
                    Stock = product.Stock,
                    ViewCount = product.ViewCount,
                   
                    Description = product.Description,
                    Details = product.Details,
                    ProductName = product.ProductName,
                    SeoAlias = product.SeoAlias,
                    SeoDescription = product.SeoDescription,
                    SeoTitle = product.SeoTitle,
                    DateCreated = product.DateCreated,
                    ThumbnailImage = product.ThumbnailImage

                };
                return View(p);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ProductDeleteRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _productApiClient.DeleteProduct(request.Id);
            if (result.IsSuccessed)
            {
                TempData["result"] = "Xóa sản phẩm thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Xóa không thành công");
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> CategoryAssign(int id)
        {
            var categoriesRq = await GetCategoryAssignRequest(id);
            return View(categoriesRq);
        }

        [HttpPost]
        public async Task<IActionResult> CategoryAssign([FromForm] CategoryAssignRequest request)
        {

            if (!ModelState.IsValid)
                return View();

            var result = await _productApiClient.CategoryAssign(request.Id, request);

            if (result.IsSuccessed)
            {
                TempData["result"] = "Cập nhật danh mục thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            var roleAssignRequest = await GetCategoryAssignRequest(request.Id);
            return View(roleAssignRequest);
        }


        private async Task<CategoryAssignRequest> GetCategoryAssignRequest(int id)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);
            var product = await _productApiClient.GetById(id, languageId);
            var categories = await _categoryApiClient.GetAll(languageId);
            var categoryRq = new CategoryAssignRequest();
            foreach (var categoryName in categories)
            {
                categoryRq.Categories.Add(new SelectItem()
                {
                    Id = categoryName.Id.ToString(),
                    Name = categoryName.CategoryName,
                    Selected = product.ResultObj.Categories.Contains(categoryName.CategoryName)
                });
            }
            return categoryRq;

        }


    }
}
