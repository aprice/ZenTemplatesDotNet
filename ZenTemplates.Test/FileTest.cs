using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenTemplates.Configuration;
using System.IO;
using ZenTemplates.Parser;

namespace ZenTemplates.Test
{
	[TestClass]
	public class FileTest
	{
		[TestMethod]
		public void SimpleFileTest()
		{
			ZenTemplatesConfiguration config = new ZenTemplatesConfiguration()
			{
				TemplateRoot = "../../Templates/Sample",
				TemplateFileExtension = ".html",
			};

			FileRepository repo = new FileRepository(config);

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
