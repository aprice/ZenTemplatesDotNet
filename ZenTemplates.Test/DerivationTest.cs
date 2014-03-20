using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ZenTemplates.Configuration;
using ZenTemplates.Parser;

namespace ZenTemplates.Test
{
	[TestClass]
	public class DerivationTest
	{
		private static FileRepository GetFileRepository()
		{
			ZenTemplatesConfiguration config = new ZenTemplatesConfiguration()
			{
				TemplateRoot = "../../Templates/Sample",
				SharedRoot = "../../Templates/Sample/Shared",
				TemplateFileExtension = ".html",
			};

			return new FileRepository(config);
		}

		[TestMethod]
		public void SimpleDerivationTest()
		{
			FileRepository repo = GetFileRepository();
			TemplateFile file = repo.LoadTemplateFile("Index", "Controller");

			TemplateParser parser = new TemplateParser(repo);
			parser.LoadTemplateFile(file);
			parser.Render();
			string result = parser.GetOutput();

			string outHtml;
			using (FileStream stream = File.OpenRead("../../Templates/Reference/Controller/Index.html"))
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
