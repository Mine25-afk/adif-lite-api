namespace Adif.Application.Abstractions.Persistence;

// Confirma en una sola transacción todo lo que los repositorios dejaron pendiente.
public interface IUnitOfWork
{
    Task<int> GuardarCambiosAsync(CancellationToken ct = default);
}
