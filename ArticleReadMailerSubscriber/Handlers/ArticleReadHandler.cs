﻿using AxxesTimes.Data;
using AxxesTimes.Events;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;

namespace ArticleReadMailerSubscriber.Handlers
{
    class ArticleReadHandler : IHandleMessages<ArticleRead>
    {
        static readonly ILog log = LogManager.GetLogger<ArticleReadHandler>();

        private readonly IArticlesRepository _articleRepository;

        public ArticleReadHandler(IArticlesRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public Task Handle(ArticleRead message, IMessageHandlerContext context)
        {
            var articleId = message.ArticleId;

            var article = _articleRepository.GetArticleById(articleId);
            if(article.Reads % 5 == 0)
            {
                // do something useful here
                log.Info($"Article {article.Id} reached a new milestone and has now {article.Reads} reads.");
            }
            else
            {
                log.Info($"No new milestone yet for article {article.Id}.");
            }

            return Task.CompletedTask;
        }
    }
}
