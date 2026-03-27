using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TaskPilots.TaskAgents.WebApis.IntegrationTests;

public sealed class ApiIntegrationTestFixture : IAsyncDisposable
{
    private readonly StringBuilder _processLogs = new();
    private Process? _apiProcess;
    private Uri? _baseAddress;

    public Uri BaseAddress => _baseAddress ?? throw new InvalidOperationException("API host not started.");

    public HttpClient CreateClient() => new()
    {
        BaseAddress = BaseAddress,
        Timeout = TimeSpan.FromSeconds(10),
    };

    public async Task EnsureStartedAsync()
    {
        if (_apiProcess is not null)
        {
            return;
        }

        var port = FindAvailablePort();
        _baseAddress = new Uri($"http://127.0.0.1:{port}");
        _apiProcess = StartApiProcess(_baseAddress);
        await WaitUntilReadyAsync(_baseAddress);
    }

    public async ValueTask DisposeAsync()
    {
        if (_apiProcess is null)
        {
            return;
        }

        try
        {
            if (!_apiProcess.HasExited)
            {
                _apiProcess.Kill(entireProcessTree: true);
                await _apiProcess.WaitForExitAsync();
            }
        }
        finally
        {
            _apiProcess.Dispose();
        }
    }

    private Process StartApiProcess(Uri baseAddress)
    {
        var webApiDllPath = FindWebApiDllPath();
        if (!File.Exists(webApiDllPath))
        {
            throw new FileNotFoundException("Could not find TaskPilots.TaskAgents.WebApis.dll for integration tests.", webApiDllPath);
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = Path.GetDirectoryName(webApiDllPath)!,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            },
            EnableRaisingEvents = true,
        };
        process.StartInfo.ArgumentList.Add(webApiDllPath);
        process.StartInfo.Environment["ASPNETCORE_URLS"] = baseAddress.ToString();
        process.StartInfo.Environment["DOTNET_ENVIRONMENT"] = "Development";

        process.OutputDataReceived += (_, args) =>
        {
            if (!string.IsNullOrWhiteSpace(args.Data))
            {
                _processLogs.AppendLine($"[stdout] {args.Data}");
            }
        };
        process.ErrorDataReceived += (_, args) =>
        {
            if (!string.IsNullOrWhiteSpace(args.Data))
            {
                _processLogs.AppendLine($"[stderr] {args.Data}");
            }
        };

        if (!process.Start())
        {
            throw new InvalidOperationException("Failed to start TaskPilots.TaskAgents.WebApis for integration tests.");
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        return process;
    }

    private async Task WaitUntilReadyAsync(Uri baseAddress)
    {
        using var client = new HttpClient
        {
            BaseAddress = baseAddress,
            Timeout = TimeSpan.FromSeconds(2),
        };

        var timeoutAt = DateTimeOffset.UtcNow.AddSeconds(20);
        while (DateTimeOffset.UtcNow < timeoutAt)
        {
            if (_apiProcess?.HasExited == true)
            {
                throw new InvalidOperationException($"TaskPilots.TaskAgents.WebApis exited before becoming ready. Logs:{Environment.NewLine}{_processLogs}");
            }

            try
            {
                var response = await client.GetAsync("/api/system/summary");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
            }
            catch
            {
            }

            await Task.Delay(250);
        }

        throw new TimeoutException($"TaskPilots.TaskAgents.WebApis did not become ready. Logs:{Environment.NewLine}{_processLogs}");
    }

    private static int FindAvailablePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static string FindWebApiDllPath()
    {
        var repoRoot = FindRepoRoot();
        return Path.Combine(repoRoot, "TaskPilots.TaskAgents.WebApis", "bin", "Debug", "net10.0", "TaskPilots.TaskAgents.WebApis.dll");
    }

    private static string FindRepoRoot()
    {
        var current = AppContext.BaseDirectory;
        while (!string.IsNullOrWhiteSpace(current))
        {
            if (File.Exists(Path.Combine(current, "TaskPilots.TaskAgents.slnx")))
            {
                return current;
            }

            current = Directory.GetParent(current)?.FullName ?? string.Empty;
        }

        throw new DirectoryNotFoundException("Could not locate repository root.");
    }
}
