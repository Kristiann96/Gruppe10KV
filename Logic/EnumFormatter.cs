using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public static class EnumFormatter
    {
        public static string ToNormalText(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return string.Join(" ", value.Split('_'))
                .ToLower()
                .Trim()
                .ReplaceFirstCharToUpper();
        }
    }

    public static class StringExtensions
    {
        public static string ReplaceFirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input[1..];
        }
    }
}
