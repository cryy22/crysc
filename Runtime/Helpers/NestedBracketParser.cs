using System.Collections.Generic;
using System.Text;

namespace Crysc.Helpers
{
    public static class NestedBracketParser
    {
        public static string[] GetTopLevelContents(string raw, char openChar = '[', char closeChar = ']')
        {
            List<string> contents = new();

            var nestLevel = 0;
            StringBuilder sb = new();

            foreach (char next in raw)
            {
                if (next == openChar)
                {
                    nestLevel--;
                    if (nestLevel == 0)
                    {
                        contents.Add(sb.ToString());
                        sb.Clear();
                        continue;
                    }
                }

                if (nestLevel > 0)
                    sb.Append(next);

                if (next == closeChar)
                    nestLevel++;
            }

            return contents.ToArray();
        }
    }
}
