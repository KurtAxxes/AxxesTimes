using AxxesTimes.Commands;
using AxxesTimes.Data;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;

namespace ReadArticleSubscriber.Handlers
{
    class ReadArticleHandler : IHandleMessages<ReadArticle>
    {
        static readonly ILog log = LogManager.GetLogger<ReadArticleHandler>();

        private readonly IArticlesRepository _articleRepository;

        public ReadArticleHandler(IArticlesRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public Task Handle(ReadArticle message, IMessageHandlerContext context)
        {
            var articleId = message.ArticleId;

            _articleRepository.UpdateArticleRead(articleId);

            log.Info($"Processed article read for article id {articleId}");

            return Task.CompletedTask;
        }
    }
}
