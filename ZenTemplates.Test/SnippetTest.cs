using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenTemplates.Parser;
using ZenTemplates.Configuration;

namespace ZenTemplates.Test
{
	[TestClass]
	public class SnippetTest
	{
		private static FileRepository GetFileRepository()
		{
			ZenTemplatesConfiguration config = new ZenTemplatesConfiguration()
			{
				TemplateRoot = "../../Templates/Sample",
				SharedRoot = "../../Templates/Sample/Shared",
				TemplateFileExtension = ".html",
				SnippetFileExtension = ".html"
			};

			return new FileRepository(config);
		}

		[TestMethod]
		public void SimpleSnippetTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><div data-z-snippet=""testSnippet"">Test placeholder</div></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><div><p>Test snippet content</p></div></body></html>";

			TemplateParser parser = new TemplateParser(GetFileRepository());
			parser.Model["testData"] = "Test data";
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}
	}
}
