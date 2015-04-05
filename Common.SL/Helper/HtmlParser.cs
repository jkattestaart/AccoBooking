using System;
//TODO HTML Agilitypack (crashed SL)


namespace Common.Helper
{
  public class HtmlParser
  {
    public static string ReadTitle(string html)
    {
      string[] str = html.Split(new string[] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);
      bool append = false;
      string title = "";
      for (int i = 0; i < str.Length; i++)
      {
        if (!append)
        {
          if (str[i].Contains("<title>"))
            append = true;
          else 
            continue;
        }
        
        
        title += str[i].Replace("<title>", "").Replace("</title>", "");

        if (str[i].Contains("</title"))
          return title.Trim();
      }
      return "";
    }
  }
}
