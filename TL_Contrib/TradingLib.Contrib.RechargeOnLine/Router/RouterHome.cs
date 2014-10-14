using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HttpServer;
using HttpServer.BodyDecoders;
using HttpServer.Logging;
using HttpServer.Modules;
using HttpServer.Resources;
using HttpServer.Routing;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Contrib.RechargeOnLine
{
    internal class RouterHome:IRouter
    {
        public virtual ProcessingResult Process(RequestContext context)
        {
            IRequest request = context.Request;
            IResponse response = context.Response;
            string loacalpath = request.Uri.AbsolutePath;
            if (loacalpath == "/deposit")
            {

                response.PageTemplate("deposit", new Dictionary<string, object>());
                return ProcessingResult.SendResponse;
            }
            else if (loacalpath == "/deposit_manual")
            {
                response.PageTemplate("deposit_manual", new Dictionary<string, object>());
                return ProcessingResult.SendResponse;
            }
            else if (loacalpath == "/withdraw")
            {
                response.PageTemplate("withdraw", new Dictionary<string, object>());
                return ProcessingResult.SendResponse;
            }

            return ProcessingResult.Continue;

        }
    }
}
