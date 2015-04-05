using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.IO;

namespace AccoBooking.Web
{
  /// <summary>
  /// Summary description for $codebehindclassname$
  /// </summary>
  [WebService(Namespace = "http://tempuri.org/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class MergeHandler
      : IHttpHandler
  {

    /*
     *  --------------------------------------------------------------------
     *  This handler process any quantity of files splitted in smaller parts
     *  using the Multipart/Post format that is standard in the Browsers
     *  --------------------------------------------------------------------
     */
    public void ProcessRequest(HttpContext context)
    {
      try
      {
        // get custom parameters
        var accoid = context.Request.Params["acco"];
        var parameters = context.Request.Params["parameter"];

        // get the uploaded file, and calculates the full path to save it in the server
        HttpPostedFile file = context.Request.Files[0];
        string serverFileName = UploaderHelper.GetServerPath(context.Server, accoid + "_" + file.FileName);

        // get parts parameters (used to upload a file broken into small parts)
        int partCount = int.Parse(context.Request.Params["partCount"]);
        int partNumber = int.Parse(context.Request.Params["partNumber"]);

        // process this new small part
        if (UploaderHelper.ProcessPart(context, file.InputStream, serverFileName, partCount, partNumber))
        {
          // write the url of the uploaded file into the response
          string url = UploaderHelper.GetUploadedFileUrl(context, "Helpers/MergeHandler.ashx", file.FileName);
          context.Response.Write(url);
        }
        else
        {
          UploaderHelper.WriteError(context, UploaderHelper.ERROR_MESSAGE);
        }
      }
      catch (Exception exc)
      {
        UploaderHelper.WriteError(context, exc.Message);
        context.Response.End();
      }
    }


    public bool IsReusable
    {
      get
      {
        return false;
      }
    }
  }
}
