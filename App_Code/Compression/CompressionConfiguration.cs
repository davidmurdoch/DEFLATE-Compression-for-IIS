using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Compression
{
    public class CompressionConfiguration : ConfigurationSection
    {
        #region Constructors
        static CompressionConfiguration()
        {
            PFormat = new ConfigurationProperty(
                "format",
                typeof(CompressionFormat),
                CompressionFormat.deflate, // deflate is preferred
                ConfigurationPropertyOptions.None
            );

            PStaticTypes = new ConfigurationProperty(
                "staticTypes",
                typeof(StaticTypesElementCollection),
                null,
                ConfigurationPropertyOptions.IsRequired
            );

            PProperties = new ConfigurationPropertyCollection {
                PFormat,
                PStaticTypes
            };
        }
        #endregion

        #region Enums

        public enum CompressionFormat
        {
            // ReSharper disable InconsistentNaming
            deflate,
            gzip
            // ReSharper restore InconsistentNaming
        }

        #endregion

        #region Static Fields

        private static readonly ConfigurationProperty PFormat;
        private static readonly ConfigurationProperty PStaticTypes;

        private static readonly ConfigurationPropertyCollection PProperties;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the BooleanValue setting.
        /// </summary>
        [ConfigurationProperty("format", IsRequired = false)]
        public CompressionFormat Format
        {
            get { return (CompressionFormat)base[PFormat]; }
        }

        /// <summary>
        /// Override the Properties collection and return our custom one.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return PProperties; }
        }

        /// <summary>
        /// Gets the NestedElement element.
        /// </summary>
        [ConfigurationProperty("staticTypes")]
        public StaticTypesElementCollection StaticTypes
        {
            get { return (StaticTypesElementCollection)base[PStaticTypes]; }
        }
        #endregion

        #region GetSection Pattern
        private static CompressionConfiguration _section;

        /// <summary>
        /// Gets the configuration section using the default element name.
        /// </summary>
        /// <remarks>
        /// If an HttpContext exists, uses the WebConfigurationManager
        /// to get the configuration section from web.config.
        /// </remarks>
        public static CompressionConfiguration GetSection()
        {
            return GetSection("httpCompression");
        }

        /// <summary>
        /// Gets the configuration section using the specified element name.
        /// </summary>
        /// <exception cref="ConfigurationException"></exception>
        /// <remarks>
        /// If an HttpContext exists, uses the WebConfigurationManager
        /// to get the configuration section from web.config.
        /// </remarks>
        public static CompressionConfiguration GetSection(string definedName)
        {
            if (_section == null)
            {
                string cfgFileName = ".config";
                if (HttpContext.Current == null)
                {
                    _section = ConfigurationManager.GetSection(definedName) as CompressionConfiguration;
                }
                else
                {
                    _section = WebConfigurationManager.GetSection(definedName) as CompressionConfiguration;
                    cfgFileName = "web.config";
                }

                if (_section == null)
                {
                    throw new ConfigurationErrorsException("The <" + definedName + "> section is not defined in your " + cfgFileName + " file!");
                }
            }

            return _section;
        }
        #endregion

        #region Methods

        public bool IsContentTypeCompressed(string contentTypeStr)
        {
            try
            {
                foreach (var staticType in from sType in StaticTypes.OfType<MimeFormatElement>() let mimeFormat = sType.MimeFormat where mimeFormat.Matches(contentTypeStr) select sType)
                {
                    return staticType.Enabled;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        public string GetCompressionType(string acceptEncoding)
        {
            bool foundDeflate = false;
            bool foundGZip = false;

            string[] formats = acceptEncoding.Split(',');

            foreach (string acceptEncodingValue in formats.Select(t => t.Trim().ToLower()))
            {
                if (acceptEncodingValue.Contains("deflate") && CanAcceptQuality(acceptEncodingValue))
                {
                    foundDeflate = true;
                }
                else if ((acceptEncodingValue.Contains("gzip") || acceptEncodingValue.StartsWith("x-gzip")) && CanAcceptQuality(acceptEncodingValue))
                {
                    foundGZip = true;
                }
                else if (acceptEncodingValue.StartsWith("*") && CanAcceptQuality(acceptEncodingValue))
                {
                    foundGZip = true;
                    foundDeflate = true;
                }
            }

            if (Format == CompressionFormat.deflate && foundDeflate)
            {
                return "deflate";
            }
            if (Format == CompressionFormat.gzip && foundGZip)
            {
                return "gzip";
            }

            return foundDeflate ? "deflate" : (foundGZip ? "gzip" : null);
        }

        static bool CanAcceptQuality(string acceptEncodingValue)
        {
            int qParam = acceptEncodingValue.IndexOf("q=");

            float val = 1.0f;

            if (qParam >= 0)
            {
                try
                {
                    val = float.Parse(acceptEncodingValue.Substring(qParam + 2,
                        acceptEncodingValue.Length - (qParam + 2)));
                }
                catch (FormatException)
                {

                }
            }
            return (val > 0.0f);
        }

        #endregion
    }
}