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
			string path = context.Request.Path.Trim('/');
			TemplateParser<object> parser = TemplateParser.GetTemplateParser<object>(path);
			if (parser == null)
			{
				context.Response.StatusCode = 404;
			}
			else
			{
				string modelFile = path.Substring(0, path.LastIndexOf('.')) + ".json";
				object model = LoadModelJson(modelFile);
				if (model == null)
				{
					string modelDir = modelFile.Contains("/") ? modelFile.Substring(0, modelFile.LastIndexOf("/")) : ".";
					model = LoadModelJson(modelDir + "global.json");
				}

				if (model == null)
				{
					model = LoadModelJson(ZenTemplatesConfiguration.Current.SharedRoot + "/global.json");
				}

				if (model == null)
				{
					model = LoadModelJson(ZenTemplatesConfiguration.Current.TemplateRoot + "/global.json");
				}

				if (model != null)
				{
					parser.Model = model;
				}

				parser.Render();
				context.Response.ContentType = "text/html";
				context.Response.Write(parser.GetOutput());
			}
		}

		private object LoadModelJson(string fileName)
		{
			object model = null;
			if (File.Exists(fileName))
			{
				string rawJson;
				using (FileStream stream = File.OpenRead(fileName))
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
