using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;
using DomainModel;

namespace AccoBooking.Web
{
  /// <summary>
  /// Class used to help the handling of files uploaded to the server using the C1Uploader
  /// You can use it in your projects, modify it, or whatever.
  /// This file is an examples and is provided "as is".
  /// </summary>
  public static class UploaderHelper
  {
    private const string STORAGE_SUBFOLDER_VIRTUAL = "temp/";
    private const string STORAGE_SUBFOLDER = @"temp\";
    public const string ERROR_MESSAGE = "Couldn't upload the file. Please verify it's a correct file";

    // --------------------------------------------------------------------------------------
    #region ** validation

    /// <summary>
    /// Verifies if the stream is a valid file, for security reassons.
    /// *** This is an example, more advanced security checkings might be necessary ***
    /// </summary>
    /// <param name="stream">Input stream</param>
    /// <param name="context">HttpContext to write the error in case it's an invalid file</param>
    /// <returns>true if it's a valid file</returns>
    public static bool IsValidFile(HttpContext context, Stream stream)
    {
      bool valid = true;
      try
      {
        // check that it really is an image by loading it into an Image object
        //@@@@@@@ System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
      }
      catch
      {
        // doesn't look like a valid image..
        valid = false;
      }
      return valid;
    }

    /// <summary>
    /// Verifies if the stream is a valid, for security reassons.
    /// *** This is an example, more advanced security checkings might be necessary ***
    /// </summary>
    /// <param name="context">HttpContext to write the error in case it's an invalid image</param>
    /// <param name="file">HttpPostedFile received</param>
    /// <returns>true if it's a valid image</returns>
    internal static bool IsValidFile(HttpContext context, HttpPostedFile file)
    {
      return IsValidFile(context, file.InputStream);
    }

    #endregion

    // -----------------------------------------------------------------------------------
    #region ** process file & file part

    /// <summary>
    /// Process small parts and save them to disk when they are valid
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <param name="stream"></param>
    /// <param name="fileName"></param>
    /// <param name="partCount"></param>
    /// <param name="partNumber"></param>
    /// <returns>true if the file was processed ok</returns>
    public static bool ProcessPart(HttpContext context, Stream stream, string serverFileName, int partCount, int partNumber)
    {
      int length = (int)stream.Length;

      // if it's the first part
      if (partNumber == 1)
      {
        FileStream fileStream = File.Open(serverFileName, FileMode.Create);
        byte[] buffer = new byte[length];
        stream.Read(buffer, 0, length);
        fileStream.Write(buffer, 0, length);

        if (partCount != partNumber)
        {
          // not the last one, save temporarilly
          fileStream.Close();
        }
        else if (IsValidFile(context, stream))
        {
          // it's the last one and it's a valid
          fileStream.Close();
        }
        else
        {
          CleanStream(fileStream, serverFileName);
          return false;
        }
      }
      else
      {
        // put the stream in a buffer
        byte[] buffer = new byte[length];
        stream.Read(buffer, 0, length);

        // if it isn't the last part
        if (partCount != partNumber)
        {
          // read the temporal file saved in the server and append the new part
          FileStream storedFile = File.Open(serverFileName, FileMode.Append);
          storedFile.Write(buffer, 0, buffer.Length);
          storedFile.Close();
        }
        else
        {
          // this is the last part 
          // read the temporal file saved in the server and append the new part
          FileStream storedFile = File.Open(serverFileName, FileMode.Open);
          storedFile.Position = storedFile.Length;
          storedFile.Write(buffer, 0, buffer.Length);

          // verify that it's a valid i
          if (UploaderHelper.IsValidFile(context, storedFile))
          {
            storedFile.Close();
          }
          else
          {
            // remove the file content before closing it
            CleanStream(storedFile, serverFileName);
            return false;
          }
        }
      }

      return true;
    }

    #endregion

    // --------------------------------------------------------------------------------------
    #region ** private stuff

    /// <summary>
    /// Calculates the URL of the uploaded file in the server
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <param name="handlerName">Name of the handler calling this method</param>
    /// <param name="fileName">Name of the method</param>
    /// <returns>The url for the file, to be accessed through internet</returns>
    public static string GetUploadedFileUrl(HttpContext context, string handlerName, string fileName)
    {
      return context.Request.Url.AbsoluteUri.Replace(handlerName, STORAGE_SUBFOLDER_VIRTUAL + fileName);
    }

    /// <summary>
    /// Calculates the full path for an uploaded file in the server 
    /// </summary>
    /// <param name="server">Server context</param>
    /// <param name="fileName">Name of the file</param>
    /// <returns>The full path to the file in the server</returns>
    public static string GetServerPath(HttpServerUtility server, string fileName)
    {
      //@@@ omdat de ashx in de helpers directory staat moet die er nu uit, later een speciale url maken
      GeneralLibrary.CurrentUpload = server.MapPath(Path.Combine(STORAGE_SUBFOLDER, Path.GetFileName(fileName))).Replace(@"Helpers\", "");
      return GeneralLibrary.CurrentUpload;
    }


    /// <summary>
    /// Writes an error in the context' response.
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <param name="message">Error message to write</param>
    public static void WriteError(HttpContext context, string message)
    {
      context.Response.StatusCode = 500;
      context.Response.Write(message);
    }

    /// <summary>
    /// Clean an openend stream, then closes it, and then remove the corresponding file
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="serverFileName"></param>
    public static void CleanStream(FileStream fileStream, string serverFileName)
    {
      int lenght = (int)fileStream.Length;
      fileStream.Position = 0;
      fileStream.Write(new byte[lenght], 0, lenght);
      fileStream.Close();

      File.Delete(serverFileName);
    }

    #endregion
  }
}
