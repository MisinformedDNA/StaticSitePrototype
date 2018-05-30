using Markdig;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace StaticSitePrototype.WyamCore
{
    public class BlogRecipe : IWyamRecipe
    {
        public BlogRecipe()
        {

        }

        public BlogRecipe(IConfiguration configuration)
        {
            PostsFolder = configuration["PostsFolder"] ?? PostsFolder;
        }

        public string PostsFolder { get; set; } = "posts";
        public string OutputFolder { get; set; } = "output";

        public async Task InvokeAsync()
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

            var layout = File.ReadAllText("Layouts/_Layout.cshtml");

            Directory.CreateDirectory($"output/{PostsFolder}");
            var directory = Directory.CreateDirectory(PostsFolder);
            var mdFiles = directory.EnumerateFiles("*.md");
            foreach (var mdFile in mdFiles)
            {
                using (StreamReader streamReader = mdFile.OpenText())
                {
                    var md = await streamReader.ReadToEndAsync();
                    string html = Markdown.ToHtml(md, pipeline);

                    layout.Replace("@RenderBody()", layout);

                    using (StreamWriter streamWriter = File.CreateText($"output/{PostsFolder}/{mdFile.Name}.html"))
                    {
                        await streamWriter.WriteAsync(html);
                    }
                }


            }
        }
    }
}
