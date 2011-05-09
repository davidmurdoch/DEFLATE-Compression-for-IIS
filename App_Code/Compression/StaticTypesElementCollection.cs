using System.Configuration;

namespace Compression
{
    [ConfigurationCollection(typeof(MimeFormatElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class StaticTypesElementCollection : ConfigurationElementCollection
    {
        #region Constructors
        /// <summary>
        /// Predefines the valid properties and prepares
        /// the property collection.
        /// </summary>
        static StaticTypesElementCollection()
        {
            PProperties = new ConfigurationPropertyCollection();
        }
        #endregion

        #region Static Fields
        private static readonly ConfigurationPropertyCollection PProperties;
        #endregion
         
        #region Properties
        protected override ConfigurationPropertyCollection Properties
        {
            get { return PProperties; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }
        #endregion

        #region Indexers
        public MimeFormatElement this[int index]
        {
            get { return (MimeFormatElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new MimeFormatElement this[string name]
        {
            get { return (MimeFormatElement)BaseGet(name); }
        }
        #endregion

        #region Overrides
        protected override ConfigurationElement CreateNewElement()
        {
            return new MimeFormatElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return element is MimeFormatElement ? (element as MimeFormatElement).MimeFormat : new MimeFormat(string.Empty);
        }

        #endregion
    }
}