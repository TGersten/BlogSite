using BlogPosts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Diagnostics;

namespace BlogPosts.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=BlogPost;Integrated Security=true;TrustServerCertificate=yes;";


        public IActionResult Index(string page)
        {
            HomeViewModel hvm = new();
            BlogPostDB db = new(_connectionString);
            if (page == null)
            {
                hvm.BlogPosts = db.GetThreeBlogPosts(1);
            }
            else
            {
                hvm.BlogPosts = db.GetThreeBlogPosts(int.Parse(page));

            }
            foreach (BlogPost b in hvm.BlogPosts)
            {
                if (b.Text.Length > 200)
                {
                    b.Text = b.Text.Substring(0, 200) + "...";

                }

            }
            return View(hvm);
        }

        public IActionResult ViewBlog(int Id)
        {
            BlogPostDB db = new(_connectionString);
            ViewBlogModel vbm = new();

            vbm.Post = db.GetPostById(Id);
            vbm.Post.Comments = db.GetCommntsForBlogPost(Id);
            vbm.SplitText = vbm.Post.Text.Split("\n");

            if (vbm.Post == null)
            {
                return Redirect("/home/index");

            }

            return View(vbm);
        }


        [HttpPost]
        public IActionResult AddComment(Comment comment)
        {
            Response.Cookies.Append("commneterName", comment.Name, new CookieOptions
            {
                Expires = new DateTimeOffset(DateTime.Today.AddDays(7))
            });

            var db = new BlogPostDB(_connectionString);
            db.AddComment(comment);
            return Redirect($"/home/ViewBlog?id={comment.PostId}");
        }
    }
}


