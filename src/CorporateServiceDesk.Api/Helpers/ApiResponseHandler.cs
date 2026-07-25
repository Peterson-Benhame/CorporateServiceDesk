using Microsoft.AspNetCore.Mvc;
using System.Net;
using CorporateServiceDesk.Application.Common.Abstractions.Notifications;


namespace CorporateServiceDesk.Api.Helpers
{
    /// <summary>
    /// Classe para gerar respostas para controllers.
    /// </summary>
    public static class ApiResponseHandler
    {
        /// <summary>
        /// Gera uma resposta adequada com base no Notification fornecido.
        /// </summary>
        /// <typeparam name="T">O tipo do valor contido no Notification.</typeparam>
        /// <param name="notification">O objeto Notification contendo status, mensagem e valor.</param>
        /// <param name="controller">O controller atual, para usar métodos como CreatedAtAction.</param>
        /// <param name="actionName">Nome da ação para a qual o CreatedAtAction deve apontar (caso aplicável).</param>
        /// <param name="routeValues">Rota ou valores de rota para o CreatedAtAction (caso aplicável).</param>
        /// <returns>Um objeto IActionResult que representa a resposta apropriada para a API.</returns>
        public static IActionResult GenerateResponse<T>(
            Result<T> notification,
            ControllerBase controller,
            string? actionName = null,
            object? routeValues = null)
        {
            var resultType = notification.ErrorType
                ?? (notification.IsSuccess
                    ? EnumErrorType.OK
                    : EnumErrorType.InternalServerError);

            return resultType switch
            {
                EnumErrorType.OK => controller.Ok(notification.Value),
                EnumErrorType.Created => actionName != null && routeValues != null
                    ? controller.CreatedAtAction(actionName, routeValues, notification.Value)
                    : controller.StatusCode((int)HttpStatusCode.Created, notification.Value),
                EnumErrorType.NoContent => controller.NoContent(),
                EnumErrorType.BadRequest => controller.BadRequest(notification.Error),
                EnumErrorType.NotFound => controller.NotFound(notification.Error),
                EnumErrorType.Conflict => controller.Conflict(notification.Error),
                EnumErrorType.InternalServerError => controller.StatusCode(500, notification.Error),
                _ => controller.StatusCode((int)resultType, notification.Error)
            };
        }
    }
}

