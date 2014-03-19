using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenTemplates.Configuration;
using System.IO;
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
				TemplateFileExtension = ".html",
			};

			return new FileRepository(config);
		}

		[TestMethod]
		public void SimpleDerivationTest()
		{
			FileRepository repo = GetFileRepository();
			FileInfo file = repo.GetTemplateFile("Index", "Controller");

			TemplateParser parser = new TemplateParser();
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
