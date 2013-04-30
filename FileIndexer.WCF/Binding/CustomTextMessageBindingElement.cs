using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace FileIndexer.WCF.Binding
{
    /// <summary>
    /// based on example from http://msdn.microsoft.com/en-us/library/system.servicemodel.channels.messageencodingbindingelement.aspx
    /// </summary>
    public class CustomTextMessageBindingElement : MessageEncodingBindingElement, IWsdlExportExtension
    {
        private MessageVersion _msgVersion;
        private string _mediaType;
        private string _encoding;
        private readonly XmlDictionaryReaderQuotas _readerQuotas;

        CustomTextMessageBindingElement(CustomTextMessageBindingElement binding)
            : this(binding.Encoding, binding.MediaType, binding.MessageVersion)
        {
            _readerQuotas = new XmlDictionaryReaderQuotas();
            binding.ReaderQuotas.CopyTo(_readerQuotas);
        }

        public CustomTextMessageBindingElement(string encoding, string mediaType, MessageVersion msgVersion)
        {
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            if (mediaType == null)
                throw new ArgumentNullException("mediaType");

            if (msgVersion == null)
                throw new ArgumentNullException("msgVersion");

            _msgVersion = msgVersion;
            _mediaType = mediaType;
            _encoding = encoding;
            _readerQuotas = new XmlDictionaryReaderQuotas();
        }

        public CustomTextMessageBindingElement(string encoding = "UTF-8", string mediaType = "text/xml")
            : this(encoding, mediaType, MessageVersion.Soap11WSAddressing10)
        {
        }

        public override MessageVersion MessageVersion
        {
            get { return _msgVersion; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _msgVersion = value;
            }
        }

        public string MediaType
        {
            get { return _mediaType; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _mediaType = value;
            }
        }

        public string Encoding
        {
            get { return _encoding; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _encoding = value;
            }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { return _readerQuotas; }
        }

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new CustomTextMessageEncoderFactory(MediaType, Encoding, MessageVersion);
        }

        public override BindingElement Clone()
        {
            return new CustomTextMessageBindingElement(this);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelListener<TChannel>();
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelListener<TChannel>();
        }

        public override T GetProperty<T>(BindingContext context)
        {
            return typeof (T) == typeof (XmlDictionaryReaderQuotas)
                       ? (T) (object) _readerQuotas
                       : base.GetProperty<T>(context);
        }

        void IWsdlExportExtension.ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
        }

        void IWsdlExportExtension.ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
            CorrectExportWsdlEndpointFromTextMessageEncodingClass(exporter, context);
        }

        private void CorrectExportWsdlEndpointFromTextMessageEncodingClass(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
            var mebe = new TextMessageEncodingBindingElement {MessageVersion = _msgVersion};
            ((IWsdlExportExtension) mebe).ExportEndpoint(exporter, context);
        }
    }
}