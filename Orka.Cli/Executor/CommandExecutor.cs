﻿using Orka.Abstractions;
using Orka.Cli.Config;
using System.Diagnostics;

namespace Orka.Cli.Executor;

public static class CommandExecutor
{
    private static readonly string logPath = "c:\\temp\\orka-exec.log";

    private static void Log(string message)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
        File.AppendAllText(logPath, $"[{DateTime.Now:HH:mm:ss}] {message}\n");
    }

    public static async Task<string?> ExecuteAsync(OrkaResource step)
    {
        if (!step.Inputs.TryGetValue("command", out var raw))
        {
            Log("⚠️ No command provided.");
            return "No command found.";
        }

        var command = raw?.ToString()?.Trim('\'');

        Log($"🛠 Executing: {command}");

        try
        {
            var psi = new ProcessStartInfo("cmd.exe", $"/c {command}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(psi);
            if (process == null)
            {
                Log("❌ Failed to start process.");
                return "Failed to start process.";
            }

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            Log("✅ Process exited.");
            Log($"📤 Output: {output}");
            Log($"📛 Error: {error}");

            File.AppendAllText("c:\\temp\\orka-resource-params.log", $"[{DateTime.Now:HH:mm:ss}] 🔍 Received command param: {command}\n");


            return string.IsNullOrWhiteSpace(error) ? output : $"stderr: {error}";
        }
        catch (Exception ex)
        {
            Log($"🔥 Exception: {ex}");
            return $"Exception: {ex.Message}";
        }
    }

    public static string? ExecuteSync(OrkaResource step)
    {
        if (!step.Inputs.TryGetValue("command", out var raw))
        {
            return "No command found.";
        }

        var command = raw?.ToString()?.Trim('\'');

        var psi = new ProcessStartInfo("cmd.exe", $"/c {command}")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null)
        {
            return "Failed to start process.";
        }

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        return string.IsNullOrWhiteSpace(error) ? output : $"stderr: {error}";
    }
}
