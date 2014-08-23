using System.Collections.Generic;
using System.IO;
using System.Web;
using ZenTemplates.Configuration;
using ZenTemplates.Parser;

namespace ZenTemplates.HttpHandler
{
    public class ZenTemplatesHandler : IHttpHandler
    {
		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			Directory.SetCurrentDirectory(context.Request.PhysicalApplicationPath);
			ZenTemplatesConfiguration config = new ZenTemplatesConfiguration();
			FileRepository repo = new FileRepository(config);
			string path = context.Request.AppRelativeCurrentExecutionFilePath.Substring(2);
			TemplateParser parser = TemplateParser.GetTemplateParser(path, repo);
			if (parser == null)
			{
				context.Response.StatusCode = 404;
			}
			else
			{
				string modelFile = path.Substring(0, path.LastIndexOf('.')) + ".json";
				IDictionary<string, object> model = repo.LoadModelJson(modelFile);
				if (model == null)
				{
					model = repo.LoadModelJson("global.json");
				}

				if (model == null)
				{
					model = new Dictionary<string, object>();
				}

				model["AppSettings"] = config.AppSettingsModel.RootNode;
				parser.Model = model;

				parser.Render();
				context.Response.ContentType = "text/html";
				context.Response.Write(parser.GetOutput());
			}
		}
	}
}
