using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerAMLServiceAPI.Models
{
    public abstract class BaseModel : IModel
    {
        public virtual string ApiKey { get; set; }
        /// <summary>
        ///     Has to be the base of the URIs that are available
        ///     to copy from either Request/Response tab or Batch
        ///     Execution tab. We just append the string in the end
        ///     that decides if it is a req/resp URI, a submit BES
        ///     URI, a start BES URI, etc. 
        ///     
        ///     The implementation properties of this URI return the base URI + an extension string 
        ///     for the respective purpose. 
        ///     If you go and look in a web service dashboard and click either the request/response
        ///     or batch execution service links, you'll end up in documentation pages that have a set
        ///     of links. If you remove the last bits of them you'll get a common URI string, so it doesn't
        ///     matter which one you take, it just matters that you paste the right part of the string into
        ///     the BaseURI property.
        ///     
        ///     Outstanding thought on this is that the BES function seems to take in job_id/, and the question
        ///     is whether or not this is the string you get as a response from the submit request. It doesn't
        ///     mention if or how this is to be used, but my guess is that it is to replace that "job_id" string.
        /// </summary>
        public virtual string BaseURI { get; set; }
        public virtual string DashboardURL { get; set; }

    }
}