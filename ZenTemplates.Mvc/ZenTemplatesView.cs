using System.IO;
using System.Web.Mvc;
using ZenTemplates.Parser;

namespace ZenTemplates.Mvc
{
	public class ZenTemplatesView : IView
	{
        private string Path;

		public ZenTemplatesView(string viewPhysicalPath)
        {
            Path = viewPhysicalPath;
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
			TemplateParser<ViewDataDictionary> parser = TemplateParser.GetTemplateParser<ViewDataDictionary>(Path);
			parser.Render();
			writer.Write(parser.GetOutput());
        }
	}
}
