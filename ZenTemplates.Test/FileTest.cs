using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ZenTemplates.Configuration;
using ZenTemplates.Parser;

namespace ZenTemplates.Test
{
	[TestClass]
	public class FileTest
	{
		private static FileRepository GetFileRepository()
		{
			ZenTemplatesConfiguration config = new ZenTemplatesConfiguration()
			{
				TemplateRoot = "../../Templates/Sample",
				TemplateFileExtension = ".html",
			};

			return new FileRepository(config);
		}

		[TestMethod]
		public void TemplateLookupTest()
		{
			FileRepository repo = GetFileRepository();
			FileInfo file;

			file = repo.GetTemplateFile("simple");
			Assert.IsNotNull(file);

			file = repo.GetTemplateFile("nonexistant");
			Assert.IsNull(file);

			file = repo.GetTemplateFile("Index", "Controller");
			Assert.IsNotNull(file);

			file = repo.GetTemplateFile("simple", "Controller");
			Assert.IsNotNull(file);
		}

		[TestMethod]
		public void SimpleFileTest()
		{
			FileRepository repo = GetFileRepository();

			FileInfo file = repo.GetTemplateFile("simple");
			Assert.IsNotNull(file);

			TemplateParser parser = new TemplateParser();
			parser.LoadTemplateFile(file);

			parser.Model["testData"] = "Test data";
			parser.Render();
			string result = parser.GetOutput();

			string outHtml;
			using (FileStream stream = File.OpenRead("../../Templates/Reference/simple.html"))
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					outHtml = reader.ReadToEnd();
				}
			}
			Assert.AreEqual(outHtml, result);
		}
	}
}
