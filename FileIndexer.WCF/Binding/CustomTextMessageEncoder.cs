using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace FileIndexer.WCF.Binding
{
    /// <summary>
    /// Code based on http://msdn.microsoft.com/en-us/library/ms751486.aspx
    /// </summary>
    public class CustomTextMessageEncoder : MessageEncoder
    {
        private readonly CustomTextMessageEncoderFactory _factory;
        private readonly XmlWriterSettings _writerSettings;
        private readonly string _contentType;

        public CustomTextMessageEncoder(CustomTextMessageEncoderFactory factory)
        {
            _factory = factory;

            _writerSettings = new XmlWriterSettings {Encoding = Encoding.GetEncoding(factory.CharSet)};
            _contentType = string.Format("{0}; charset={1}", _factory.MediaType, _writerSettings.Encoding.HeaderName);
        }

        public override string ContentType
        {
            get { return _contentType; }
        }

        public override string MediaType
        {
            get { return _factory.MediaType; }
        }

        public override MessageVersion MessageVersion
        {
            get  { return _factory.MessageVersion; }
        }

        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            var msgContents = new byte[buffer.Count];
            Array.Copy(buffer.Array, buffer.Offset, msgContents, 0, msgContents.Length);
            bufferManager.ReturnBuffer(buffer.Array);

            return ReadMessage(new MemoryStream(msgContents), int.MaxValue);
        }

        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            var reader = XmlReader.Create(stream);
            return Message.CreateMessage(reader, maxSizeOfHeaders, MessageVersion);
        }

        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            byte[] messageXmlBytes;
            int messageLength;
            WriteMessageToXml(message, out messageXmlBytes, out messageLength);

            var totalLength = messageLength + messageOffset;
            var totalBytes = bufferManager.TakeBuffer(totalLength);
            Array.Copy(messageXmlBytes, 0, totalBytes, messageOffset, messageLength);

            return new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
        }

        public override void WriteMessage(Message message, Stream stream)
        {
            using (var writer = XmlWriter.Create(stream, _writerSettings))
            {
                message.WriteMessage(writer);
                writer.Close();
            }
        }

        private void WriteMessageToXml(Message message, out byte[] messageXmlBytes, out int messageLength)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, _writerSettings))
                {
                    message.WriteMessage(writer);
                    writer.Flush();
                    messageXmlBytes = stream.GetBuffer();
                    messageLength = (int) stream.Position;
                    writer.Close();
                }
                stream.Close();
            }
        }
    }
}
