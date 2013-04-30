using System.ServiceModel.Channels;

namespace FileIndexer.WCF.Binding
{
    /// <summary>
    /// Code based on http://msdn.microsoft.com/en-us/library/ms751486.aspx
    /// </summary>
    public class CustomTextMessageEncoderFactory : MessageEncoderFactory
    {
        private readonly MessageEncoder _encoder;
        private readonly MessageVersion _version;
        private readonly string _mediaType;
        private readonly string _charSet;

        internal CustomTextMessageEncoderFactory(string mediaType, string charSet, MessageVersion version)
        {
            _version = version;
            _mediaType = mediaType;
            _charSet = charSet;
            _encoder = new CustomTextMessageEncoder(this);
        }

        public override MessageEncoder Encoder
        {
            get { return _encoder; }
        }

        public override MessageVersion MessageVersion
        {
            get { return _version; }
        }

        internal string MediaType
        {
            get { return _mediaType; }
        }

        internal string CharSet
        {
            get { return _charSet; }
        }
    }
}