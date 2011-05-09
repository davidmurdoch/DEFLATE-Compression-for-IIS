using System;
using System.ComponentModel;

namespace Compression
{
    [TypeConverter(typeof(MimeFormatConverter))]
    public struct MimeFormat
    {
        public string Format
        {
            get
            {
                return Type + "/" + SubType;
            }
        }
        public string Type;
        public string SubType;
        
        public MimeFormat(string mimeFormatStr)
        {
            var parts = mimeFormatStr.Split('/');
            if (parts.Length != 2)
            {
                throw new Exception("Invalid MimeFormat");
            }
            
            Type = parts[0];
            SubType = parts[1];
        }
        public MimeFormat(string mimeType, string mimeSubType)
        {
            Type = mimeType;
            SubType = mimeSubType;
        }
        public bool Matches(string mimeFormatStr)
        {
            try
            {
                var mimeFormat = new MimeFormat(mimeFormatStr);
                return (Type == "*" || Type.Equals(mimeFormat.Type, StringComparison.CurrentCultureIgnoreCase)) && (SubType == "*" || SubType.Equals(mimeFormat.SubType, StringComparison.CurrentCultureIgnoreCase));
            }
            catch
            {
                return false;
            }
        }
    }
}