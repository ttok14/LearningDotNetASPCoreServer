using Microsoft.AspNetCore.Mvc.Filters;

namespace LearningServer01
{
    public class LogActionFilter : IActionFilter
    {
        private readonly ILogger<LogActionFilter> _logger;

        public LogActionFilter(ILogger<LogActionFilter> logger)
        {
            _logger = logger;
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller.GetType().Name;
            var action = context.ActionDescriptor.RouteValues["action"];

            _logger.LogInformation(">>> [CALL] {Controller} -> {Action}", controller, action);
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            var action = context.ActionDescriptor.RouteValues["action"];

            if (context.Exception != null)
            {
                _logger.LogError("!!! [ERROR] {Action} 실행 중 예외 발생: {Message}", action, context.Exception.Message);
            }
            else
            {
                _logger.LogInformation("<<< [END] {Action} 처리 완료", action);
            }
        }
    }
}
