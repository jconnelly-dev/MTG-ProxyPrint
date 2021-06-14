using System;
using System.Collections.Generic;
using System.Text;

namespace MTG_ProxyPrint
{
    public class DatastoreRequest
    {
        public string Type { get; set; }
        public string Url { get; set; }
        public string Body { get; set; }
        public string Header { get; set; }
        public string ContentType { get; set; }

        public DatastoreRequest() { }
    }
}
