using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicConsumer
{
    public interface IMagicConsumer
    {
        public string DownloadCardImage(string deckPath, string cardName);
    }
}
