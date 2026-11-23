using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TeacherERP.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/paymob")]
public sealed class PaymobReturnController : ControllerBase
{
    private const string ReturnPage = """
        <!doctype html>
        <html lang="ar" dir="rtl">
        <head>
          <meta charset="utf-8">
          <meta name="viewport" content="width=device-width, initial-scale=1">
          <title>العودة من بوابة الدفع</title>
          <style>
            body { font-family: system-ui, sans-serif; margin: 3rem auto; max-width: 38rem; padding: 0 1rem; color: #172033; }
            main { border: 1px solid #d8deea; border-radius: 1rem; padding: 2rem; }
          </style>
        </head>
        <body>
          <main>
            <h1>تم الرجوع من بوابة الدفع</h1>
            <p>يجري تأكيد حالة الدفع. ارجع إلى التطبيق لتحديث سجل المدفوعات.</p>
            <p>تأكيد الدفع النهائي يعتمد على إشعار Paymob الآمن، وليس على هذه الصفحة.</p>
          </main>
        </body>
        </html>
        """;

    [HttpGet("return")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public ContentResult Return() => Content(ReturnPage, "text/html; charset=utf-8");
}
