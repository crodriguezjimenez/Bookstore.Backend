using System;

namespace OrderService.Application.Commands.Delegates;

public delegate void AuditHandler(string branchesQueried, int booksFoundCount);

public static class AuditServices
{
    
    public static void LogToConsole(string branches, int count)
    {
        Console.WriteLine($"\n[AUDITORÍA - CONSOLA] {DateTime.Now:HH:mm:ss} - Se consultaron las sucursales: [{branches}]. Registros encontrados: {count}.\n");
    }

    public static void SendToTelemetry(string branches, int count)
    {
        Console.WriteLine($"[AUDITORÍA - TELEMETRÍA] Enviando métrica... BranchCount: {branches.Split(',').Length}, ResultsCount: {count} al dashboard de Deloitte.");
    }
}
