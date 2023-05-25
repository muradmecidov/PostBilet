using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using WebApplication2.Areas.Admin.ViewModels;
using WebApplication2.DAL;
using WebApplication2.Models;
using WebApplication2.Utilities.Constants;
using WebApplication2.Utilities.Extensions;

namespace WebApplication2.Areas.Admin.Controllers
{
        [Area("Admin")]
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

		public PostController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}

		public async Task<IActionResult> Index()
        {
            List<Post> posts= await _context.Posts.Where(p=>!p.IsDeleted).OrderByDescending(p=>p.Id).ToListAsync();
            return View(posts);
        }

        public  IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostVM model)
        {
            if (!ModelState.IsValid) return View(model);
            if (!model.Photo.CheckContentType("image/"))
            {
                ModelState.AddModelError("Photo", ErrorMessages.FileMustBeImageTaype);
                return View(model);
            }
			if (!model.Photo.CheckFileSize(200))
			{
				ModelState.AddModelError("Photo", ErrorMessages.FileMustBeImageSize);
				return View(model);

			}
            string rootpath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images");
            string filename= await model.Photo.SaveAsync(rootpath);
            Post post = new Post()
            {
                Title = model.Title,
                Description = model.Description,
                ImagePath = filename,
            };

			await _context.Posts.AddAsync(post);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));

		}





		public async Task<IActionResult> Update(int id)
		{
			Post post = await _context.Posts.FindAsync(id);
			if (post == null) { return NotFound(); }
			UpdatePostVM updatePostVM = new UpdatePostVM()
			{
				Description=post.Description,
				Title=post.Title,
				Id=id
			};
			return View(updatePostVM);

		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(UpdatePostVM model)
		{
			if (ModelState.IsValid) return View(model);
            if (model.Photo.CheckContentType("image/"))
            {
                ModelState.AddModelError("Photo", ErrorMessages.FileMustBeImageTaype);
                return View(model);
            }
            if (model.Photo.CheckFileSize(200))
            {
                ModelState.AddModelError("Photo", ErrorMessages.FileMustBeImageSize);
                return View(model);
            }
			string rootpath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images");
			string filename = (await _context.Posts.FindAsync(model.Id))?.ImagePath;
			string filepath = Path.Combine(rootpath,filename);
			if (System.IO.File.Exists(filepath))
			{
				System.IO.File.Delete(filepath);
			}
            await model.Photo.SaveAsync(rootpath);


			string rootpath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images");
            string filename = await model.Photo.SaveAsync(rootpath);
            Post post = new Post()
            {
                Title = model.Title,
                Description = model.Description,
                ImagePath = filename,
            };








            //Post result = await _context.Posts.FirstOrDefaultAsync(t => t.Id == model.Id);
            //if (result is null)
            //{
            //    TempData["Exists"] = "Bu Post bazada yoxdur";
            //    return RedirectToAction(nameof(Index));
            //}
            //result.Title = model.Title;
            //result.Description = model.Description;
            //result.ImagePath = model.Photo;
            //await _context.SaveChangesAsync();


            return View(model);

		}


		public async Task<IActionResult> Delete(int id)
		{
			Post post = await _context.Posts.FindAsync(id);
			if (post == null) { return NotFound(); }
			string filepath = Path.Combine( _webHostEnvironment.WebRootPath,"asstes","images",post.ImagePath);
			if (System.IO.File.Exists(filepath))
			{
				System.IO.File.Delete(filepath);
			}
			_context.Posts.Remove(post);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));

		}



	}
}
