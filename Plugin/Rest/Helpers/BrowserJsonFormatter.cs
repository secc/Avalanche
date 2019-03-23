using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Avalanche.Rest.Helpers
{
    //https://stackoverflow.com/questions/9847564/how-do-i-get-asp-net-web-api-to-return-json-instead-of-xml-using-chrome/20556625#20556625
    public class BrowserJsonFormatter : JsonMediaTypeFormatter
    {
        public BrowserJsonFormatter()
        {
            this.SupportedMediaTypes.Add( new MediaTypeHeaderValue( "text/html" ) );
            this.SerializerSettings.Formatting = Formatting.Indented;
        }

        public override void SetDefaultContentHeaders( Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType )
        {
            base.SetDefaultContentHeaders( type, headers, mediaType );
            headers.ContentType = new MediaTypeHeaderValue( "application/json" );
        }
    }
}
