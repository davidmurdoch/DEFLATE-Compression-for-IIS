using System;
using System.ComponentModel;
using System.Configuration;
using System.Web;

namespace Compression
{
    public class MimeFormatElement: ConfigurationElement
    {
        #region Constructors
        /// <summary>
        /// Predefines the valid properties and prepares
        /// the property collection.
        /// </summary>
        static MimeFormatElement()
        {
            // Predefine properties here
            var mimeFormatType = typeof(MimeFormat);
            PMimeFormat = new ConfigurationProperty(
                "mimeFormat",
                mimeFormatType,
                "*/*",
                ConfigurationPropertyOptions.IsRequired
            );
            PEnabled = new ConfigurationProperty(
                "enabled",
                typeof(bool),
                false,
                ConfigurationPropertyOptions.IsRequired
            );

            PProperties = new ConfigurationPropertyCollection {
                PMimeFormat, PEnabled
            };
        }
        #endregion

        #region Static Fields

        private static readonly ConfigurationProperty PMimeFormat;
        private static readonly ConfigurationProperty PEnabled;
        private static readonly ConfigurationPropertyCollection PProperties;

        #endregion

        #region Properties
        /// <summary>
        /// Gets the MimeFormat setting.
        /// </summary>
        [ConfigurationProperty("mimeFormat", IsRequired = true)]
        public MimeFormat MimeFormat
        {
            get { return (MimeFormat)base[PMimeFormat]; }
        }

        /// <summary>
        /// Gets the Enabled setting.
        /// </summary>
        [ConfigurationProperty("enabled")]
        public bool Enabled
        {
            get { return (bool)base[PEnabled]; }
        }

        /// <summary>
        /// Override the Properties collection and return our custom one.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return PProperties; }
        }
        #endregion
    }
}