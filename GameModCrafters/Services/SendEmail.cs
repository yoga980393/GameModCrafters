using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System.Threading.Tasks;

namespace GameModCrafters.Services
{
    public class SendEmail
    {
        public string GetVerifyEmailCssContent()
        {
            string cssContent = @"
            body, p, div {
              font-family: tahoma,geneva,sans-serif;
              font-size: 14px;
            }
            body {
              color: #000000;
            }
            body a {
              color: #000000;
              text-decoration: none;
            }
            p { margin: 0; padding: 0; }
            table.wrapper {
              width:100% !important;
              table-layout: fixed;
              -webkit-font-smoothing: antialiased;
              -webkit-text-size-adjust: 100%;
              -moz-text-size-adjust: 100%;
              -ms-text-size-adjust: 100%;
            }
            img.max-width {
              max-width: 100% !important;
            }
            .column.of-2 {
              width: 50%;
            }
            .column.of-3 {
              width: 33.333%;
            }
            .column.of-4 {
              width: 25%;
            }
            ul ul ul ul  {
              list-style-type: disc !important;
            }
            ol ol {
              list-style-type: lower-roman !important;
            }
            ol ol ol {
              list-style-type: lower-latin !important;
            }
            ol ol ol ol {
              list-style-type: decimal !important;
            }
            .cover-image {
              max-width: 100%;
              height: auto !important;
              display: block;
              color: #000000;
              text-decoration: none;
              font-family: Helvetica, Arial, sans-serif;
              font-size: 16px;
              border-radius: 10px;
            }
            @media screen and (max-width:480px) {
              .preheader .rightColumnContent,
              .footer .rightColumnContent {
                text-align: left !important;
              }
              .preheader .rightColumnContent div,
              .preheader .rightColumnContent span,
              .footer .rightColumnContent div,
              .footer .rightColumnContent span {
                text-align: left !important;
              }
              .preheader .rightColumnContent,
              .preheader .leftColumnContent {
                font-size: 80% !important;
                padding: 5px 0;
              }
              table.wrapper-mobile {
                width: 100% !important;
                table-layout: fixed;
              }
              img.max-width {
                height: auto !important;
                max-width: 100% !important;
              }
              a.bulletproof-button {
                display: block !important;
                width: auto !important;
                font-size: 80%;
                padding-left: 0 !important;
                padding-right: 0 !important;
              }
              .columns {
                width: 100% !important;
              }
              .column {
                display: block !important;
                width: 100% !important;
                padding-left: 0 !important;
                padding-right: 0 !important;
                margin-left: 0 !important;
                margin-right: 0 !important;
              }
              .social-icon-column {
                display: inline-block !important;
              }
             ";
            return cssContent;
        }
        public string GetVerifyEmailHtmlContent(string cssContent,string confirmationLink)
        {
            string htmlContent = $@"
        <html>
        <head>
             <style>{cssContent}</style>
        </head>
        <body>
      <center class=""wrapper"" data-link-color=""#000000"" data-body-style=""font-size:14px; font-family:tahoma,geneva,sans-serif; color:#000000; background-color:#FFFFFF;"">
        <div class=""webkit"">
          <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" class=""wrapper"" bgcolor=""#FFFFFF"">
            <tr>
              <td valign=""top"" bgcolor=""#FFFFFF"" width=""100%"">
                <table width=""100%"" role=""content-container"" class=""outer"" align=""center"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                  <tr>
                    <td width=""100%"">
                      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                        <tr>
                          <td>
                            <!--[if mso]>
    <center>
    <table><tr><td width=""700"">
  <![endif]-->
                                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:100%; max-width:700px;"" align=""center"">
                                      <tr>
                                        <td role=""modules-container"" style=""padding:0px 0px 0px 0px; color:#000000; text-align:left;"" bgcolor=""#FFFFFF"" width=""100%"" align=""left""><table class=""module preheader preheader-hide"" role=""module"" data-type=""preheader"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""display: none !important; mso-hide: all; visibility: hidden; opacity: 0; color: transparent; height: 0; width: 0;"">
    <tr>
      <td role=""module-content"">
        <p>歡迎加入我們其中一員!!</p>
      </td>
    </tr>
  </table><table class=""module"" role=""module"" data-type=""text"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""a9749b00-f200-4352-a302-1d63bdf0a1fc"" data-mc-module-version=""2019-10-22"">
   
  </table><table class=""wrapper"" role=""module"" data-type=""image"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""9f868e33-21ac-40c8-86bc-8c97d7cd69f0"">
    <tbody>
      <tr>
        <td style=""font-size:6px; line-height:10px; padding:0px 0px 0px 0px;"" valign=""top"" align=""center"">
           <img class=""cover-image"" width=""600"" alt="""" data-proportionally-constrained=""true"" data-responsive=""true"" src=""https://fakeimg.pl/1400x600/ff8000,128/000?text=GameModCrafters&font=noto"">
        </td>
      </tr>
    </tbody>
  </table><table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" width=""100%"" role=""module"" data-type=""columns"" style=""padding:30px 10px 0px 10px;"" bgcolor=""#FFFFFF"" data-distribution=""1"">
    <tbody>
      <tr role=""module-content"">
        <td height=""100%"" valign=""top""><table width=""520"" style=""width:520px; border-spacing:0; border-collapse:collapse; margin:0px 80px 0px 80px;"" cellpadding=""0"" cellspacing=""0"" align=""left"" border=""0"" bgcolor="""" class=""column column-0"">
      <tbody>
        <tr>
          <td style=""padding:0px;margin:0px;border-spacing:0;""><table class=""wrapper"" role=""module"" data-type=""image"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""b8c5fbf7-011d-4743-b847-3e4473737d9e"">
    
  </table><table class=""module"" role=""module"" data-type=""text"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""9fb55e76-a41f-446c-9893-536720221578"" data-mc-module-version=""2019-10-22"">
    <tbody>
      <tr>
        <td style=""padding:20px 0px 20px 0px; line-height:24px; text-align:inherit;"" height=""100%"" valign=""top"" bgcolor="""" role=""module-content""><div><div style=""font-family: inherit; text-align: center""><span style=""font-size: 24px""><strong>確認您的郵件地址</strong></span></div><div></div></div></td>
      </tr>
    </tbody>
  </table><table class=""module"" role=""module"" data-type=""text"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""abfcf137-fac5-4666-93df-b6940cd872ca"" data-mc-module-version=""2019-10-22"">
    <tbody>
      <tr>
        <td style=""padding:0px 0px 25px 0px; line-height:22px; text-align:inherit;"" height=""100%"" valign=""top"" bgcolor="""" role=""module-content""><div><div style=""font-family: inherit; text-align: center"">請點擊下方按鈕以確認您的郵件地址，享受我們提供的高品質產品和愉悅的使用體驗。</div><div></div></div></td>
      </tr>
    </tbody>
  </table><table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""module"" data-role=""module-button"" data-type=""button"" role=""module"" style=""table-layout:fixed;"" width=""100%"" data-muid=""d61458c8-876e-44ce-9554-b8854b1f44ba"">
      <tbody>
        <tr>
          <td align=""center"" bgcolor="""" class=""outer-td"" style=""padding:0px 0px 0px 0px;"">
            <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""wrapper-mobile"" style=""text-align:center;"">
              <tbody>
                <tr>
                <td align=""center"" bgcolor=""#FDBE84"" class=""inner-td"" style=""border-radius:6px; font-size:16px; text-align:center; background-color:inherit;"">
                  <a href=""{confirmationLink}"" style=""background-color:#FDBE84; border:0px solid #333333; border-color:#333333; border-radius:100px; border-width:0px; color:#000000; display:inline-block; font-size:14px; font-weight:bold; letter-spacing:1px; line-height:normal; padding:25px 85px 25px 85px; text-align:center; text-decoration:none; border-style:solid;"" target=""_blank"">確認郵件地址</a>
                </td>
                </tr>
              </tbody>
            </table>
          </td>
        </tr>
      </tbody>
    </table></td>
        </tr>
      </tbody>
    </table></td>
      </tr>
    </tbody>
  </table><table class=""module"" role=""module"" data-type=""spacer"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""7d922942-9306-4c3a-9888-e47841af10d1"">
   
    </table></td>
                                      </tr>
                                    </table>
                                    <!--[if mso]>
                                  </td>
                                </tr>
                              </table>
                            </center>
                            <![endif]-->
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
        </div>
      </center>
    </body>
        </html>";
            return htmlContent;
        }
        public string GetRestPasswordCssContent()
        {
            string cssContent = @"
            body, p, div {
              font-family: tahoma,geneva,sans-serif;
              font-size: 14px;
            }
            body {
              color: #000000;
            }
            body a {
              color: #000000;
              text-decoration: none;
            }
            p { margin: 0; padding: 0; }
            table.wrapper {
              width:100% !important;
              table-layout: fixed;
              -webkit-font-smoothing: antialiased;
              -webkit-text-size-adjust: 100%;
              -moz-text-size-adjust: 100%;
              -ms-text-size-adjust: 100%;
            }
            img.max-width {
              max-width: 100% !important;
            }
            .column.of-2 {
              width: 50%;
            }
            .column.of-3 {
              width: 33.333%;
            }
            .column.of-4 {
              width: 25%;
            }
            ul ul ul ul  {
              list-style-type: disc !important;
            }
            ol ol {
              list-style-type: lower-roman !important;
            }
            ol ol ol {
              list-style-type: lower-latin !important;
            }
            ol ol ol ol {
              list-style-type: decimal !important;
            }
            .cover-image {
              max-width: 100%;
              height: auto !important;
              display: block;
              color: #000000;
              text-decoration: none;
              font-family: Helvetica, Arial, sans-serif;
              font-size: 16px;
              border-radius: 10px;
            }
            @media screen and (max-width:480px) {
              .preheader .rightColumnContent,
              .footer .rightColumnContent {
                text-align: left !important;
              }
              .preheader .rightColumnContent div,
              .preheader .rightColumnContent span,
              .footer .rightColumnContent div,
              .footer .rightColumnContent span {
                text-align: left !important;
              }
              .preheader .rightColumnContent,
              .preheader .leftColumnContent {
                font-size: 80% !important;
                padding: 5px 0;
              }
              table.wrapper-mobile {
                width: 100% !important;
                table-layout: fixed;
              }
              img.max-width {
                height: auto !important;
                max-width: 100% !important;
              }
              a.bulletproof-button {
                display: block !important;
                width: auto !important;
                font-size: 80%;
                padding-left: 0 !important;
                padding-right: 0 !important;
              }
              .columns {
                width: 100% !important;
              }
              .column {
                display: block !important;
                width: 100% !important;
                padding-left: 0 !important;
                padding-right: 0 !important;
                margin-left: 0 !important;
                margin-right: 0 !important;
              }
              .social-icon-column {
                display: inline-block !important;
              }
             ";
            return cssContent;
        }
        public string GetRestPasswordHtmlContent(string cssContent ,string UserName,string confirmationLink)
        {
            string htmlContent = $@"
        <html>
        <head>
             <style>{cssContent}</style>
        </head>
        <body>
      <center class=""wrapper"" data-link-color=""#000000"" data-body-style=""font-size:14px; font-family:tahoma,geneva,sans-serif; color:#000000; background-color:#FFFFFF;"">
        <div class=""webkit"">
          <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" class=""wrapper"" bgcolor=""#FFFFFF"">
            <tr>
              <td valign=""top"" bgcolor=""#FFFFFF"" width=""100%"">
                <table width=""100%"" role=""content-container"" class=""outer"" align=""center"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                  <tr>
                    <td width=""100%"">
                      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                        <tr>
                          <td>
                            <!--[if mso]>
    <center>
    <table><tr><td width=""700"">
  <![endif]-->
                                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:100%; max-width:700px;"" align=""center"">
                                      <tr>
                                        <td role=""modules-container"" style=""padding:0px 0px 0px 0px; color:#000000; text-align:left;"" bgcolor=""#FFFFFF"" width=""100%"" align=""left""><table class=""module preheader preheader-hide"" role=""module"" data-type=""preheader"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""display: none !important; mso-hide: all; visibility: hidden; opacity: 0; color: transparent; height: 0; width: 0;"">
    <tr>
      <td role=""module-content"">
        <p>Hello {UserName} !</p>
      </td>
    </tr>
  </table><table class=""module"" role=""module"" data-type=""text"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""a9749b00-f200-4352-a302-1d63bdf0a1fc"" data-mc-module-version=""2019-10-22"">
   
  </table><table class=""wrapper"" role=""module"" data-type=""image"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""9f868e33-21ac-40c8-86bc-8c97d7cd69f0"">
    <tbody>
      <tr>
        <td style=""font-size:6px; line-height:10px; padding:0px 0px 0px 0px;"" valign=""top"" align=""center"">
           <img class=""cover-image"" width=""600"" alt="""" data-proportionally-constrained=""true"" data-responsive=""true"" src=""https://fakeimg.pl/1400x600/ff8000,128/000?text=GameModCrafters&font=noto"">
        </td>
      </tr>
    </tbody>
  </table><table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" width=""100%"" role=""module"" data-type=""columns"" style=""padding:30px 10px 0px 10px;"" bgcolor=""#FFFFFF"" data-distribution=""1"">
    <tbody>
      <tr role=""module-content"">
        <td height=""100%"" valign=""top""><table width=""520"" style=""width:520px; border-spacing:0; border-collapse:collapse; margin:0px 80px 0px 80px;"" cellpadding=""0"" cellspacing=""0"" align=""left"" border=""0"" bgcolor="""" class=""column column-0"">
      <tbody>
        <tr>
          <td style=""padding:0px;margin:0px;border-spacing:0;""><table class=""wrapper"" role=""module"" data-type=""image"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""b8c5fbf7-011d-4743-b847-3e4473737d9e"">
    
  </table><table class=""module"" role=""module"" data-type=""text"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""9fb55e76-a41f-446c-9893-536720221578"" data-mc-module-version=""2019-10-22"">
    <tbody>
      <tr>
        <td style=""padding:20px 0px 20px 0px; line-height:24px; text-align:inherit;"" height=""100%"" valign=""top"" bgcolor="""" role=""module-content""><div><div style=""font-family: inherit; text-align: center""><span style=""font-size: 24px""><strong>{UserName}</strong></span></div><div></div></div></td>
      </tr>
    </tbody>
  </table><table class=""module"" role=""module"" data-type=""text"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""abfcf137-fac5-4666-93df-b6940cd872ca"" data-mc-module-version=""2019-10-22"">
    <tbody>
      <tr>
        <td style=""padding:0px 0px 25px 0px; line-height:22px; text-align:inherit;"" height=""100%"" valign=""top"" bgcolor="""" role=""module-content""><div><div style=""font-family: inherit; text-align: center"">請點擊下方按鈕以重置您的密碼。</div><div></div></div></td>
      </tr>
    </tbody>
  </table><table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""module"" data-role=""module-button"" data-type=""button"" role=""module"" style=""table-layout:fixed;"" width=""100%"" data-muid=""d61458c8-876e-44ce-9554-b8854b1f44ba"">
      <tbody>
        <tr>
          <td align=""center"" bgcolor="""" class=""outer-td"" style=""padding:0px 0px 0px 0px;"">
            <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""wrapper-mobile"" style=""text-align:center;"">
              <tbody>
                <tr>
                <td align=""center"" bgcolor=""#FDBE84"" class=""inner-td"" style=""border-radius:6px; font-size:16px; text-align:center; background-color:inherit;"">
                  <a href=""{confirmationLink}"" style=""background-color:#FDBE84; border:0px solid #333333; border-color:#333333; border-radius:100px; border-width:0px; color:#000000; display:inline-block; font-size:14px; font-weight:bold; letter-spacing:1px; line-height:normal; padding:25px 85px 25px 85px; text-align:center; text-decoration:none; border-style:solid;"" target=""_blank"">重置密碼</a>
                </td>
                </tr>
              </tbody>
            </table>
          </td>
        </tr>
      </tbody>
    </table></td>
        </tr>
      </tbody>
    </table></td>
      </tr>
    </tbody>
  </table><table class=""module"" role=""module"" data-type=""spacer"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""7d922942-9306-4c3a-9888-e47841af10d1"">
   
    </table></td>
                                      </tr>
                                    </table>
                                    <!--[if mso]>
                                  </td>
                                </tr>
                              </table>
                            </center>
                            <![endif]-->
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
        </div>
      </center>
    </body>
        </html>";
            return htmlContent;
        }
    }
}
