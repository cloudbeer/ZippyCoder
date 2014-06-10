using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZippyCoder
{
    public class Helper
    {
        public static string ToJavaClassName(string input)
        {
            if (input.StartsWith("sys_")) input = input.Substring(4);

            var inputs = input.Split('_', '-');
            var rtn = string.Empty;
            foreach (var ip in inputs)
            {
                if (!string.IsNullOrWhiteSpace(ip))
                {
                    rtn += ip.First().ToString().ToUpper() + String.Join("", ip.Skip(1));
                }
            }
            return rtn;
        }
        public static string ToJavaClassName(string input, string suffixOveride)
        {
            if (input.StartsWith(suffixOveride)) input = input.Substring(suffixOveride.Length);

            var inputs = input.Split('_', '-');
            var rtn = string.Empty;
            foreach (var ip in inputs)
            {
                if (!string.IsNullOrWhiteSpace(ip))
                {
                    rtn += ip.First().ToString().ToUpper() + String.Join("", ip.Skip(1));
                }
            }
            return rtn;
        }
        public static string ToJavaPropertyName(string input, string suffixOveride)
        {
            var ip = ToJavaClassName(input, suffixOveride);
            return ip.First().ToString().ToLower() + String.Join("", ip.Skip(1));
        }
        public static string ToJavaPropertyName(string input)
        {
            var ip = ToJavaClassName(input);
            return ip.First().ToString().ToLower() + String.Join("", ip.Skip(1));
        }
        public static string ToJavaPropertySetName(string input)
        {
            var ip = ToJavaClassName(input);
            return "set"+ip;
        }
        public static string ToJavaPropertyGetName(string input)
        {
            var ip = ToJavaClassName(input);
            return "get" + ip;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(ToJavaPropertyGetName("_i_hava_a_dream"));
            Console.ReadKey();
        }
    }
}
