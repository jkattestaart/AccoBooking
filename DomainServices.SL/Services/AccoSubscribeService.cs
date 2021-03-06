﻿using System;
using System.Threading.Tasks;
using DomainModel;
using IdeaBlade.EntityModel;

namespace DomainServices.Services
{
  public class AccoSubscribeService
  {
    public static async Task ExecuteAsync(int accoid, string language)
    {
      EntityManager mgr = new AccoBookingEntities();

      var result = await mgr.InvokeServerMethodAsync(Library.Acco, Method.Subscribe, accoid, language);

      var message = (string) result;

      if (!String.IsNullOrEmpty(message))
        throw (new Exception(message));
    }

  }
}
