using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ZenTemplates.Configuration;
using ZenTemplates.Parser;

namespace ZenTemplates.Test
{
	[TestClass]
	public class InferenceTest
	{
		[TestMethod]
		public void InferredInjectionByClassTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p class=""testData"">Test placeholder</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p class=""testData"">Test data</p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.Model["testData"] = "Test data";
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void InferredInjectionByIDTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p id=""testData"">Test placeholder</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p id=""testData"">Test data</p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.Model["testData"] = "Test data";
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void InferredDerivationTest()
		{
			ZenTemplatesConfiguration config = new ZenTemplatesConfiguration()
			{
				TemplateRoot = "../../Templates/Sample",
				SharedRoot = "../../Templates/Sample/Shared",
				TemplateFileExtension = ".html",
			};

			FileRepository repo = new FileRepository(config);
			TemplateFile file = repo.LoadTemplateFile("Implied", "Controller");

			TemplateParser parser = new TemplateParser(repo);
			parser.LoadTemplateFile(file);
			parser.Render();
			string result = parser.GetOutput();

			string outHtml;
			using (FileStream stream = File.OpenRead("../../Templates/Reference/Controller/Implied.html"))
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
