using OfficeOpenXml;

namespace RubricasApp.Web.Configuration
{
    public static class EPPlusConfig
    {
        private static bool _licenseConfigured = false;

        public static void ConfigureLicense()
        {
            if (!_licenseConfigured)
            {
                try
                {
                    // Configuración para EPPlus 8+ - Uso no comercial
                    ExcelPackage.License.SetNonCommercialPersonal("RubricasApp");
                    _licenseConfigured = true;
                }
                catch (Exception ex)
                {
                    // Log del error para diagnóstico
                    Console.WriteLine($"ERROR: Error configurando licencia EPPlus: {ex.Message}");
                }
            }
        }

        public static void EnsureLicenseConfigured()
        {
            ConfigureLicense();
        }
    }
}
