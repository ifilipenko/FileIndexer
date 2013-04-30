using System.ServiceModel.Channels;

namespace FileIndexer.WCF.Binding
{
    public class CustomTextEncodingHttpBinding : CustomBinding
    {
        public CustomTextEncodingHttpBinding(string encoding)
            : base(GetBindingElements(encoding))
        {
        }

        private static BindingElement[] GetBindingElements(string encoding)
        {
            var httpBindingElement = new HttpTransportBindingElement();
            var textBindingElement = new CustomTextMessageBindingElement(encoding);
            return new BindingElement[] { textBindingElement, httpBindingElement };
        }
    }
}
