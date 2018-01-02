using System.Net.Http.Headers;
using Sitecore.XConnect.Web.Controllers;

namespace Sitecore.Support.XConnect.Web.Controllers
{
  using Microsoft.OData.UriParser;
  using Sitecore.XConnect;
  using Sitecore.XConnect.Diagnostics.Telemetry;
  using Sitecore.XConnect.Operations;
  using Sitecore.XConnect.Schema;
  using Sitecore.XConnect.Serialization;
  using Sitecore.XConnect.Service;
  using Sitecore.XConnect.Web;
  using Sitecore.XConnect.Web.Infrastructure;
  using Sitecore.XConnect.Web.Infrastructure.FilterVisitors;
  using Sitecore.XConnect.Web.Infrastructure.Operations;
  using Sitecore.XConnect.Web.Infrastructure.Serialization.WebApi;
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using System.Net;
  using System.Net.Http;
  using System.Runtime.CompilerServices;
  using System.Threading.Tasks;
  using System.Web;
  using System.Web.Http;
  using System.Web.OData;
  using System.Web.OData.Routing;
  using Sitecore.XConnect.Web.Infrastructure;

  public class ContactsController : Sitecore.XConnect.Web.Controllers.ContactsController
  {
    public ContactsController(XdbCollectionService context, XdbEdmModel model, ExpandOptionsParser expandOptionsParser, OperationResponseMapper responseMapper, IEnumerable<IGetQueryMapper<Contact>> getQueryMappers, IPerformanceCounters counters) : base(context, model, expandOptionsParser, responseMapper, getQueryMappers, counters)
    {
    }


    [HttpDelete, ODataRoute("Contacts({contactId})/Identifiers(Source={source},Identifier={identifier})")]
    public new Task<HttpResponseMessage> DeleteFromIdentifiers([FromODataUri] Guid contactId, [FromODataUri] string source, [FromODataUri] string identifier)
    {
      if ((source == null) || (identifier == null))
      {
        throw new HttpResponseException(base.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Messages.ContactsController_PostToIdentifiers_Source_and_Identifier_are_required));
      }
      IEntityReference<Contact> contactReferenceForIdentifierOperation = this.GetContactReferenceForIdentifierOperation(contactId);
      return base.CreateResponse(new RemoveContactIdentifierOperation(new ContactIdentifierReference(contactReferenceForIdentifierOperation, source, HttpUtility.UrlDecode(identifier))));
    }
    private IEntityReference<Contact> GetContactReferenceForIdentifierOperation(Guid contactId)
    {
      IEntityReference<Contact> reference = base.Request.GetReference<Contact>(contactId);
      if (!(reference is Contact))
      {
        string tag = base.Request.Headers.IfMatch.FirstOrDefault<EntityTagHeaderValue>()?.Tag;
        if (tag != null)
        {
          Contact entity = new Contact();
          Guid guid = Guid.Parse(tag.Substring(1, tag.Length - 2));
          DeserializationHelpers.SetId(entity, contactId);
          DeserializationHelpers.SetConcurrencyToken(entity, new Guid?(guid));
          return entity;
        }
      }
      return reference;
    }

}
