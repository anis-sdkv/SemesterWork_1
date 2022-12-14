//     using Azure;
// using Azure.Core;
// using HttpServer.Logger;
// using System.IO;
// using System.Net;
// using System.Text;
// using System.Text.Json;
// using System;
//
// namespace HttpServer.ServerLogic
// {
//     class ResponseBuilder
//     {
//         private static ServerLogger _logger;
//         public static ServerLogger Logger
//         {
//             get
//             {
//                 if (_logger == null)
//                     throw new Exception("ResponseBuilder: the logger is not setted.");
//                 return _logger;
//             }
//             set => _logger = value;
//         }
//
//         private HttpListenerContext _context;
//
//         private HttpListenerResponse Response
//         {
//             get
//             {
//                 if (_context.Response == null)
//                     throw new Exception("The request has already been sent.");
//                 return _context.Response;
//             }
//         }
//
//         private byte[]? _buffer;
//         public ResponseBuilder(HttpListenerContext context)
//         {
//             _context = context;
//         }
//
//         public ResponseBuilder NewResponse(HttpListenerContext context)
//         {
//             _context = context;
//             return this;
//         }
//
//         public ResponseBuilder SetObject(object o)
//         {
//             Response.ContentType = "application/json";
//             _buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(o));
//
//             return this;
//         }
//
//         public ResponseBuilder SetContent(
//             byte[] buffer,
//             string type)
//         {
//             Response.ContentType = type;
//             _buffer = buffer;
//             return this;
//         }
//
//         public ResponseBuilder SetContent(string path)
//         {
//             if (!File.Exists(path))
//                 throw new FileNotFoundException();
//             Response.ContentType = MimeMap.GetMimeType(Path.GetExtension(path));
//             _buffer = File.ReadAllBytes(path);
//             return this;
//         }
//
//         public async Task<ResponseBuilder> SetContentAsync(string path)
//         {
//             Response.ContentType = MimeMap.GetMimeType(Path.GetExtension(path));
//             _buffer = await GetPageAsync(path);
//             return this;
//         }
//
//         private async Task<byte[]> GetPageAsync(string path)
//         {
//             if (!File.Exists(path))
//                 throw new FileNotFoundException();
//
//             await using var fs = File.OpenRead(path);
//             var buffer = new byte[fs.Length];
//             await fs.ReadAsync(buffer);
//             return buffer;
//         }
//
//         public ResponseBuilder SetMessage(string message)
//         {
//             Response.ContentType = "text/plain;charset=UTF-8";
//             _buffer = Encoding.UTF8.GetBytes(message);
//             return this;
//         }
//
//         public ResponseBuilder SetNotFoundMessage()
//         {
//             Response.StatusCode = (int)HttpStatusCode.NotFound;
//             Response.ContentType = "text/plain;charset=UTF-8";
//             _buffer = Encoding.UTF8.GetBytes("Ресурс не найден!");
//             return this;
//         }
//         public ResponseBuilder SetRedirect(string url)
//         {
//             Response.StatusCode = 303;
//             Response.Redirect(url);
//             return this;
//         }
//
//         public ResponseBuilder SetCookie(Cookie cookie)
//         {
//             Response.SetCookie(cookie);
//             return this; 
//         }
//
//         public ResponseBuilder SetStatusCode(int code)
//         {
//             Response.StatusCode = code;
//             return this;
//         }
//         public async Task SendAsync()
//         {
//             if (_buffer != null)
//             {
//                 Response.ContentLength64 = _buffer.Length;
//                 var output = Response.OutputStream;
//                 await output.WriteAsync(_buffer);
//             }
//             Response.Close();
//             _buffer = null;
//             Logger.Message($"Запрос обработан: {_context.Request.Url}");
//             _context = null;
//         }
//     }
// }
