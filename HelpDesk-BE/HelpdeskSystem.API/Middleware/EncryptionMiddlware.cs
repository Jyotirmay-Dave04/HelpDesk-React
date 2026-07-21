using System.Text;
using System.Text.Json;

public class EncryptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly byte[] _key;

    public EncryptionMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _key = Convert.FromBase64String(config["Encryption:Key"]!); // 32 bytes -> AES-256
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip encryption for non-API routes (swagger, health checks, etc.) if needed
        if (!context.Request.Path.StartsWithSegments("/api")
            || context.Request.Path.StartsWithSegments("/api/Config/encryptionKey"))
        {
            await _next(context);
            return;
        }

        // ---- Decrypt request body ----
        if (context.Request.ContentLength is > 0 &&
            context.Request.Method is "POST" or "PUT" or "PATCH")
        {
            context.Request.EnableBuffering();

            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var rawBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (!string.IsNullOrWhiteSpace(rawBody))
            {
                using var doc = JsonDocument.Parse(rawBody);
                var encryptedPayload = doc.RootElement.GetString()!;
                var decryptedJson = AesGcmHelper.Decrypt(encryptedPayload, _key);

                var bytes = Encoding.UTF8.GetBytes(decryptedJson);
                context.Request.Body = new MemoryStream(bytes);
                context.Request.ContentLength = bytes.Length;
            }
        }

        // ---- Capture and encrypt response body ----
        var originalBodyStream = context.Response.Body;
        using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        await _next(context);

        buffer.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(buffer).ReadToEndAsync();

        context.Response.Body = originalBodyStream;

        if (!string.IsNullOrEmpty(responseText))
        {
            var encrypted = AesGcmHelper.Encrypt(responseText, _key);
            var wrapped = JsonSerializer.Serialize(encrypted);
            var wrappedBytes = Encoding.UTF8.GetBytes(wrapped);

            context.Response.ContentType = "application/json";
            context.Response.ContentLength = wrappedBytes.Length;
            await context.Response.Body.WriteAsync(wrappedBytes);
        }
    }
}