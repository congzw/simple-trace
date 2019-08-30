using System;
using System.Collections.Generic;
using System.Linq;
using SimpleTrace.Common;

namespace SimpleTrace.TraceClients.Commands
{
    public static class CommandExtensions
    {
        public static string GetDesc(this ICommand cmd)
        {
            if (cmd == null)
            {
                return null;
            }

            var argsDesc = string.Empty;
            if (cmd.Args != null)
            {
                if (cmd.Args is IClientSpanLocate clientTraceLocate)
                {
                    argsDesc = clientTraceLocate.ToDisplayKey();
                }
                else if (cmd.Args is SaveSpansArgs saveSpansArgs)
                {
                    argsDesc = "saveSpans: " + saveSpansArgs.Items.Count;
                }
            }
            return string.Format("{0} {1:yyyyMMdd-HH:mm:ss} {2}", cmd.CommandType, cmd.CreateUtc, argsDesc);
        }

        public static string TryGetTraceId(this ICommand cmd)
        {
            if (cmd?.Args == null)
            {
                return null;
            }

            if (cmd.Args is IClientTraceLocate clientTraceLocate)
            {
                return clientTraceLocate.TraceId;
            }

            return null;
        }

        public static IList<IClientSpanLocate> TryGetIClientSpanLocates(this ICommand cmd)
        {
            if (cmd?.Args == null)
            {
                return new List<IClientSpanLocate>();
            }

            if (cmd.Args is IBatchClientSpanLocate<IClientSpanLocate> batchClientTraceLocate)
            {
                return batchClientTraceLocate.Items;
            }
            return new List<IClientSpanLocate>();
        }
    }
}
