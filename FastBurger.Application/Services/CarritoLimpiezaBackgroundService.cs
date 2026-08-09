using FastBurger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FastBurger.Application.Services;

public class CarritoLimpiezaBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CarritoLimpiezaBackgroundService> _logger;

    private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan UmbralAbandono = TimeSpan.FromMinutes(30);

    public CarritoLimpiezaBackgroundService(IServiceScopeFactory scopeFactory, ILogger<CarritoLimpiezaBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CarritoLimpiezaBackgroundService iniciado. Intervalo={Intervalo}, Umbral={Umbral}.", Intervalo, UmbralAbandono);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await LimpiarAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error en ciclo de limpieza de carritos abandonados.");
            }

            try
            {
                await Task.Delay(Intervalo, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("CarritoLimpiezaBackgroundService detenido.");
    }

    private async Task LimpiarAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FastBurgerContext>();

        var umbral = DateTime.Now.Subtract(UmbralAbandono);

        var idsAbandonados = await context.Carritos
            .Where(c => c.Estado == "activo" && c.FechaActualizacion < umbral)
            .Select(c => c.IdCarrito)
            .ToListAsync(ct);

        if (idsAbandonados.Count == 0)
        {
            _logger.LogDebug("No se encontraron carritos abandonados en este ciclo.");
            return;
        }

        var itemsBorrados = await context.CarritoItems
            .Where(ci => idsAbandonados.Contains(ci.IdCarrito))
            .ExecuteDeleteAsync(ct);

        var carritosBorrados = await context.Carritos
            .Where(c => idsAbandonados.Contains(c.IdCarrito))
            .ExecuteDeleteAsync(ct);

        _logger.LogInformation("Se eliminaron {Carritos} carritos abandonados ({Items} items).", carritosBorrados, itemsBorrados);
    }
}