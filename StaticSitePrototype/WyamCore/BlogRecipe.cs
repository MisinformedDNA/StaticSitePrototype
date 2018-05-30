using Markdig;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
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
            PostsDirectory = configuration["PostsDirectory"] ?? PostsDirectory;
        }

        public string PostsDirectory { get; set; } = "posts";
        public string OutputDirectory { get; set; } = "output";

        public async Task InvokeAsync()
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

            var layout = File.ReadAllText("Layouts/_Layout.cshtml");

            string outputPostsDirectory = Path.Combine(OutputDirectory, PostsDirectory);
            Directory.CreateDirectory(outputPostsDirectory);

            var inputDirectory = new DirectoryInfo(PostsDirectory);
            if (!inputDirectory.Exists)
                return;

            var htmlTasks = inputDirectory
                .EnumerateFiles("*.md")
                .Select(f => TransformPost(f, layout, pipeline, outputPostsDirectory));
            await Task.WhenAll(htmlTasks);
        }

        private static async Task TransformPost(FileInfo inputFile, string layout, MarkdownPipeline pipeline, string outputPostsDirectory)
        {
            string md;
            using (StreamReader streamReader = inputFile.OpenText())
            {
                md = await streamReader.ReadToEndAsync();
            }

            var fileName = Path.GetFileNameWithoutExtension(inputFile.Name);
            var outputFullName = Path.Combine(outputPostsDirectory, $"{fileName}.html");

            string html = Markdown.ToHtml(md, pipeline);
            var fullHtml = layout.Replace("@RenderBody()", html);

            using (StreamWriter streamWriter = File.CreateText(outputFullName))
            {
                await streamWriter.WriteAsync(fullHtml);
            }
        }
    }
}
