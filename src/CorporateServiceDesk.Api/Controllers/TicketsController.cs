using CorporateServiceDesk.Api.Contracts.Tickets.Request;
using CorporateServiceDesk.Api.Contracts.Tickets.Response;
using CorporateServiceDesk.Api.Helpers;
using CorporateServiceDesk.Api.Contracts.Common;
using CorporateServiceDesk.Application.Common.Abstractions.Notifications;
using CorporateServiceDesk.Application.Tickets.Create;
using CorporateServiceDesk.Application.Tickets.Queries;
using CorporateServiceDesk.Application.Tickets.Queries.List;
using Microsoft.AspNetCore.Mvc;

namespace CorporateServiceDesk.Api.Controllers
{
    /// <summary>
    /// Disponibiliza operações para abertura, consulta e pesquisa de chamados.
    /// </summary>
    /// <remarks>
    /// Os endpoints deste controller representam a entrada HTTP do módulo de chamados.
    /// A execução das regras e consultas é delegada aos casos de uso da camada Application.
    /// </remarks>
    [ApiController]
    [Route("api/tickets")]
    public sealed class TicketsController : ControllerBase
    {
        /// <summary>
        /// Lista chamados utilizando paginação e filtros tipados.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Endpoint indicado para consultas comuns. É possível filtrar por status,
        /// prioridade, solicitante, atendente, período de abertura e texto presente
        /// no título do chamado.
        /// </para>
        /// <para>
        /// A página inicial é 1, o tamanho padrão é 10 e o limite máximo é 100 itens.
        /// Quando <c>CountTotal</c> for verdadeiro, a resposta também apresentará
        /// <c>TotalCount</c> e <c>TotalPages</c>.
        /// </para>
        /// <para>
        /// Se nenhuma ordenação for informada, os chamados serão ordenados pela data
        /// de abertura em ordem decrescente, utilizando o identificador como desempate.
        /// Uma consulta sem resultados retorna HTTP 200 com a coleção de itens vazia.
        /// </para>
        /// </remarks>
        /// <param name="request">
        /// Paginação, ordenação e filtros tipados enviados pela query string.
        /// </param>
        /// <param name="useCase">
        /// Caso de uso responsável pela validação e listagem dos chamados.
        /// </param>
        /// <param name="cancellationToken">
        /// Token para cancelar a operação caso a requisição HTTP seja interrompida.
        /// </param>
        /// <returns>Uma página contendo os chamados encontrados e seus metadados.</returns>
        /// <response code="200">
        /// Consulta realizada com sucesso, inclusive quando nenhum chamado for encontrado.
        /// </response>
        /// <response code="400">
        /// Paginação, período, filtro ou ordenação inválidos.
        /// </response>
        /// <response code="500">Erro interno inesperado durante a consulta.</response>
        [HttpGet]
        [ProducesResponseType(
            typeof(PagedResponse<QueryTicketListItemResponse>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List(
            [FromQuery] ListTicketsRequest request,
            [FromServices] QueryListTicketsUseCase useCase,
            CancellationToken cancellationToken)
        {
            var result = await useCase.ExecuteAsync(request.Map(), cancellationToken);
            var response = result.Map(page =>
                PagedResponseMapper.Map(page, QueryTicketListItemResponseMapper.Map));

            return ApiResponseHandler.GenerateResponse(response, this);
        }

        /// <summary>
        /// Pesquisa chamados utilizando critérios dinâmicos.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Endpoint destinado a pesquisas avançadas. Cada critério informa uma coluna,
        /// um operador, um valor e o conector lógico utilizado em relação ao critério
        /// anterior.
        /// </para>
        /// <para>
        /// As colunas disponíveis são título, status, prioridade, solicitante,
        /// atendente, data de abertura e data de encerramento. Os operadores aceitos
        /// dependem do tipo da coluna. Por exemplo, <c>Contains</c> é permitido para
        /// título, enquanto comparações de maior e menor são permitidas para datas.
        /// </para>
        /// <para>
        /// Critérios consecutivos conectados por <c>Or</c> formam um grupo. Os grupos
        /// são combinados por <c>And</c>. Não são aceitas propriedades ou expressões
        /// livres, protegendo a consulta contra campos não autorizados.
        /// </para>
        /// </remarks>
        /// <param name="request">
        /// Paginação, ordenação e coleção ordenada de critérios da pesquisa.
        /// </param>
        /// <param name="useCase">
        /// Caso de uso responsável por validar e executar a pesquisa avançada.
        /// </param>
        /// <param name="cancellationToken">
        /// Token para cancelar a operação caso a requisição HTTP seja interrompida.
        /// </param>
        /// <returns>Uma página contendo os chamados que atendem à expressão de pesquisa.</returns>
        /// <response code="200">
        /// Pesquisa realizada com sucesso, inclusive quando a coleção estiver vazia.
        /// </response>
        /// <response code="400">
        /// Coluna, operador, valor, conector, paginação ou ordenação inválidos.
        /// </response>
        /// <response code="500">Erro interno inesperado durante a pesquisa.</response>
        [HttpPost("search")]
        [ProducesResponseType(
            typeof(PagedResponse<QueryTicketListItemResponse>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Search(
            [FromBody] SearchTicketsRequest request,
            [FromServices] QuerySearchTicketsUseCase useCase,
            CancellationToken cancellationToken)
        {
            var result = await useCase.ExecuteAsync(request.Map(), cancellationToken);
            var response = result.Map(page =>
                PagedResponseMapper.Map(page, QueryTicketListItemResponseMapper.Map));

            return ApiResponseHandler.GenerateResponse(response, this);
        }

        /// <summary>
        /// Cria um novo chamado no sistema.
        /// </summary>
        /// <remarks>
        /// Recebe os dados necessários para abertura do chamado, converte a requisição em um
        /// <see cref="CreateTicketCommand"/> e delega o processamento ao
        /// <see cref="CreateTicketUseCase"/>.
        /// 
        /// Após a criação, retorna o chamado com status HTTP 201 e inclui no cabeçalho
        /// <c>Location</c> o endereço para consulta do recurso criado por meio da ação
        /// <see cref="GetById"/>.
        /// </remarks>
        /// <param name="request">
        /// Dados necessários para criação do chamado, incluindo título, descrição,
        /// solicitante e prioridade.
        /// </param>
        /// <param name="useCase">
        /// Caso de uso responsável por validar as regras de negócio e criar o chamado.
        /// </param>
        /// <param name="cancellationToken">
        /// Token utilizado para cancelar a operação assíncrona.
        /// </param>
        /// <returns>
        /// Uma resposta HTTP 201 contendo os dados do chamado criado.
        /// Caso os dados informados sejam inválidos, retorna HTTP 400 com os detalhes
        /// dos erros de validação.
        /// </returns>
        /// <response code="201">Chamado criado com sucesso.</response>
        /// <response code="400">Os dados enviados são inválidos.</response>
        /// <response code="409">
        /// Já existe um chamado ativo com o mesmo título para o solicitante.
        /// </response>
        /// <response code="500">Erro interno inesperado durante a criação.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreateTicketResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateTicketRequest request, [FromServices] CreateTicketUseCase useCase, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new CreateTicketCommand(request.Title, request.Description, request.RequesterId, request.Priority);
            var result = await useCase.ExecuteAsync(command, cancellationToken);


            Result<CreateTicketResponse> responseResult = result.Map(CreateTicketResponseMapper.Map);

            return ApiResponseHandler.GenerateResponse(responseResult, this,  nameof(GetById), new { id = result.Value?.Id });
        }

        /// <summary>
        /// Obtém os detalhes de um chamado pelo identificador.
        /// </summary>
        /// <remarks>
        /// Localiza o chamado correspondente ao identificador informado na rota e
        /// retorna seus dados detalhados.
        ///
        /// Este endpoint também é utilizado pelo método de criação para preencher o
        /// cabeçalho HTTP <c>Location</c> da resposta 201 Created.
        /// </remarks>
        /// <param name="id">Identificador único do chamado.</param>
        /// <param name="useCase">Caso de uso responsável por consultar o chamado.</param>
        /// <param name="cancellationToken">Token para cancelamento da operação.</param>
        /// <returns>
        /// Retorna HTTP 200 com os dados do chamado quando localizado ou HTTP 404
        /// quando não existir um chamado com o identificador informado.
        /// </returns>
        /// <response code="200">Chamado localizado com sucesso.</response>
        /// <response code="404">Chamado não encontrado.</response>
        /// <response code="500">Erro interno inesperado durante a consulta.</response>
        [HttpGet("{id:guid}", Name = nameof(GetById))]
        [ProducesResponseType(typeof(QueryTicketDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, [FromServices] QueryGetTicketByIdUseCase useCase, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ticket = await useCase.ExecuteAsync(id, cancellationToken);

            if (!ticket.IsSuccess)
            {
                return ApiResponseHandler.GenerateResponse(ticket, this, nameof(GetById), new { id });
            }

            Result<QueryTicketDetailsResponse> responseResult = ticket.Map(QueryTicketDetailsResponseMapper.Map);

            return ApiResponseHandler.GenerateResponse(responseResult, this);
        }
    }
}

