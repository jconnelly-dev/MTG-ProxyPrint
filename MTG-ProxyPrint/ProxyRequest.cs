using System;
using System.Collections.Generic;
using System.IO;


namespace MTG_ProxyPrint
{
    public interface ProxyRequest
    {
        // [KEY: unique deck name], [VALUE: unsorted list of card names].
        Dictionary<string, List<string>> CollectDeckLists();
    }

    public class ProxyTextFile : ProxyRequest
    {
        List<FileInfo> _textFiles;

        public ProxyTextFile()
        {
            this._textFiles = null;
        }

        public Dictionary<string, List<string>> CollectDeckLists()
        {
            if (_textFiles == null || _textFiles.Count <= 0)
            {
                return null;
            }

            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            return result;
        }
    }
}