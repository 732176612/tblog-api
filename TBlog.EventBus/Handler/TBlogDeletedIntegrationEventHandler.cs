using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TBlog.Common;
using TBlog.EventBus;
using TBlog.IService;

namespace TBlog.EventBus
{
    public class UserRegisterIntegrationEventHandler : IIntegrationEventHandler<UserRegisterIntegrationEvent>
    {
        //private readonly IUserService _userServices;
        private readonly ILogger<UserRegisterIntegrationEventHandler> _logger;

        public UserRegisterIntegrationEventHandler(
            //IUserService blogArticleServices,
            ILogger<UserRegisterIntegrationEventHandler> logger)
        {
            //_userServices = blogArticleServices;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserRegisterIntegrationEvent @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, "Blog.Core", @event);

            ConsoleHelper.WriteSuccessLine($"----- Handling integration event: {@event.Id} at Blog.Core - ({@event})");

            //To do 注册成功
            await Task.Yield();
        }

    }

    public class UserRegisterIntegrationEvent : IntegrationEvent
    {
        public string UserID { get; private set; }

        public UserRegisterIntegrationEvent(string userID)
            => UserID = userID;
    }
}
