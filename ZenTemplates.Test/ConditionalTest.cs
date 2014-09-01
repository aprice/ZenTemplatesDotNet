﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenTemplates.Parser;

namespace ZenTemplates.Test
{
	[TestClass]
	public class ConditionalTest
	{
		[TestMethod]
		public void SimpleTrueIfTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p data-z-if=""testData"">Test content</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test content</p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.Model["testData"] = true;
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void SimpleFalseIfTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p data-z-if=""testData"">Test content</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.Model["testData"] = false;
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void ComplexExpresionIfTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p data-z-if=""testData && (testData2 || !testData3)"">Test content</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test content</p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.Model["testData"] = true;
			parser.Model["testData2"] = false;
			parser.Model["testData3"] = false;
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void ElseTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p id=""Test"" data-z-if=""testData"" data-z-else=""TestElse"">Test content</p><p id=""TestElse"" data-z-id=""Test"" data-z-if=""testData2"" data-z-else=""Tertiary"">Alternative content</p><p id=""Tertiary"" data-z-id=""Test""></p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p id=""Test"">Alternative content</p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.Model["testData"] = false;
			parser.Model["testData2"] = true;
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void LoremTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p data-z-lorem>Test content</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}
	}
}
