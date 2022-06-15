using System.Linq;

namespace TechExtensions.SchedyBot.BLL.Extentions
{
    public static class StringExtention
    {
        public static string ExtractDialogName(this string fullName)
        {
            var stepName = fullName.Split(".").TakeLast(4).ToArray();
            var name = stepName.ElementAt(0) + "." + stepName.ElementAt(1) + "." + stepName.ElementAt(2) + "." + stepName.ElementAt(3);
            return name;
        }

        public static bool IsNullOrEmpty(this string? s)
        {
            if (s == null)
                return true;
            if (s == string.Empty)
                return true;
            
            return false;
        }
    }
}
