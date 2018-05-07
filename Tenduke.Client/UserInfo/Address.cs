using System;
using System.Collections.Generic;
using System.Text;

namespace Tenduke.Client.UserInfo
{
    /// <summary>
    /// Object describing the OpenID Connect user info address fields supported by the
    /// 10Duke Identity and Entitlement service.
    /// </summary>
    public class Address
    {
#pragma warning disable IDE1006 // Naming Styles
        public string street_address { get; set; }
        public string locality { get; set; }
        public string region { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
