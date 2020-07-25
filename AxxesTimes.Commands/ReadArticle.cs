using NServiceBus;

namespace AxxesTimes.Commands
{
    public class ReadArticle : ICommand
    {
        public int ArticleId { get; set; }
    }
}
