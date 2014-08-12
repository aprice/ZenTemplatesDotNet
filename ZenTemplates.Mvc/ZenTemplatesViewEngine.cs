using System.Web.Mvc;
using ZenTemplates.Configuration;

namespace ZenTemplates.Mvc
{
	public class ZenTemplatesViewEngine : VirtualPathProviderViewEngine
	{
		/// <summary>
		/// Register ZenTemplatesViewEngine with ASP.NET MVC. Call this method in Application_Start.
		/// </summary>
		public static void Register()
		{
			ViewEngines.Engines.Add(new ZenTemplatesViewEngine());
		}

		
		public ZenTemplatesViewEngine()
		{
			ZenTemplatesConfiguration config = new ZenTemplatesConfiguration();
			this.ViewLocationFormats = new string[]
			{
				"~/Views/{1}/{0}" + config.TemplateFileExtension,
				"~/Views/Shared/{0}" + config.TemplateFileExtension
			};

			this.PartialViewLocationFormats = new string[]
			{
				"~/Views/{1}/{0}" + config.SnippetFileExtension,
				"~/Views/Shared/{0}" + config.SnippetFileExtension
			};
		}

		protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
		{
			string physicalpath = controllerContext.HttpContext.Server.MapPath(partialPath);
			return new ZenTemplatesView(physicalpath);
		}

		protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
		{
			string physicalpath = controllerContext.HttpContext.Server.MapPath(viewPath);
			return new ZenTemplatesView(physicalpath);
		}
	}
}
