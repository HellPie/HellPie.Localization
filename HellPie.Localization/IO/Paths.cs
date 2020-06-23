using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HellPie.Localization.IO {
    public static class Paths {
        private static string _libraryPath;
        private static string _dataPath;

        public static void LoadFrom(Assembly assembly) {
            _libraryPath = Path.GetDirectoryName(assembly.Location);
            _dataPath = BuildDataPath(assembly);
        }

        public static void LoadFrom(string libraryPath, string dataPath) {
            _libraryPath = libraryPath;
            _dataPath = dataPath;
        }

        public static string LibraryPath() {
            if(string.IsNullOrWhiteSpace(_libraryPath)) {
                LoadFrom(Assembly.GetExecutingAssembly());
            }

            return _libraryPath;
        }

        public static string DataPath() {
            if(string.IsNullOrWhiteSpace(_dataPath)) {
                LoadFrom(Assembly.GetExecutingAssembly());
            }

            return _dataPath;
        }

        private static string BuildDataPath(Assembly assembly) {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string level1 = null;
            string level2 = null;

            Attribute level1Attribute = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute)).FirstOrDefault();
            if(level1Attribute is AssemblyCompanyAttribute companyAttribute) {
                level1 = ReservedNames.Sanitize(companyAttribute.Company);
            }

            Attribute level2Attribute = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute)).FirstOrDefault();
            if(level2Attribute is AssemblyProductAttribute productAttribute) {
                level2 = ReservedNames.Sanitize(productAttribute.Product);
            }

            if(string.IsNullOrWhiteSpace(level2)) {
                level2 = ReservedNames.Sanitize(assembly.GetName().Name);
            }

            return string.IsNullOrWhiteSpace(level1) ? Path.Combine(appDataPath, level2) : Path.Combine(appDataPath, level1, level2);
        }
    }
}
