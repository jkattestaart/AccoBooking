using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using AccoBooking.ViewModels.Booking;
using C1.Silverlight.Uploader;
using C1.Silverlight;
using DomainModel;
using UploadProgressChangedEventArgs = C1.Silverlight.Uploader.UploadProgressChangedEventArgs;

namespace AccoBooking.Views.Booking
{
  public partial class PreviewMailView : UserControl
  {
    private C1Uploader _uploader;
    private FileInfo CurrentFile;
    private PreviewMailViewModel _ctx;

    public PreviewMailView()
    {
      InitializeComponent();
    }

    private void Select_Click(object sender, RoutedEventArgs e)
    {
      _ctx = this.DataContext as PreviewMailViewModel;
      var dialog = new OpenFileDialog();
      dialog.Multiselect = false;

      // Show OpenFileDialog
      dialog.ShowDialog();
      if (dialog.Files != null)
      {
        CurrentFile = dialog.File;
        Attachment.Text = dialog.File.Name;
        StartUpload();
      }
      else
      {
        CurrentFile = null;
        Attachment.Text = "";
        //CancelUpload();
      }

    }

    private void StartUpload()
    {
      _uploader = CreateUploader(FilesPerRequest.SplitFilesIntoMultipleRequests);

      // set custom parameters
      _uploader.MaximumUploadSize = 1024*1024;
      _uploader.AddFile(CurrentFile);
      _uploader.Parameters["acco"] = SessionManager.CurrentAcco.AccoId.ToString();
      _uploader.Parameters["parameter"] = "this is a CUSTOM parameter sent from the client to the server";

      // handle events
      _uploader.UploadCompleted += _uploader_UploadCompleted;
      _uploader.UploadProgressChanged += _uploader_UploadProgressChanged;

      // start upload
      _uploader.BeginUploadFiles();

      //melden viewmodel to disable send button
    }

    void _uploader_UploadCompleted(object sender, UploadCompletedEventArgs e)
    {
      //melden viewmodel to enable send button
    }

    void _uploader_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
    {
      Progress.Value = e.ProgressPercentage * 100;
      //e.ProgressPercentage geeft percentage -> achtergrond masked textbox?
    }

    private C1Uploader CreateUploader(FilesPerRequest filesPerRequest)
    {
      // the file will be handled in the server by an ASP.NET Handler through the Request.Files collection
      var mpUploader = new C1UploaderPost(filesPerRequest);
      mpUploader.Settings.Address = Extensions.GetAbsoluteUri("Helpers/MergeHandler.ashx");
      return mpUploader;

    }

  }
}
