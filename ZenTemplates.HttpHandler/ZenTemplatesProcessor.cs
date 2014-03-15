using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ZenTemplates.HttpHandler
{
    public class ZenTemplatesProcessor : IHttpHandler
    {
		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			throw new NotImplementedException();
		}
	}
}
