using System;
using System.CodeDom;
using System.IO;
using CookComputing.XmlRpc;

namespace GOS.MetaWeblogToPretzelMardown
{
    public class MetaWeblogToPretzelMardownConverter
    {
        private const string _outPath = @"C:\Temp\Blog\";
        private const string _appKey = "apkeyifneeded";
        private const string _url = @"www.webiste.com/blogs/metablog.ashx";
        private const string _user = "username";
        private const string _password = "password";

        readonly ICSMetaWeblog _proxy;

        public MetaWeblogToPretzelMardownConverter()
        {
            _proxy = XmlRpcProxyGen.Create<ICSMetaWeblog>();
            _proxy.Url = _url;
        }

        public void ExportAllBlogs()
        {
            BlogInfo[] blogs;
            try
            {
                blogs = _proxy.getUsersBlogs(_appKey, _user, _password);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return;
            }
            
            foreach (var blog in blogs)
            {
                WritePosts(blog);
            }
        }

        private void WritePosts(BlogInfo blog)
        {
            var posts = _proxy
                .getRecentPosts(blog.blogid, _user, _password, 1000);
            foreach (var post in posts)
            {
                WriteSinglePost(post);
            }
        }

        private void WriteSinglePost(Post post)
        {
            var fileTitle = ParseFileNameString(post.title);


            var path = Path.Combine(_outPath, "_posts", String.Format("{0:yyyy-MM-dd}-{1}.md", post.dateCreated, fileTitle));
            using (var fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLine("---");
                    sw.WriteLine("layout: post");
                    sw.WriteLine("title: \"{0}\"", post.title);
                    sw.WriteLine("author: {0}", "Jürgen Gutsch");
                    sw.WriteLine("comments: true");
                    sw.WriteLine("teaser: ");
                    sw.WriteLine("Tags: ");
                    if (post.categories != null)
                    {
                        foreach (var tag in post.categories)
                        {
                            var cleanedTag = ParseFileNameString(tag);
                            WriteTag(cleanedTag);
                            sw.WriteLine("    - {0}", cleanedTag);
                        }
                    }
                    sw.WriteLine("date: {0:yyyy-MM-dd}", post.dateCreated);
                    sw.WriteLine("---");
                    sw.WriteLine();
                    sw.Write(post.description);
                }
            }
        }

        private static string ParseFileNameString(string fileName)
        {
            var fileTitle = fileName.ToLower()
                .Replace(" ", "-")
                .Replace("--", "-") // cleanup dashes
                .Replace("--", "-") // cleanup dashes
                .Replace("\"", "")
                .Replace("“", "")
                .Replace("”", "")
                .Replace(",", "")
                .Replace(".", "")
                .Replace("…", "")
                .Replace(":", "")
                .Replace(";", "")
                .Replace("?", "")
                .Replace("!", "")
                .Replace("‘", "")
                .Replace("'", "")
                .Replace("/", "")
                .Replace(")", "")
                .Replace("(", "")
                .Replace("*", "")
                .Replace("ü", "ue")
                .Replace("ö", "oe")
                .Replace("ä", "ae")
                .Replace("&#224", "a")
                .Replace("&#252", "ue")
                .Replace("&#223", "ss")
                .Replace("&#246", "oe")
                .Replace("&#228", "ae")
                .Replace("&#220", "ue")
                .Replace("&amp", "und")
                .Replace("&lt", "")
                .Replace("&gt", "")
                .Replace("&quot", "")
                .Replace("#", "sharp")
                .Replace("–", "") // thats a long dash
                .Replace("--", "-") // cleanup dashes
                .Replace("--", "-") // cleanup dashes
                .Trim('-') // cleanup dashes
                .Trim();
            return fileTitle;
        }

        private void WriteTag(string tag)
        {
            var path = Path.Combine(_outPath, "tag", String.Format("{0}.md", tag));
            if (File.Exists(path))
            {
                return;
            }

            using (var fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(_tagTemplate.Replace("%%TAG%%", tag));
                }
            }
        }

        private const string _tagTemplate = @"---
layout : layout
title : %%TAG%%
---

<ul class=""posts"">
    {% for post in site.posts %}
        {% for tag in post.tags %}
            {% if tag == page.title %}
		        <li>
			        <div class=""idea"">				
				        <h2><a class=""postlink"" href=""{{ post.url }}"">{{ post.title }}</a></h2>
				        <div class=""postdate"">{{ post.date | date: ""%e %B, %Y""  }}
					        <ul>
					        {% for tag in post.tags %}
						        <li><a href = ""/tag/{{ tag }}.html"" >{{ tag }}</a></li>
					        {% endfor %}
					        </ul>
				        </div>
				        {{ post.content }}					
				        <a href = ""{{ post.url }}#disqus_thread"" > Comments </ a >
			        </div>
		        </li>
            {% endif %}
        {% endfor %}
    {% endfor %}
</ul>
";
    }
}
