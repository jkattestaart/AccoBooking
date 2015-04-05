using System;
using System.IO;
using System.Windows;

namespace Common.Helper
{
  public class ResourceReader
  {
    public static string Content(Uri resource)
    {
      string content;

      var res = Application.GetResourceStream(resource);
      var s = res.Stream;

      using (var reader = new StreamReader(s))
      {
        content = reader.ReadToEnd();
      }
      return content;
    }
  }
}
