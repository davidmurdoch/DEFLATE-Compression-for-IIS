using System;
using System.Web;
using Compression;
using Ionic.Zlib;

namespace HttpModules
{
    public class CompressionModule : IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
            // nothing to dispose.
        }

        public void Init(HttpApplication context)
        {
            context.ReleaseRequestState += OnReleaseRequestState;
        }

        #endregion

        private static void OnReleaseRequestState(object sender, EventArgs e)
        {
            CompressContent((HttpApplication) sender);
        }

        private static void CompressContent(HttpApplication context)
        {
            if (context.Context.Items.Contains("AlreadyCompressed")) return;

            context.Context.Items.Add("AlreadyCompressed", true);

            HttpResponse response = context.Response;
            if (response.Filter == null) return;

            CompressionConfiguration settings = CompressionConfiguration.GetSection();

            string acceptedTypes = context.Request.Headers["Accept-Encoding"];
            if (acceptedTypes == null) return;

            string contentType = response.ContentType.Split(';')[0].Trim();
            if (!settings.IsContentTypeCompressed(contentType)) return;

            string compressionScheme = settings.GetCompressionType(acceptedTypes);

            bool compressed = false;
            switch (compressionScheme)
            {
                case "deflate":
                    context.Response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress,
                                                                CompressionLevel.BestCompression, false);
                    compressed = true;
                    break;
                case "gzip":
                    context.Response.Filter = new GZipStream(response.Filter, CompressionMode.Compress,
                                                             CompressionLevel.BestCompression, false);
                    compressed = true;
                    break;
            }

            if (!compressed) return;

            response.AppendHeader("Content-Encoding", compressionScheme);
            response.AppendHeader("Vary", "Accept-Encoding");
        }
    }
}