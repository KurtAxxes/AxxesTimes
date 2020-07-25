using AxxesTimes.Commands;
using AxxesTimes.Data;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;

namespace ReadArticleMailer.Handlers
{
    class ArticleReadHandler : IHandleMessages<ReadArticle>
    {
        static readonly ILog log = LogManager.GetLogger<ArticleReadHandler>();

        private readonly IArticlesRepository _articleRepository;

        public ArticleReadHandler(IArticlesRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public Task Handle(ReadArticle message, IMessageHandlerContext context)
        {
            var articleId = message.ArticleId;

            // obviously you would do something cool here
            log.Info($"Sending out an email because article [{articleId}] was read!");

            return Task.CompletedTask;
        }
    }
}
