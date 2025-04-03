using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;

namespace BlogPosts.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CommentText { get; set; }
        public DateTime Date { get; set; }
        public int PostId { get; set; }
    }
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }
        public DateTime DatePosted { get; set; }


        public List<Comment> Comments { get; set; }
    }
    public class BlogPostDB
    {
        private readonly string _connectionString;

        public BlogPostDB(string connectionString)
        {
            _connectionString = connectionString;

        }

        public List<BlogPost> GetThreeBlogPosts(int page)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            if (page == 1)
            {
                cmd.CommandText = "SELECT TOP 3 * FROM Posts";
            }
            else
            {
                cmd.CommandText = @"SELECT * FROM Posts ORDER BY Date DESC OFFSET @amount ROWS
FETCH NEXT 3 ROWS ONLY";
                cmd.Parameters.AddWithValue("@amount", (page - 1) * 3);
            }


            connection.Open();
            List<BlogPost> blogPosts = new();

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                blogPosts.Add(new()
                {
                    Id = (int)reader["Id"],
                    Title = (string)reader["Title"],
                    Text = (string)reader["Text"],
                    Name = (string)reader["Name"],
                    DatePosted = (DateTime)reader["DatePosted"],


                });
            }

            return blogPosts;

        }

        public BlogPost GetPostById(int Id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Posts WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", Id);
            connection.Open();

            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            BlogPost blogPost = new()
            {
                Id = (int)reader["Id"],
                Title = (string)reader["Title"],
                Text = (string)reader["Text"],
                Name = (string)reader["Name"],
                DatePosted = (DateTime)reader["DatePosted"],
            };

            return blogPost;

        }

        public List<Comment> GetCommntsForBlogPost(int Id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Comments WHERE PostId = @id";
            cmd.Parameters.AddWithValue("@id", Id);
            connection.Open();

            var reader = cmd.ExecuteReader();
         
            List<Comment> comments = new();
            while (reader.Read())
            {
                comments.Add(new()
                {

                    Name = (string)reader["Name"],
                    CommentText = (string)reader["CommentText"],
                    Date = (DateTime)reader["DateCommented"],
                });
            }
            return comments;
        }

        public void AddComment(Comment comment)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Comments(Name, CommentText, DateCommented,PostId) 
VALUES(@name,@commentText,@date, @Id) 
";

            cmd.Parameters.AddWithValue("@Name", comment.Name);
            cmd.Parameters.AddWithValue("@commentText", comment.CommentText);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.Parameters.AddWithValue("@Id", comment.PostId);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}

