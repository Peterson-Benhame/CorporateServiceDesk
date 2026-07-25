
using CorporateServiceDesk.Infrastructure.Pagination.Interfaces.Inputs;
using System.Collections;

namespace CorporateServiceDesk.Infrastructure.Pagination.Interfaces.Results
{
    public interface IPaginatedResult : IOperationResult<IList>
    {

        int? TotalCount { get; set; }
        int? Pages { get; set; }
        int Count { get; set; }
        int Page { get; set; }

        /// <summary>
        /// Realiza a paginação sobre uma consulta, aplicando uma função de seleção personalizada.
        /// </summary>
        /// <typeparam name="TSource">Tipo do objeto de origem na consulta.</typeparam>
        /// <typeparam name="TResult">Tipo do objeto de resultado após aplicar a função de seleção.</typeparam>
        /// <param name="pagination">Objeto contendo informações de paginação.</param>
        /// <param name="source">Consulta de origem para ser paginada.</param>
        /// <param name="selectFunction">Função que converte o objeto de origem em objeto de resultado.</param>
        void Paginate<TSource, TResult>(IPagination pagination, IQueryable<TSource> source, Func<TSource, TResult> selectFunction);

        /// <summary>
        /// Adiciona uma mensagem ao resultado paginado.
        /// </summary>
        /// <param name="message">Mensagem a ser adicionada.</param>
        /// <returns>O próprio <see cref="IPaginatedResult"/> com a mensagem adicionada.</returns>
        new IPaginatedResult AddMessage(string message);
    }
}