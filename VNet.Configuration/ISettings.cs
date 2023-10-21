using System.Collections.ObjectModel;

namespace VNet.Configuration
{
    public class TabItemModel
    {
        public string Header { get; }
        public string Content { get; }
        public TabItemModel(string header, string content)
        {
            Header = header;
            Content = content;
        }
    }

    public interface ISettings
    {

    }
}