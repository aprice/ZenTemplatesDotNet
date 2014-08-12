using Newtonsoft.Json;
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
				IDictionary<string, object> model = LoadModelJson(repo.GetModelFile(modelFile));
				if (model == null)
				{
					model = LoadModelJson(repo.GetModelFile("global.json"));
				}

				if (model == null)
				{
					model = new Dictionary<string, object>();
				}

				model["AppSettings"] = config.AppSettingsModel;
				parser.Model = model;

				parser.Render();
				context.Response.ContentType = "text/html";
				context.Response.Write(parser.GetOutput());
			}
		}

		private static IDictionary<string, object> LoadModelJson(FileInfo file)
		{
			IDictionary<string, object> model = null;
			if (file != null && file.Exists)
			{
				string rawJson;
				using (FileStream stream = file.OpenRead())
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						rawJson = reader.ReadToEnd();
					}
				}
				model = JsonConvert.DeserializeObject<IDictionary<string, object>>(rawJson, new JsonConverter[] { new NestedDictionaryConverter() });
			}

			return model;
		}
	}
}
