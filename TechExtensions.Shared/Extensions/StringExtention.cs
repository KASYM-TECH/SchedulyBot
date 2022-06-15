using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechExtensions.Shared.Extensions
{
    public static class StringExtention
    {
        public static bool IsNullOrEmpty(this string str) { return str == null; }
    }
}
