using HarmonyLib;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace HarmonyDemo
{
    public static class Program
    {
        private static async Task Main()
        {
            var patcher = new Harmony(nameof(SocketsHttpHandler));

            patcher.Patch(typeof(SocketsHttpHandler).Assembly.GetType("System.Net.Http.HttpConnectionPoolManager")!
                    .GetMethod("GetConnectionKey", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic),
                new HarmonyMethod(typeof(Program).GetMethod(nameof(GetConnectionKeyBefore), BindingFlags.Static | BindingFlags.NonPublic)),
                new HarmonyMethod(typeof(Program).GetMethod(nameof(GetConnectionKeyEnd), BindingFlags.Static | BindingFlags.NonPublic)));

            await Test();
            await Test();
            await Task.Delay(1000);
            await Test();
        }

        private static async Task Test()
        {
            using var client = new HttpClient();

            using var request = new HttpRequestMessage(HttpMethod.Get, "https://iovip.qbox.me/test/FtPFFB2e_qh1isgop4jyxG68k4tE_w2880_h1800.jpg@200w_200h_100q.webp");

            request.Headers.Host = "image.tuhu.cn";

            using var response = await client.SendAsync(request);

            Console.WriteLine(response.StatusCode);
        }

        private static void GetConnectionKeyBefore(HttpRequestMessage request, out string __state)
        {
            __state = null;

            if (ParseHostNameFromHeader(request.Headers.Host) == request.RequestUri.IdnHost ||
                request.Headers.Host == null) return;

            __state = request.Headers.Host;
            request.Headers.Host = null;

            static string ParseHostNameFromHeader(string hostHeader)
            {
                if (hostHeader == null) return null;

                // See if we need to trim off a port.
                var colonPos = hostHeader.IndexOf(':');
                if (colonPos < 0) return hostHeader;

                var ipV6AddressEnd = hostHeader.IndexOf(']');
                if (ipV6AddressEnd == -1)
                    return hostHeader.Substring(0, colonPos);

                colonPos = hostHeader.LastIndexOf(':');
                return colonPos > ipV6AddressEnd ? hostHeader.Substring(0, colonPos) : hostHeader;
            }
        }

        private static void GetConnectionKeyEnd(HttpRequestMessage request, string __state)
        {
            if (__state != null) request.Headers.Host = __state;
        }
    }
}
