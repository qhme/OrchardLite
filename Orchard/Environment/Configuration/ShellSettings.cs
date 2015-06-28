using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Environment.Configuration
{
    /// <summary>
    /// Represents the minimalistic set of fields stored. This 
    /// model is obtained from the IShellSettingsManager, which by default reads this
    /// from the App_Data settings.txt files.
    /// </summary>
    public class ShellSettings
    {
        private string[] _modules;
        private readonly IDictionary<string, string> _values;

        public ShellSettings()
        {
            _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Modules = new string[0];
        }

        public ShellSettings(ShellSettings settings)
        {
            _values = new Dictionary<string, string>(settings._values, StringComparer.OrdinalIgnoreCase);

            DataProvider = settings.DataProvider;
            DataConnectionString = settings.DataConnectionString;
            EncryptionAlgorithm = settings.EncryptionAlgorithm;
            EncryptionKey = settings.EncryptionKey;
            HashAlgorithm = settings.HashAlgorithm;
            HashKey = settings.HashKey;
            Modules = settings.Modules;
        }

        public string this[string key]
        {
            get
            {
                string retVal;
                return _values.TryGetValue(key, out retVal) ? retVal : null;
            }
            set { _values[key] = value; }
        }

        /// <summary>
        /// Gets all keys held by this shell settings.
        /// </summary>
        public IEnumerable<string> Keys { get { return _values.Keys; } }


        /// <summary>
        /// The database provider for the tenant
        /// </summary>
        public string DataProvider
        {
            get { return this["DataProvider"] ?? ""; }
            set { this["DataProvider"] = value; }
        }

        /// <summary>
        /// The database connection string
        /// </summary>
        public string DataConnectionString
        {
            get { return this["DataConnectionString"]; }
            set { this["DataConnectionString"] = value; }
        }

        /// <summary>
        /// The encryption algorithm used for encryption services
        /// </summary>
        public string EncryptionAlgorithm
        {
            get { return this["EncryptionAlgorithm"]; }
            set { this["EncryptionAlgorithm"] = value; }
        }

        /// <summary>
        /// The encryption key used for encryption services
        /// </summary>
        public string EncryptionKey
        {
            get { return this["EncryptionKey"]; }
            set { _values["EncryptionKey"] = value; }
        }

        /// <summary>
        /// The hash algorithm used for encryption services
        /// </summary>
        public string HashAlgorithm
        {
            get { return this["HashAlgorithm"]; }
            set { this["HashAlgorithm"] = value; }
        }

        /// <summary>
        /// The hash key used for encryption services
        /// </summary>
        public string HashKey
        {
            get { return this["HashKey"]; }
            set { this["HashKey"] = value; }
        }

        /// <summary>
        /// List of available modules for this tenant
        /// </summary>
        public string[] Modules
        {
            get
            {
                return _modules ?? (Modules = (_values["Modules"] ?? "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                                                                     .Select(t => t.Trim())
                                                                     .ToArray();
            }
            set
            {
                _modules = value;
                this["Modules"] = string.Join(";", value);
            }
        }
    }
}
