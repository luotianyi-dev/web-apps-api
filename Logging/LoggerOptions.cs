using Microsoft.Extensions.Logging.Console;

namespace TianyiNetwork.Web.AppsApi.Logging
{
    public class LoggerOptions : ConsoleFormatterOptions
    {
        public int MultiLinePrefixLength = 33;
    }
}
