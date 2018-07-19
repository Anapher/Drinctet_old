using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Drinctet.Harvester
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HarvesterNameAttribute : Attribute
    {
        public string Name { get; }
        public string Short { get; set; }

        public HarvesterNameAttribute(string name)
        {
            Name = name;
        }

        public static string GetName(Type harvesterType)
        {
            var attribute = harvesterType.GetCustomAttribute<HarvesterNameAttribute>();
            if (attribute != null)
                return attribute.Name;

            return harvesterType.Name.Replace("Harvester", null);
        }

        public static string GetShort(Type harvesterType)
        {
            var attribute = harvesterType.GetCustomAttribute<HarvesterNameAttribute>();
            if (attribute?.Short != null)
                return attribute.Short;

            var name = harvesterType.Name;
            name = name.Replace("Harvester", null);
            if (name.Count(char.IsUpper) < 2)
                return name;

            return Regex.Match(harvesterType.Name, "[A-Z]+[a-z]*(?<name>([A-Z]+[a-z]*)).*").Groups["name"].Value;
        }
    }
}